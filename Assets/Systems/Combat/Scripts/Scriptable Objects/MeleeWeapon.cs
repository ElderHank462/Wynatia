using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Melee Weapon")]
public class MeleeWeapon : ScriptableObject
{
    public string weaponName;
    public int damage = 0;
    public float cooldownTime = 0;
    public float powerAttackDamageMultiplier = 1;
    public float powerAttackCooldownMultiplier = 1;
}
