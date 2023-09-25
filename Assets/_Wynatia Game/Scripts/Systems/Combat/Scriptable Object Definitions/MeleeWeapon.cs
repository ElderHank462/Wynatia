using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Melee")]
public class MeleeWeapon : ScriptableObject
{
    public enum Traits{
        Vicious
    }
    
    public string weaponName;
    public int baseDamage = 0;
    public int weight = 0;
    public int sharpness = 0;
    public float cooldownTime = 0;
    public float powerAttackDamageMultiplier = 1;
    public float powerAttackCooldownMultiplier = 1;
    public Traits[] traits;
}
