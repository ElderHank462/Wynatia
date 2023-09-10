using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponEquipper : MonoBehaviour, IEquipper
{
    [SerializeField] private GameObject raycastBlocker;
    [SerializeField] private GameObject threeOptionPanel;
    [SerializeField] private GameObject fourOptionPanel;

    [SerializeField] private Button threeA;
    [SerializeField] private Button threeB;

    [SerializeField] private Button fourA;
    [SerializeField] private Button fourB;
    [SerializeField] private Button fourC;

    private Item weaponToEquip;
    private PlayerInventory playerInventory;
    private PlayerEquipment playerEquipment;
    private Transform meleeWeaponContainer;

    public enum Layout{
        Equip_MainBoth,
        Equip_MainOff,
        Equip_Off__Replace_Main,
        Replace_MainOffBoth,
        Replace_MainOff
    }

    #region Equipping
    public void EquipItem(PlayerInventory.InventoryItem item){
        playerInventory = FindObjectOfType<PlayerInventory>();
        playerEquipment = FindObjectOfType<PlayerEquipment>();
        meleeWeaponContainer = FindObjectOfType<PlayerCombatAgent>().mainHandContainer.transform;

        Layout popupLayout;

        if(item.count > 1 && !item.sObj.twoHanded){
            if(!playerEquipment.weaponR && !playerEquipment.weaponL){
                // Equip to main, off, both, or cancel?
                popupLayout = Layout.Equip_MainBoth;
            }
            else if(playerEquipment.weaponR && !playerEquipment.weaponL){
                // Equip to off, replace main, or cancel?
                popupLayout = Layout.Equip_Off__Replace_Main;
            }
            else{
                // It isn't possible to have a weapon in the off hand but not in the main hand,
                // so this branch can only be run if there are weapons in both hands

                // Replace main, off, both, or cancel?
                popupLayout = Layout.Replace_MainOffBoth;
            }
        }
        else{
            if(!playerEquipment.weaponR && !playerEquipment.weaponL){
                // Equip to main
                playerEquipment.EquipSlot(out playerEquipment.weaponR, item.sObj);
                AddWeaponToMainHandContainer(item.sObj);
                ClosePopup();
                return;
            }
            else if(playerEquipment.weaponR && !playerEquipment.weaponL){
                // Equip to off, replace main, or cancel?
                popupLayout = Layout.Equip_Off__Replace_Main;
            }
            else{
                // It isn't possible to have a weapon in the off hand but not in the main hand,
                // so this branch can only be run if there are weapons in both hands

                // Replace main, off, or cancel?
                popupLayout = Layout.Replace_MainOff;
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
        }
    }

    void SetupEquipMB(){
        threeA.onClick.RemoveAllListeners();
        threeB.onClick.RemoveAllListeners();

        threeA.onClick.AddListener(delegate {playerEquipment.EquipSlot(out playerEquipment.weaponR, weaponToEquip); AddWeaponToMainHandContainer(weaponToEquip); ClosePopup();});
        threeB.onClick.AddListener(delegate {playerEquipment.EquipSlot(out playerEquipment.weaponR, weaponToEquip); 
            playerEquipment.EquipSlot(out playerEquipment.weaponL, weaponToEquip); AddWeaponToMainHandContainer(weaponToEquip); AddWeaponToOffHandContainer(weaponToEquip); ClosePopup();});

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

            threeA.onClick.AddListener(delegate {playerEquipment.UnequipSlot(ref playerEquipment.weaponR); 
                playerEquipment.EquipSlot(out playerEquipment.weaponR, weaponToEquip); AddWeaponToMainHandContainer(weaponToEquip); ClosePopup();});
            threeB.onClick.AddListener(delegate {playerEquipment.UnequipSlot(ref playerEquipment.weaponL); 
                playerEquipment.EquipSlot(out playerEquipment.weaponL, weaponToEquip); AddWeaponToOffHandContainer(weaponToEquip); ClosePopup();});
                
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

            fourA.onClick.AddListener(delegate {playerEquipment.UnequipSlot(ref playerEquipment.weaponR); 
                playerEquipment.EquipSlot(out playerEquipment.weaponR, weaponToEquip); AddWeaponToMainHandContainer(weaponToEquip); ClosePopup();});
            fourB.onClick.AddListener(delegate {playerEquipment.UnequipSlot(ref playerEquipment.weaponL); 
                playerEquipment.EquipSlot(out playerEquipment.weaponL, weaponToEquip); AddWeaponToOffHandContainer(weaponToEquip); ClosePopup();});
            fourC.onClick.AddListener(delegate {playerEquipment.UnequipSlot(ref playerEquipment.weaponR); 
                playerEquipment.UnequipSlot(ref playerEquipment.weaponL);  
                playerEquipment.EquipSlot(out playerEquipment.weaponR, weaponToEquip);
                playerEquipment.EquipSlot(out playerEquipment.weaponL, weaponToEquip); AddWeaponToMainHandContainer(weaponToEquip); AddWeaponToOffHandContainer(weaponToEquip); 
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

        threeA.onClick.AddListener(delegate {playerEquipment.UnequipSlot(ref playerEquipment.weaponL); 
            playerEquipment.EquipSlot(out playerEquipment.weaponR, weaponToEquip); AddWeaponToMainHandContainer(weaponToEquip); ClosePopup();});
        threeB.onClick.AddListener(delegate {playerEquipment.EquipSlot(out playerEquipment.weaponL, weaponToEquip); AddWeaponToOffHandContainer(weaponToEquip); ClosePopup();});

        string textA = "Main Hand (Replace)";
        string textB = "Off Hand";
        threeA.GetComponentInChildren<TextMeshProUGUI>().SetText(textA);
        threeB.GetComponentInChildren<TextMeshProUGUI>().SetText(textB);

        threeOptionPanel.SetActive(true);
    }

    public void ClosePopup(){
        Destroy(gameObject);
    }


    void AddWeaponToMainHandContainer(Item weaponItem){
        if(weaponItem.meleeWeaponScriptableObject != null){
            foreach(Transform child in FindObjectOfType<PlayerCombatAgent>().mainHandContainer.transform){
                Destroy(child.gameObject);
            }
            
            GameObject g = Instantiate(weaponItem.worldObject, FindObjectOfType<PlayerCombatAgent>().mainHandContainer.transform);
            g.GetComponent<WorldItem>().instanceKinematic = true;
            // Layer 2 is built-in and always equals "Ignore Raycast"
            g.layer = 2;
            foreach(Transform child in g.transform){
                child.gameObject.layer = 2;
            }
        }
    }
    void AddWeaponToOffHandContainer(Item weaponItem){
        if(weaponItem.meleeWeaponScriptableObject != null){
            foreach(Transform child in FindObjectOfType<PlayerCombatAgent>().offHandContainer.transform){
                Destroy(child.gameObject);
            }
            
            GameObject g = Instantiate(weaponItem.worldObject, FindObjectOfType<PlayerCombatAgent>().offHandContainer.transform);
            g.GetComponent<WorldItem>().instanceKinematic = true;
            // Layer 2 is built-in and always equals "Ignore Raycast"
            g.layer = 2;
            foreach(Transform child in g.transform){
                child.gameObject.layer = 2;
            }
        }
    }
    
    #endregion

    #region Unequipping
    
    public void UnequipItem(PlayerInventory.InventoryItem item){
        playerInventory = FindObjectOfType<PlayerInventory>();
        playerEquipment = FindObjectOfType<PlayerEquipment>();

        if(playerEquipment.weaponR == item.sObj && playerEquipment.weaponL != item.sObj){
            playerEquipment.UnequipSlot(ref playerEquipment.weaponR);
            RemoveWeaponFromMainHandContainer();
            if(playerEquipment.weaponL){
                // This function also manages combat instances of weapons
                playerEquipment.OffToMainHand();
            }

        }
        else if(playerEquipment.weaponR != item.sObj && playerEquipment.weaponL == item.sObj){
            playerEquipment.UnequipSlot(ref playerEquipment.weaponL);
            RemoveWeaponFromOffHandContainer();
        }
        else{
            // This function only gets called if this weapon is equipped somewhere,
            // so if this else statement is reached then a copy of the weapon must 
            // be equipped to both hands
            
            // Unequip the weapon from both hands
            playerEquipment.UnequipSlot(ref playerEquipment.weaponL);
            RemoveWeaponFromOffHandContainer();
            playerEquipment.UnequipSlot(ref playerEquipment.weaponR);
            RemoveWeaponFromMainHandContainer();
        }
    }

    void RemoveWeaponFromMainHandContainer(){
        foreach(Transform child in FindObjectOfType<PlayerCombatAgent>().mainHandContainer.transform){
            Destroy(child.gameObject);
        }
    }

    void RemoveWeaponFromOffHandContainer(){
        foreach(Transform child in FindObjectOfType<PlayerCombatAgent>().offHandContainer.transform){
            Destroy(child.gameObject);
        }
    }

    #endregion
}
