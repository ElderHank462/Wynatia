using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ItemUIButton : MonoBehaviour
{
    public Toggle equippedToggle;
    public Toggle selectedToggle;
    public Item sObj;
    public GameObject gObj;
    public int count;

    public void Setup(Item s, GameObject g, int c){
        sObj = s;
        gObj = g;
        count = c;

        string text;

        if(count == 1){
            text = sObj.itemName;
        }
        else{
            text = sObj.itemName + " (" + count + ")";
        }

        transform.GetComponentInChildren<TextMeshProUGUI>().SetText(text);
    }

    public void ToggleEquipped(){
        bool currentState = transform.GetComponentInChildren<Toggle>().isOn;
        
        equippedToggle.isOn = !currentState;
    }

    public void ToggleSelected(){
        bool currentState = transform.GetComponentInChildren<Toggle>().isOn;
        
        selectedToggle.isOn = !currentState;
    }

    public void ToggleSelected(bool state){
        selectedToggle.isOn = state;
    }
}
