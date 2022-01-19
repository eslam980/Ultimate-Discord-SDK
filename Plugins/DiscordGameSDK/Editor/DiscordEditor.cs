using UnityEditor;
using UnityEngine;
using UDiscord;
public class DiscordEditor : ExtendedEditorWindow
{
    bool sidebars = true;
    int currentTab = 0;
    public static DiscordManager manager;
    Texture2D Icon;
    static bool alreadyOpened;
    public static void Open(DiscordManager stats)
    {
        alreadyOpened = true;
        DiscordEditor editor = GetWindow<DiscordEditor>("Ultimate Discord Editor");
        editor.serializedObject = new SerializedObject(stats);
        manager = stats;
    }
    void OnEnable()
    {
        if(DiscordManager.App != null && manager == null && !alreadyOpened)
        {
            Open(DiscordManager.App);
        }
        //Icon = ( Texture2D ) Resources.Load( "UltimateDiscordMainIcon" );
    }
    void OnDisable()
    {
        manager = null;
        alreadyOpened = false;
    }

    void OnGUI()
    {
        
        if(sidebars)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical("box" , GUILayout.MaxWidth(150) , GUILayout.ExpandHeight(true));

            DrawButton();

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));

        DrawSelectedPropertiesPanel();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        ApplyChanges();
    }

    void DrawSelectedPropertiesPanel()
    {

        DrawProp("Discord_AppID");

        DrawProp("Discord_SteamID");

        DrawProp("Discord_Stay");
                
        DrawProp("Discord_Start");

        EditorGUILayout.Space(8);
        EditorGUILayout.BeginHorizontal();

        if(GUILayout.Button("Rich Presence", EditorStyles.toolbarButton))
        {
            currentTab = 1;
        }

        if(GUILayout.Button("Events", EditorStyles.toolbarButton))
        {
            currentTab = 2;
        }
        EditorGUILayout.EndHorizontal();

        switch (currentTab)
        {
            case 1 : //Rich Presence
                EditorGUILayout.BeginVertical("box");

                DrawProp("Richpresence");

                if(manager.Discord_AppID == 0)
                {
                    EditorGUILayout.BeginHorizontal("box");
                    EditorGUILayout.HelpBox("There Are No ID for Discord , Please Add One" , MessageType.Warning);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();

            break;

            case 2 : // Events
                EditorGUILayout.BeginVertical("box");

                DrawProp("OnJoin");
                DrawProp("OnConnect");
                DrawProp("OnDisconnect");
                DrawProp("OnDestroy");

                EditorGUILayout.EndVertical();

            break;

            case 3 : // private
                EditorGUILayout.BeginVertical("box");


                EditorGUILayout.EndVertical();

            break;
        }
    }

    void DrawButton()
    {
        if(GUILayout.Button("URL"))
        {
            Application.OpenURL( "https://www.assetstore.unity3d.com/#!/content/95420" );
        }

        if(GUILayout.Button("Debug/Private"))
        {
            currentTab = 3;
        }

        if(GUILayout.Button("Add2"))
        {
            Debug.Log("Added character");
        }
    }

    void ifyouwanttousefkingICon()
    {
        EditorGUILayout.BeginHorizontal();

		GUILayout.FlexibleSpace();

		GUILayout.Space( 15 );
        
		GUILayout.Label(Icon, GUILayout.MaxWidth( 300 ), GUILayout.MaxHeight( 100 ) , GUILayout.ExpandWidth(true) , GUILayout.ExpandHeight(true));

		GUILayout.FlexibleSpace();

		EditorGUILayout.EndHorizontal();
    }
}
