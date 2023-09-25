using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Ranged")]
public class RangedWeapon : ScriptableObject
{
    public enum Traits{
        Vicious
    }
    
    public string weaponName;
    public int baseDamage = 0;
    public int weight = 0;
    public int sharpness = 0;
    public float cooldownTime = 0;
    public float drawTime = 0.5f;
    public Ammunition.Type ammunitionType;
    public Traits[] traits;
}
