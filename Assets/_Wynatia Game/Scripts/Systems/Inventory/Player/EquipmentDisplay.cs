using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EquipmentDisplay : MonoBehaviour
{
    private EquipMenu equipMenu;
    
    [SerializeField] Item myItem;
    public string mySlot;
    [SerializeField] GameObject textPrefab;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] List<TextMeshProUGUI> statTexts = new List<TextMeshProUGUI>();

    void Start(){
        // Setup(myItem);
    }

    public void Setup(KeyValuePair<string, Item> item, EquipMenu eMenu){
        equipMenu = eMenu;
        mySlot = item.Key;
        
        if(item.Value){
            // Assign item
            myItem = item.Value;
            // Retrieve text
            List<string> labelStrings = ReturnStatLabelText(myItem.type);
            List<string> dataStrings = ReturnStatDataText(myItem.type);
            // Set common text
            nameText.SetText(myItem.itemName + " (" + mySlot + ")");
            typeText.SetText(myItem.type.ToString());
            // Instantiate and populate text
            for (int i = 0; i < labelStrings.Count; i++)
            {
                GameObject t = Instantiate(textPrefab, transform);
                t.GetComponent<TextAssigner>().SetText(0, labelStrings[i]);
                t.GetComponent<TextAssigner>().SetText(1, dataStrings[i]);
            }
        }
        else{
            nameText.SetText("Empty" + " (" + mySlot + ")");
            typeText.SetText("");
        }


        
        // Assign model to highlight
        

        // Set ui toggle action to setup comparison window, update button palette, and highlight the model
        Toggle toggleComponent = GetComponent<Toggle>();
        toggleComponent.onValueChanged.AddListener(delegate{ OnSelected(toggleComponent.isOn); });

    }

    void OnSelected(bool setup){
        // setup comparison window, update button palette, and highlight the model
        if(setup)
            equipMenu.SetupComparisonWindow(myItem, mySlot);
        else
            equipMenu.ResetComparisonWindow();
    }

    List<string> ReturnStatLabelText(Item.ItemType itemType){
        List<string> strings = new List<string>();
        
        switch(itemType){
            case Item.ItemType.Necklace:
            case Item.ItemType.Bracelet:
            case Item.ItemType.Ring:
            break;

            case Item.ItemType.Weapon:
            // case Item.ItemType.MagicWeapon:
                if(myItem.meleeWeaponScriptableObject){
                    strings.Add("Damage:");
                    strings.Add("Power Attack Damage:");
                    strings.Add("Cooldown Time:");
                    strings.Add("Power Attack Cooldown Time:");
                    strings.Add("Weight:");
                    strings.Add("Sharpness:");
                    strings.Add("Traits:");
                    if(myItem.meleeWeaponScriptableObject.traits.Length > 1){
                        for (int i = 1; i < myItem.meleeWeaponScriptableObject.traits.Length; i++)
                        {
                            strings.Add("");
                        }
                    }
                }
                else if(myItem.rangedWeaponScriptableObject){
                    strings.Add("Damage:");
                    strings.Add("Draw Time:");
                    strings.Add("Cooldown Time:");
                    strings.Add("Weight:");
                    strings.Add("Sharpness:");
                    strings.Add("Ammunition Type:");
                    strings.Add("Traits:");
                    if(myItem.rangedWeaponScriptableObject.traits.Length > 1){
                        for (int i = 1; i < myItem.rangedWeaponScriptableObject.traits.Length; i++)
                        {
                            strings.Add("");
                        }
                    }
                }


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

    List<string> ReturnStatDataText(Item.ItemType itemType){
        List<string> strings = new List<string>();
        
        switch(itemType){
            case Item.ItemType.Necklace:
            case Item.ItemType.Bracelet:
            case Item.ItemType.Ring:
            break;

            case Item.ItemType.Weapon:
            // case Item.ItemType.MagicWeapon:
                if(myItem.meleeWeaponScriptableObject){
                    strings.Add(myItem.meleeWeaponScriptableObject.baseDamage.ToString());
                    string powerAttackDamage = (myItem.meleeWeaponScriptableObject.baseDamage * myItem.meleeWeaponScriptableObject.powerAttackDamageMultiplier).ToString();
                    strings.Add(powerAttackDamage);
                    strings.Add(myItem.meleeWeaponScriptableObject.cooldownTime.ToString());
                    string powerAttackCooldownTime = (myItem.meleeWeaponScriptableObject.powerAttackCooldownMultiplier * myItem.meleeWeaponScriptableObject.cooldownTime).ToString();
                    strings.Add(powerAttackCooldownTime);
                    strings.Add(myItem.meleeWeaponScriptableObject.weight.ToString());
                    strings.Add(myItem.meleeWeaponScriptableObject.sharpness.ToString());

                    if(myItem.meleeWeaponScriptableObject.traits.Length == 0){
                        strings.Add("None");
                    }
                    else if(myItem.meleeWeaponScriptableObject.traits.Length == 1){
                        strings.Add(myItem.meleeWeaponScriptableObject.traits[0].ToString());
                    }
                    else{
                        for (int i = 0; i < myItem.meleeWeaponScriptableObject.traits.Length - 1; i++)
                        {
                            strings.Add(myItem.meleeWeaponScriptableObject.traits[i].ToString() + ",");
                        }
                        strings.Add(myItem.meleeWeaponScriptableObject.traits[0].ToString());
                    }
                }
                else if(myItem.rangedWeaponScriptableObject){
                    strings.Add(myItem.rangedWeaponScriptableObject.baseDamage.ToString());
                    strings.Add(myItem.rangedWeaponScriptableObject.drawTime.ToString());
                    strings.Add(myItem.rangedWeaponScriptableObject.cooldownTime.ToString());
                    strings.Add(myItem.rangedWeaponScriptableObject.weight.ToString());
                    strings.Add(myItem.rangedWeaponScriptableObject.sharpness.ToString());
                    strings.Add(myItem.rangedWeaponScriptableObject.ammunitionType.ToString());
                    

                    if(myItem.rangedWeaponScriptableObject.traits.Length == 0){
                        strings.Add("None");
                    }
                    else if(myItem.rangedWeaponScriptableObject.traits.Length == 1){
                        strings.Add(myItem.rangedWeaponScriptableObject.traits[0].ToString());
                    }
                    else{
                        for (int i = 0; i < myItem.rangedWeaponScriptableObject.traits.Length - 1; i++)
                        {
                            strings.Add(myItem.rangedWeaponScriptableObject.traits[i].ToString() + ",");
                        }
                        strings.Add(myItem.rangedWeaponScriptableObject.traits[0].ToString());
                    }
                }
                

                
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
            