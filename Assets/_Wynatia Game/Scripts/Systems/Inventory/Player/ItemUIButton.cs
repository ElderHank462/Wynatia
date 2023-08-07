using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemUIButton : MonoBehaviour
{
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
}
