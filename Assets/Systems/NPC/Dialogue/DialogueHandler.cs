using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DialogueHandler
{
    static NPC_Data initiator;
    static NPC_Data responder;
    static DialogueMap.Line iLine;
    static DialogueMap.Line rLine;
    static DialogueMap.Line[] rResponses;

    
    static NPC_Data.Interest rInterest;




    //(note to remember where I was)
    //I want to call AnalyzeContent from DialogueMap and have the function return the NPC_DialogueLine that
    //DialogueMap should respond with
    public static DialogueMap.Line AnalyzeContent(DialogueMap.Line dialogueLine, DialogueMap.Line[] io, DialogueMap.Line[] rr){
        // Setup
        initiator = null;
        responder = null;
        iLine = new DialogueMap.Line();
        rLine = new DialogueMap.Line();
        rResponses = null;
        
        iLine = dialogueLine;
        initiator = dialogueLine.speaker;
        responder = rr[0].speaker;
        rResponses = rr;

        DialogueMap.Line responseLine = new DialogueMap.Line();

        NPC_DialogueLine.ContentItem content;
        for (int i = 0; i < dialogueLine.content.Length; i++)
        {
            content = dialogueLine.content[i];
            
            if(content.type == "offer"){
                responseLine = AnalyzeOffer(content);
            }
        }

        return responseLine;
    }

    static DialogueMap.Line AnalyzeOffer(NPC_DialogueLine.ContentItem content){
        //Analyzes OFFER CONTENT
        
        DialogueMap.Line line = new DialogueMap.Line();
        if(content.subtype == "gold"){
            line = AnalyzeGoldOffer(content);
        }
        return line;
    }

    static DialogueMap.Line AnalyzeGoldOffer(NPC_DialogueLine.ContentItem content){
        // converts OFFER CONTENT to INTEREST
       
        NPC_Data.Interest goldInterest = new NPC_Data.Interest();
        DialogueMap.Line line = new DialogueMap.Line();
        
        List<NPC_Data.Interest> matchingInterests = new List<NPC_Data.Interest>();
        for (int i = 0; i < responder.interests.Count; i++)
        {
            Debug.Log("Checking interest " + i + " of NPC " + responder.characterName);
            if(responder.interests[i].subtype == "gold"){
                Debug.Log("Found interest in gold. Interest.specifics: " + responder.interests[i].specifics);
                matchingInterests.Add(responder.interests[i]);
                
                // break;
                // ^ This will be removed with support for multiple interests
                // For now the function will just use the first matching interest it finds
            }
            else if(i == responder.interests.Count){
                // Refuse offer
                // or maybe use a default response?
                Debug.Log("NPC does not have an interest in an offer with subtype 'gold.'");
                line = SearchForLineWithContent(rResponses, new NPC_DialogueLine.ContentItem("offer", "gold", "refuse"));
                return line;
            }
        }

        int contentGoldAmount = int.Parse(content.specifics);
        // int interestGoldAmount;

        foreach (var item in matchingInterests)
        {
            // interestGoldAmount = int.Parse(item.specifics);
            
            if(item.specifics != "n-a" && int.Parse(item.specifics) <= contentGoldAmount){
                if(goldInterest.specifics == null){
                    goldInterest = item;
                }
                else if(int.Parse(item.specifics) >= int.Parse(goldInterest.specifics)){
                    goldInterest = item;
                }
            }
            else if(item.specifics == "n-a" && goldInterest.specifics == null){
                goldInterest = item;
            }
        }

        Debug.Log("goldInterest: " + goldInterest.specifics);

        rInterest = goldInterest;
        
        if(content.specifics != "n-a" && goldInterest.specifics != "n-a"){

            contentGoldAmount = int.Parse(content.specifics);
            int interestGoldAmount = int.Parse(goldInterest.specifics);

            if(contentGoldAmount >= interestGoldAmount){
                // Accept offer
                Debug.Log("NPC accepts offer of " + contentGoldAmount + " gold.");
                // Gets the responder's acceptance line
                line = SearchForLineWithContent(rResponses, content);
            }
            else{
                // Refuse offer
                Debug.Log("NPC is not interested in a gold offer of " + contentGoldAmount + "; they want at least " + interestGoldAmount + " gold.");
                // or maybe use a default response?
                line = SearchForLineWithContent(rResponses, new NPC_DialogueLine.ContentItem("offer", "gold", "refuse"));
            }
        }
        else if(content.specifics != "n-a"){
            //Implement support for multiple different interests with varying amounts of gold
            contentGoldAmount = int.Parse(content.specifics);
            Debug.Log("NPC accepts offer of " + contentGoldAmount + " gold.");
            line = SearchForLineWithContent(rResponses, content);
        }
        else{
            Debug.LogError("The dialogue line offering gold must have a value assigned; the value cannot be 'n-a.'");
        }
        
        return line;
    }

    static DialogueMap.Line SearchForLineWithContent(DialogueMap.Line[] dialogueArray, NPC_DialogueLine.ContentItem contentLookingFor){
        List<DialogueMap.Line> specificLines = new List<DialogueMap.Line>();
        List<DialogueMap.Line> generalLines = new List<DialogueMap.Line>();

        bool endLoop = false;

        foreach (var line in dialogueArray)
        {
            if(!endLoop){
                foreach (var lineContent in line.content)
                {
                    Debug.Log("Currently checking if line has matching content. Looking for content of type: " + contentLookingFor.type
                    + "; Subtype: " + contentLookingFor.subtype + "; Specifics: " + contentLookingFor.specifics + ". Current lineContentItem has type: "
                     + lineContent.type + "; Subtype: " + lineContent.subtype + "; Specifics: " + lineContent.specifics + ".");

                    if(lineContent.type == contentLookingFor.type && lineContent.subtype == contentLookingFor.subtype && lineContent.specifics == contentLookingFor.specifics){
                        specificLines.Add(line);
                        endLoop = true;
                        break;
                    }
                    else if(lineContent.type == contentLookingFor.type && lineContent.subtype == contentLookingFor.subtype){
                        if(lineContent.type == "offer" && lineContent.subtype == "gold"){
                            // Debug.Log("");
                            if(contentLookingFor.specifics == "refuse"){
                                if(lineContent.specifics == "refuse"){
                                    specificLines.Add(line);
                                    endLoop = true;
                                    break;
                                }
                                else{
                                    break;
                                }
                            }
                            else if(lineContent.specifics == "refuse"){
                                break;
                            }
                            
                            if(lineContent.specifics != "n-a"){
                                int offerAmt = int.Parse(contentLookingFor.specifics);
                                int lineAmt = int.Parse(lineContent.specifics);
                                if(offerAmt >= lineAmt){
                                    specificLines.Add(line);
                                    break;
                                }
                            }
                            else{
                                generalLines.Add(line);
                                break;
                            }
                        }
                    }
                }
            }
            else{
                break;
            }
        }

        if(specificLines.Count > 0){
            return specificLines[Random.Range(0, specificLines.Count)];
        }
        else if(generalLines.Count > 0){
            return generalLines[Random.Range(0, generalLines.Count)];
        }
        else{
            // No dialogue lines found for this NPC that contain the matching content
            // Use a fallback line instead (defined in the line with the offer?)
            if(contentLookingFor.specifics == "refuse"){
                // Use interest refuse line
                return new DialogueMap.Line(dialogueArray[0].speaker, rInterest.refuseLine.content, rInterest.refuseLine.dialogue);
            }
            else{
                // Use interest accept line
                return new DialogueMap.Line(dialogueArray[0].speaker, rInterest.acceptLine.content, rInterest.acceptLine.dialogue);
            }
            
        }

    }


}
