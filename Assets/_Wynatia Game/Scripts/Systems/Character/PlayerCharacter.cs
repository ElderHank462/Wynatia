using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;

    public int maxStamina = 100;
    public int currentStamina = 100;

    public int level = 1;

    public FloatingHealthBar healthBar;

// Intended to be equipped whenever melee weapons are unequipped; for monk characters this scriptable object could change over time, too
    public Item unarmedStrike;

    
    void Start(){
        healthBar.SetHealth(currentHealth, maxHealth);
    }


    public void ModifyHealth(int amount){
        currentHealth += amount;

        if(currentHealth > maxHealth)
            currentHealth = maxHealth;

        healthBar.SetHealth(currentHealth, maxHealth);
    }
}
