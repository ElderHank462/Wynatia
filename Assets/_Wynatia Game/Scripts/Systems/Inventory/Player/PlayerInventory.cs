using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public GameObject inventoryWindow;
    public Transform playerInventoryContent;
    public ItemActionsMenu itemActionsMenu;
    public DropItemMenu dropItemMenu;
    public ItemInspector itemInspector;
    public GameObject dropItemWarningPopup;
    public GameObject pickupPopup;
    public TextMeshProUGUI pickupBindingText;
    public TextMeshProUGUI pickupItemText;

    public float dropItemDistance = 1.5f;
    public float dropItemInterval = 0.2f;
    public float pickupItemDistance = 2.25f;
    public LayerMask worldItemLayerMask;

    private InventoryItem selectedItem;
    [SerializeField]
    private List<Item> equippedItems = new List<Item>();
    private Transform playerCamera;
    private List<GameObject> worldItemsToInstantiate = new List<GameObject>();
    private Vector3 dropPoint;
    private List<WorldItem> worldItemsInRange = new List<WorldItem>();
    private WorldItem hoveredItem;

    void Start(){       
        playerCamera = Camera.main.transform;
        LoadInventory();
    }

    void Update(){
        if(worldItemsInRange.Count > 0 && !Pause.PauseManagement.paused){
            RaycastHit hit;
            if(Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, pickupItemDistance, worldItemLayerMask)){
                if(hit.collider.gameObject.TryGetComponent<WorldItem>(out WorldItem w))
                    hoveredItem = w;
                else{
                    if(hit.transform.parent.TryGetComponent<WorldItem>(out w)){
                        hoveredItem = w;
                    }
                    else{
                        Debug.LogError("Collider player inventory raycasted for is too deeply nested in the item's GameObject.");
                    }
                }

                pickupItemText.SetText(hoveredItem.scriptableObject.itemName);
                pickupBindingText.SetText(GetComponent<PlayerInput>().currentActionMap.FindAction("pickupItem").GetBindingDisplayString());
                pickupPopup.SetActive(true);
            }
            else{
                hoveredItem = null;
                pickupPopup.SetActive(false);
            }
        }
    }

    void OnInteract(){
        if(hoveredItem != null){
            hoveredItem.AddToInventory(this);
            hoveredItem = null;
        }
    }

    // Called from WorldItem script
    public void StartItemRaycast(WorldItem w){
        worldItemsInRange.Add(w);
    }

    public void StopItemRaycast(WorldItem w){
        worldItemsInRange.Remove(w);

        if(worldItemsInRange.Count == 0)
            pickupPopup.SetActive(false);
    }

    void SaveInventory(){
        ES3.Save("Player_Inventory", this);
    }

    void LoadInventory(){
        if(ES3.KeyExists("Player_Inventory")){
            ES3.LoadInto("Player_Inventory", this);
            SetupItemUIButtons();
            RefreshSelected();
        }
    }

    void OnApplicationQuit(){
        SaveInventory();
    }

    // Called via PlayerInput module
    void OnInventory(){
        if(!inventoryWindow.activeSelf){
            Pause.PauseManagement.Pause();
        }
        inventoryWindow.SetActive(!inventoryWindow.activeSelf);
        if(!inventoryWindow.activeSelf){
            Pause.PauseManagement.Unpause();
            SaveInventory();
            StartCoroutine(InstantiateWorldItems(dropPoint));
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
        // Debug.Log()
        if(SelectedItemEquipped()){
            UnequipSelectedItem();
        }
        if(selectedItem.count > 1 && dropItemMenu.dropQuantitySlider.value < selectedItem.count){
            for (int i = 0; i < dropItemMenu.dropQuantitySlider.value; i++)
            {
                worldItemsToInstantiate.Add(selectedItem.gObj);
            }
            selectedItem.count -= Mathf.RoundToInt(dropItemMenu.dropQuantitySlider.value);
            dropItemMenu.Setup(selectedItem.count, selectedItem.sObj.itemName);
        }
        else{
            for (int i = 0; i < selectedItem.count; i++)
            {
                worldItemsToInstantiate.Add(selectedItem.gObj);
            }
            playerInventory.Remove(selectedItem);
            ToggleItemActionsMenu(selectedItem);
            itemInspector.gameObject.SetActive(false);
            selectedItem = null;
        }
        SetupItemUIButtons();
    }

    public void ConsumeSelectedItem(){
        // Do something based on the item's ItemEffect
        if(selectedItem.count > 1){
            selectedItem.count--;
            dropItemMenu.Setup(selectedItem.count, selectedItem.sObj.itemName);
        }
        else{
            playerInventory.Remove(selectedItem);
            ToggleItemActionsMenu(selectedItem);
            itemInspector.gameObject.SetActive(false);
        }
        SetupItemUIButtons();
    }

    public void UnequipSelectedItem(bool unequipAll = false){
        if(!unequipAll){
            // Remove the item from the equipped items so that when SetupItemUIButtons runs,
            // it toggles the item to be unequipped
            equippedItems.Remove(selectedItem.sObj);
        }
        else{
            // Loop backwards so the indexes of the items we are trying to remove don't change
            for (int i = equippedItems.Count - 1; i > -1; i--)
            {
                if(equippedItems[i] == selectedItem.sObj){
                    equippedItems.RemoveAt(i);
                }
            }
        }

        // Setup the item actions menu to show equip
        itemActionsMenu.Setup(selectedItem);
        // Hide item equipped toggle
        SetupItemUIButtons();
        RefreshSelected();
    }

    public void RemoveFromEquipped(Item r, bool unequipAll = false){
        if(!unequipAll){
            // Remove the item from the equipped items so that when SetupItemUIButtons runs,
            // it toggles the item to be unequipped
            equippedItems.Remove(r);
        }
        else{
            // Loop backwards so the indexes of the items we are trying to remove don't change
            for (int i = equippedItems.Count - 1; i > -1; i--)
            {
                if(equippedItems[i] == r){
                    equippedItems.RemoveAt(i);
                }
            }
        }

        // Setup the item actions menu to show equip
        itemActionsMenu.Setup(selectedItem);
        SetupItemUIButtons();
        RefreshSelected();
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
                if(ItemEquipped(item.sObj)){
                    iUIButton.GetComponent<ItemUIButton>().ToggleEquipped();
                }

                iUIButton.GetComponent<Button>().onClick.AddListener(delegate { ToggleItemActionsMenu(item); });
            }
    }

    public void RefreshSelected(){
        if(selectedItem != null){
            foreach(Transform iB in playerInventoryContent){
                if(iB.GetComponent<ItemUIButton>().sObj == selectedItem.sObj){
                    iB.GetComponent<ItemUIButton>().ToggleSelected(true);
                }
                else{
                    iB.GetComponent<ItemUIButton>().ToggleSelected(false);
                }
            }
        }
        else{
            foreach(Transform iB in playerInventoryContent){
                iB.GetComponent<ItemUIButton>().ToggleSelected(false);
            }
        }
    }

    void ToggleItemActionsMenu(InventoryItem item){
        if(!itemActionsMenu.gameObject.activeSelf){
            // If we're about to activate the item actions menu, set this item to the selected item
            selectedItem = item;
            itemActionsMenu.Setup(selectedItem);
            dropItemMenu.Setup(selectedItem.count, selectedItem.sObj.itemName);
            itemInspector.SetupItemInspector(item.gObj);
            //Activate
            itemActionsMenu.gameObject.SetActive(!itemActionsMenu.gameObject.activeSelf);
            itemInspector.gameObject.SetActive(true);
        }
        else if(item != selectedItem)
        {
            selectedItem = item;
            itemActionsMenu.Setup(selectedItem);
            dropItemMenu.Setup(selectedItem.count, selectedItem.sObj.itemName);
            itemInspector.SetupItemInspector(item.gObj);

            //A different item has been clicked
            //Keep the menu active
        }
        else{
            selectedItem = null;
            //Deactivate
            itemActionsMenu.gameObject.SetActive(!itemActionsMenu.gameObject.activeSelf);
            itemInspector.gameObject.SetActive(false);
        }
        RefreshSelected();
    }

    public IEnumerator InstantiateWorldItems(Vector3 positionToInstantiate){
        if(worldItemsToInstantiate.Count != 0){
            Transform itemContainer = GameObject.FindWithTag("Item Container").transform;
            
            foreach (var item in worldItemsToInstantiate){
                Instantiate(item, positionToInstantiate, Quaternion.identity, itemContainer);
                yield return new WaitForSeconds(dropItemInterval);
            }

            worldItemsToInstantiate.Clear();
        }
        else{
            yield return new WaitForSeconds(0);
        }
        
    }

    public void CheckIfRoomToDrop(){
        // Check forward
        if(!Physics.Raycast(playerCamera.position, playerCamera.forward, dropItemDistance)){
            dropPoint = playerCamera.position + playerCamera.forward * dropItemDistance;
            dropItemMenu.gameObject.SetActive(true);
        }
        // Check backward
        else if(!Physics.Raycast(playerCamera.position, -playerCamera.forward, dropItemDistance)){
            dropPoint = playerCamera.position + -playerCamera.forward * dropItemDistance;
            dropItemMenu.gameObject.SetActive(true);
        }
        // Check right
        else if(!Physics.Raycast(playerCamera.position, playerCamera.right, dropItemDistance)){
            dropPoint = playerCamera.position + playerCamera.right * dropItemDistance;
            dropItemMenu.gameObject.SetActive(true);
        }
        // Check left
        else if(!Physics.Raycast(playerCamera.position, -playerCamera.right, dropItemDistance)){
            dropPoint = playerCamera.position + -playerCamera.right * dropItemDistance;
            dropItemMenu.gameObject.SetActive(true);
        }
        // Check up
        else if(!Physics.Raycast(playerCamera.position, playerCamera.up, dropItemDistance)){
            dropPoint = playerCamera.position + playerCamera.up * dropItemDistance;
            dropItemMenu.gameObject.SetActive(true);
        }
        else{
            dropItemWarningPopup.SetActive(true);
        }
    }

    public void AddToEquippedItemsList(Item item){
        // Add the item to the equipped items so that when SetupItemUIButtons runs,
        // it toggles the item to be equipped
        equippedItems.Add(item);

        // Setup the item actions menu to show unequip
        itemActionsMenu.Setup(selectedItem);

        // Show item equipped toggle
        SetupItemUIButtons();
        RefreshSelected();
    }

    // Checks if SELECTED item is equipped
    public bool SelectedItemEquipped(){
        if(equippedItems.Contains(selectedItem.sObj)){
            return true;
        }
        return false;
    }

    // Checks if INPUTED item is equipped
    public bool ItemEquipped(Item item){
        if(equippedItems.Contains(item)){
            return true;
        }

        return false;
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
