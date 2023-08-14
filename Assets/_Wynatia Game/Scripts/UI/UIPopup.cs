using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class UIPopup : MonoBehaviour
{
    [SerializeField] private Button[] optionButtons;    
    [SerializeField] private TextMeshProUGUI textComponent;


    public void SetOnClick(int index, System.Action function){
        Button b = optionButtons[index];

        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(delegate{function();});
    }

    public void SetOnClick(int index, System.Action function1, System.Action function2){
        Button b = optionButtons[index];

        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(delegate{function1();});
        b.onClick.AddListener(delegate{function2();});
    }

    public void SetOnClick(int index, System.Action function1, System.Action function2, System.Action function3){
        Button b = optionButtons[index];

        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(delegate{function1();});
        b.onClick.AddListener(delegate{function2();});
        b.onClick.AddListener(delegate{function3();});
    }

    public void SetOnClick(int index, System.Action function1, System.Action function2, System.Action function3, System.Action function4){
        Button b = optionButtons[index];

        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(delegate{function1();});
        b.onClick.AddListener(delegate{function2();});
        b.onClick.AddListener(delegate{function3();});
        b.onClick.AddListener(delegate{function4();});
    }

    public void SetButtonText(int index, string textToSet){
        optionButtons[index].GetComponentInChildren<TextMeshProUGUI>().SetText(textToSet);
    }

    public void SetText(string text){
        textComponent.SetText(text);
    }

}
