using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsManager : MonoBehaviour
{
    public int mouseSensitivityMax = 90;
    public int mouseSensitivityMin = 10;

    public PlayerMovement player;
    
    public Slider mouseSensitivity;
    
    // Start is called before the first frame update
    void Start()
    {
        RecallPlayerPrefs();
    }

    void RecallPlayerPrefs(){
        if(PlayerPrefs.HasKey("Mouse Sensitivity")){
            int savedSensitivity = PlayerPrefs.GetInt("Mouse Sensitivity");

            mouseSensitivity.maxValue = mouseSensitivityMax;
            mouseSensitivity.minValue = mouseSensitivityMin;

            mouseSensitivity.value = savedSensitivity;
        }
        else{
            mouseSensitivity.maxValue = mouseSensitivityMax;
            mouseSensitivity.minValue = mouseSensitivityMin;
            //Set to default value
            mouseSensitivity.value = mouseSensitivityMax - ((mouseSensitivityMax - mouseSensitivityMin) / 2);
        }
    }

    public void ChangeMouseSensitivity(float newValue){
        player.rotSpeed = Mathf.RoundToInt(newValue);
        PlayerPrefs.SetInt("Mouse Sensitivity", Mathf.RoundToInt(newValue));
    }
}
