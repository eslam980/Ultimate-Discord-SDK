#region Reference
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace UDiscord
{
    public class DiscordManager : MonoBehaviour
    {
        public static DiscordManager App; void Awake() { App = this;}
        public long Discord_AppID;
        public string Discord_SteamID;
        public bool Discord_Stay = false;
        public bool Discord_Start = false;

        [Header("RichPresence Settings")]
        public RichPresence Richpresence = new RichPresence();

        [Header("Events Settings")]
        public UnityEvent OnJoin = new UnityEvent();
        public UnityEvent OnConnect = new UnityEvent();
        public UnityEvent OnDisconnect = new UnityEvent();
        public UnityEvent OnDestroy = new UnityEvent();
        public Discord.Discord discord;

            void Start()
            {
                if(Discord_Stay) { DontDestroyOnLoad(this);}
                if(Discord_Start) OnConnect.AddListener(CallDiscord); OnConnect?.Invoke();
            }
            void Update()
            {
                discord.RunCallbacks();
            }
            #region ID
            //this is A Better way to call Discord.Discord to get ID and connect to discord
            public void GetID(long ID)
            {
                // Default is 0 you can 
            discord = new Discord.Discord(ID , (UInt64)Discord.CreateFlags.Default);
            }
            public void GetID(long ID , ulong flag)
            {
                // Default is 0 you can 
                Discord.Discord discord = new Discord.Discord(ID , flag);
            }
            #endregion
            #region Rich Fuctions
            // CreateRich : is for Get ID and Activities of Discord to Show Rich in discord
            public void CreateRich(long ID , ulong flag,string detail, string state = null, long start = -1, long end = -1, string largeKey = null,string largeText = null, string smallKey = null, string smallText = null, string partyId = null, int size = -1, int max = -1, string match = null, string join = null, string spectate = null)
            {
                GetID(ID);
                var activityManager = discord.GetActivityManager();
                var activity = new Discord.Activity 
                {
                    Details = detail ?? Richpresence.details,
                    State = state ?? Richpresence.state,
                    Timestamps =
                    {
                        Start = (start == -1) ? Richpresence.startTimestamp : start,
                        End = (end == -1) ? Richpresence.endTimestamp : end
                    },
                    Assets =
                    {
                        LargeImage = largeKey ?? Richpresence.largeImageKey,  // Larger Image Asset Key
                        LargeText = largeText ?? Richpresence.largeImageText,  // Large Image Tooltip

                        SmallImage = smallKey ?? Richpresence.smallImageKey,  // Small Image Asset Key
                        SmallText = smallText ?? Richpresence.smallImageText,  // Small Image Tooltip
                    },
                    Party =
                    {
                        Id = partyId ?? Richpresence.partyId,
                        Size = {
                            CurrentSize = (size == -1) ? Richpresence.partySize : size,
                            MaxSize = (max == -1) ? Richpresence.partyMax : max
                        },
                    },
                    Secrets =
                    {
                        Match = match ?? Richpresence.matchSecret,
                        Join = join ?? Richpresence.joinSecret,
                        Spectate = spectate ?? Richpresence.spectateSecret
                    }
                };
                activityManager.UpdateActivity(activity, (res) => {
                    if(res == Discord.Result.Ok)
                        Debug.Log("Discord Status Is On!");
                    else
                    Debug.LogError("Discord Status Failed!");
                });
            }
        // CallDiscord : Is for Connect Discord with unity (Template) , Just Use Bool Discord_Start to See Template on Action after Filling RichPresence
        void CallDiscord()
        {
            //getting Discord User ID and Default Stats
            discord = new Discord.Discord(Discord_AppID , (UInt64)Discord.CreateFlags.Default);
            var activityManager = discord.GetActivityManager();
            var LobbyManager = discord.GetLobbyManager();
            var activity = new Discord.Activity 
            {
                Details = Richpresence.details,
                State = Richpresence.state,
                Timestamps =
                {
                    Start = Richpresence.startTimestamp,
                    End = Richpresence.endTimestamp
                },
                Assets =
                {
                    LargeImage = Richpresence.largeImageKey,  // Larger Image Asset Key
                    LargeText = Richpresence.largeImageText,  // Large Image Tooltip
                    SmallImage = Richpresence.smallImageKey,  // Small Image Asset Key
                    SmallText = Richpresence.smallImageText,  // Small Image Tooltip
                },
                Party =
                {
                    Id = Richpresence.partyId,
                    Size = {
                        CurrentSize = Richpresence.partySize,
                        MaxSize = Richpresence.partyMax,
                    },
                },
                Secrets =
                {
                    Match = Richpresence.matchSecret,
                    Join = Richpresence.joinSecret,
                    Spectate = Richpresence.spectateSecret
                }
            };          
            activityManager.UpdateActivity(activity, (res) => {

                activityManager.SendInvite(485905734618447895, Discord.ActivityActionType.Join, "", (inviteUserResult) =>
                {
                    Console.WriteLine("Invite User {0}", inviteUserResult);
                });

                if(res == Discord.Result.Ok)
                    Debug.Log("Discord Status Is On!");
                else
                Debug.LogError("Discord Status Failed!");


            });
            // Received when someone accepts a request to join or invite.
// Use secrets to receive back the information needed to add the user to the group/party/match
activityManager.OnActivityJoin += secret => {
    Console.WriteLine("OnJoin {0}", secret);
    LobbyManager.ConnectLobbyWithActivitySecret(secret, (Discord.Result result, ref Discord.Lobby lobby) =>
    {
        Console.WriteLine("Connected to lobby: {0}", lobby.Id);
        // Connect to voice chat, used in this case to actually know in overlay if your successful in connecting.
        LobbyManager.ConnectVoice(lobby.Id, (Discord.Result voiceResult) => {

            if (voiceResult == Discord.Result.Ok)
            {
                Console.WriteLine("New User Connected to Voice! Say Hello! Result: {0}", voiceResult);
               // PhotonNetwork.JoinRoom(Richpresence.state);
            }
            else
            {
                Console.WriteLine("Failed with Result: {0}", voiceResult);
            };
        });
        //Connect to given lobby with lobby Id
        LobbyManager.ConnectNetwork(lobby.Id);
        LobbyManager.OpenNetworkChannel(lobby.Id, 0, true);
        foreach (var user in LobbyManager.GetMemberUsers(lobby.Id))
        {
            //Send a hello message to everyone in the lobby
            LobbyManager.SendNetworkMessage(lobby.Id, user.Id, 0,
                Encoding.UTF8.GetBytes(String.Format("Hello, {0}!", user.Username)));
        }
        //Sends this off to a Activity callback named here as 'UpdateActivity' passing in the discord instance details and lobby details
        UpdateActivity(discord, lobby);
    });
};

void UpdateActivity(Discord.Discord discord, Discord.Lobby lobby)
    {
        //Creates a Static String for Spectate Secret.
        string discordSpectateSecret = "wdn3kvj320r8vk3";
        var activity = new Discord.Activity
        {
            State = "Playing Co-Op",
            Details = "In a Multiplayer Match!",
            Timestamps =
            {
                Start = 5,
            },
            Assets =
            {
                LargeImage = "matchimage1",
                LargeText = "Inside the Arena!",
            },
            Party = {
                Id = lobby.Id.ToString(),
                Size = {
                    CurrentSize = LobbyManager.MemberCount(lobby.Id),
                    MaxSize = (int)lobby.Capacity,
                },
            },
            Secrets = {
                Spectate = discordSpectateSecret,
                Join = Richpresence.joinSecret,
            },
            Instance = true,
        };

        activityManager.UpdateActivity(activity, result =>
        {
            Debug.LogFormat("Updated to Multiplayer Activity: {0}", result);

            // Send an invite to another user for this activity.
            // Receiver should see an invite in their DM.
            // Use a relationship user's ID for this.
            // activityManager
            //   .SendInvite(
            //       364843917537050624,
            //       Discord.ActivityActionType.Join,
            //       "",
            //       inviteResult =>
            //       {
            //           Console.WriteLine("Invite {0}", inviteResult);
            //       }
            //   );
        });
    }
            //StartCoroutine(test());
        }
            //UpdateRich : Is Fuction to Update Rich by Typing : UpdateRich(detail: "Detail text" , state : "State text");
            public void UpdateRich(string detail = null, string state = null, long start = -1, long end = -1, string largeKey = null,string largeText = null, string smallKey = null, string smallText = null, string partyId = null, int size = -1, int max = -1, string match = null, string join = null, string spectate = null)
            {
                var activityManager = discord.GetActivityManager();
                var activity = new Discord.Activity 
                {
                    Details = detail ?? Richpresence.details,
                    State = state ?? Richpresence.state,
                    Timestamps =
                    {
                        Start = (start == -1) ? Richpresence.startTimestamp : start,
                        End = (end == -1) ? Richpresence.endTimestamp : end
                    },
                    Assets =
                    {
                        LargeImage = largeKey ?? Richpresence.largeImageKey,  // Larger Image Asset Key
                        LargeText = largeText ?? Richpresence.largeImageText,  // Large Image Tooltip

                        SmallImage = smallKey ?? Richpresence.smallImageKey,  // Small Image Asset Key
                        SmallText = smallText ?? Richpresence.smallImageText,  // Small Image Tooltip
                    },
                    Party =
                    {
                        Id = partyId ?? Richpresence.partyId,
                        Size = {
                            CurrentSize = (size == -1) ? Richpresence.partySize : size,
                            MaxSize = (max == -1) ? Richpresence.partyMax : max
                        },
                    },
                    Secrets =
                    {
                        Match = match ?? Richpresence.matchSecret,
                        Join = join ?? Richpresence.joinSecret,
                        Spectate = spectate ?? Richpresence.spectateSecret
                    }
                };
                activityManager.UpdateActivity(activity, (res) => {
                    if(res == Discord.Result.Ok)
                        Debug.Log("Discord Status Is On!");
                    else
                    Debug.LogError("Discord Status Failed!");
                });
            }
            #endregion
            #region Next Patch
            // Watchout !! 
            //This Region for Testing by Developer , if you don't understand what is this don't edit them
            void InviteGame()
            {
                var activityManager = discord.GetActivityManager();
                var lobbyManager = discord.GetLobbyManager();
                activityManager.OnActivityJoin += secret => 
                {
                    //PhotonNetwork.JoinRoom(Richpresence.state);
                    Console.WriteLine("OnJoin {0}", secret);
                    lobbyManager.ConnectLobbyWithActivitySecret(secret, (Discord.Result result, ref Discord.Lobby lobby) =>
                        {
                            //PhotonNetwork.JoinRoom(Richpresence.state);
                            Console.WriteLine("Connected to lobby: {0}", lobby.Id);
                            // Connect to voice chat, used in this case to actually know in overlay if your successful in connecting.
                            lobbyManager.ConnectVoice(lobby.Id, (Discord.Result voiceResult) => {
                                if (voiceResult == Discord.Result.Ok)
                                {
                                    Console.WriteLine("New User Connected to Voice! Say Hello! Result: {0}", voiceResult);
                                    //PhotonNetwork.JoinRoom(Richpresence.state);
                                }
                                else
                                {
                                    Console.WriteLine("Failed with Result: {0}", voiceResult);
                                };
                            });
                            //Connect to given lobby with lobby Id
                            lobbyManager.ConnectNetwork(lobby.Id);
                            lobbyManager.OpenNetworkChannel(lobby.Id, 0, true);
                            foreach (var user in lobbyManager.GetMemberUsers(lobby.Id))
                            {
                                //Send a hello message to everyone in the lobby
                                lobbyManager.SendNetworkMessage(lobby.Id, user.Id, 0,
                                    Encoding.UTF8.GetBytes(String.Format("Hello, {0}!", user.Username)));
                            }
                            //Sends this off to a Activity callback named here as 'UpdateActivity' passing in the discord instance details and lobby details
                            UpdateActivity(lobby);
                        });
                };

                void UpdateActivity(Discord.Lobby lobby)
                    {
                        //Creates a Static String for Spectate Secret.
                        string discordSpectateSecret = "wdn3kvj320r8vk3";
                        var activity = new Discord.Activity
                        {
                            State = "Playing Co-Op",
                            Details = "In a Multiplayer Match!",
                            Timestamps =
                            {
                                Start = 5,
                            },
                            Party = {
                                Id = lobby.Id.ToString(),
                                Size = {
                                    CurrentSize = lobbyManager.MemberCount(lobby.Id),
                                    MaxSize = (int)lobby.Capacity,
                                },
                            },
                            Secrets = {
                                Spectate = discordSpectateSecret,
                                Join = "MTI4NzM0OjFpMmhuZToxMjMxMjM= ",
                            },
                            Instance = true,
                        };

                        activityManager.UpdateActivity(activity, result =>
                        {
                            Debug.LogFormat("Updated to Multiplayer Activity: {0}", result);

                        activityManager.SendInvite(Discord_AppID,Discord.ActivityActionType.Join,"", inviteResult =>
                            {
                                Console.WriteLine("Invite {0}", inviteResult);
                            });
                        });
                    }
            }
            #endregion
            #region Others
            //OnDisable means when object is disabled or game turned off
            void OnDisable()
            {
                Debug.Log("Discord Shutdown after 10 sec");
                Shutdown();
                discord.Dispose();
            }
            [DllImport("RichDiscord", EntryPoint = "Discord_Shutdown", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Shutdown();
            #endregion

    }

    #region Rich Presence
        //Activity Type : Means the Type of Discord like :
        /*
        Playing : Discord do , Playing A Game
        Streaming : Discord do , Streaming A Game
        Listening : Discord do , Listening A Song
        Watching : Discord do , Watching A Movie
        */
        public enum ActivityType
        {
            Playing,
            Streaming,
            Listening,
            Watching,
        }
        //CreateFlags : Just Make it 0 ;)
        public enum CreateFlags
        {
            Default = 0,
            NoRequireDiscord = 1,
        }
        //Activities : Is All the Activities you can add in Discord like type , id , name , state
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public partial struct Activities
        {
            public ActivityType Type;

            public Int64 ApplicationId;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string Name;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string State;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string Details;

            public ActivityTimestamps Timestamps;

            public ActivityAssets Assets;

            public ActivityParty Party;

            public ActivitySecrets Secrets;

            public bool Instance;
        }
        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public partial struct ActivitySecrets
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string Match;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string Join;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string Spectate;
        }
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public partial struct ActivityTimestamps
        {
            public Int64 Start;

            public Int64 End;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public partial struct ActivityAssets
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string LargeImage;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string LargeText;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string SmallImage;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string SmallText;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public partial struct PartySize
        {
            public Int32 CurrentSize;

            public Int32 MaxSize;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public partial struct ActivityParty
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string Id;

            public PartySize Size;
        }
        #endregion
            #region RichPresence
            
            [Serializable]
            public class RichPresence
            {
                [Header("Main Settings")]
                public string details;           // max 128 bytes
                public string state;            // max 128 bytes
                public long startTimestamp;
                public long endTimestamp;

                [Header("Image Settings")]
                public string largeImageKey;    // max 32 bytes
                public string largeImageText;   // max 128 bytes
                public string smallImageKey;    // max 32 bytes
                public string smallImageText; 

                [Header("Party Settings")] // max 128 bytes
                public string partyId;          // max 128 bytes
                public int partySize;
                public int partyMax;
                
                [Header("Match Settings")]
                public string matchSecret;      // max 128 bytes
                public string joinSecret;       // max 128 bytes
                public string spectateSecret;   // max 128 bytes
            }
    #endregion
}