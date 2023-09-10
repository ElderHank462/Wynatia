using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GraphicsManager : MonoBehaviour
{
    Camera playerCamera;
    
    [SerializeField] private TMP_Dropdown qualityLevelDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Text fieldOfViewDisplayText;
    [SerializeField] private Slider fieldOfViewSlider;

    Resolution[] resolutions;
    Resolution defaultResolution;

    int defaultQualityLevel;
    int defaultResolutionIndex;
    
    // Start is called before the first frame update
    void Start()
    {
        #region Quality Preset
        defaultQualityLevel = QualitySettings.GetQualityLevel();

        qualityLevelDropdown.ClearOptions();
        qualityLevelDropdown.AddOptions(new List<string>(QualitySettings.names));
        qualityLevelDropdown.SetValueWithoutNotify(defaultQualityLevel);
        #endregion

        #region Resolution
        resolutions = Screen.resolutions;
        List<string> resolutionStrings = new List<string>();
        foreach (var item in resolutions)
        {
            resolutionStrings.Add(item.width + "x" + item.height + " @" + item.refreshRate + " Hz");
        }
        
        var currentResolutionString = Screen.currentResolution.width + "x" + Screen.currentResolution.height + " @" + Screen.currentResolution.refreshRate + " Hz";
        // Apparently if IndexOf can't find the item we're looking for it will return -1
        defaultResolutionIndex = resolutionStrings.IndexOf(currentResolutionString);
        if(defaultResolutionIndex < 0){
            resolutionStrings.Add(currentResolutionString);
            defaultResolutionIndex = resolutionStrings.Count -1;
            defaultResolution = Screen.currentResolution;
        }
        else
            defaultResolution = resolutions[defaultResolutionIndex];
        
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionStrings);
        resolutionDropdown.SetValueWithoutNotify(defaultResolutionIndex);
        #endregion
    
        playerCamera = Camera.main;
    
        if(PlayerPrefs.HasKey("playerCamera_fieldOfView")){
            playerCamera.fieldOfView = PlayerPrefs.GetInt("playerCamera_fieldOfView");
            int f = PlayerPrefs.GetInt("playerCamera_fieldOfView");
            fieldOfViewSlider.value = f;
            fieldOfViewDisplayText.text = f.ToString() + "°";
        }
    }

    public void OnQualityLevelChanged(int newLevel){
        QualitySettings.SetQualityLevel(newLevel);
    }

    public void OnResolutionChanged(int newResolution){
        if(newResolution >= resolutions.Length){
            SetResolution(defaultResolution);
        }
        else{
            SetResolution(resolutions[newResolution]);
        }
    }

    public void OnFieldOfViewChanged(float newFieldOfView){
        playerCamera.fieldOfView = Mathf.RoundToInt(newFieldOfView);
        fieldOfViewDisplayText.text = newFieldOfView.ToString() + "°";

        PlayerPrefs.SetInt("playerCamera_fieldOfView", Mathf.RoundToInt(newFieldOfView));
    }

    void SetResolution(Resolution r){
        Screen.SetResolution(r.width, r.height, FullScreenMode.ExclusiveFullScreen, r.refreshRate);
    }

    public void OnReset(){
        QualitySettings.SetQualityLevel(defaultQualityLevel);
        qualityLevelDropdown.SetValueWithoutNotify(defaultQualityLevel);

        SetResolution(defaultResolution);
        resolutionDropdown.SetValueWithoutNotify(defaultResolutionIndex);
    }

}
