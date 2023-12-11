using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquipmentDisplay : MonoBehaviour
{
    [SerializeField] Item myItem;
    [SerializeField] GameObject textPrefab;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] List<TextMeshProUGUI> statTexts = new List<TextMeshProUGUI>();

    public void Setup(Item item){
        // Assign item
        myItem = item;
        // Retrieve text
        
        // Instantiate and populate text

        // Assign model to highlight

        // Set ui toggle action to update button palette and highlight the model
    }

    List<string> ReturnStatText(Item.ItemType itemType){
        List<string> strings = new List<string>();
        
        switch(itemType){
            case Item.ItemType.Necklace:
            case Item.ItemType.Bracelet:
            case Item.ItemType.Ring:
            break;

            case Item.ItemType.Weapon:
            case Item.ItemType.MagicWeapon:
                strings.Add("Damage: " + myItem.meleeWeaponScriptableObject.baseDamage.ToString());
                string powerAttackDamage = (myItem.meleeWeaponScriptableObject.baseDamage * myItem.meleeWeaponScriptableObject.powerAttackDamageMultiplier).ToString();
                strings.Add("Power Attack Damage: x" + myItem.meleeWeaponScriptableObject.powerAttackDamageMultiplier.ToString() + " (" + powerAttackDamage + ")");
                strings.Add("Damage: " + myItem.meleeWeaponScriptableObject.baseDamage.ToString());
                strings.Add("Damage: " + myItem.meleeWeaponScriptableObject.baseDamage.ToString());
                strings.Add("Damage: " + myItem.meleeWeaponScriptableObject.baseDamage.ToString());
            break;

            case Item.ItemType.Arrows:
            case Item.ItemType.Bolts:
            case Item.ItemType.Projectile:

            break;

            case Item.ItemType.Shield:

            case Item.ItemType.Clothing:
            case Item.ItemType.MagicClothing:

            case Item.ItemType.Headwear:

            case Item.ItemType.Armor:
            case Item.ItemType.MagicArmor:
            break;
        }



        return strings;
    }
}
            