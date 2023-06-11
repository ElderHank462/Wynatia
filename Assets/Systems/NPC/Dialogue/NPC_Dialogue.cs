using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (NPC_Data))]
public class NPC_Dialogue : MonoBehaviour
{
    NPC_Data data;

    private NPC_Data.NPCRelationship mostExtremeStatus;

    NPC_Data.NPCRelationship myMostExtremeRelationship;
    NPC_Data.NPCRelationship otherMostExtremeRelationship;
    NPC_Data.NPCBias myMostExtremeBias;
    NPC_Data.NPCBias otherMostExtremeBias;
    
    public GameObject dialoguePlayer;
    public GameObject npcDialogueBox;

    void Awake(){
        data = GetComponent<NPC_Data>();
    }

    private void OnTriggerEnter(Collider other){
        //When NPC comes near another NPC, get reference to other NPC
        if(other.CompareTag("NPC")){
            if(other.GetComponent<NPC_Data>().talkable){
                Debug.Log("Came into range of interactable NPC.");
                //Decide which NPC is calling the code
                if(transform.GetInstanceID() > other.GetInstanceID()){
                    Debug.Log(data.name + " is checking the relations between these two NPCs: " + data.name + " and " + other.GetComponent<NPC_Data>().characterName);
                    CheckOthersRelations(other.GetComponent<NPC_Dialogue>());
                }
            }
        }
    }
    
    void CheckOthersRelations(NPC_Dialogue other){
        NPC_Data.NPCRelationship relationship;
        mostExtremeStatus = new NPC_Data.NPCRelationship(null, 0);

        // Check if OTHER npc has a non-zero relationship towards me
        for (int i = -3; i <= 3; i++)
        {
            relationship = new NPC_Data.NPCRelationship(data, i);
            // Debug.Log("Checking to see if the other NPC has a relationship towards: " + gameObject.name + " of status " + i);
            if(i != 0 && other.data.relationships.Contains(relationship)){
                // Debug.Log("NPC " + other.gameObject.name + " has a relationship with " + gameObject.name + " of status " + i);
                mostExtremeStatus = relationship;
                otherMostExtremeRelationship = relationship;
                CheckMyRelations(other.data);
                return;
            }
        }

        //If for loop hasn't returned out, check whether I have any relationships with this NPC
        CheckMyRelations(other.data);
        
    }

    void CheckMyRelations(NPC_Data other){
        NPC_Data.NPCRelationship relationship;

        for (int i = -3; i <= 3; i++)
        {
            relationship = new NPC_Data.NPCRelationship(other, i);
            // Debug.Log("Checking to see if I have a relationship towards: " + other.gameObject.name + " of status " + i);
            if(i != 0 && data.relationships.Contains(relationship))
            {
                // Debug.Log("NPC " + gameObject.name + " has a relationship with " + other.gameObject.name + " of status " + i);
                if(Mathf.Abs(i) > Mathf.Abs(mostExtremeStatus.status))
                    mostExtremeStatus = relationship;

                myMostExtremeRelationship = relationship;
                DecideInitiator(other);
                return;
            }
            else if(i == 0 && data.relationships.Contains(relationship)){
                myMostExtremeRelationship = relationship;
            }
        }
    
        if(Mathf.Abs(mostExtremeStatus.status) > 0){
            DecideInitiator(other);
            return;
        }

        //If neither NPC has a relationship with the other, check biases
        Debug.Log("No significant relationships found; checking biases.");
        CheckBias(other.GetComponent<NPC_Dialogue>());
    }
    
    void CheckBias(NPC_Dialogue other){
        NPC_Data.NPCBias myActiveBias = new NPC_Data.NPCBias(null, 0);
        NPC_Data.NPCBias otherActiveBias = new NPC_Data.NPCBias(null, 0);
        
        List<NPC_Faction> myFactions = new List<NPC_Faction>();
        List<NPC_Faction> otherFactions = new List<NPC_Faction>();
        
        List<NPC_Faction> myBiases = new List<NPC_Faction>();
        List<NPC_Faction> otherBiases = new List<NPC_Faction>();

        foreach (NPC_Data.NPCFaction item in data.factions)
        {
            myFactions.Add(item.faction);
        }

        foreach (NPC_Data.NPCFaction item in other.data.factions)
        {
            otherFactions.Add(item.faction);
        }
        
        foreach (NPC_Data.NPCBias item in data.biases)
        {
            myBiases.Add(item.towards);
        }

        foreach (NPC_Data.NPCBias item in other.data.biases)
        {
            otherBiases.Add(item.towards);
        }

        // Debug.Log(data.characterName + "'s factions: " + myFactions.Count);
        // Debug.Log(other.data.characterName + "'s factions: " + otherFactions.Count);
        // Debug.Log(data.characterName + "'s biases: " + myBiases.Count);
        // Debug.Log(other.data.characterName + "'s biases: " + otherBiases.Count);
        
        bool factionTrump = false;

        //Do either of us have biases towards each other w/abs greater than 1?
        foreach (NPC_Data.NPCBias item in data.biases)
        {
            //Replicate this mess in the other foreach loop below :)
            if(!factionTrump)
            {
                if(otherFactions.Contains(item.towards) && Mathf.Abs(item.status) > 1 && Mathf.Abs(item.status) > Mathf.Abs(myActiveBias.status)){
                    foreach (NPC_Data.NPCFaction factionStruct in other.data.factions)
                    {
                        if(factionStruct.faction == item.towards){
                            if(!factionStruct.visible){
                                if(item.towards.membersRecognize){
                                    myActiveBias = item;
                                    factionTrump = true;
                                    break;
                                }
                            }
                            else{
                                myActiveBias = item;
                            }
                        }
                    }
                }
            }
        }
        
        bool otherFactionTrump = false;
        foreach (NPC_Data.NPCBias item in other.data.biases)
        {
            // if(myFactions.Contains(item.towards) && Mathf.Abs(item.status) > 1 && Mathf.Abs(item.status) > Mathf.Abs(otherActiveBias.status)){
            //     otherActiveBias = item;
            // }

            if(!otherFactionTrump)
            {
                if(myFactions.Contains(item.towards) && Mathf.Abs(item.status) > 1 && Mathf.Abs(item.status) > Mathf.Abs(otherActiveBias.status)){
                    foreach (NPC_Data.NPCFaction factionStruct in data.factions)
                    {
                        if(factionStruct.faction == item.towards){
                            if(!factionStruct.visible){

                                if(item.towards.membersRecognize){
                                    otherActiveBias = item;
                                    otherFactionTrump = true;
                                    break;
                                }
                            }
                            else{
                                otherActiveBias = item;
                            }

                        }
                    }
                }
            }
        }
        
        myMostExtremeBias = myActiveBias;
        otherMostExtremeBias = otherActiveBias;

        if(Mathf.Abs(myActiveBias.status) >= Mathf.Abs(otherActiveBias.status) && myActiveBias.status != 0){
            // InitiateDialogue(data);
            SelectDialogueSet(myActiveBias.status, other.data, myActiveBias.towards);
        }
        else if(Mathf.Abs(myActiveBias.status) <= Mathf.Abs(otherActiveBias.status) && Mathf.Abs(otherActiveBias.status) != 0){
            other.SelectDialogueSet(otherActiveBias.status, data, otherActiveBias.towards);
        }
        else{
            Debug.Log("NPCs had no relations or biases towards each other; no dialogue was initiated.");
        }
    }

    void DecideInitiator(NPC_Data other){

        
        if(mostExtremeStatus.towards != data){
            // I initiate
            // InitiateDialogue(data);
            SelectDialogueSet(mostExtremeStatus.status, otherMostExtremeRelationship, other);
            // OtherNPCsSet();
        }
        else{
            // Other initiates
            // other.GetComponent<NPC_Dialogue>().InitiateDialogue(other);
            other.GetComponent<NPC_Dialogue>().SelectDialogueSet(mostExtremeStatus.status, myMostExtremeRelationship, data);
        }
    }

//INDIVIDUAL overload
    public void SelectDialogueSet(int status, NPC_Data.NPCRelationship otherRelationship, NPC_Data individual){
        //This function is the first in the chain of events that is called on the 
        //character INITIATING dialogue as opposed to the character with the higher
        //INSTANCE ID (which could coincidentally be the same character)
        
        otherMostExtremeRelationship = otherRelationship;

        DialogueSet selectedSet = null;

        //Check if there is a unique dialogue group for this individual
        foreach (var uniqueDialogueGroup in data.uniqueDialogueGroups)
        {
            if(uniqueDialogueGroup.I_towards == individual){
                if(status == -3)
                    selectedSet = uniqueDialogueGroup.dneg3;
                if(status == -2)
                    selectedSet = uniqueDialogueGroup.dneg2;
                if(status == -1)
                    selectedSet = uniqueDialogueGroup.dneg1;
                if(status == 0)
                    selectedSet = uniqueDialogueGroup.d0;
                if(status == 1)
                    selectedSet = uniqueDialogueGroup.dpos1;
                if(status == 2)
                    selectedSet = uniqueDialogueGroup.dpos2;
                if(status == 3)
                    selectedSet = uniqueDialogueGroup.dpos3;
                
                InitiateDialogue(individual, selectedSet, OtherNPCsSet(otherMostExtremeRelationship, data));
                return;
            }
        }

        //If there is, select a dialogue set of the appropriate disposition from the unique set
        //Otherwise, use the generic set
        if(status == -3)
            selectedSet = data.genericDialogueGroup.dneg3;
        if(status == -2)
            selectedSet = data.genericDialogueGroup.dneg2;
        if(status == -1)
            selectedSet = data.genericDialogueGroup.dneg1;
        if(status == 0)
            selectedSet = data.genericDialogueGroup.d0;
        if(status == 1)
            selectedSet = data.genericDialogueGroup.dpos1;
        if(status == 2)
            selectedSet = data.genericDialogueGroup.dpos2;
        if(status == 3)
            selectedSet = data.genericDialogueGroup.dpos3;
        //  ^  Initiate dialogue with selected dialogue set
        
        InitiateDialogue(individual, selectedSet, OtherNPCsSet(otherMostExtremeRelationship, data));
    }
//FACTION overload
    public void SelectDialogueSet(int status, NPC_Data other, NPC_Faction faction){
        DialogueSet selectedSet = null;

        //Check if there is a unique dialogue group for this faction
        foreach (var uniqueDialogueGroup in data.uniqueDialogueGroups)
        {
            if(uniqueDialogueGroup.F_towards == faction){
                if(status == -3)
                    selectedSet = uniqueDialogueGroup.dneg3;
                if(status == -2)
                    selectedSet = uniqueDialogueGroup.dneg2;
                if(status == -1)
                    selectedSet = uniqueDialogueGroup.dneg1;
                if(status == 0)
                    selectedSet = uniqueDialogueGroup.d0;
                if(status == 1)
                    selectedSet = uniqueDialogueGroup.dpos1;
                if(status == 2)
                    selectedSet = uniqueDialogueGroup.dpos2;
                if(status == 3)
                    selectedSet = uniqueDialogueGroup.dpos3;
                
                InitiateDialogue(other, selectedSet, OtherNPCsSet(otherMostExtremeBias, otherMostExtremeBias.towards));
                return;
            }
        }

        //If there is, select a dialogue set of the appropriate disposition from the unique set
        //Otherwise, use the generic set
        if(status == -3)
            selectedSet = data.genericDialogueGroup.dneg3;
        if(status == -2)
            selectedSet = data.genericDialogueGroup.dneg2;
        if(status == -1)
            selectedSet = data.genericDialogueGroup.dneg1;
        if(status == 0)
            selectedSet = data.genericDialogueGroup.d0;
        if(status == 1)
            selectedSet = data.genericDialogueGroup.dpos1;
        if(status == 2)
            selectedSet = data.genericDialogueGroup.dpos2;
        if(status == 3)
            selectedSet = data.genericDialogueGroup.dpos3;
        //  ^  Initiate dialogue with selected dialogue set
        
        InitiateDialogue(other, selectedSet, OtherNPCsSet(otherMostExtremeBias, otherMostExtremeBias.towards));
    }
//INDIVIDUAL overload
    public DialogueSet OtherNPCsSet(NPC_Data.NPCRelationship relationship, NPC_Data individual){
        

        int status = relationship.status;
        DialogueSet selectedSet = null;

        //Check if there is a unique dialogue group for this individual
        foreach (var uniqueDialogueGroup in data.uniqueDialogueGroups)
        {
            //If there is, select a dialogue set of the appropriate disposition from the unique set
            if(uniqueDialogueGroup.I_towards == individual){
                if(status == -3)
                    selectedSet = uniqueDialogueGroup.dneg3;
                if(status == -2)
                    selectedSet = uniqueDialogueGroup.dneg2;
                if(status == -1)
                    selectedSet = uniqueDialogueGroup.dneg1;
                if(status == 0)
                    selectedSet = uniqueDialogueGroup.d0;
                if(status == 1)
                    selectedSet = uniqueDialogueGroup.dpos1;
                if(status == 2)
                    selectedSet = uniqueDialogueGroup.dpos2;
                if(status == 3)
                    selectedSet = uniqueDialogueGroup.dpos3;
                
                return selectedSet;
            }
        }

        //Otherwise, use the generic set
        if(status == -3)
            selectedSet = data.genericDialogueGroup.dneg3;
        if(status == -2)
            selectedSet = data.genericDialogueGroup.dneg2;
        if(status == -1)
            selectedSet = data.genericDialogueGroup.dneg1;
        if(status == 0)
            selectedSet = data.genericDialogueGroup.d0;
        if(status == 1)
            selectedSet = data.genericDialogueGroup.dpos1;
        if(status == 2)
            selectedSet = data.genericDialogueGroup.dpos2;
        if(status == 3)
            selectedSet = data.genericDialogueGroup.dpos3;
        //  ^  Initiate dialogue with selected dialogue set
        
        return selectedSet;
    }
//FACTION overload
    public DialogueSet OtherNPCsSet(NPC_Data.NPCBias bias, NPC_Faction faction){
        int status = bias.status;
        DialogueSet selectedSet = null;

        //Check if there is a unique dialogue group for this individual
        foreach (var uniqueDialogueGroup in data.uniqueDialogueGroups)
        {
            //If there is, select a dialogue set of the appropriate disposition from the unique set
            if(uniqueDialogueGroup.F_towards == faction){
                if(status == -3)
                    selectedSet = uniqueDialogueGroup.dneg3;
                if(status == -2)
                    selectedSet = uniqueDialogueGroup.dneg2;
                if(status == -1)
                    selectedSet = uniqueDialogueGroup.dneg1;
                if(status == 0)
                    selectedSet = uniqueDialogueGroup.d0;
                if(status == 1)
                    selectedSet = uniqueDialogueGroup.dpos1;
                if(status == 2)
                    selectedSet = uniqueDialogueGroup.dpos2;
                if(status == 3)
                    selectedSet = uniqueDialogueGroup.dpos3;
                
                return selectedSet;
            }
        }

        //Otherwise, use the generic set
        if(status == -3)
            selectedSet = data.genericDialogueGroup.dneg3;
        if(status == -2)
            selectedSet = data.genericDialogueGroup.dneg2;
        if(status == -1)
            selectedSet = data.genericDialogueGroup.dneg1;
        if(status == 0)
            selectedSet = data.genericDialogueGroup.d0;
        if(status == 1)
            selectedSet = data.genericDialogueGroup.dpos1;
        if(status == 2)
            selectedSet = data.genericDialogueGroup.dpos2;
        if(status == 3)
            selectedSet = data.genericDialogueGroup.dpos3;
        //  ^  Initiate dialogue with selected dialogue set
        
        return selectedSet;
    }


    void InitiateDialogue(NPC_Data otherData, DialogueSet dialogueSet, DialogueSet otherDialogueSet){
        mostExtremeStatus = new NPC_Data.NPCRelationship(null, 0);
        myMostExtremeRelationship = new NPC_Data.NPCRelationship(null, 0);
        otherMostExtremeRelationship = new NPC_Data.NPCRelationship(null, 0);
        myMostExtremeBias = new NPC_Data.NPCBias(null, 0);
        otherMostExtremeBias = new NPC_Data.NPCBias(null, 0);

        
        Debug.Log(data.characterName + " initiated dialogue with: " + otherData.characterName + ". " + data.characterName + 
        " is using dialogue set: " + dialogueSet.name + "; " + otherData.characterName + " is using dialogue set: " + otherDialogueSet);

        GameObject instObject = Instantiate((GameObject)Resources.Load("Prefabs/Empty"));
        instObject.name = "DialogueMap";
        instObject.AddComponent<DialogueMap>();
        instObject.GetComponent<DialogueMap>().Setup(data, otherData, dialogueSet.openingLines, 
        otherDialogueSet.openingLines, dialogueSet.otherLines, otherDialogueSet.otherLines);

        // (new DialogueMap(data, otherData, dialogueSet.openingLines, otherDialogueSet.openingLines, dialogueSet.otherLines, otherDialogueSet.otherLines))
    }

    public ToNPC_DialogueBox CreateDialogueBox(){
        GameObject canvas = Instantiate(npcDialogueBox);
        canvas.transform.SetParent(transform);
        return canvas.GetComponent<ToNPC_DialogueBox>();
    }

}
