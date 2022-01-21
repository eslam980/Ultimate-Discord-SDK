# Ultimate Discord SDK

This is A Discord SDK for Unity
![image](https://user-images.githubusercontent.com/50788147/149820675-d0170829-f4cf-415b-9452-7bf6ff018f33.png)

## Features
- Easy To Use.
- Join/Request System,
- Full Unity Integration
- Modular Code (Easy To Understand)
- Safe Mode.
- Custom Inspector.
- PUN 2 / Mirror Integration.

[![Discord-Levlinvite.png](https://i.postimg.cc/43FrqQQM/Discord-Levlinvite.png)](https://postimg.cc/3ymSDp6C)

## Usage
- Ultimate Discord uses **Dll/Offical Discord C# SDK**
- Create C# Script or Use The Template
- Use NameSpace ```UDiscord``` to Access The Code

## How To Get Started
- Put **Plugins** File into to your Unity Project.
- Make Script or Use The Template.
- Use ```Using UDiscord``` to Access All The Fuctions

## Examples
- Button : Add Button Into The Scene And Make A Script
- Add ```Discord Manager``` into The Scene , Assign **AppID**
- Code Example :
```csharp

using UnityEngine;
using UDiscord; // Use This To Get Discord Manager

public class ExampleButton : MonoBehaviour
{
    public void OnClick()
    {
        // Calling DiscordManager.App.UpdateRich : To Update Dicsord Rich But Will Change detail , state Only 
        // Only Things That you put it in UpdateRich , Will Change , Others will stay the Same.
        
        DiscordManager.App.UpdateRich(detail : "Playing Solo" , state : "Private Lobby");
    }
}

```
[![Discord-Image.png](https://i.postimg.cc/0j1H1WMt/Discord-Image.png)](https://postimg.cc/bDLRQRRb)

- Other Example Is A **OnTriggerEnter()**  To Change Rich will entering Another Area
- Code Example :
```csharp

using UnityEngine;
using UDiscord; // Use This To Get Discord Manager

public class ExampleTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //When AnyThing enter Area
        DiscordManager.App.UpdateRich(detail : "Playing Solo" , state : "Desert");

        //other Way
        if(other.gameObject.CompareTag("Player"))
        {
            DiscordManager.App.UpdateRich(detail : "Playing Solo" , state : "Sky");
        }
    }
}

```

## Discord Manager
This is The Manager that handle all the Work to Connect with discord

## Discord_Start
This Boolan to make the Template Work on Start here is the code **Code** for it ```OnConnect.AddListener(CallDiscord); OnConnect?.Invoke();```
