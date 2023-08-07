using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    public Slider healthBar;
    
    public void SetHealth(int currentHealth, int maxHealth){
        healthBar.value = (float)((float)currentHealth / (float)maxHealth);
    }
}
