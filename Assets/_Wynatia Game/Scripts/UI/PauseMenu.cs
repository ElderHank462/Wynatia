using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Toggle displayToggle;
    public Toggle controlsToggle;
    public Toggle systemToggle;
    

    public GameObject pauseMenu;
    public GameObject displayMenu;
    public GameObject controlsMenu;
    public GameObject systemMenu;
    public GameObject[] popupsThatMaintainPause;
    public UIPopup saveSettingsPopup;

    private PlayerInput playerInput;
    private bool settingsModified = false;


    void Start(){
        displayToggle.onValueChanged.AddListener(delegate {ToggleMenu(displayMenu, displayToggle.isOn);});
        controlsToggle.onValueChanged.AddListener(delegate {ToggleMenu(controlsMenu, controlsToggle.isOn);});
        systemToggle.onValueChanged.AddListener(delegate {ToggleMenu(systemMenu, systemToggle.isOn);});

        displayToggle.isOn = false;
        controlsToggle.isOn = false;
        systemToggle.isOn = false;

        ToggleMenu(displayMenu, false);
        ToggleMenu(controlsMenu, false);
        ToggleMenu(systemMenu, false);

        playerInput = FindObjectOfType<PlayerInput>();
        playerInput.actions["menu"].started += context => ChangePauseMenuState();
        playerInput.actions["close"].started += context => ChangePauseMenuState();
    }

    void ToggleMenu(GameObject m, bool state){
        if(!settingsModified)
            m.SetActive(state);
    }

    //Enables and disables the menu UI and corresponding action maps
    public void ChangePauseMenuState(){
        bool dontUnpause = false;
        foreach (var item in popupsThatMaintainPause)
        {
            if(item.activeSelf){
                dontUnpause = true;
            }
        }

        if(!Pause.PauseManagement.paused){
            Pause.PauseManagement.Pause();
            playerInput.SwitchCurrentActionMap("menu");
            pauseMenu.SetActive(true);
        }
        else if(settingsModified){
            saveSettingsPopup.gameObject.SetActive(true);
            // settingsModified = false;
        }
        else if(dontUnpause && pauseMenu.activeSelf){
            saveSettingsPopup.gameObject.SetActive(false);
            playerInput.SwitchCurrentActionMap("player_controls");
            pauseMenu.SetActive(false);
            return;
        }
        else{
            Unpause();
        }
    }

    public void Unpause(){
        Pause.PauseManagement.Unpause();
        playerInput.SwitchCurrentActionMap("player_controls");
        saveSettingsPopup.gameObject.SetActive(false);
        pauseMenu.SetActive(false);
    }

    public void FlagSettingsAsModified(){
        settingsModified = true;
        displayToggle.interactable = false;
        controlsToggle.interactable = false;
        systemToggle.interactable = false;
    }

    public void FlagSettingsAsSaved(){
        settingsModified = false;
        displayToggle.interactable = true;
        controlsToggle.interactable = true;
        systemToggle.interactable = true;
    }


}
