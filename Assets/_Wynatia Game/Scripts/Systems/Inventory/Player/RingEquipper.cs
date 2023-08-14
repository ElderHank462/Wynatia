using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RingEquipper : MonoBehaviour, IEquipper
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private GameObject multipleRingsPopup;
    [SerializeField] private GameObject replaceRingPopup;
    [SerializeField] private Button confirmReplaceButton;
    [SerializeField] private Button confirmAddMultipleButton;
    [SerializeField] private Slider equipAmountSlider;
    [SerializeField] private TextMeshProUGUI equipAmountText;

    private List<Item> equippedRings = new List<Item>();

    private Item item;

    void Start(){
        multipleRingsPopup.SetActive(false);
        replaceRingPopup.SetActive(false);
    }
    
    // Start is called before the first frame update
    public void EquipItem(PlayerInventory.InventoryItem inventoryItemClass)
    {
        PlayerEquipment pE = FindObjectOfType<PlayerEquipment>();
        
        item = inventoryItemClass.sObj;
        
        if(inventoryItemClass.count > 1){
            if(pE.FreeRingSlot()){
                if(pE.NumFreeRingSlots() == 1){
                    pE.EquipSlot(out pE.GetFreeRingSlot(), item);
                }
                else{
                    equipAmountSlider.minValue = 1;
                    if(pE.NumFreeRingSlots() > inventoryItemClass.count)
                        equipAmountSlider.maxValue = inventoryItemClass.count;
                    else
                        equipAmountSlider.maxValue = pE.NumFreeRingSlots();
                    equipAmountSlider.value = 1;

                    equipAmountSlider.onValueChanged.RemoveAllListeners();
                    equipAmountSlider.onValueChanged.AddListener(delegate { equipAmountText.SetText(equipAmountSlider.value.ToString());});
                    equipAmountText.SetText(equipAmountSlider.value.ToString());

                    confirmAddMultipleButton.onClick.RemoveAllListeners();
                    confirmAddMultipleButton.onClick.AddListener(delegate {AddMultiple(Mathf.RoundToInt(equipAmountSlider.value));});

                    multipleRingsPopup.SetActive(true);
                }
            }
            else{
                PopulateEquippedRings();
            }
        }
        else{
            if(pE.FreeRingSlot()){
                pE.EquipSlot(out pE.GetFreeRingSlot(), item);
            }
            else{
                PopulateEquippedRings();
            }


        }
    }

    void AddMultiple(int num){
        PlayerEquipment pE = FindObjectOfType<PlayerEquipment>();

        for (int i = 0; i < num; i++)
        {
            pE.EquipSlot(out pE.GetFreeRingSlot(), item);
        }
    }

    void PopulateEquippedRings(){
        PlayerEquipment pE = FindObjectOfType<PlayerEquipment>();
        List<string> ringNames = new List<string>();

        dropdown.ClearOptions();

        #region Adding Rings to List
        if(pE.ringL1 != null){
            equippedRings.Add(pE.ringL1);
            ringNames.Add(pE.ringL1.itemName + " (Left Hand, Thumb)");
        }
        if(pE.ringL2 != null){
            equippedRings.Add(pE.ringL2);
            ringNames.Add(pE.ringL2.itemName + " (Left Hand, Index Finger)");
        }
        if(pE.ringL3 != null){
            equippedRings.Add(pE.ringL3);
            ringNames.Add(pE.ringL3.itemName + " (Left Hand, Middle Finger)");
        }
        if(pE.ringL4 != null){
            equippedRings.Add(pE.ringL4);
            ringNames.Add(pE.ringL4.itemName + " (Left Hand, Ring Finger)");
        }
        if(pE.ringL5 != null){
            equippedRings.Add(pE.ringL5);
            ringNames.Add(pE.ringL5.itemName + " (Left Hand, Pinky Finger)");
        }
        if(pE.ringR1 != null){
            equippedRings.Add(pE.ringR1);
            ringNames.Add(pE.ringR1.itemName + " (Right Hand, Thumb)");
        }
        if(pE.ringR2 != null){
            equippedRings.Add(pE.ringR2);
            ringNames.Add(pE.ringR2.itemName + " (Right Hand, Index Finger)");
        }
        if(pE.ringR3 != null){
            equippedRings.Add(pE.ringR3);
            ringNames.Add(pE.ringR3.itemName + " (Right Hand, Middle Finger)");
        }
        if(pE.ringR4 != null){
            equippedRings.Add(pE.ringR4);
            ringNames.Add(pE.ringR4.itemName + " (Right Hand, Ring Finger)");
        }
        if(pE.ringR5 != null){
            equippedRings.Add(pE.ringR5);
            ringNames.Add(pE.ringR5.itemName + " (Right Hand, Pinky Finger)");
        }
        #endregion

        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.AddOptions(ringNames);
        dropdown.value = 0;
        confirmReplaceButton.onClick.RemoveAllListeners();
        confirmReplaceButton.onClick.AddListener(delegate { ReplaceRingAtIndex(dropdown.value);});
        dropdown.RefreshShownValue();

        replaceRingPopup.SetActive(true);

    }

    public void ReplaceRingAtIndex(int index){
        PlayerEquipment pE = FindObjectOfType<PlayerEquipment>();

        if(index == 0) {
            pE.UnequipSlot(ref pE.ringL1);
            pE.EquipSlot(out pE.ringL1, item);
        }
        else if(index == 1){
            pE.UnequipSlot(ref pE.ringL2);
            pE.EquipSlot(out pE.ringL2, item);
        }
        else if(index == 2){
            pE.UnequipSlot(ref pE.ringL3);
            pE.EquipSlot(out pE.ringL3, item);
        }
        else if(index == 3){
            pE.UnequipSlot(ref pE.ringL4);
            pE.EquipSlot(out pE.ringL4, item);
        }
        else if(index == 4){
            pE.UnequipSlot(ref pE.ringL5);
            pE.EquipSlot(out pE.ringL5, item);
        }
        else if(index == 5){
            pE.UnequipSlot(ref pE.ringR1);
            pE.EquipSlot(out pE.ringR1, item); 
        }
        else if(index == 6){
            pE.UnequipSlot(ref pE.ringR2);
            pE.EquipSlot(out pE.ringR2, item);  
        } 
        else if(index == 7){
            pE.UnequipSlot(ref pE.ringR3);
            pE.EquipSlot(out pE.ringR3, item); 
        }       
        else if(index == 8){
            pE.UnequipSlot(ref pE.ringR4);
            pE.EquipSlot(out pE.ringR4, item); 
        }      
        else{
            pE.UnequipSlot(ref pE.ringR5);
            pE.EquipSlot(out pE.ringR5, item);
        }
        
    }

    public void UnequipItem(PlayerInventory.InventoryItem itemToUnequip){
        PlayerEquipment pE = FindObjectOfType<PlayerEquipment>();
        
        item = itemToUnequip.sObj;

        #region Check each slot - if it has this item, unequip that slot
        if(pE.ringL1 == item){
            pE.UnequipSlot(ref pE.ringL1);
        }
        if(pE.ringL2 == item){
            pE.UnequipSlot(ref pE.ringL2);
        }
        if(pE.ringL3 == item){
            pE.UnequipSlot(ref pE.ringL3);
        }
        if(pE.ringL4 == item){
            pE.UnequipSlot(ref pE.ringL4);
        }
        if(pE.ringL5 == item){
            pE.UnequipSlot(ref pE.ringL5);
        }
        if(pE.ringR1 == item){
            pE.UnequipSlot(ref pE.ringR1);
        }
        if(pE.ringR2 == item){
            pE.UnequipSlot(ref pE.ringR2);
        }
        if(pE.ringR3 == item){
            pE.UnequipSlot(ref pE.ringR3);
        }
        if(pE.ringR4 == item){
            pE.UnequipSlot(ref pE.ringR4);
        }
        if(pE.ringR5 == item){
            pE.UnequipSlot(ref pE.ringR5);
        }
        #endregion

    }
}
