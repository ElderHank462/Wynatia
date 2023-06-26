using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NPC/Unique Dialogue Group")]
public class UniqueDialogueGroup : ScriptableObject
{
    public NPC_ID I_towards;
    public NPC_Faction F_towards;
    
    public DialogueSet dneg3;
    public DialogueSet dneg2;
    public DialogueSet dneg1;
    public DialogueSet d0;
    public DialogueSet dpos1;
    public DialogueSet dpos2;
    public DialogueSet dpos3;
}
