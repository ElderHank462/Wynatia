using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    public Slider healthBar;
    public float maxSizeThreshold = 200;

    public Slider phantomBar;
    public float phantomBarTime = 1f;
    [SerializeField] private AnimationCurve phantomBarCurve;

    float elapsedTime = 0;
    float start;
    float end;
    

    public void SetHealth(int currentHealth, int maxHealth){
        if(maxHealth <= maxSizeThreshold){
            transform.localScale = new Vector3(maxHealth / maxSizeThreshold, 1, 1);
        }
        else{
            transform.localScale = new Vector3(1, 1, 1);
        }

        start = phantomBar.value;
        end = (float)((float)currentHealth / (float)maxHealth);
        elapsedTime = 0;

        healthBar.value = (float)((float)currentHealth / (float)maxHealth);
    }

    void Update(){
        
        if(elapsedTime < phantomBarTime){
            elapsedTime += Time.deltaTime;
            phantomBar.value = Mathf.Lerp(start, end, phantomBarCurve.Evaluate(elapsedTime / phantomBarTime));
            // Debug.Log(phantomBar.value);
        }

    }
}
