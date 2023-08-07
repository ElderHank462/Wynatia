using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToNPC_DialogueBox : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void Hide(){
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public int ShowForDuration(string textToSet){
        transform.localPosition = new Vector3(0, 1.75f, 0);

        SetText(textToSet);

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }

        return textToSet.Length;
    }

    public void SetText(string dialogue){
        text.SetText(dialogue);
    }
}
