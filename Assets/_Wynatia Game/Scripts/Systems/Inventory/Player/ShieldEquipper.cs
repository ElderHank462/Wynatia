using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEquipper : MonoBehaviour, IEquipper
{
    [SerializeField] private GameObject replaceOffHandPopup;
    
    private PlayerEquipment pE;
    private Item item;

    public void EquipItem(PlayerInventory.InventoryItem inventoryItemClass){
        pE = FindObjectOfType<PlayerEquipment>();
        item = inventoryItemClass.sObj;

        if(pE.weaponL){
            // Replace or cancel?
            replaceOffHandPopup.SetActive(true);
        }
        else{
            pE.EquipSlot(out pE.weaponL, item);
        }
    }

    public void ConfirmReplaceOffHand(){
        pE.UnequipSlot(ref pE.weaponL);
        pE.EquipSlot(out pE.weaponL, item);
    }

    public void UnequipItem(PlayerInventory.InventoryItem inventoryItemClass){
        pE = FindObjectOfType<PlayerEquipment>();
        item = inventoryItemClass.sObj;

        pE.UnequipSlot(ref pE.weaponL);
    }
}
