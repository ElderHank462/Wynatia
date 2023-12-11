using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;

public class HealInstantaneous : MonoBehaviour, IConsumable
{
    public int amount;

    public Color vignetteColor = Color.yellow;
    Vignette vignetteEffect;
    
    public void Consume(GameObject target){
        // If target is the player
        if(target.GetComponent<PlayerCharacter>()){
            PlayerCharacter playerCharacter = FindObjectOfType<PlayerCharacter>();

            playerCharacter.ModifyHealth(amount);

            // Visual effect
            FindObjectOfType<PostProcessEffectManager>().AnimateVignette(vignetteColor, 0.462f);

        }
    }
}
