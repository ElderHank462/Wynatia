using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToNPC_DialogueBox : MonoBehaviour
{
    public TextMeshProUGUI textAsset;

    Vector3 bottomLeft;
    Vector3 bottomRight;
    Vector3 topLeft;
    Vector3 topRight;

    TMP_CharacterInfo firstChar;
    TMP_CharacterInfo lastChar;
    
    // Start is called before the first frame update
    void Start()
    {
        // textAsset = GetComponent<TextMeshProUGUI>();
    }

    public void FillText(){

    }

    // // Update is called once per frame
    // void Update()
    // {


    //     firstChar = textAsset.textInfo.characterInfo[textAsset.textInfo.wordInfo[0].firstCharacterIndex];
    //     lastChar = textAsset.textInfo.characterInfo[textAsset.textInfo.wordInfo[0].lastCharacterIndex];
        
    //     float topEdge = (firstChar.fontAsset.faceInfo.capLine * firstChar.scale) + firstChar.baseLine;

    //     bottomLeft = transform.TransformPoint(new Vector3(firstChar.origin, firstChar.baseLine, 0));
    //     bottomRight = transform.TransformPoint(new Vector3(lastChar.xAdvance, lastChar.baseLine, 0));
    //     topLeft = transform.TransformPoint(new Vector3(firstChar.origin, topEdge, 0));
    //     topRight = transform.TransformPoint(new Vector3(lastChar.xAdvance, topEdge, 0));
    // }
}
