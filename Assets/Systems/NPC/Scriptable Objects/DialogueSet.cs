using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NPC/Dialogue Set")]
public class DialogueSet : ScriptableObject
{
    public NPC_DialogueLine[] openingLines;

    public NPC_DialogueLine[] otherLines;
}
