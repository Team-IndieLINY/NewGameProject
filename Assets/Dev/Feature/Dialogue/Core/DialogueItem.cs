using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class DialogueItem
{
    public string Text { get; private set; }
    public bool IsMaster { get; private set; }
    public string PoseKey { get; private set; }
    public string CharacterName { get; private set; }

    public static readonly DialogueItem Default = new DialogueItem(
        "Default_Text",
        false,
        "Default_Name",
        "Default_PoseKey"
    );

    public DialogueItem(string text, bool isMaster, string characterName, string poseKey="")
    {
        Text = text;
        IsMaster = isMaster;
        PoseKey = poseKey;
        CharacterName = characterName;
    }
}
