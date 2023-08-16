using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ControlsManager : MonoBehaviour
{
    public GameObject rebinderPrefab;
    public GameObject rebindPopup;
    public Transform rebinderContainer;
    [SerializeField] GameObject applyButtonCover;
    [SerializeField] GameObject undoButtonCover;
    [SerializeField] UIPopup bindingConflictPopup;

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
        LoadBindings();
        
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
        bool hold = false;
        string key = playerInputComponent.actions.actionMaps[0].actions[actionIndex].name + "_holdEnabled";
        if(PlayerPrefs.HasKey(key)){
            if(PlayerPrefs.GetInt(key) == 1){
                hold = true;
            }
        }

        rebinderToSetup.GetComponent<Rebinder>().Setup(playerInputComponent.actions.actionMaps[0], actionIndex, rebindPopup, this, hold);
    }
    // Setup as a rebinder for a composite part
    void SetupRebinder(GameObject rebinderToSetup, int actionIndex, int bindingIndex){
        rebinderToSetup.GetComponent<Rebinder>().Setup(playerInputComponent.actions.actionMaps[0], actionIndex, bindingIndex, rebindPopup, this);
    }
// Standard binding overload
    public void OnRebind(InputAction actionToRebind, Rebinder rebinder){
        IEnumerable<InputBinding> originalBindings = playerInputComponent.actions.bindings;


        // Enable UI that displays the action that is being rebound and blocks other inputs
        rebindPopup.SetActive(true);
        rebindPopup.GetComponentInChildren<TextMeshProUGUI>().SetText("Rebinding '" + actionToRebind.name + ".' \n Press Escape to cancel.");
        // Perform interactive rebind with escape key canceling
        var rebind = actionToRebind.PerformInteractiveRebinding()
            .WithCancelingThrough("<Keyboard>/escape")
            .OnCancel(operation => { rebinder.UpdateText(); rebindPopup.SetActive(false); operation.Dispose();})
            .OnComplete(operation => { CheckForConflicts(); rebinder.UpdateText(); rebindPopup.SetActive(false); operation.Dispose();});

        rebind.Start();
        
        bool settingsModified = true;

// IF THIS WORKS, COPY IT TO THE OTHER ONREBIND OVERLOAD
        void CheckForConflicts(){
            InputBinding newBinding = playerInputComponent.actions[actionToRebind.name].bindings[0];
            foreach (var binding in originalBindings)
            {
                if(binding.effectivePath == newBinding.effectivePath){
                    if(binding.action != actionToRebind.name){
                        // Undo rebind, show warning on UI
                        bindingConflictPopup.SetText("'"+ binding.ToDisplayString() + "' is already in use by another action. To bind '" + binding.ToDisplayString() +
                        "' to this action, either bind '" + binding.action + "' to a different key or select an alternate key to bind '" + actionToRebind.name + "' to.");
                        bindingConflictPopup.gameObject.SetActive(true);

                        // (binding to apply, binding group, binding to override)
                        actionToRebind.ApplyBindingOverride(rebinder.originalBinding.path, null, newBinding.path);
                    }
                    else{
                        settingsModified = false;
                    }
                }
            }
        }

        SetupSavePopup(settingsModified);
    }
// Overload for a binding that is part of a composite
    public void OnRebind(InputAction actionToRebind, Rebinder rebinder, int bindingIndex){
        IEnumerable<InputBinding> originalBindings = playerInputComponent.actions.bindings;

        // Enable UI that displays the action that is being rebound and blocks other inputs
        rebindPopup.SetActive(true);
        rebindPopup.GetComponentInChildren<TextMeshProUGUI>().SetText("Rebinding '" + actionToRebind.name + ".' \n Press Escape to cancel.");
        // Perform interactive rebind with escape key canceling
        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnCancel(operation => { rebinder.UpdateText(); rebindPopup.SetActive(false); operation.Dispose();})
            .OnComplete(operation => { CheckForConflicts(); rebinder.UpdateText(); rebindPopup.SetActive(false); operation.Dispose();});

        rebind.Start();

        bool settingsModified = true;

        void CheckForConflicts(){
            InputBinding newBinding = playerInputComponent.actions[actionToRebind.name].bindings[bindingIndex];
            foreach (var binding in originalBindings)
            {
                if(binding.effectivePath == newBinding.effectivePath){
                    if(binding.action != actionToRebind.name){
                        // Undo rebind, show warning on UI
                        // bindingConflictPopup.SetText("'"+ binding.ToDisplayString() + "' is already in use by another action. To bind '" + binding.ToDisplayString() +
                        // "' to this action, either bind '" + binding.action + "' to a different key or select an alternate key to bind '" + actionToRebind.name + "' to.");
                        // bindingConflictPopup.gameObject.SetActive(true);
                        ResolveBindingConflict(actionToRebind, rebinder, binding, newBinding);
                        
                        // actionToRebind.ApplyBindingOverride(rebinder.originalBinding.path, null, newBinding.path);
                    }
                    else if (binding.isPartOfComposite && binding.id != newBinding.id){
                        // If binding is part of the same action as the new binding (and not the same binding as the new binding),
                        // register conflict
                        ResolveBindingConflict(actionToRebind, rebinder, binding, newBinding);
                    }
                    else{
                        settingsModified = false;
                    }
                }
            }
        }

        SetupSavePopup(settingsModified);
    }

    void ResolveBindingConflict(InputAction actionToReset, Rebinder r, InputBinding conflictingBinding, InputBinding newBinding){
        bindingConflictPopup.SetText("'"+ conflictingBinding.ToDisplayString() + "' is already in use by another action. To bind '" + conflictingBinding.ToDisplayString() +
            "' to this action, either bind '" + conflictingBinding.action + "' to a different key or select an alternate key to bind '" + actionToReset.name + "' to.");
        bindingConflictPopup.gameObject.SetActive(true);
        // (binding to apply, binding group, binding to override)
        actionToReset.ApplyBindingOverride(r.originalBinding.path, null, newBinding.path);
    }

    void SetupSavePopup(bool settingsModified = true){
        if(settingsModified){
            FlagSaM();
        }
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
            rebinder.RecallInitialState();
        }
    }

    public void FlagSaM(){
        pauseMenu.FlagSettingsAsModified();
        applyButtonCover.SetActive(false);
        undoButtonCover.SetActive(false);
    }

    public void SaveBindings(){
        var rebinds = playerInputComponent.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("Keybindings", rebinds);

        foreach (Transform rebinder in rebinderContainer)
        {
            Rebinder r = rebinder.GetComponent<Rebinder>();
            if(r.holdToggle.isOn){
                PlayerPrefs.SetInt(r.action.name + "_holdEnabled", 1);
                PlayerPrefs.SetFloat(r.action.name + "_holdTime", r.holdTime);
            }
            else{
                PlayerPrefs.SetInt(r.action.name + "_holdEnabled", 0);
            }
        }
    }

    void LoadBindings(){
        if(PlayerPrefs.HasKey("Keybindings")){
            var rebinds = PlayerPrefs.GetString("Keybindings");
            playerInputComponent.actions.LoadBindingOverridesFromJson(rebinds);
        }
    }

}
