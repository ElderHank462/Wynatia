using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    public enum GearWheelSegment{
        North,
        South,
        East,
        West,
        None
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
    public GameObject gearWheel;
    public GameObject assignGearWheel;

    private InventoryItem selectedItem;
    [SerializeField]
    private List<Item> equippedItems = new List<Item>();
    private Transform playerCamera;
    private List<GameObject> worldItemsToInstantiate = new List<GameObject>();
    private Vector3 dropPoint;
    private List<WorldItem> worldItemsInRange = new List<WorldItem>();
    private WorldItem hoveredItem;
    PlayerInput playerInput;
    private float previousTimeScale = 1;
    private int northSelectedIndex = 0;
    private int eastSelectedIndex = 0;
    private int southSelectedIndex = 0;
    private int westSelectedIndex = 0;
    private List<InventoryItem> northItems = new List<InventoryItem>();
    private List<InventoryItem> eastItems = new List<InventoryItem>();
    private List<InventoryItem> southItems = new List<InventoryItem>();
    private List<InventoryItem> westItems = new List<InventoryItem>();
    [SerializeField] List<Button> gearWheelAssignButtons = new List<Button>();
    [SerializeField] List<Toggle> gearWheelSegments = new List<Toggle>();

    GearWheelSegment selectedSegment = GearWheelSegment.None;


    [Tooltip("Element 0: gear wheel text, Element 1: assignment gear wheel text, Element 2: segment item count text, Element 3: assign item count text")]
    [SerializeField] List<TextMeshProUGUI> northItemTextObjects = new List<TextMeshProUGUI>();
    // [SerializeField] TextMeshProUGUI northItemText;
    [SerializeField] TextMeshProUGUI eastItemText;
    [SerializeField] TextMeshProUGUI southItemText;
    [SerializeField] TextMeshProUGUI westItemText;
    // [SerializeField] TextMeshProUGUI northItemAssignText;
    [SerializeField] TextMeshProUGUI eastItemAssignText;
    [SerializeField] TextMeshProUGUI southItemAssignText;
    [SerializeField] TextMeshProUGUI westItemAssignText;
    [SerializeField] Transform northItemRoom;
    [SerializeField] Transform eastItemRoom;
    [SerializeField] Transform southItemRoom;
    [SerializeField] Transform westItemRoom;

    public RectTransform debugArrow;
    private Vector2 mousePosition = new Vector2();



    void Start(){       
        playerCamera = Camera.main.transform;
        LoadInventory();
        playerInput = GetComponent<PlayerInput>();

        playerInput.actions["Gear Wheel"].started += _ => OpenGearWheel();
        playerInput.actions["Gear Wheel"].canceled += _ => CloseGearWheel(selectedSegment);
        playerInput.actions["Cancel Gear Wheel"].performed += _ => CheckWhichWheelIsOpen();
        playerInput.actions["Change Gear Wheel Item"].performed += _ => ChangeGearWheelItem();

        playerInput.actions["Cancel Gear Wheel"].Disable();
        playerInput.actions["Navigate Gear Wheel"].Disable();

        gearWheel.SetActive(false);
        assignGearWheel.SetActive(false);
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
                pickupBindingText.SetText(GetComponent<PlayerInput>().currentActionMap.FindAction("Interact").GetBindingDisplayString());
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
            if(!ES3.KeyExists("overwriteSaveData")){
                ES3.LoadInto("Player_Inventory", this);
                SetupItemUIButtons();
                RefreshSelected();
            }
        }
    }

    void OnApplicationQuit(){
        SaveInventory();
    }

    // Called via PlayerInput component's broadcasts
    void OnInventory(){
        inventoryWindow.SetActive(!inventoryWindow.activeSelf);

        if(inventoryWindow.activeSelf){
            Pause.PauseManagement.Pause(FindObjectOfType<PlayerInput>());
            playerInput.actions["Gear Wheel"].Disable();
        }
        else{
            Pause.PauseManagement.Unpause();
            playerInput.actions["Gear Wheel"].Enable();
            playerInput.actions["look"].Enable();
            SaveInventory();
            StartCoroutine(InstantiateWorldItems(dropPoint));
        }

        FindObjectOfType<PlayerCombatAgent>().UpdateCombatAgentVariables();
        FindObjectOfType<PlayerCombatAgent>().RepairCombatAgentAfterMenuClose();
    }

    void CheckWhichWheelIsOpen(){
        if(gearWheel.activeSelf){
            CloseGearWheel(GearWheelSegment.None);
        }
        else if(assignGearWheel.activeSelf){
            CloseAssignGearWheel();
        }
    }

    // Called via PlayerInput component's broadcasts
    void OpenGearWheel(){
        List<TextMeshProUGUI> pertinentTextObjects;
        
        // Setup each gear wheel segment
        pertinentTextObjects = new List<TextMeshProUGUI>(){northItemTextObjects[0], northItemTextObjects[2]};
        SetupGearWheelSegment(northItems, northSelectedIndex, pertinentTextObjects, northItemRoom);
        // SetupGearWheelSegment(eastItems, eastSelectedIndex, eastItemText, eastItemRoom);
        // SetupGearWheelSegment(southItems, southSelectedIndex, southItemText, southItemRoom);
        // SetupGearWheelSegment(westItems, westSelectedIndex, westItemText, westItemRoom);
        
        foreach(Toggle toggle in gearWheelSegments){
            toggle.isOn = false;
        }
        
        // Show the gear wheel
        gearWheel.SetActive(true);
        // Slow time
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0.5f;
        // Disable unwanted actions
        playerInput.actions["Sheathe Weapon"].Disable();
        playerInput.actions["Attack"].Disable();
        playerInput.actions["Off-Hand Attack"].Disable();
        playerInput.actions["Inventory"].Disable();
        playerInput.actions["Interact"].Disable();
        playerInput.actions["menu"].Disable();
        playerInput.actions["look"].Disable();
        playerInput.actions["Melee Power Attack"].Disable();
        playerInput.actions["Melee Off-Hand Power Attack"].Disable();
        playerInput.actions["Ranged Attack"].Disable();
        playerInput.actions["Cancel Ranged Attack"].Disable();

        // Enable cancel gear wheel
        playerInput.actions["Cancel Gear Wheel"].Enable();
        playerInput.actions["Navigate Gear Wheel"].Enable();
        
        mousePosition = Vector2.zero;


    }

    // Item Text list elements: Element 0: item name text (for whichever wheel is active), Element 1: segment item count text
    void SetupGearWheelSegment(List<InventoryItem> segmentItems, int selectedIndex, List<TextMeshProUGUI> itemText, Transform itemRoom){
        // If no items in segment, display grayed out text saying "No items assigned"
        foreach(Transform child in itemRoom){
            Destroy(child.gameObject);
        }
        
        if(segmentItems.Count == 0){
            itemText[0].color = Color.gray;
            itemText[0].text = "No items assigned";
            itemText[1].text = "0/8";
        }
        else{
            itemText[0].color = Color.white;
            itemText[0].text = segmentItems[selectedIndex].sObj.itemName + " (" + segmentItems[selectedIndex].count + ")";
            itemText[1].text = segmentItems.Count + "/8";

            
            GameObject itemModel = Instantiate(segmentItems[selectedIndex].gObj, itemRoom.position, itemRoom.rotation, itemRoom);
            itemModel.GetComponent<WorldItem>().instanceKinematic = true;
            Destroy(itemModel.GetComponent<WorldItem>());
            itemModel.GetComponent<Rigidbody>().isKinematic = true;

        }
    }
    
    void OnNavigateGearWheel(){

        Vector2 input = playerInput.actions["Navigate Gear Wheel"].ReadValue<Vector2>();
        if(Mathf.Abs(mousePosition.x + input.x) < 20){
            mousePosition.x += input.x;
        }
        if(Mathf.Abs(mousePosition.y + input.y) < 20){
            mousePosition.y += input.y;
        }

        debugArrow.right = mousePosition;

        float arrowRot = debugArrow.eulerAngles.z;

        GearWheelSegment closestSegment = GearWheelSegment.North;
        
        if(arrowRot >= 45 && arrowRot < 135){
            // North
            closestSegment = GearWheelSegment.North;
        }
        else if(arrowRot >= 0 && arrowRot < 45 || arrowRot >= 315){
            // East
            closestSegment = GearWheelSegment.East;
        }
        else if(arrowRot >= 225 && arrowRot < 315){
            // South
            closestSegment = GearWheelSegment.South;
        }
        else{
            // West
            closestSegment = GearWheelSegment.West;
        }


        if(closestSegment == GearWheelSegment.North){
            gearWheelSegments[0].isOn = true;
        }
        if(closestSegment == GearWheelSegment.East){
            gearWheelSegments[1].isOn = true;
        }
        if(closestSegment == GearWheelSegment.South){
            gearWheelSegments[2].isOn = true;
        }
        if(closestSegment == GearWheelSegment.West){
            gearWheelSegments[3].isOn = true;
        }

        selectedSegment = closestSegment;
    }

    public void OpenWheelToAssign(InventoryItem itemToAssign){
        List<TextMeshProUGUI> pertinentTextObjects;
        
        // Setup each gear wheel segment
        pertinentTextObjects = new List<TextMeshProUGUI>(){northItemTextObjects[1], northItemTextObjects[3]};
        SetupGearWheelSegment(northItems, northSelectedIndex, pertinentTextObjects, northItemRoom);
        // SetupGearWheelSegment(eastItems, eastSelectedIndex, eastItemAssignText, eastItemRoom);
        // SetupGearWheelSegment(southItems, southSelectedIndex, southItemAssignText, southItemRoom);
        // SetupGearWheelSegment(westItems, westSelectedIndex, westItemAssignText, westItemRoom);
        
        assignGearWheel.SetActive(true);

        // Disable unwanted actions
        playerInput.actions["Sheathe Weapon"].Disable();
        playerInput.actions["Attack"].Disable();
        playerInput.actions["Off-Hand Attack"].Disable();
        playerInput.actions["Inventory"].Disable();
        playerInput.actions["Interact"].Disable();
        playerInput.actions["menu"].Disable();
        playerInput.actions["look"].Disable();
        playerInput.actions["Melee Power Attack"].Disable();
        playerInput.actions["Melee Off-Hand Power Attack"].Disable();
        playerInput.actions["Ranged Attack"].Disable();
        playerInput.actions["Cancel Ranged Attack"].Disable();

        // Enable cancel gear wheel
        playerInput.actions["Cancel Gear Wheel"].Enable();

        foreach(Button button in gearWheelAssignButtons){
            button.onClick.RemoveAllListeners();
        }
        gearWheelAssignButtons[0].onClick.AddListener(delegate{ AssignItemToGearWheel(itemToAssign, GearWheelSegment.North);});
        // gearWheelAssignButtons[1].onClick.AddListener(delegate{ AssignItemToGearWheel(itemToAssign, GearWheelSegment.East);});
        // gearWheelAssignButtons[2].onClick.AddListener(delegate{ AssignItemToGearWheel(itemToAssign, GearWheelSegment.South);});
        // gearWheelAssignButtons[3].onClick.AddListener(delegate{ AssignItemToGearWheel(itemToAssign, GearWheelSegment.West);});
    }

    public void AssignItemToGearWheel(InventoryItem item, GearWheelSegment segment){
        // I'll want to add some checks here to ensure an item can only be place in one segment at a time
        // Maybe when adding the item to a segment, check if segments already contain the item
        // List<InventoryItem> SegmentContainingItem() { return List<InventoryItem> or null}
        // If so, delete from segment that contains it and add it to the new one
        // Show warning popup?
        List<InventoryItem> formerlyAssignedSegment = SegmentListContainingItem(item);
        if(formerlyAssignedSegment != null){
            formerlyAssignedSegment.Remove(item);
        }
        
        // Add item to appropriate list
        if(segment == GearWheelSegment.North){
            northItems.Add(item);
            // SetupGearWheelSegment(northItems, northSelectedIndex, new List<TextMeshProUGUI>(){northItemTextObjects[1], northItemTextObjects[3]}, northItemRoom);
        }
        else if(segment == GearWheelSegment.East){
            eastItems.Add(item);
            // SetupGearWheelSegment(eastItems, eastSelectedIndex, eastItemAssignText, eastItemRoom);
        }
        else if(segment == GearWheelSegment.South){           
            southItems.Add(item);
            // SetupGearWheelSegment(southItems, southSelectedIndex, southItemAssignText, southItemRoom);
        }
        else{
            westItems.Add(item);
            // SetupGearWheelSegment(westItems, westSelectedIndex, westItemAssignText, westItemRoom);
        }

        CloseAssignGearWheel();
    }

    void ChangeGearWheelItem(){
        InputAction action = playerInput.actions["Change Gear Wheel Item"];
        
        float scroll = action.ReadValue<Vector2>().y;

        if(scroll > 0){
            // Next item
            ChangeSegmentSelectedItem(selectedSegment, 1);
        }
        else{
            // Previous item
            ChangeSegmentSelectedItem(selectedSegment, -1);
        }
        
    }

    void ChangeSegmentSelectedItem(GearWheelSegment segment, int inc){
        if(segment == GearWheelSegment.North){
            northSelectedIndex = IncrementSegmentSelectedIndex(northItems, northSelectedIndex, inc);

            List<TextMeshProUGUI> pertinentTextObjects; 
            if(gearWheel.activeSelf){
                pertinentTextObjects = new List<TextMeshProUGUI>(){northItemTextObjects[0], northItemTextObjects[2]};
                SetupGearWheelSegment(northItems, northSelectedIndex, pertinentTextObjects, northItemRoom);
            }
            else{
                pertinentTextObjects = new List<TextMeshProUGUI>(){northItemTextObjects[1], northItemTextObjects[3]};
                SetupGearWheelSegment(northItems, northSelectedIndex, pertinentTextObjects, northItemRoom);
            }
        }
        else if(segment == GearWheelSegment.East){

        }
        else if(segment == GearWheelSegment.South){

        }
        else{

        }
    }

    int IncrementSegmentSelectedIndex(List<InventoryItem> segmentList, int i, int inc){
        i++;

        // If outside bounds, carry over
        if(i >= segmentList.Count){
            i = 0;
        }
        else if(i < 0){
            i = segmentList.Count - 1;
        }

        // If position in list is null, carry over
        if(segmentList[i] == null){
            i = 0;
        }

        return i;
    }


    List<InventoryItem> SegmentListContainingItem(InventoryItem itemToSearchFor){
        if(northItems.Contains(itemToSearchFor)){
            return northItems;
        }
        else if(eastItems.Contains(itemToSearchFor)){
            return eastItems;
        }
        else if(southItems.Contains(itemToSearchFor)){
            return southItems;
        }
        else if(westItems.Contains(itemToSearchFor)){
            return westItems;
        }

        return null;
    }

    void CloseAssignGearWheel(){
        // Hide gear wheel
        assignGearWheel.SetActive(false);

        playerInput.actions["Inventory"].Enable();
        playerInput.actions["menu"].Enable();

        // Disable cancel gear wheel
        playerInput.actions["Cancel Gear Wheel"].Disable();
    }

    void CloseGearWheel(GearWheelSegment gearWheelSegment){
        // Hide gear wheel
        gearWheel.SetActive(false);
        // Return time to what it was before opening the gear wheel
        Time.timeScale = previousTimeScale;
        // Enable pause action, other menu actions
        // if(!inventoryWindow.activeSelf){
            playerInput.actions["Sheathe Weapon"].Enable();
            playerInput.actions["Attack"].Enable();
            playerInput.actions["Off-Hand Attack"].Enable();
            playerInput.actions["Interact"].Enable();
            playerInput.actions["Melee Power Attack"].Enable();
            playerInput.actions["Melee Off-Hand Power Attack"].Enable();
            playerInput.actions["Ranged Attack"].Enable();
            playerInput.actions["Cancel Ranged Attack"].Enable();
            playerInput.actions["look"].Enable();
        // }
        playerInput.actions["Inventory"].Enable();
        playerInput.actions["menu"].Enable();

        // Disable cancel gear wheel
        playerInput.actions["Cancel Gear Wheel"].Disable();
        playerInput.actions["Navigate Gear Wheel"].Disable();

        // Equip/use item
        if(gearWheelSegment == GearWheelSegment.North){
            // Pass northItems[northSelectedIndex] into function that will detect item's type and call appropriate function (equip, consume, use, etc.)
            DetectItemTypeAndPerformUseAction(northItems[northSelectedIndex]);
        }
        if(gearWheelSegment == GearWheelSegment.None){
            selectedSegment = GearWheelSegment.None;
        }

    }

    void DetectItemTypeAndPerformUseAction(InventoryItem itemToUse){
        // Consumable
        if(itemToUse.sObj.type == Item.ItemType.Potion ||
           itemToUse.sObj.type == Item.ItemType.Food)
        {
            // Perform item's effect
            ConsumeItem(itemToUse);
        }
        else if(itemToUse.sObj.type == Item.ItemType.Weapon ||
                itemToUse.sObj.type == Item.ItemType.MagicWeapon)
        {
            // Time to overhaul the equipping system
        }

    }


    public void AddItem(InventoryItem a){
        foreach (var item in playerInventory)
        {
            if(item.sObj.itemName == a.sObj.itemName){
                item.count++;
                
                // If item is currently equipped ammunition, update the ammo display
                PlayerEquipment playerEquipment = FindObjectOfType<PlayerEquipment>();
                if(playerEquipment.ammunition)
                {
                    if(playerEquipment.ammunition == a.sObj){
                        // Update ammo display
                        FindObjectOfType<PlayerCombatAgent>().UpdateAmmunitionDisplay();
                    }
                }

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
            RemoveItemFromInventory(selectedItem);
            ToggleItemActionsMenu(selectedItem);
            itemInspector.gameObject.SetActive(false);
            selectedItem = null;
        }
        SetupItemUIButtons();
    }

// Use this function whenever you want to delete an item from the player's inventory
    void RemoveItemFromInventory(InventoryItem itemToRemove){
        playerInventory.Remove(itemToRemove);
        RemoveItemFromGearWheel(itemToRemove);
    }

    void RemoveItemFromGearWheel(InventoryItem itemToRemove){
        List<InventoryItem>[] segmentLists = new List<InventoryItem>[]{ northItems, eastItems, southItems, westItems};

        foreach(var list in segmentLists){
            if(list.Contains(itemToRemove)){
                list.Remove(itemToRemove);
                return;
            }
        }
    }

    public void ConsumeItem(InventoryItem itemToConsume){
        // Do something based on the item's ItemEffect
        itemToConsume.gObj.GetComponent<IConsumable>().Consume(gameObject);
        
        if(itemToConsume.count > 1){
            itemToConsume.count--;
            dropItemMenu.Setup(itemToConsume.count, itemToConsume.sObj.itemName);


        }
        else{
            RemoveItemFromInventory(itemToConsume);
            ToggleItemActionsMenu(itemToConsume);
            itemInspector.gameObject.SetActive(false);
        }
        SetupItemUIButtons();
    }

    public void UnequipSelectedItem(bool unequipAll = false){
        if(!unequipAll){
            // Remove the item from the equipped items so that when SetupItemUIButtons runs,
            // it toggles the item to be unequipped
            if(equippedItems.Contains(selectedItem.sObj))
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
        if(selectedItem != null)
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
                GameObject g = Instantiate(item, positionToInstantiate, Quaternion.identity, itemContainer);
                g.GetComponent<WorldItem>().EnablePhysicsColliders();
                g.GetComponent<WorldItem>().EnablePickup();

                if(g.GetComponent<Projectile>()){
                    g.GetComponent<Projectile>().enabled = false;
                    Debug.Log("projectile script disabled");
                }

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

    public int ReturnItemCount(Item item){
        // If ammo count ever appears as -1, something went wrong
        int ic = -1;
        foreach (var iItem in playerInventory)
        {
            if(iItem.sObj == item){
                ic = iItem.count;
                break;
            }
        }
        return ic;
    }

    public void DecrementItemCount(Item item, bool usingUpAmmunition = false){
        foreach (var iItem in playerInventory)
        {
            if(iItem.sObj == item){
                iItem.count--;

                if(iItem.count <= 0){
                    Debug.Log("item count: " + iItem.count);
                    // Remove item from inventory
                    if(ItemEquipped(item)){
                        Debug.Log("item is equipped");
                        
                        RemoveFromEquipped(item);
                        
                        PlayerEquipment playerEquipment = FindObjectOfType<PlayerEquipment>();
                        Debug.Log("playerEquipment found on gameobject: " + playerEquipment.gameObject.name);
                        playerEquipment.UnequipSlot(ref playerEquipment.ammunition);
                        Debug.Log("ammunition is now: " + playerEquipment.ammunition);
                        selectedItem = null;
                        // Hide item actions and inspector
                        itemActionsMenu.gameObject.SetActive(false);
                        itemInspector.gameObject.SetActive(false);
                    }

                    RemoveItemFromInventory(iItem);
                }

                SetupItemUIButtons();
                return;
            }
        }
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
