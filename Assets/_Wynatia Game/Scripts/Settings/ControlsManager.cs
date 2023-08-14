using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsManager : MonoBehaviour
{
    public GameObject rebinderPrefab;
    public GameObject rebindPopup;
    public Transform rebinderContainer;
    [SerializeField] GameObject applyButtonCover;
    [SerializeField] GameObject undoButtonCover;

    private PlayerInput playerInputComponent;
    private PauseMenu pauseMenu;
    [SerializeField] int numberOfActionsToExclude = 0;
    
    // called whenever script's gameobject is enabled
    void OnEnable()
    {
        playerInputComponent = FindObjectOfType<PlayerInput>();
        pauseMenu = FindObjectOfType<PauseMenu>();

        applyButtonCover.SetActive(true);
        undoButtonCover.SetActive(true);
        
        // (if bindings don't automatically save)
        // Load saved bindings from playerprefs
        
        // Generate rebinders
        if(rebinderContainer.childCount == 0){
            int numberOfRebindersToGenerate = playerInputComponent.actions.actionMaps[0].actions.Count - numberOfActionsToExclude;
            
            for (int actionIndex = 0; actionIndex < numberOfRebindersToGenerate; actionIndex++)
            {
                InputAction a = playerInputComponent.actions.actionMaps[0].actions[actionIndex];

                if(a.bindings[0].isComposite){
                    // For loop starts at 1 because the structure of bindings is such that the first (empty)
                    // binding is marked as composite, while the subsequent ones are 'isPartOfComposite'
                    for (int bindingIndex = 1; bindingIndex < a.bindings.Count; bindingIndex++)
                    {
                        GameObject r = Instantiate(rebinderPrefab, rebinderContainer);
                        SetupRebinder(r, actionIndex, bindingIndex);
                    }
                }
                else{
                    GameObject r = Instantiate(rebinderPrefab, rebinderContainer);
                    // Setup with:
                    // action map, action index to rebind, rebind ui popup gameobject, instance of this script so it can add listener for onrebind
                    SetupRebinder(r, actionIndex);
                }
                
            }
        }
    }

    // Setup for single binding action
    void SetupRebinder(GameObject rebinderToSetup, int actionIndex){
        rebinderToSetup.GetComponent<Rebinder>().Setup(playerInputComponent.actions.actionMaps[0], actionIndex, rebindPopup, this);
    }
    // Setup as a rebinder for a composite part
    void SetupRebinder(GameObject rebinderToSetup, int actionIndex, int bindingIndex){
        rebinderToSetup.GetComponent<Rebinder>().Setup(playerInputComponent.actions.actionMaps[0], actionIndex, bindingIndex, rebindPopup, this);
    }
// Standard binding overload
    public void OnRebind(InputAction actionToRebind, Rebinder rebinder){
        // Enable UI that displays the action that is being rebound and blocks other inputs
        rebindPopup.SetActive(true);
        rebindPopup.GetComponentInChildren<TextMeshProUGUI>().SetText("Rebinding '" + actionToRebind.name + ".' \n Press Escape to cancel.");
        // Perform interactive rebind with escape key canceling
        var rebind = actionToRebind.PerformInteractiveRebinding()
            .WithCancelingThrough("<Keyboard>/escape")
            .OnCancel(operation => { rebinder.UpdateText(); rebindPopup.SetActive(false); operation.Dispose();})
            .OnComplete(operation => { rebinder.UpdateText(); rebindPopup.SetActive(false); operation.Dispose();});

        rebind.Start();

        SetupSavePopup();
    }
// Overload for a binding that is part of a composite
    public void OnRebind(InputAction actionToRebind, Rebinder rebinder, int bindingIndex){
        // Enable UI that displays the action that is being rebound and blocks other inputs
        rebindPopup.SetActive(true);
        rebindPopup.GetComponentInChildren<TextMeshProUGUI>().SetText("Rebinding '" + actionToRebind.name + ".' \n Press Escape to cancel.");
        // Perform interactive rebind with escape key canceling
        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnCancel(operation => { rebinder.UpdateText(); rebindPopup.SetActive(false); operation.Dispose();})
            .OnComplete(operation => { rebinder.UpdateText(); rebindPopup.SetActive(false); operation.Dispose();});

        rebind.Start();

        SetupSavePopup();
    }

    void SetupSavePopup(){
        pauseMenu.FlagSettingsAsModified();
        applyButtonCover.SetActive(false);
        undoButtonCover.SetActive(false);
        pauseMenu.saveSettingsPopup.SetOnClick(0, UndoChanges, pauseMenu.Unpause);
        pauseMenu.saveSettingsPopup.SetOnClick(1, SaveBindings, pauseMenu.Unpause);
        pauseMenu.saveSettingsPopup.SetText("Do you want to save your changes?");
        pauseMenu.saveSettingsPopup.SetButtonText(0, "No");
        pauseMenu.saveSettingsPopup.SetButtonText(1, "Yes");
    }

    public void UndoChanges(){
        foreach (Transform child in rebinderContainer)
        {
            Rebinder rebinder = child.GetComponent<Rebinder>();
            rebinder.action.ApplyBindingOverride(rebinder.bIndex, rebinder.originalBinding.path);
            rebinder.UpdateText();
        }
    }

    public void SaveBindings(){
        var rebinds = playerInputComponent.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("Keybindings", rebinds);
    }

    void LoadBindings(){
        if(PlayerPrefs.HasKey("Keybindings")){
            var rebinds = PlayerPrefs.GetString("Keybindings");
            playerInputComponent.actions.LoadBindingOverridesFromJson(rebinds);
        }
    }

}
