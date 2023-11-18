using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemMenu : MonoBehaviour
{
    public void QuitGame(){
        Application.Quit();
    }

    public void ResetSaveDataAndReload(){
        ES3.Save("overwriteSaveData", true);

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
