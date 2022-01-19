using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDiscord; // Use This To Get Discord Manager
public class ExampleButton : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        DiscordManager.App.UpdateRich(detail : "Playing Solo" , state : "Desert");

        //other Way
        if(other.gameObject.CompareTag("Player"))
        {
            DiscordManager.App.UpdateRich(detail : "Playing Solo" , state : "Sky");
        }
    }
}
