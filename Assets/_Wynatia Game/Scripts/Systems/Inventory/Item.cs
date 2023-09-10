using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory Item")]
public class Item : ScriptableObject
{
    
    public enum ItemType{
        Currency,
        Trinket,
        MagicTrinket,
        Necklace,
        Bracelet,
        Ring,
        Materials,
        Potion,
        Food,
        Weapon,
        Shield,
        MagicWeapon,
        Clothing,
        MagicClothing,
        Headwear,
        Armor,
        MagicArmor
    }

    public enum ItemEffect{
        None,
        Heal
    }

    
    public string itemName;
    public ItemType type;
    public ItemEffect effect;
    public int itemLevel;
    public int value = 0;
    public GameObject worldObject;


    #region Weapon Paramaters
    public bool twoHanded = false;
    public MeleeWeapon meleeWeaponScriptableObject;

    #endregion
}
