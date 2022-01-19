using UnityEngine;
using UnityEditor;
using UDiscord;

[CustomEditor(typeof(DiscordManager))]
public class DiscordOpenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Open Discord Editor"))
        {
            DiscordEditor.Open((DiscordManager)target);
        }
    }
}
