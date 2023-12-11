using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessEffectManager : MonoBehaviour
{
    public VolumeProfile profile;

    private bool fadingVignette = false;

    float startTime;
    float inflectionPointTime;
    float completionTime;
    float progress = 0;
    float maxI = 0;

    Vignette vignetteEffect;


    public void AnimateVignette(Color color, float maxIntensity, float fadeInTime = 0.5f, float fadeOutTime = 3.5f){
        
        if(profile.TryGet(out vignetteEffect)){
            vignetteEffect.color.value = color;
            vignetteEffect.intensity.value = 0;

            startTime = Time.time;
            inflectionPointTime = Time.time + fadeInTime;
            completionTime = inflectionPointTime + fadeOutTime;
            maxI = maxIntensity;

            vignetteEffect.active = true;

            // StartCoroutine(FadeVignette(vignetteEffect, maxIntensity, fadeInTime, fadeOutTime));
            fadingVignette = true;
        }


    }

    void Update(){
        if(fadingVignette){
            if(Time.time < inflectionPointTime){
                progress = (Time.time - startTime) / (inflectionPointTime - startTime);
                vignetteEffect.intensity.value = progress * maxI;
                // Debug.Log(progress);
            }
            else if (Time.time < completionTime){
                progress = (Time.time - inflectionPointTime) / (completionTime - inflectionPointTime);
                vignetteEffect.intensity.value = maxI - (progress * maxI);
                // Debug.Log(progress);
            }
            else{
                vignetteEffect.active = false;
                fadingVignette = false;
            }
        }
    }

    IEnumerator FadeVignette(Vignette v, float maxI, float i, float o){
        
        // float startTime = Time.time;
        // float inflectionPointTime = Time.time + i;
        // float completionTime = inflectionPointTime + o;

        v.active = true;

        // float progress = 0;

        for(float t = Time.time; t < inflectionPointTime; t += Time.deltaTime){
            // Fade in
            progress = (t - startTime) / (inflectionPointTime - startTime);
            v.intensity.value = progress * maxI;
            Debug.Log(progress);
        }

        // while(Time.time < inflectionPointTime){
            
        // }
        progress = 0;
        // while(Time.time >= inflectionPointTime && Time.time < completionTime){
        //     // Fade out
        //     progress = (Time.time - inflectionPointTime) / (completionTime - inflectionPointTime);
        //     v.intensity.value = maxI - (progress * maxI);
        // }

        v.active = false;

        yield return new WaitForSeconds(0);
    }
}
