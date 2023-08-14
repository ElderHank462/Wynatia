using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BraceletEquipper : MonoBehaviour, IEquipper
{
    [SerializeField] private GameObject threeOptionPopup;
    [SerializeField] private GameObject fourOptionPopup;
    
    private PlayerInventory playerInventory;
    private PlayerEquipment playerEquipment;

    private Item item;
    
    public void EquipItem(PlayerInventory.InventoryItem inventoryItemClass){
        playerEquipment = FindObjectOfType<PlayerEquipment>();

        item = inventoryItemClass.sObj;
        
        if(inventoryItemClass.count > 1){
            if(!playerEquipment.braceletL && !playerEquipment.braceletR){
                // Equip to one or both wrists?
                UIPopup uIPopup = threeOptionPopup.GetComponent<UIPopup>();
                
                uIPopup.SetText("Do you want to equip this bracelet to one or both wrists?");
                uIPopup.SetButtonText(0, "One");
                uIPopup.SetButtonText(1, "Both");

                uIPopup.SetOnClick(0, delegate {playerEquipment.EquipSlot(out playerEquipment.braceletR, item);});
                uIPopup.SetOnClick(1, delegate {playerEquipment.EquipSlot(out playerEquipment.braceletL, item);},
                    delegate {playerEquipment.EquipSlot(out playerEquipment.braceletR, item);});
                threeOptionPopup.SetActive(true);
            }
            else if(!playerEquipment.braceletR){
                // Equip to one or both wrists?
                UIPopup uIPopup = threeOptionPopup.GetComponent<UIPopup>();
                
                uIPopup.SetText("Do you want to equip this bracelet to one or both wrists?" +
                    " Equipping to both wrists will unequip " + playerEquipment.braceletL.itemName);
                uIPopup.SetButtonText(0, "One");
                uIPopup.SetButtonText(1, "Both");

                uIPopup.SetOnClick(0, delegate {playerEquipment.EquipSlot(out playerEquipment.braceletR, item);});
                uIPopup.SetOnClick(1, delegate {playerEquipment.EquipSlot(out playerEquipment.braceletR, item);},
                    ConfirmReplaceLeft);
                threeOptionPopup.SetActive(true);
            }
            else if(!playerEquipment.braceletL){
                // Equip to one or both wrists?
                UIPopup uIPopup = threeOptionPopup.GetComponent<UIPopup>();

                uIPopup.SetText("Do you want to equip this bracelet to one or both wrists?" +
                    " Equipping to both wrists will unequip " + playerEquipment.braceletR.itemName);
                uIPopup.SetButtonText(0, "One");
                uIPopup.SetButtonText(1, "Both");

                uIPopup.SetOnClick(0, delegate {playerEquipment.EquipSlot(out playerEquipment.braceletL, item);});
                uIPopup.SetOnClick(1, delegate {playerEquipment.EquipSlot(out playerEquipment.braceletL, item);},
                    ConfirmReplaceRight);
                threeOptionPopup.SetActive(true);
            }
            else{
                // Replace left, right, both, or cancel?
                UIPopup uIPopup = fourOptionPopup.GetComponent<UIPopup>();
                
                uIPopup.SetText("Do you want to equip this bracelet to one or both wrists? Current bracelet on left: "
                    + playerEquipment.braceletL.itemName + "; current bracelet on right: " + playerEquipment.braceletR.itemName);
                uIPopup.SetButtonText(0, "Left");
                uIPopup.SetButtonText(1, "Right");
                uIPopup.SetButtonText(2, "Both");
                
                uIPopup.SetOnClick(0, ConfirmReplaceLeft);
                uIPopup.SetOnClick(1, ConfirmReplaceRight);
                uIPopup.SetOnClick(2, ConfirmReplaceBoth);
                fourOptionPopup.SetActive(true);
            }
        }
        else{
            if(!playerEquipment.braceletL && !playerEquipment.braceletR){
                // Equip to right wrist
                playerEquipment.EquipSlot(out playerEquipment.braceletR, item);
            }
            else if(!playerEquipment.braceletL){
                // Equip to left wrist
                playerEquipment.EquipSlot(out playerEquipment.braceletL, item);
            }
            else{
                // Replace left, right, or cancel?
                UIPopup uIPopup = threeOptionPopup.GetComponent<UIPopup>();
                
                uIPopup.SetText("Do you want to replace bracelet on left hand (" +
                    playerEquipment.braceletL.itemName + ") or right hand (" + playerEquipment.braceletR.itemName + ")?");
                
                uIPopup.SetButtonText(0, "Right");
                uIPopup.SetButtonText(1, "Left");
                
                uIPopup.SetOnClick(0, ConfirmReplaceRight);
                uIPopup.SetOnClick(1, ConfirmReplaceLeft);
                
                threeOptionPopup.SetActive(true);
            }
        }
    }

    public void ConfirmReplaceBoth(){
        playerEquipment = FindObjectOfType<PlayerEquipment>();

        playerEquipment.UnequipSlot(ref playerEquipment.braceletL);
        playerEquipment.EquipSlot(out playerEquipment.braceletL, item);
        playerEquipment.UnequipSlot(ref playerEquipment.braceletR);
        playerEquipment.EquipSlot(out playerEquipment.braceletR, item);
    }

    public void ConfirmReplaceLeft(){
        playerEquipment = FindObjectOfType<PlayerEquipment>();
       
        // Equip
        playerEquipment.UnequipSlot(ref playerEquipment.braceletL);
        playerEquipment.EquipSlot(out playerEquipment.braceletL, item);
    }

    public void ConfirmReplaceRight(){
        playerEquipment = FindObjectOfType<PlayerEquipment>();
       
        // Equip
        playerEquipment.UnequipSlot(ref playerEquipment.braceletR);
        playerEquipment.EquipSlot(out playerEquipment.braceletR, item);
    }

    public void UnequipItem(PlayerInventory.InventoryItem inventoryItemClass){
        playerEquipment = FindObjectOfType<PlayerEquipment>();
        
        if(playerEquipment.braceletL == inventoryItemClass.sObj && playerEquipment.braceletR != inventoryItemClass.sObj){
            playerEquipment.UnequipSlot(ref playerEquipment.braceletL);
        }
        else if(playerEquipment.braceletL != inventoryItemClass.sObj && playerEquipment.braceletR == inventoryItemClass.sObj){
            playerEquipment.UnequipSlot(ref playerEquipment.braceletR);
        }
        else{
            playerEquipment.UnequipSlot(ref playerEquipment.braceletL);
            playerEquipment.UnequipSlot(ref playerEquipment.braceletR);
        }

    }
}
