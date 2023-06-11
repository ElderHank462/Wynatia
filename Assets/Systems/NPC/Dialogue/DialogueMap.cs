using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueMap : MonoBehaviour
{
    public NPC_Data npcA;
    public NPC_Data npcB;

    public List<Line> dialoguePath = new List<Line>();

    Line[] npcA_OpeningLines;
    Line[] npcB_OpeningLines;
    Line[] npcA_Lines;
    Line[] npcB_Lines;

    GameObject dialoguePlayerPrefab;

 
    [System.Serializable]
    public struct Line{
        public NPC_Data speaker;
        public NPC_DialogueLine.ContentItem[] content;
        public string dialogueText;

        public Line(NPC_Data spe, NPC_DialogueLine.ContentItem[] con, string dialogue){
            speaker = spe;
            content = con;
            dialogueText = dialogue;
        }
    }

    public void Setup(NPC_Data a, NPC_Data b, NPC_DialogueLine[] aOpenLines, NPC_DialogueLine[] bOpenLines, NPC_DialogueLine[] aLines, NPC_DialogueLine[] bLines){
        // NPC A is the one who initiates the conversation
        npcA = a;
        npcB = b;

        dialoguePlayerPrefab = npcA.GetComponent<NPC_Dialogue>().dialoguePlayer;
        
        npcA_OpeningLines = new Line[aOpenLines.Length];
        for (int i = 0; i < aOpenLines.Length; i++)
        {
            npcA_OpeningLines[i].speaker = npcA;
            npcA_OpeningLines[i].content = aOpenLines[i].content;
            npcA_OpeningLines[i].dialogueText = aOpenLines[i].dialogue;
        }
        
        npcB_OpeningLines = new Line[bOpenLines.Length];
        for (int i = 0; i < bOpenLines.Length; i++)
        {
            npcB_OpeningLines[i].speaker = npcB;
            npcB_OpeningLines[i].content = bOpenLines[i].content;
            npcB_OpeningLines[i].dialogueText = bOpenLines[i].dialogue;
        }


        npcA_Lines = new Line[aLines.Length];
        for (int i = 0; i < aLines.Length; i++)
        {
            npcA_Lines[i].speaker = npcA;
            npcA_Lines[i].content = aLines[i].content;
            npcA_Lines[i].dialogueText = aLines[i].dialogue;
        }
        
        npcB_Lines = new Line[bLines.Length];
        for (int i = 0; i < bLines.Length; i++)
        {
            npcB_Lines[i].speaker = npcB;
            npcB_Lines[i].content = bLines[i].content;
            npcB_Lines[i].dialogueText = bLines[i].dialogue;
        }

        A_SelectRandomOpener();
    }

    void A_SelectRandomOpener(){
        int i = Random.Range(0, npcA_OpeningLines.Length);
        dialoguePath = new List<Line>();
        dialoguePath.Add(npcA_OpeningLines[i]);
        
        if(CheckIfGreetingOnly(npcA_OpeningLines[i].content)){
            B_SelectRandomOpener();
        }
        else{
            Debug.Log("Dialogue line contains content content other than a greeting.");
        }
    }

    void B_SelectRandomOpener(){
        int i = Random.Range(0, npcB_OpeningLines.Length);
        dialoguePath.Add(npcB_OpeningLines[i]);
        
        if(CheckIfGreetingOnly(npcB_OpeningLines[i].content)){
            //Both A and B's opening lines are greeting only, start dialogue with the now complete dialogue path
            Debug.Log("Dialogue path complete: NPCs will greet each other then end dialogue.");
            StartDialogue();
        }
  
    }

    bool CheckIfGreetingOnly(NPC_DialogueLine.ContentItem[] content){
        bool greetingOnly = false;
        foreach (var contentItem in content)
        {
            if(contentItem.type == "greeting"){
                greetingOnly = true; 
            }
            else{ greetingOnly = false; break; }
        }
        return greetingOnly;
    }

    void StartDialogue(){
        GameObject instObject = Instantiate(dialoguePlayerPrefab);
        instObject.name = "DialoguePlayer";
        instObject.GetComponent<DialoguePlayer>().Setup(dialoguePath.ToArray(), npcA.GetComponent<NPC_Dialogue>(), npcB.GetComponent<NPC_Dialogue>());
        Destroy(gameObject);
    }
}
