using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class Rebinder : MonoBehaviour
{
    public InputBinding originalBinding;

    [SerializeField] private Button rebindButton;
    [SerializeField] private TextMeshProUGUI actionLabel;
    [SerializeField] private TextMeshProUGUI bindingText;
    [SerializeField] private Toggle holdToggle;
    [SerializeField] private Slider holdTimeSlider;
    [SerializeField] private TextMeshProUGUI holdTimeText;
    private float holdTime = 0.5f;

    public InputAction action;
    private GameObject rebindPopup;
    private ControlsManager controlsManager;
    public bool partOfComposite = false;

    public int bIndex;

    public void Setup(InputActionMap actionMap, int actionIndex, GameObject popup, ControlsManager cm){
        #region Display setup
        actionLabel.SetText(actionMap.actions[actionIndex].name);
        bindingText.SetText(actionMap.actions[actionIndex].GetBindingDisplayString());

        holdTimeSlider.interactable = false;
        holdTimeText.SetText(holdTime.ToString() + "s");
        #endregion

        SetVariables(actionMap.actions[actionIndex], popup, cm);
    }

    public void Setup(InputActionMap actionMap, int actionIndex, int bindingIndex, GameObject popup, ControlsManager cm){
        #region Display setup
        partOfComposite = true;

        holdToggle.gameObject.SetActive(false);
        holdTimeSlider.gameObject.SetActive(false);
        
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
        if(partOfComposite){
            bindingText.SetText(action.bindings[bIndex].ToDisplayString());
        }
        else{
            bindingText.SetText(action.GetBindingDisplayString());
        }
    }

    public void OnHoldToggleChanged(bool enabled){
        holdTimeSlider.interactable = enabled;
        holdTimeText.SetText(holdTime.ToString() + "s");

        if(enabled){
            action.ApplyBindingOverride(new InputBinding{ overrideInteractions = $"hold(duration={holdTime})"});
        }
        else{
            action.ApplyBindingOverride(new InputBinding{ overrideInteractions = "press"});
        }
    }

    public void OnHoldTimeSliderChanged(float value){
        holdTime = value;
        holdTimeText.SetText(holdTime.ToString() + "s");
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
            originalBinding = action.bindings[0];
        }
        else{
            rebindButton.onClick.AddListener(delegate {controlsManager.OnRebind(action, this, bIndex); StartCoroutine(PreventExtraneousRebinding());});
            originalBinding = action.bindings[bIndex];
        }
    }

}
