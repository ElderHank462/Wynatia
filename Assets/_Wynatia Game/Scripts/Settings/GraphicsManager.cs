using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraphicsManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown qualityLevelDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

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
