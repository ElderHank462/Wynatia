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
    public struct Interest{
        // public int interestLevel;
        public string type;
        public string subtype;
        public string specifics;
        public NPC_DialogueLine acceptLine;
        public NPC_DialogueLine refuseLine;

        public Interest(int level, string interestType, string interestSubtype, string interestSpecifics, NPC_DialogueLine accept, NPC_DialogueLine refuse){
            // interestLevel = level;
            type = interestType;
            subtype = interestSubtype;
            specifics = interestSpecifics;
            acceptLine = accept;
            refuseLine = refuse;
        }
    }

    [System.Serializable]
    public struct FactionAssociation{
        public NPC_Data character;
        public NPC_Faction association;
    }
    
    [Header("Make sure GameObject's tag is set to NPC")]
    public string characterName = "NPC name";
    public NPC_ID id;

    public bool talkable = true;

    public List<NPCBias> biases;
    public List<NPCRelationship> relationships;
    public List<NPCFaction> factions;

    public FactionAssociation[] knownFactionAssociations;
    public List<Interest> interests;

    public GenericDialogueGroup genericDialogueGroup;
    public List<UniqueDialogueGroup> uniqueDialogueGroups;


}
