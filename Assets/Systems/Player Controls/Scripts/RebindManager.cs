using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using TMPro;

public class RebindManager : MonoBehaviour
{
    public InputActionAsset controls;
    public Transform rebindContainer;
    InputActionRebindingExtensions.RebindingOperation rebindOperation;
    public TextMeshProUGUI[] bindingText;
    public GameObject rebindPrefab;

    
    void Start(){
        SetupBindingsMenu();
    }

    public void SetupBindingsMenu(){
        //***Need to understand and use UIElements to do create rebind menu via code :( ***

        // bindingText = new TextMeshProUGUI[controls.actionMaps[0].actions.Count];
        
        // for (int i = 0; i < controls.actionMaps[0].actions.Count; i++)
        // {
        //     //Instantiate prefab
        //     Instantiate(rebindPrefab, rebindContainer);
        //     GameObject newRebindButton = rebindContainer.GetChild(i).gameObject;
        //     if(!newRebindButton){
        //         Debug.LogError("Couldn't identify newly instantiated rebindButton. Halted setup.");
        //         return;
        //     }
        //     // rename prefab to action name
        //     newRebindButton.name = controls.actionMaps[0].actions[i].name;
        //     // add binding texts to bindingText array
        //     if(!newRebindButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>()){
        //         Debug.LogError("Couldn't find bindingText of newly instantiated rebindButton. Halted setup.");
        //         return;
        //     }
        //     bindingText[i] = newRebindButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        //     // set label text to capitalized action name
        //     if(!newRebindButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>()){
        //         Debug.LogError("Couldn't find label text for newly instantiated rebindButton. Halted setup.");
        //         return;
        //     }
        //     newRebindButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = controls.actionMaps[0].actions[i].name.ToUpper();
        //     // add RebindControl(i) event to prefab button
        //     if(newRebindButton.GetComponent<Button>() == null){
        //         Debug.LogError("Couldn't find Button component of newly instantiated rebindButton. Halted setup.");
        //         return;
        //     }
        //     newRebindButton.GetComponent<Button>().clicked += () => RebindControl(i);
        // }
        // // Update all rebind text
        // UpdateAllRebindText();
    }


    public void UpdateAllRebindText(){

        for (int i = 0; i < bindingText.Length; i++)
        {
            if(bindingText[i]){
                bindingText[i].text = controls.actionMaps[0].actions[i].GetBindingDisplayString();
            }
        }
    }

    public void RebindControl(int index){
        var action = controls.actionMaps[0].actions[index];
        
        TextMeshProUGUI text = rebindContainer.Find(action.name).GetChild(0).GetComponent<TextMeshProUGUI>();

        if(text == null){
            return;
        }

        rebindOperation = action.PerformInteractiveRebinding()
            .OnComplete(_ => UpdateText(text, action))
            .Start();
    }

    public void UpdateText(TextMeshProUGUI text, InputAction action){
        rebindOperation.Dispose();
        text.SetText(action.GetBindingDisplayString());
    }

}
