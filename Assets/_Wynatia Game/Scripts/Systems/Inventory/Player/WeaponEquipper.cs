using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponEquipper : MonoBehaviour, IEquipper
{
    [SerializeField] private GameObject raycastBlocker;
    [SerializeField] private GameObject twoOptionPanel;
    [SerializeField] private GameObject threeOptionPanel;
    [SerializeField] private GameObject fourOptionPanel;

    [SerializeField] private Button twoA;

    [SerializeField] private Button threeA;
    [SerializeField] private Button threeB;

    [SerializeField] private Button fourA;
    [SerializeField] private Button fourB;
    [SerializeField] private Button fourC;

    private Item weaponToEquip;
    private PlayerInventory playerInventory;
    private PlayerEquipment playerEquipment;
    private PlayerCombatAgent playerCombatAgent;

    public enum Layout{
        Equip_MainBoth,
        Equip_MainOff,
        Equip_Off__Replace_Main,
        Replace_MainOffBoth,
        Replace_MainOff,
        Replace_Main,
        Replace_Main__Unequip_Off
    }

    #region Equipping
    public void EquipItem(PlayerInventory.InventoryItem item){
        playerInventory = FindObjectOfType<PlayerInventory>();
        playerEquipment = FindObjectOfType<PlayerEquipment>();
        playerCombatAgent = FindObjectOfType<PlayerCombatAgent>();

        Layout popupLayout;

        // Two-handed applies to both two-handed melee weapons and ranged weapons
        if(item.count > 1 && !item.sObj.twoHanded){
            if(!playerEquipment.rightHand && !playerEquipment.leftHand){
                // Equip to main, off, both, or cancel?
                popupLayout = Layout.Equip_MainBoth;
            }
            else if(playerEquipment.rightHand && !playerEquipment.leftHand){
                if(playerEquipment.rightHand.twoHanded){
                    // Replace main or cancel?
                    popupLayout = Layout.Replace_Main;
                }
                else{
                    // Equip to off, replace main, or cancel?
                    popupLayout = Layout.Equip_Off__Replace_Main;
                }
            }
            else{
                // It isn't possible to have a weapon in the off hand but not in the main hand,
                // so this branch can only be run if there are weapons in both hands

                // Replace main, off, both, or cancel?
                popupLayout = Layout.Replace_MainOffBoth;
            }
        }
        else{
            if(!playerEquipment.rightHand && !playerEquipment.leftHand){
                // Equip to main
                playerEquipment.EquipSlot(out playerEquipment.rightHand, item.sObj);
                if(item.sObj.meleeWeaponScriptableObject)
                    AddWeaponToMainHandContainer(item.sObj);
                else if(item.sObj.rangedWeaponScriptableObject)
                    AddWeaponToRangedContainer(item.sObj);
                ClosePopup();
                return;
            }
            else if(playerEquipment.rightHand && !playerEquipment.leftHand){
                if(!playerEquipment.rightHand.twoHanded && !item.sObj.twoHanded){
                    // Equip to off, replace main, or cancel?
                    popupLayout = Layout.Equip_Off__Replace_Main;
                }
                else{
                    // Replace main or cancel?
                    popupLayout = Layout.Replace_Main;
                }
            }
            else{
                // It isn't possible to have a weapon in the off hand but not in the main hand,
                // so this branch can only be run if there are weapons in both hands
                if(!item.sObj.twoHanded){
                    // Replace main, off, or cancel?
                    popupLayout = Layout.Replace_MainOff;
                }
                else{
                    // Replace both hands with one two-handed weapon or cancel?
                    popupLayout = Layout.Replace_Main__Unequip_Off;
                }
            }
        }
        raycastBlocker.SetActive(true);
        SetupPopout(popupLayout, item.sObj);
    }

    public void SetupPopout(Layout layout, Item item){
        weaponToEquip = item;

        threeOptionPanel.SetActive(false);
        fourOptionPanel.SetActive(false);
        
        switch(layout){
            // case Layout.Equip_MainOff: SetupEquipMO(false);
            // break;

            case Layout.Equip_MainBoth: SetupEquipMB();
            break;
            
            case Layout.Replace_MainOff: SetupReplaceMO(false);
            break;
            
            case Layout.Replace_MainOffBoth: SetupReplaceMO(true);
            break;
            
            case Layout.Equip_Off__Replace_Main: SetupEquipOffReplaceMain();
            break;

            case Layout.Replace_Main: SetupReplaceMain();
            break;

            case Layout.Replace_Main__Unequip_Off: SetupReplaceMainUnequipOff();
            break;
        }
    }

    void SetupReplaceMainUnequipOff(){
        twoA.onClick.RemoveAllListeners();

        if(weaponToEquip.meleeWeaponScriptableObject){
            twoA.onClick.AddListener(delegate {playerEquipment.UnequipSlot(ref playerEquipment.rightHand); playerEquipment.UnequipSlot(ref playerEquipment.leftHand);
                    playerEquipment.EquipSlot(out playerEquipment.rightHand, weaponToEquip); RemoveWeaponFromOffHandContainer(); AddWeaponToMainHandContainer(weaponToEquip); ClosePopup();});
        }
        else if(weaponToEquip.rangedWeaponScriptableObject){
            twoA.onClick.AddListener(delegate {playerEquipment.UnequipSlot(ref playerEquipment.rightHand); playerEquipment.UnequipSlot(ref playerEquipment.leftHand);
                    playerEquipment.EquipSlot(out playerEquipment.rightHand, weaponToEquip); ClearAllWeaponContainers(); AddWeaponToRangedContainer(weaponToEquip); ClosePopup();});
        }

        twoA.GetComponentInChildren<TextMeshProUGUI>().SetText("Yes");

        twoOptionPanel.GetComponentInChildren<TextMeshProUGUI>().SetText("Are you sure you want to equip this weapon? Doing so will also unequip your off-hand weapon.");

        twoOptionPanel.SetActive(true);
    }

    void SetupReplaceMain(){
        twoA.onClick.RemoveAllListeners();

        if(weaponToEquip.meleeWeaponScriptableObject){
            twoA.onClick.AddListener(delegate {playerEquipment.UnequipSlot(ref playerEquipment.rightHand); 
                playerEquipment.EquipSlot(out playerEquipment.rightHand, weaponToEquip); ClearAllWeaponContainers(); AddWeaponToMainHandContainer(weaponToEquip); ClosePopup();});
        }
        else if(weaponToEquip.rangedWeaponScriptableObject){
            twoA.onClick.AddListener(delegate {playerEquipment.UnequipSlot(ref playerEquipment.rightHand); 
                playerEquipment.EquipSlot(out playerEquipment.rightHand, weaponToEquip); ClearAllWeaponContainers(); AddWeaponToRangedContainer(weaponToEquip); ClosePopup();});
        }

        twoA.GetComponentInChildren<TextMeshProUGUI>().SetText("Yes");

        twoOptionPanel.GetComponentInChildren<TextMeshProUGUI>().SetText("Are you sure you want to replace your current weapon?");

        twoOptionPanel.SetActive(true);
    }

    void SetupEquipMB(){
        threeA.onClick.RemoveAllListeners();
        threeB.onClick.RemoveAllListeners();

        threeA.onClick.AddListener(delegate {playerEquipment.EquipSlot(out playerEquipment.rightHand, weaponToEquip); AddWeaponToMainHandContainer(weaponToEquip); ClosePopup();});
        threeB.onClick.AddListener(delegate {playerEquipment.EquipSlot(out playerEquipment.rightHand, weaponToEquip); 
            playerEquipment.EquipSlot(out playerEquipment.leftHand, weaponToEquip); AddWeaponToMainHandContainer(weaponToEquip); AddWeaponToOffHandContainer(weaponToEquip); ClosePopup();});

        string textA = "Main Hand";
        string textB = "One to Each Hand";
        threeA.GetComponentInChildren<TextMeshProUGUI>().SetText(textA);
        threeB.GetComponentInChildren<TextMeshProUGUI>().SetText(textB);

        threeOptionPanel.SetActive(true);
        
    }

    void SetupReplaceMO(bool includeBoth){
        if(!includeBoth){
            threeA.onClick.RemoveAllListeners();
            threeB.onClick.RemoveAllListeners();

            threeA.onClick.AddListener(delegate {playerEquipment.UnequipSlot(ref playerEquipment.rightHand); 
                playerEquipment.EquipSlot(out playerEquipment.rightHand, weaponToEquip); AddWeaponToMainHandContainer(weaponToEquip); ClosePopup();});
            threeB.onClick.AddListener(delegate {playerEquipment.UnequipSlot(ref playerEquipment.leftHand); 
                playerEquipment.EquipSlot(out playerEquipment.leftHand, weaponToEquip); AddWeaponToOffHandContainer(weaponToEquip); ClosePopup();});
                
            string textA = "Main Hand (Replace)";
            string textB = "Off Hand (Replace)";
            threeA.GetComponentInChildren<TextMeshProUGUI>().SetText(textA);
            threeB.GetComponentInChildren<TextMeshProUGUI>().SetText(textB);

            threeOptionPanel.SetActive(true);
        }
        else{
            fourA.onClick.RemoveAllListeners();
            fourB.onClick.RemoveAllListeners();
            fourC.onClick.RemoveAllListeners();

            fourA.onClick.AddListener(delegate {playerEquipment.UnequipSlot(ref playerEquipment.rightHand); 
                playerEquipment.EquipSlot(out playerEquipment.rightHand, weaponToEquip); AddWeaponToMainHandContainer(weaponToEquip); ClosePopup();});
            fourB.onClick.AddListener(delegate {playerEquipment.UnequipSlot(ref playerEquipment.leftHand); 
                playerEquipment.EquipSlot(out playerEquipment.leftHand, weaponToEquip); AddWeaponToOffHandContainer(weaponToEquip); ClosePopup();});
            fourC.onClick.AddListener(delegate {playerEquipment.UnequipSlot(ref playerEquipment.rightHand); 
                playerEquipment.UnequipSlot(ref playerEquipment.leftHand);  
                playerEquipment.EquipSlot(out playerEquipment.rightHand, weaponToEquip);
                playerEquipment.EquipSlot(out playerEquipment.leftHand, weaponToEquip); AddWeaponToMainHandContainer(weaponToEquip); AddWeaponToOffHandContainer(weaponToEquip); 
                ClosePopup();});

            string textA = "Main Hand (Replace)";
            string textB = "Off Hand (Replace)";
            string textC = "One to Each Hand (Replace)";
            fourA.GetComponentInChildren<TextMeshProUGUI>().SetText(textA);
            fourB.GetComponentInChildren<TextMeshProUGUI>().SetText(textB);
            fourC.GetComponentInChildren<TextMeshProUGUI>().SetText(textC);

            fourOptionPanel.SetActive(true);
        }
    }

    void SetupEquipOffReplaceMain(){
        threeA.onClick.RemoveAllListeners();
        threeB.onClick.RemoveAllListeners();

        threeA.onClick.AddListener(delegate {playerEquipment.UnequipSlot(ref playerEquipment.rightHand); 
            playerEquipment.EquipSlot(out playerEquipment.rightHand, weaponToEquip); AddWeaponToMainHandContainer(weaponToEquip); ClosePopup();});
        threeB.onClick.AddListener(delegate {playerEquipment.EquipSlot(out playerEquipment.leftHand, weaponToEquip); AddWeaponToOffHandContainer(weaponToEquip); ClosePopup();});

        string textA = "Main Hand (Replace)";
        string textB = "Off Hand";
        threeA.GetComponentInChildren<TextMeshProUGUI>().SetText(textA);
        threeB.GetComponentInChildren<TextMeshProUGUI>().SetText(textB);

        threeOptionPanel.SetActive(true);
    }

    public void ClosePopup(){
        if(playerEquipment.ammunition){
            if(!playerEquipment.rightHand || !playerEquipment.rightHand.rangedWeaponScriptableObject
                    || playerEquipment.rightHand.rangedWeaponScriptableObject.ammunitionType != playerEquipment.ammunition.ammunitionScriptableObject.type){
                playerEquipment.UnequipSlot(ref playerEquipment.ammunition);
            }
        }
        
        Destroy(gameObject);
    }

    void AddWeaponToRangedContainer(Item weaponItem){
        if(weaponItem.rangedWeaponScriptableObject != null){
            playerEquipment.InstantiateWeapon(weaponItem.worldObject, FindObjectOfType<PlayerCombatAgent>().rangedContainer.transform);
        }
    }

    void AddWeaponToMainHandContainer(Item weaponItem){
        if(weaponItem.meleeWeaponScriptableObject != null){
            playerEquipment.InstantiateWeapon(weaponItem.worldObject, FindObjectOfType<PlayerCombatAgent>().rightHandContainer.transform);

        }
    }
    void AddWeaponToOffHandContainer(Item weaponItem){
        if(weaponItem.meleeWeaponScriptableObject != null){
            playerEquipment.InstantiateWeapon(weaponItem.worldObject, FindObjectOfType<PlayerCombatAgent>().leftHandContainer.transform);
        }
    }
    
    #endregion

    #region Unequipping
    
    public void UnequipItem(PlayerInventory.InventoryItem item){
        playerInventory = FindObjectOfType<PlayerInventory>();
        playerEquipment = FindObjectOfType<PlayerEquipment>();

        // Unequip main hand
        if(playerEquipment.rightHand == item.sObj && playerEquipment.leftHand != item.sObj){
            if(playerEquipment.rightHand.meleeWeaponScriptableObject)
                RemoveWeaponFromMainHandContainer();
            else
                RemoveWeaponFromRangedContainer();

            playerEquipment.UnequipSlot(ref playerEquipment.rightHand);

            if(playerEquipment.leftHand){
                // This function also manages combat instances of weapons
                playerEquipment.OffToMainHand();
            }

        }
        // Unequip off hand
        else if(playerEquipment.rightHand != item.sObj && playerEquipment.leftHand == item.sObj){
            playerEquipment.UnequipSlot(ref playerEquipment.leftHand);
            RemoveWeaponFromOffHandContainer();
        }
        else{
            // This function only gets called if this weapon is equipped somewhere,
            // so if this else statement is reached then a copy of the weapon must 
            // be equipped to both hands
            
            // Unequip the weapon from both hands
            playerEquipment.UnequipSlot(ref playerEquipment.leftHand);
            RemoveWeaponFromOffHandContainer();
            playerEquipment.UnequipSlot(ref playerEquipment.rightHand);
            RemoveWeaponFromMainHandContainer();
        }
    }

    void ClearAllWeaponContainers(){
        foreach(Transform child in FindObjectOfType<PlayerCombatAgent>().rightHandContainer.transform){
            Destroy(child.gameObject);
        }
        foreach(Transform child in FindObjectOfType<PlayerCombatAgent>().leftHandContainer.transform){
            Destroy(child.gameObject);
        }
        foreach(Transform child in FindObjectOfType<PlayerCombatAgent>().rangedContainer.transform){
            if(child.gameObject.name != "ammunitionContainer")
                Destroy(child.gameObject);
        }
    }

    void RemoveWeaponFromRangedContainer(){
        foreach(Transform child in FindObjectOfType<PlayerCombatAgent>().rangedContainer.transform){
            if(child.gameObject.name != "ammunitionContainer")
                Destroy(child.gameObject);
        }
    }

    void RemoveWeaponFromMainHandContainer(){
        foreach(Transform child in FindObjectOfType<PlayerCombatAgent>().rightHandContainer.transform){
            Destroy(child.gameObject);
        }
    }

    void RemoveWeaponFromOffHandContainer(){
        foreach(Transform child in FindObjectOfType<PlayerCombatAgent>().leftHandContainer.transform){
            Destroy(child.gameObject);
        }
    }

    #endregion
}
