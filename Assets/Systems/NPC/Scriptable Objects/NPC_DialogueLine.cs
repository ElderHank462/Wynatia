using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NPC/Dialogue Line")]
public class NPC_DialogueLine : ScriptableObject
{
    public string dialogue;
    public string[] content;
}
