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
    
    // obsolete, I think
    public void OnEnable()
    {
        
        playerInputComponent = FindObjectOfType<PlayerInput>();
        pauseMenu = FindObjectOfType<PauseMenu>();

        applyButtonCover.SetActive(true);
        undoButtonCover.SetActive(true);
        
        // (if bindings don't automatically save)
        // Load saved bindings from playerprefs
        LoadBindings();
        
        foreach(Transform child in rebinderContainer){
            Destroy(child.gameObject);
        }

        // Generate rebinders
        // if(rebinderContainer.childCount == 0){
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
        // }

        // foreach(InputAction action in playerInputComponent.actions.actionMaps[0].actions){
        //     if(action.name == "Attack" || action.name == "Ranged Attack")
        //         Debug.Log(action.name + ": " + action.bindings[0].ToDisplayString());
        // }
    }

    public void Initialize(){
        playerInputComponent = FindObjectOfType<PlayerInput>();
        pauseMenu = FindObjectOfType<PauseMenu>();

        applyButtonCover.SetActive(true);
        undoButtonCover.SetActive(true);
        
        // (if bindings don't automatically save)
        // Load saved bindings from playerprefs
        LoadBindings();
        
        foreach(Transform child in rebinderContainer){
            Destroy(child.gameObject);
        }

        // Generate rebinders
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

        // foreach(InputAction action in playerInputComponent.actions.actionMaps[0].actions){
        //     if(action.name == "Attack" || action.name == "Ranged Attack")
        //         Debug.Log(action.name + ": " + action.bindings[0].ToDisplayString());
        // }
    }

    // Setup for single binding action
    void SetupRebinder(GameObject rebinderToSetup, int actionIndex){
        // bool hold = false;
        // string key = playerInputComponent.actions.actionMaps[0].actions[actionIndex].name + "_holdEnabled";
        // if(PlayerPrefs.HasKey(key)){
        //     if(PlayerPrefs.GetInt(key) == 1){
        //         hold = true;
        //     }
        // }

        rebinderToSetup.GetComponent<Rebinder>().Setup(playerInputComponent.actions.actionMaps[0], actionIndex, rebindPopup, this);
    }
    // Setup as a rebinder for a composite part
    void SetupRebinder(GameObject rebinderToSetup, int actionIndex, int bindingIndex){
        rebinderToSetup.GetComponent<Rebinder>().Setup(playerInputComponent.actions.actionMaps[0], actionIndex, bindingIndex, rebindPopup, this);
    }
// Standard binding overload
    public void OnRebind(InputAction actionToRebind, Rebinder rebinder){
        
        IEnumerable<InputBinding> originalBindings = playerInputComponent.actions.bindings;

        bool settingsModified = true;

        // Enable UI that displays the action that is being rebound and blocks other inputs
        rebindPopup.SetActive(true);
        rebindPopup.GetComponentInChildren<TextMeshProUGUI>().SetText("Rebinding '" + actionToRebind.name + ".' \n Press Escape to cancel.");
        // Perform interactive rebind with escape key canceling
        var rebind = actionToRebind.PerformInteractiveRebinding()
            .WithCancelingThrough("<Keyboard>/escape")
            .OnCancel(operation => { rebinder.UpdateText(); rebindPopup.SetActive(false); settingsModified = false; operation.Dispose();})
            .OnComplete(operation => { CheckForConflicts(); if(actionToRebind.name == "Attack"){ UpdateAllAttackBindings();}; 
                if(actionToRebind.name == "Off-Hand Attack"){ UpdateOffHandBindings();}; rebinder.UpdateText(); rebindPopup.SetActive(false); operation.Dispose();});

        rebind.Start();

        void CheckForConflicts(){
            InputBinding newBinding = playerInputComponent.actions[actionToRebind.name].bindings[0];
            foreach (var binding in originalBindings)
            {
                if(binding.effectivePath == newBinding.effectivePath){
                    if(binding.action != actionToRebind.name){
                        if(actionToRebind.name == "Attack"){
                            if(binding.action != "Melee Power Attack" && binding.action != "Ranged Attack"){
                                UndoRebind();
                            }
                        }
                        else if(actionToRebind.name == "Off-Hand Attack"){
                            if(binding.action != "Melee Off-Hand Power Attack" && binding.action != "Cancel Ranged Attack"){
                                UndoRebind();
                            }
                        }
                        else{
                            UndoRebind();
                        }
                    }
                }

                void UndoRebind(){
                    bindingConflictPopup.SetText("'"+ binding.ToDisplayString() + "' is already in use by another action. To bind '" + binding.ToDisplayString() +
                    "' to this action, either bind '" + binding.action + "' to a different key or select an alternate key to bind '" + actionToRebind.name + "' to.");
                    bindingConflictPopup.gameObject.SetActive(true);

                    settingsModified = false;
                    // (binding to apply, binding group, binding to override)
                    ResolveBindingConflict(actionToRebind, rebinder, binding, newBinding);
                }
            }

            SetupSavePopup(settingsModified);
        }

        void UpdateAllAttackBindings(){
            playerInputComponent.actions["Melee Power Attack"].ApplyBindingOverride(0, actionToRebind.bindings[0].effectivePath);
            playerInputComponent.actions["Ranged Attack"].ApplyBindingOverride
                (0, new InputBinding{ overrideInteractions = "slowTap(duration=" + 1 + ")",
                overridePath = actionToRebind.bindings[0].effectivePath });
        }

        void UpdateOffHandBindings(){
            playerInputComponent.actions["Melee Off-Hand Power Attack"].ApplyBindingOverride(0, actionToRebind.bindings[0].effectivePath);
            playerInputComponent.actions["Cancel Ranged Attack"].ApplyBindingOverride(0, actionToRebind.bindings[0].effectivePath);

        }

        // foreach(InputAction action in playerInputComponent.actions.actionMaps[0].actions){
        //     if(action.name == "Attack" || action.name == "Ranged Attack")
        //         Debug.Log(action.name + ": " + action.bindings[0].ToDisplayString());
        // }
    }
// Overload for a binding that is part of a composite
    public void OnRebind(InputAction actionToRebind, Rebinder rebinder, int bindingIndex){
        IEnumerable<InputBinding> originalBindings = playerInputComponent.actions.bindings;

        bool settingsModified = true;

        // Enable UI that displays the action that is being rebound and blocks other inputs
        rebindPopup.SetActive(true);
        rebindPopup.GetComponentInChildren<TextMeshProUGUI>().SetText("Rebinding '" + actionToRebind.name + ".' \n Press Escape to cancel.");
        // Perform interactive rebind with escape key canceling
        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnCancel(operation => { rebinder.UpdateText(); rebindPopup.SetActive(false); settingsModified = false; operation.Dispose();})
            .OnComplete(operation => { CheckForConflicts(); rebinder.UpdateText(); rebindPopup.SetActive(false); operation.Dispose();});

        rebind.Start();

        void CheckForConflicts(){
            InputBinding newBinding = playerInputComponent.actions[actionToRebind.name].bindings[bindingIndex];
            foreach (var binding in originalBindings)
            {
                if(binding.effectivePath == newBinding.effectivePath){
                    if(binding.action != actionToRebind.name){
                        // Undo rebind, show warning on UI
                        ResolveBindingConflict(actionToRebind, rebinder, binding, newBinding);
                        
                        settingsModified = false;
                        // actionToRebind.ApplyBindingOverride(rebinder.originalBinding.path, null, newBinding.path);
                    }
                    else if (binding.isPartOfComposite && binding.id != newBinding.id){
                        // If binding is part of the same action as the new binding (and not the same binding as the new binding),
                        // register conflict
                        ResolveBindingConflict(actionToRebind, rebinder, binding, newBinding);
                        settingsModified = false;
                    }
                }
            }

            SetupSavePopup(settingsModified);
        }

    }

    void ResolveBindingConflict(InputAction actionToReset, Rebinder r, InputBinding conflictingBinding, InputBinding newBinding){
        bindingConflictPopup.SetText("'"+ conflictingBinding.ToDisplayString() + "' is already in use by another action. To bind '" + conflictingBinding.ToDisplayString() +
            "' to this action, either bind '" + conflictingBinding.action + "' to a different key or select an alternate key to bind '" + actionToReset.name + "' to.");
        bindingConflictPopup.gameObject.SetActive(true);
        // (binding to apply, binding group, binding to override)
        actionToReset.ApplyBindingOverride(r.originalBinding, null, newBinding.path);
    }

    void SetupSavePopup(bool settingsModified = true){
        if(settingsModified){
            FlagSaM();
        }
        pauseMenu.saveSettingsPopup.SetOnClick(0, UndoChanges, pauseMenu.Unpause, pauseMenu.FlagSettingsAsSaved);
        pauseMenu.saveSettingsPopup.SetOnClick(1, SaveBindings, pauseMenu.Unpause, pauseMenu.FlagSettingsAsSaved);
        pauseMenu.saveSettingsPopup.SetText("Do you want to save your changes?");
        pauseMenu.saveSettingsPopup.SetButtonText(0, "No");
        pauseMenu.saveSettingsPopup.SetButtonText(1, "Yes");
    }

    public void UndoChanges(){
        foreach (Transform child in rebinderContainer)
        {
            Rebinder rebinder = child.GetComponent<Rebinder>();
            rebinder.action.ApplyBindingOverride(rebinder.bIndex, rebinder.originalBinding);
            rebinder.UpdateText();
            rebinder.RecallInitialState();
        }
    }

    public void FlagSaM(){
        pauseMenu.FlagSettingsAsModified();
        applyButtonCover.SetActive(false);
        undoButtonCover.SetActive(false);
    }

    public void FlagSaS(){
        pauseMenu.FlagSettingsAsSaved();
        applyButtonCover.SetActive(true);
        undoButtonCover.SetActive(true);
    }

    public void SaveBindings(){
        var rebinds = FindObjectOfType<PlayerInput>().actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("Keybindings", rebinds);
    }

    void LoadBindings(){
        if(PlayerPrefs.HasKey("Keybindings")){
            var rebinds = PlayerPrefs.GetString("Keybindings");
            playerInputComponent.actions.LoadBindingOverridesFromJson(rebinds);


            if(FindObjectOfType<PlayerEquipment>().bothHands){
                if(FindObjectOfType<PlayerEquipment>().bothHands.rangedWeaponScriptableObject){
                    float drawTime = FindObjectOfType<PlayerEquipment>().bothHands.rangedWeaponScriptableObject.drawTime;
                    // Debug.Log(drawTime);
                    
                    InputAction rangedAttackAction = FindObjectOfType<PlayerInput>().actions["Ranged Attack"];

                    rangedAttackAction.ApplyBindingOverride(new InputBinding{ overrideInteractions = "slowTap(duration=" + drawTime + ")", 
                        overridePath = rangedAttackAction.bindings[0].effectivePath});
                }
                    

            }

        }
    }

}
