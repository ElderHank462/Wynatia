using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextAssigner : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] textObjects;

    public void SetText(int index, string text){
        if(index == -1){
            foreach (var textObject in textObjects)
            {
                textObject.SetText(text);
            }
        }
        else if(index >= 0 && index < textObjects.Length){
            textObjects[index].SetText(text);
        }
        else{
            Debug.LogError("The index supplied was outside the range of the textObjects array.");
        }
    }

    public void SetColor(int index, Color color){
        if(index == -1){
            foreach (var textObject in textObjects)
            {
                textObject.color = color;
            }
        }
        else if(index >= 0 && index < textObjects.Length){
            textObjects[index].color = color;
        }
        else{
            Debug.LogError("The index supplied was outside the range of the textObjects array.");
        }
    }
}
