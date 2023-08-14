using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemActionsMenu : MonoBehaviour
{
    [Header("The PlayerInventory and PlayerEquipment classes are auto-retrieved, no need to set them in the inspector.")]
    public PlayerInventory playerInventory;
    public PlayerEquipment playerEquipment;
    public Button dropItemButton;
    public Button itemActionAButton;
    public Button itemActionBButton;
    // public GameObject unequipWeaponPopupR;
    // public GameObject unequipWeaponPopupL;
    

    [SerializeField] private Transform equipperContainer;
    [SerializeField] private GameObject weaponEquipper;
    [SerializeField] private GameObject necklaceEquipper;
    [SerializeField] private GameObject braceletEquipper;
    [SerializeField] private GameObject ringEquipper;
    [SerializeField] private GameObject shieldEquipper;
    [SerializeField] private GameObject armorEquipper;
    [SerializeField] private GameObject clothingEquipper;
    [SerializeField] private GameObject headwearEquipper;


    public void Setup(PlayerInventory.InventoryItem item){
        playerInventory = FindObjectOfType<PlayerInventory>();
        playerEquipment = FindObjectOfType<PlayerEquipment>();

        Item.ItemType itemType = item.sObj.type;

        itemActionBButton.gameObject.SetActive(false);

        itemActionAButton.onClick.RemoveAllListeners();
        itemActionBButton.onClick.RemoveAllListeners();
        TextMeshProUGUI textA = itemActionAButton.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI textB = itemActionBButton.GetComponentInChildren<TextMeshProUGUI>();
        
        dropItemButton.interactable = true;

        foreach (Transform child in equipperContainer)
        {
            Destroy(child.gameObject);
        }

        switch(itemType){
            case Item.ItemType.Currency:
            case Item.ItemType.Trinket:
            case Item.ItemType.MagicTrinket:
            case Item.ItemType.Materials:
                SetButtonStates(false);
                break;
            // Consumables
            case Item.ItemType.Potion:
            case Item.ItemType.Food:
                SetButtonStates(true);
                SetButtonTexts("Consume");
                break;
            
            // Necklace
            case Item.ItemType.Necklace:
                GameObject nE = Instantiate(necklaceEquipper, equipperContainer);
                
                if(!playerInventory.SelectedItemEquipped()){
                    SetButtonTexts("Equip");
                    // Use corresponding Equipper
                    itemActionAButton.onClick.AddListener(delegate {nE.GetComponent<IEquipper>().EquipItem(item);});
                }
                else{
                    SetButtonTexts("Unequip");
                    itemActionAButton.onClick.AddListener(delegate {nE.GetComponent<IEquipper>().UnequipItem(item);});
                    dropItemButton.interactable = false;
                }
                break;

            // Bracelets
            case Item.ItemType.Bracelet:
                GameObject bE = Instantiate(braceletEquipper, equipperContainer);
                
                if(!playerInventory.SelectedItemEquipped()){
                    SetButtonTexts("Equip");
                    // Use corresponding Equipper
                    itemActionAButton.onClick.AddListener(delegate {bE.GetComponent<IEquipper>().EquipItem(item);});
                }
                else{
                    SetButtonTexts("Unequip");
                    itemActionAButton.onClick.AddListener(delegate {bE.GetComponent<IEquipper>().UnequipItem(item);});
                    dropItemButton.interactable = false;
                }
                break;

            // Rings
            case Item.ItemType.Ring:
                GameObject rE = Instantiate(ringEquipper, equipperContainer);
                
                if(!playerInventory.SelectedItemEquipped()){
                    SetButtonTexts("Equip");
                    // Use corresponding Equipper
                    itemActionAButton.onClick.AddListener(delegate {rE.GetComponent<IEquipper>().EquipItem(item);});
                }
                else{
                    SetButtonTexts("Unequip");
                    itemActionAButton.onClick.AddListener(delegate {rE.GetComponent<IEquipper>().UnequipItem(item);});
                    dropItemButton.interactable = false;
                }
                break;

            // Weapons
            case Item.ItemType.Weapon:
            case Item.ItemType.MagicWeapon:
                SetButtonStates(true);
                // Debug.Log("case = weapon or magicweapon");

                GameObject wE = Instantiate(weaponEquipper, equipperContainer);

                if(!playerInventory.SelectedItemEquipped()){
                    SetButtonTexts("Equip");
                    // Use weapon Equipper
                    itemActionAButton.onClick.AddListener(delegate {wE.GetComponent<IEquipper>().EquipItem(item);});

                    // Old version
                    // itemActionAButton.onClick.AddListener(playerInventory.EquipSelectedItem);
                }
                else{
                    SetButtonTexts("Unequip");
                    itemActionAButton.onClick.AddListener(delegate {wE.GetComponent<IEquipper>().UnequipItem(item);});
                    dropItemButton.interactable = false;

                    // Old version
                    // itemActionAButton.onClick.AddListener(delegate {playerEquipment.UnequipWeaponCheck(item);});
                }
                break;

            // The items below are all SINGLE SLOT items
            // Shield
            case Item.ItemType.Shield:
                GameObject sE = Instantiate(shieldEquipper, equipperContainer);
                
                if(!playerInventory.SelectedItemEquipped()){
                    SetButtonTexts("Equip");
                    // Use corresponding Equipper
                    itemActionAButton.onClick.AddListener(delegate {sE.GetComponent<IEquipper>().EquipItem(item);});
                }
                else{
                    SetButtonTexts("Unequip");
                    itemActionAButton.onClick.AddListener(delegate {sE.GetComponent<IEquipper>().UnequipItem(item);});
                    dropItemButton.interactable = false;
                }
                break;
            // Clothing
            case Item.ItemType.Clothing:
            case Item.ItemType.MagicClothing:
                GameObject cE = Instantiate(clothingEquipper, equipperContainer);
                
                if(!playerInventory.SelectedItemEquipped()){
                    SetButtonTexts("Equip");
                    // Use corresponding Equipper
                    itemActionAButton.onClick.AddListener(delegate {cE.GetComponent<IEquipper>().EquipItem(item);});
                }
                else{
                    SetButtonTexts("Unequip");
                    itemActionAButton.onClick.AddListener(delegate {cE.GetComponent<IEquipper>().UnequipItem(item);});
                    dropItemButton.interactable = false;
                }
                break;
            // Headwear
            case Item.ItemType.Headwear:
                GameObject hE = Instantiate(headwearEquipper, equipperContainer);
                
                if(!playerInventory.SelectedItemEquipped()){
                    SetButtonTexts("Equip");
                    // Use corresponding Equipper
                    itemActionAButton.onClick.AddListener(delegate {hE.GetComponent<IEquipper>().EquipItem(item);});
                }
                else{
                    SetButtonTexts("Unequip");
                    itemActionAButton.onClick.AddListener(delegate {hE.GetComponent<IEquipper>().UnequipItem(item);});
                    dropItemButton.interactable = false;
                }
                break;
            // Armor
            case Item.ItemType.Armor:
            case Item.ItemType.MagicArmor:
                GameObject aE = Instantiate(armorEquipper, equipperContainer);
                
                if(!playerInventory.SelectedItemEquipped()){
                    SetButtonTexts("Equip");
                    // Use corresponding Equipper
                    itemActionAButton.onClick.AddListener(delegate {aE.GetComponent<IEquipper>().EquipItem(item);});
                }
                else{
                    SetButtonTexts("Unequip");
                    itemActionAButton.onClick.AddListener(delegate {aE.GetComponent<IEquipper>().UnequipItem(item);});
                    dropItemButton.interactable = false;
                }
                break;
        }

    }

    GameObject ReturnEquipper(Item.ItemType itemType){
        // GameObject e;
        // if(itemType == Item.ItemType.Necklace){
        //     e = necklaceEquipper;
        // }
        // else if(itemType == Item.ItemType.Bracelet){
        //     e = braceletEquipper;
        // }
        // else if(itemType == Item.ItemType.Ring){
        //     e = ringEquipper;
        // }
        // else if(itemType == Item.ItemType.Weapon || itemType == Item.ItemType.MagicWeapon){
        //     e = weaponEquipper;
        // }
        // else if(itemType == Item.ItemType.Shield){
        //     e = shieldEquipper;
        // }
        // else if(itemType == Item.ItemType.Clothing || itemType == Item.ItemType.MagicClothing){
        //     e = clothingEquipper;
        // }
        // else if(itemType == Item.ItemType.Headwear){
        //     e = headwearEquipper;
        // }
        // else if(itemType == Item.ItemType.Armor || itemType == Item.ItemType.MagicArmor){
        //     e = armorEquipper;
        // }
        // else{
        //     e = null;
        // }
        
        switch(itemType){
            // Necklace
            case Item.ItemType.Necklace:
                return necklaceEquipper;
            // Bracelets
            case Item.ItemType.Bracelet:
                return braceletEquipper;
            // Rings
            case Item.ItemType.Ring:
                return ringEquipper;
            // Weapons
            case Item.ItemType.Weapon:
            case Item.ItemType.MagicWeapon:
                return weaponEquipper;
            // Shield
            case Item.ItemType.Shield:
                return shieldEquipper;
            // Clothing
            case Item.ItemType.Clothing:
            case Item.ItemType.MagicClothing:
                return clothingEquipper;
            // Headwear
            case Item.ItemType.Headwear:
                return headwearEquipper;
            // Armor
            case Item.ItemType.Armor:
            case Item.ItemType.MagicArmor:
                return armorEquipper;

            default: 
                return null;
        }
        
        // return e;
    }

    void SetButtonStates(bool a, bool b = false){
        itemActionAButton.gameObject.SetActive(a);
        itemActionBButton.gameObject.SetActive(b);
    }

    void SetButtonTexts(string a, string b = null){
        itemActionAButton.GetComponentInChildren<TextMeshProUGUI>().SetText(a);

        if(b != null)
            itemActionBButton.GetComponentInChildren<TextMeshProUGUI>().SetText(b);
    }

}
