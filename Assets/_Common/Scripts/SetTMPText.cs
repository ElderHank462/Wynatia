using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetTMPText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;

    public void SetText(string text){
        textComponent.SetText(text);
    }
}
