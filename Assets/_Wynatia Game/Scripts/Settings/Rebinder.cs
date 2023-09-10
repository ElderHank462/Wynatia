using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class Rebinder : MonoBehaviour
{
    public string originalBinding;

    [SerializeField] private Button rebindButton;
    [SerializeField] private TextMeshProUGUI actionLabel;
    [SerializeField] private TextMeshProUGUI bindingText;
    // public bool holdEnabled = false;
    public float holdTime = 0.5f;

    public InputAction action;
    private GameObject rebindPopup;
    private ControlsManager controlsManager;
    public bool partOfComposite = false;
    public int bIndex;



    public void Setup(InputActionMap actionMap, int actionIndex, GameObject popup, ControlsManager cm){
        #region Display setup
        actionLabel.SetText(actionMap.actions[actionIndex].name);
        bindingText.SetText(actionMap.actions[actionIndex].GetBindingDisplayString());
        #endregion

        SetVariables(actionMap.actions[actionIndex], popup, cm);
    }

    public void Setup(InputActionMap actionMap, int actionIndex, int bindingIndex, GameObject popup, ControlsManager cm){
        #region Display setup
        partOfComposite = true;
        
        string compositePartString;
        if(bindingIndex == 1){
            //Up
            compositePartString = "Up";
        }
        else if(bindingIndex == 2){
            //Down
            compositePartString = "Down";
        }
        else if(bindingIndex == 3){
            // Left
            compositePartString = "Left";
        }
        else{
            // Right
            compositePartString = "Right";
        }
        
        actionLabel.SetText(actionMap.actions[actionIndex].name + " (" + compositePartString + ")");
        bindingText.SetText(actionMap.actions[actionIndex].bindings[bindingIndex].ToDisplayString());
        #endregion

        bIndex = bindingIndex;

        SetVariables(actionMap.actions[actionIndex], popup, cm);
    }

    public void UpdateText(){
        string oldText = bindingText.text;
        
        if(partOfComposite){
            bindingText.SetText(action.bindings[bIndex].ToDisplayString());
        }
        else{
            bindingText.SetText(action.GetBindingDisplayString());
        }

        if(oldText == bindingText.text){
            controlsManager.FlagSaS();
        }
    }

    public void UpdateOriginalBinding(){
        if(partOfComposite){
            // May need to be fed the action from the PlayerInput component
            originalBinding = action.bindings[bIndex].effectivePath;
        }
        else{
            originalBinding = action.bindings[0].effectivePath;
        }
    }


    public void RecallInitialState(){
        // if(PlayerPrefs.HasKey(action.name + "_holdEnabled")){
        //     if(PlayerPrefs.GetInt(action.name + "_holdEnabled") == 1){
        //         holdEnabledOnInitialize = true;
        //     }
        //     else{
        //         holdEnabledOnInitialize = false;
        //     }
        // }
        // holdTimeSlider.interactable = holdEnabledOnInitialize;
        // holdToggle.isOn = holdEnabledOnInitialize;
    }

    IEnumerator PreventExtraneousRebinding(){
        rebindButton.interactable = false;
        yield return new WaitForSecondsRealtime(0.1f);
        rebindButton.interactable = true;
    }

    void SetVariables(InputAction a, GameObject p, ControlsManager c){
        action = a;
        rebindPopup = p;
        controlsManager = c;
        
        if(!partOfComposite){
            rebindButton.onClick.AddListener(delegate {controlsManager.OnRebind(action, this); StartCoroutine(PreventExtraneousRebinding());});
            originalBinding = action.bindings[0].effectivePath;
        }
        else{
            rebindButton.onClick.AddListener(delegate {controlsManager.OnRebind(action, this, bIndex); StartCoroutine(PreventExtraneousRebinding());});
            originalBinding = action.bindings[bIndex].effectivePath;
        }
    }


}
