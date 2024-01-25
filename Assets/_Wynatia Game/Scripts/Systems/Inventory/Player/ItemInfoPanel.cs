using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoPanel : MonoBehaviour
{
    public GameObject headerPrefab;
    public GameObject infoBlockPrefab;
    public Transform infoBlockContainer;


    public void Setup(Item item){
        foreach (Transform child in infoBlockContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Display universal info
        // Name
        CreateHeaderBlock(item.itemName);
        // Type
        CreateInfoBlock(item.type.ToString());
        // Effect
        CreateInfoBlock("Effect:", item.effect.ToString());
        // Item Level
        CreateInfoBlock("Level:", item.itemLevel.ToString());
        // Value
        CreateInfoBlock("Value:", item.value.ToString());
        // Description
        CreateInfoBlock(item.description);

        // (spacer)
        CreateHeaderBlock("");

        // Specific header for item type
        CreateHeaderBlock(item.type.ToString() + " Details");
        // Depending on the item's type, display different info
        switch(item.type){
            case(Item.ItemType.Weapon):
            case(Item.ItemType.Arrows):
            case(Item.ItemType.Bolts):
            case(Item.ItemType.Projectile):
                if(item.meleeWeaponScriptableObject){
                    MeleeWeapon m = item.meleeWeaponScriptableObject;
                    CreateInfoBlock("Damage:", m.baseDamage.ToString());
                    CreateInfoBlock("Power Attack Damage:", Mathf.RoundToInt(m.baseDamage * m.powerAttackDamageMultiplier).ToString());
                    CreateInfoBlock("Cooldown Time:", m.cooldownTime.ToString());
                    CreateInfoBlock("Power Attack Cooldown Time:", (m.cooldownTime * m.powerAttackCooldownMultiplier).ToString());
                    CreateInfoBlock("Weight:", m.weight.ToString());
                    CreateInfoBlock("Sharpness:", m.sharpness.ToString());
                    if(m.traits.Length == 0){
                        CreateInfoBlock("Traits:", "None");
                    }
                    else if (m.traits.Length == 1){
                        CreateInfoBlock("Traits:", m.traits[0].ToString());
                    }
                    else{
                        CreateInfoBlock("Traits:", m.traits[0].ToString());
                        for (int i = 1; i < m.traits.Length; i++)
                        {
                            CreateInfoBlock("", m.traits[i].ToString());
                        }
                    }
                }
                else if(item.rangedWeaponScriptableObject){
                    RangedWeapon r = item.rangedWeaponScriptableObject;
                    CreateInfoBlock("Damage:", r.baseDamage.ToString());
                    CreateInfoBlock("Draw Time:", r.drawTime.ToString());
                    CreateInfoBlock("Cooldown Time:", r.cooldownTime.ToString());
                    CreateInfoBlock("Weight:", r.weight.ToString());
                    CreateInfoBlock("Sharpness:", r.sharpness.ToString());
                    CreateInfoBlock("Ammunition Type:", r.ammunitionType.ToString());
                    if(r.traits.Length == 0){
                        CreateInfoBlock("Traits:", "None");
                    }
                    else if (r.traits.Length == 1){
                        CreateInfoBlock("Traits:", r.traits[0].ToString());
                    }
                    else{
                        CreateInfoBlock("Traits:", r.traits[0].ToString());
                        for (int i = 1; i < r.traits.Length; i++)
                        {
                            CreateInfoBlock("", r.traits[i].ToString());
                        }
                    }
                }
                else if(item.ammunitionScriptableObject){
                    Ammunition a = item.ammunitionScriptableObject;
                    CreateInfoBlock("Damage:", a.damage.ToString());
                    CreateInfoBlock("Weight:", a.weight.ToString());
                    CreateInfoBlock("Sharpness:", a.sharpness.ToString());
                    CreateInfoBlock("Ammunition Type:", a.type.ToString());
                    if(a.traits.Length == 0){
                        CreateInfoBlock("Traits:", "None");
                    }
                    else if (a.traits.Length == 1){
                        CreateInfoBlock("Traits:", a.traits[0].ToString());
                    }
                    else{
                        CreateInfoBlock("Traits:", a.traits[0].ToString());
                        for (int i = 1; i < a.traits.Length; i++)
                        {
                            CreateInfoBlock("", a.traits[i].ToString());
                        }
                    }
                }
            break;

        }
    }

    void CreateInfoBlock(string variable, string value = ""){
        GameObject g = Instantiate(infoBlockPrefab, infoBlockContainer);
        g.GetComponent<TextAssigner>().SetText(0, variable);
        g.GetComponent<TextAssigner>().SetText(1, value);
    }

    void CreateHeaderBlock(string text){
        GameObject g = Instantiate(headerPrefab, infoBlockContainer);
        g.GetComponent<TextAssigner>().SetText(0, text);
    }
}
