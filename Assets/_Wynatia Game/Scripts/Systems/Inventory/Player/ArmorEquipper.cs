using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorEquipper : MonoBehaviour, IEquipper
{
    [SerializeField] private GameObject replaceArmorPopup;
    
    private PlayerInventory playerInventory;
    private PlayerEquipment playerEquipment;

    private Item item;
    
    public void EquipItem(PlayerInventory.InventoryItem inventoryItemClass){
        playerInventory = FindObjectOfType<PlayerInventory>();
        playerEquipment = FindObjectOfType<PlayerEquipment>();

        item = inventoryItemClass.sObj;
        
        if(playerEquipment.armor){
            // Replace or cancel?
            replaceArmorPopup.SetActive(true);
        }
        else{
            // Equip armor
            playerEquipment.EquipSlot(out playerEquipment.armor, item);
        }
    }

    public void ConfirmReplace(){
        playerEquipment = FindObjectOfType<PlayerEquipment>();
       
        // Equip armor
        playerEquipment.UnequipSlot(ref playerEquipment.armor);
        playerEquipment.EquipSlot(out playerEquipment.armor, item);
    }

    public void UnequipItem(PlayerInventory.InventoryItem inventoryItemClass){
        playerInventory = FindObjectOfType<PlayerInventory>();
        playerEquipment = FindObjectOfType<PlayerEquipment>();
        playerEquipment.UnequipSlot(ref playerEquipment.armor);
    }
}
