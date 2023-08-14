using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothingEquipper : MonoBehaviour, IEquipper
{
    [SerializeField] private GameObject replaceClothingPopup;
    private PlayerEquipment playerEquipment;

    private Item item;
    
    public void EquipItem(PlayerInventory.InventoryItem inventoryItemClass){
        playerEquipment = FindObjectOfType<PlayerEquipment>();

        item = inventoryItemClass.sObj;
        
        if(playerEquipment.clothing){
            // Replace or cancel?
            replaceClothingPopup.SetActive(true);
        }
        else{
            // Equip clothing
            playerEquipment.EquipSlot(out playerEquipment.clothing, item);
        }
    }

    public void ConfirmReplace(){
        playerEquipment = FindObjectOfType<PlayerEquipment>();
       
        // Equip clothing
        playerEquipment.UnequipSlot(ref playerEquipment.clothing);
        playerEquipment.EquipSlot(out playerEquipment.clothing, item);
    }

    public void UnequipItem(PlayerInventory.InventoryItem inventoryItemClass){
        playerEquipment = FindObjectOfType<PlayerEquipment>();
        playerEquipment.UnequipSlot(ref playerEquipment.clothing);
    }
}

