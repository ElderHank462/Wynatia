using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguePlayer : MonoBehaviour
{
    public string[] strings;
    ToNPC_DialogueBox aBox;
    ToNPC_DialogueBox bBox;

    public void Setup(DialogueMap.Line[] lineStructs, NPC_Dialogue npcA, NPC_Dialogue npcB){
        strings = new string[lineStructs.Length];
        for (int i = 0; i < lineStructs.Length; i++)
        {
            strings[i] = lineStructs[i].dialogueText;
        }

        aBox = npcA.CreateDialogueBox();
        bBox = npcB.CreateDialogueBox();

        aBox.Hide();
        bBox.Hide();

        StartCoroutine(RunDialogue());
    }
    public IEnumerator RunDialogue(){
        for (int i = 0; i < strings.Length; i++)
        {
            string st = strings[i];
            float charCount;
            
            if(i%2==0){
                //Call function from npcA
                charCount = aBox.ShowForDuration(strings[i]);
            }
            else{
                //Call function from npcB
                charCount = bBox.ShowForDuration(strings[i]);
            }

            if(charCount < 10){
                charCount = 10;
            }
            if(charCount > 50){
                charCount = 50;
            }

            yield return new WaitForSeconds(charCount * 0.1f);
            aBox.Hide();
            bBox.Hide();
        }

        
        Destroy(aBox.gameObject);
        Destroy(bBox.gameObject);
        Destroy(gameObject);
    }

}
