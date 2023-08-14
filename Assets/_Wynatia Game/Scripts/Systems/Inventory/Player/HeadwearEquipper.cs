using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadwearEquipper : MonoBehaviour, IEquipper
{
    [SerializeField] private GameObject replaceHeadwearPopup;
    
    private PlayerInventory playerInventory;
    private PlayerEquipment playerEquipment;

    private Item item;
    
    public void EquipItem(PlayerInventory.InventoryItem inventoryItemClass){
        playerEquipment = FindObjectOfType<PlayerEquipment>();

        item = inventoryItemClass.sObj;
        
        if(playerEquipment.headwear){
            // Replace or cancel?
            replaceHeadwearPopup.SetActive(true);
        }
        else{
            // Equip headwear
            playerEquipment.EquipSlot(out playerEquipment.headwear, item);
        }
    }

    public void ConfirmReplace(){
        playerEquipment = FindObjectOfType<PlayerEquipment>();
       
        // Equip headwear
        playerEquipment.UnequipSlot(ref playerEquipment.headwear);
        playerEquipment.EquipSlot(out playerEquipment.headwear, item);
    }

    public void UnequipItem(PlayerInventory.InventoryItem inventoryItemClass){
        playerEquipment = FindObjectOfType<PlayerEquipment>();
        playerEquipment.UnequipSlot(ref playerEquipment.headwear);
    }
}

