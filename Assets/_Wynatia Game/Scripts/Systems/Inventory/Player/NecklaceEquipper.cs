using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecklaceEquipper : MonoBehaviour, IEquipper
{
    [SerializeField] private GameObject replaceNecklacePopup;
    private PlayerEquipment playerEquipment;

    private Item item;
    
    public void EquipItem(PlayerInventory.InventoryItem inventoryItemClass){
        playerEquipment = FindObjectOfType<PlayerEquipment>();

        item = inventoryItemClass.sObj;
        
        if(playerEquipment.necklace){
            // Replace or cancel?
            replaceNecklacePopup.SetActive(true);
        }
        else{
            // Equip necklace
            playerEquipment.EquipSlot(out playerEquipment.necklace, item);
        }
    }

    public void ConfirmReplace(){
        playerEquipment = FindObjectOfType<PlayerEquipment>();
       
        // Equip necklace
        playerEquipment.UnequipSlot(ref playerEquipment.necklace);
        playerEquipment.EquipSlot(out playerEquipment.necklace, item);
    }

    public void UnequipItem(PlayerInventory.InventoryItem inventoryItemClass){
        playerEquipment = FindObjectOfType<PlayerEquipment>();
        playerEquipment.UnequipSlot(ref playerEquipment.necklace);
    }
}

