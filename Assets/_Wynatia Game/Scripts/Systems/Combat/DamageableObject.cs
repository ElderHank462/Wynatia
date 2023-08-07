using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour, IDamageable
{
    public int maxHealth;
    public int currentHealth;
    public GameObject hitEffect;
    public FloatingHealthBar floatingHealthBar;

    void Start(){
        currentHealth = maxHealth;
    }

    public void Damage(int dmg, Vector3 hitPosition){
        currentHealth -= dmg;
        floatingHealthBar.SetHealth(currentHealth, maxHealth);
        Instantiate(hitEffect, hitPosition, Quaternion.Euler(-transform.eulerAngles));
        if(currentHealth <= 0){
            Destroy(gameObject);
        }
    }
}
