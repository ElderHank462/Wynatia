using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (NPC_Dialogue))]
public class NPC_Data : MonoBehaviour
{

    [System.Serializable]
    public struct NPCBias{
        // public NPCFaction towards;
        public NPC_Faction towards;
        public int status;

        public NPCBias(NPC_Faction to, int stat){
            towards = to;
            status = stat;
        }

    }
    
    [System.Serializable]
    public struct NPCRelationship{
        public NPC_Data towards;
        public int status;

        public NPCRelationship(NPC_Data to, int stat){
            towards = to;
            status = stat;
        }
    }

    [System.Serializable]
    public struct NPCFaction{
        public NPC_Faction faction;
        public bool visible;
    }

    [System.Serializable]
    public struct FactionAssociation{
        public NPC_Data character;
        public NPC_Faction association;
    }
    
    [Header("Make sure GameObject's tag is set to NPC")]
    public string characterName = "NPC name";

    public bool talkable = true;

    public List<NPCBias> biases;
    public List<NPCRelationship> relationships;
    public List<NPCFaction> factions;

    public FactionAssociation[] knownFactionAssociations;

    public GenericDialogueGroup genericDialogueGroup;


}
