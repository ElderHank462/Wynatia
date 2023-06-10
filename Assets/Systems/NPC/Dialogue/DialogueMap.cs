using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueMap : MonoBehaviour
{
    public NPC_Data npcA;
    public NPC_Data npcB;

    public Line[] dialoguePath;

    Line[] npcA_Lines;
    Line[] npcB_Lines;

    
    public DialogueMap(NPC_Data a, NPC_Data b, NPC_DialogueLine[] aLines, NPC_DialogueLine[] bLines){
        // NPC A is the one who initiates the conversation
        npcA = a;
        npcB = b;
        
        npcA_Lines = new Line[aLines.Length];
        for (int i = 0; i < aLines.Length; i++)
        {
            npcA_Lines[i].speaker = npcA;
            npcA_Lines[i].content = aLines[i].content;
        }
        
        npcB_Lines = new Line[bLines.Length];
        for (int i = 0; i < bLines.Length; i++)
        {
            npcB_Lines[i].speaker = npcB;
            npcB_Lines[i].content = bLines[i].content;
        }
    }

    void Awake(){
        //Select opening line
        // If opening line only contains greeting, npcB responds with any of their opening lines
        // 
    }

    public struct Line{
        public NPC_Data speaker;
        public int indexInSet;
        public NPC_DialogueLine.ContentItem[] content;

        public Line(NPC_Data spe, int ind, NPC_DialogueLine.ContentItem[] con){
            speaker = spe;
            indexInSet = ind;
            content = con;
        }
    }
}
