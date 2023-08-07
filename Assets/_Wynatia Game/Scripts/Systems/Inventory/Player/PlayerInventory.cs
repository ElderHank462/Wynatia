using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class InventoryItem{
        public Item sObj;
        public GameObject gObj;
        public int count;

        public InventoryItem(Item s, GameObject g, int c){
            sObj = s;
            gObj = g;
            count = c;
        }
    }
    
    [SerializeField]
    List<InventoryItem> playerInventory = new List<InventoryItem>();

    public GameObject inventoryItemUIButton;
    public Transform playerInventoryContent;
    public Transform itemActionsMenu;
    public DropItemMenu dropItemMenu;

    public Item testItemToAddA;
    public bool testAddItemA;
    public Item testItemToAddB;
    public bool testAddItemB;

    private InventoryItem selectedItem;
    

    void Update(){
        if(testAddItemA){
            InventoryItem newItem = new InventoryItem(testItemToAddA, null, 1);
            AddItem(newItem);
            testAddItemA = false;
        }
        if(testAddItemB){
            InventoryItem newItem = new InventoryItem(testItemToAddB, null, 1);
            AddItem(newItem);
            testAddItemB = false;
        }
    }

    public void AddItem(InventoryItem a){
        foreach (var item in playerInventory)
        {
            if(item.sObj.itemName == a.sObj.itemName){
                item.count++;
                SetupItemUIButtons();
                return;
            }
        }
        
        List<InventoryItem> prevList = new List<InventoryItem>();
        foreach (var item in playerInventory)
        {
            prevList.Add(item);
        }

        playerInventory.Clear();

        if(prevList.Count == 0){
            playerInventory.Add(a);
        }
        else{
            bool aAdded = false;
            
            for (int i = 0; i < prevList.Count; i++)
            {
                if(AlphabeticallyFirst(a.sObj.itemName, prevList[i].sObj.itemName) && !aAdded){
                    playerInventory.Add(a);
                    playerInventory.Add(prevList[i]);
                    aAdded = true;
                }
                else{
                    playerInventory.Add(prevList[i]);
                }
            }

            if(!aAdded){
                playerInventory.Add(a);
            }

            
        }

        SetupItemUIButtons();
    }

    public void DropSelectedItem(){
        if(selectedItem.count > 1 && dropItemMenu.dropQuantitySlider.value < selectedItem.count){
            selectedItem.count -= Mathf.RoundToInt(dropItemMenu.dropQuantitySlider.value);
            dropItemMenu.Setup(selectedItem.count);
        }
        else{
            playerInventory.Remove(selectedItem);
            ToggleItemActionsMenu(selectedItem);
        }
        SetupItemUIButtons();
    }

    void SetupItemUIButtons(){
            foreach (Transform child in playerInventoryContent)
            {
                Destroy(child.gameObject);
            }
            
            foreach (var item in playerInventory)
            {
                GameObject iUIButton = GameObject.Instantiate(inventoryItemUIButton, playerInventoryContent);
                iUIButton.GetComponent<ItemUIButton>().Setup(item.sObj, item.gObj, item.count);
                iUIButton.GetComponent<Button>().onClick.AddListener(delegate { ToggleItemActionsMenu(item); });
            }
    }

    void ToggleItemActionsMenu(InventoryItem item){
        if(!itemActionsMenu.gameObject.activeSelf){
            // If we're about to activate the item actions menu, set this item to the selected item
            selectedItem = item;
            dropItemMenu.Setup(selectedItem.count);
        }
        else{
            selectedItem = null;
        }
        
        itemActionsMenu.gameObject.SetActive(!itemActionsMenu.gameObject.activeSelf);
    }

    bool AlphabeticallyFirst(string a, string b){
        if(a.CompareTo(b) < 0){
            return true;
        }
        else{
            return false;
        }
    }
}
