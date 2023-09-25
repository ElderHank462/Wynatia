using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Ammunition")]
public class Ammunition : ScriptableObject
{
    public enum Type{
        Arrows,
        Bolts,
        Projectile
    }
    public enum Traits{
        Vicious
    }
    
    public string ammunitionName;
    public Type type;

    public int damage;
    public int weight;
    public int sharpness;
    public Traits[] traits;
}
