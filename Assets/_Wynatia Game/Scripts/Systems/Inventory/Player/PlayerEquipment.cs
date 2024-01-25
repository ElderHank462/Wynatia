using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEquipment : MonoBehaviour
{
    // public 
    #region Slots
    public Item headwear;

    public Item necklace;

    public Item braceletL, braceletR;
    
    public Item ringL1, ringL2, ringL3, ringL4, ringL5;
    public Item ringR1, ringR2, ringR3, ringR4, ringR5;

    public Item leftHand, rightHand;
    public Item bothHands;
    public Item ammunition;

    public Item shield;

    public Item clothing;

    public Item armor;
#endregion

#region Non-weapon instance containers
    [SerializeField] Transform necklaceContainer;
#endregion

    private PlayerCombatAgent playerCombatAgent;
    private PlayerCharacter playerCharacter;

    #region Weapon Instance Management

    public void PopulateWeaponContainers(){
        // Clear all containers
        ClearAllWeaponContainers();
        // If both hands has an item equipped, instantiate that item, return
        if(bothHands){
            // If melee
            if(bothHands.meleeWeaponScriptableObject){
                AddWeaponToBothHandsContainer(bothHands);
            }
            // If ranged
            else{
                AddWeaponToRangedContainer(bothHands);
            }
            return;
        }
        // Else, instantiate left and right
        else{
            AddWeaponToMainHandContainer(rightHand);
            AddWeaponToOffHandContainer(leftHand);
        }
    }
    void AddWeaponToRangedContainer(Item weaponItem){
        if(weaponItem.rangedWeaponScriptableObject != null){
            InstantiateWeapon(weaponItem.worldObject, FindObjectOfType<PlayerCombatAgent>().rangedContainer.transform);
        }
    }

    public void AddWeaponToMainHandContainer(Item weaponItem){
        if(weaponItem.meleeWeaponScriptableObject != null){
            InstantiateWeapon(weaponItem.worldObject, FindObjectOfType<PlayerCombatAgent>().rightHandContainer.transform);
        }
    }
    void AddWeaponToBothHandsContainer(Item weaponItem){
        if(weaponItem.meleeWeaponScriptableObject != null){
            InstantiateWeapon(weaponItem.worldObject, FindObjectOfType<PlayerCombatAgent>().bothHandsContainer.transform);

        }
    }

    void AddWeaponToOffHandContainer(Item weaponItem){
        if(weaponItem.meleeWeaponScriptableObject != null){
            
            InstantiateWeapon(weaponItem.worldObject, FindObjectOfType<PlayerCombatAgent>().leftHandContainer.transform);
            
        }
    }


    void RemoveWeaponFromRangedContainer(){
        foreach(Transform child in FindObjectOfType<PlayerCombatAgent>().rangedContainer.transform){
            if(child.gameObject.name != "ammunitionContainer")
                Destroy(child.gameObject);
        }
    }

    void RemoveWeaponFromMainHandContainer(){
        foreach(Transform child in FindObjectOfType<PlayerCombatAgent>().rightHandContainer.transform){
            Destroy(child.gameObject);
        }
    }

    void RemoveWeaponFromOffHandContainer(){
        foreach(Transform child in FindObjectOfType<PlayerCombatAgent>().leftHandContainer.transform){
            Destroy(child.gameObject);
        }
    }

    void ClearAllWeaponContainers(){
        foreach(Transform child in FindObjectOfType<PlayerCombatAgent>().rightHandContainer.transform){
            Destroy(child.gameObject);
        }
        foreach(Transform child in FindObjectOfType<PlayerCombatAgent>().leftHandContainer.transform){
            Destroy(child.gameObject);
        }
        foreach(Transform child in FindObjectOfType<PlayerCombatAgent>().bothHandsContainer.transform){
            Destroy(child.gameObject);
        }
        foreach(Transform child in FindObjectOfType<PlayerCombatAgent>().rangedContainer.transform){
            if(child.gameObject.name != "ammunitionContainer")
                Destroy(child.gameObject);
        }
    }

    #endregion
// Updates instances of all non-weapon equipment
    void UpdateEquipmentInstances(){
        PopulateEquipmentContainer(necklaceContainer, necklace, ref necklace);

        
    }
// Checks if the instance found under the container has a scriptable object that matches the sObj
    bool InstanceMatches(Transform container, Item item){
        bool matches = false;

        foreach (Transform child in container)
        {
            if(child.GetComponent<WorldItem>()){
                if(child.GetComponent<WorldItem>().scriptableObject == item){
                    matches = true;
                    break;
                }
            }
        }

        return matches;
    }

    void PopulateEquipmentContainer(Transform container, Item item, ref Item slot){
        if(slot){
            if(!InstanceMatches(container, item)){
                foreach (Transform child in container)
                {
                    Destroy(child.gameObject);
                }

                GameObject g = Instantiate(item.worldObject, container);
                g.GetComponent<WorldItem>().DisablePickup();
                g.GetComponent<WorldItem>().DisablePhysicsColliders();

            }
        }
        else{
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
        }
        
    }

    void Start(){
        playerCombatAgent = FindObjectOfType<PlayerCombatAgent>();
        playerCharacter = FindObjectOfType<PlayerCharacter>();
        LoadEquipment();



    }

    void OnApplicationQuit(){
        SaveEquipment();
    }

    #region Saving and Loading

    void SaveEquipment(){
        ES3.Save("Player_Equipment", this);
    }
    void LoadEquipment(){
        // Equip unarmed strike
        rightHand = FindObjectOfType<PlayerCharacter>().unarmedStrike;
        leftHand = FindObjectOfType<PlayerCharacter>().unarmedStrike;
        
        if(ES3.KeyExists("Player_Equipment") && !ES3.KeyExists("overwriteSaveData")){
            ES3.LoadInto("Player_Equipment", this);

            playerCombatAgent.rightHandContainer = playerCombatAgent.transform.Find("mainHandContainer").gameObject;
            playerCombatAgent.leftHandContainer = playerCombatAgent.transform.Find("offHandContainer").gameObject;
            playerCombatAgent.rangedContainer = playerCombatAgent.transform.Find("rangedContainer").gameObject;

            if(bothHands){
                if(bothHands.meleeWeaponScriptableObject)
                        InstantiateWeapon(bothHands.worldObject, playerCombatAgent.bothHandsContainer.transform);
                    else
                        InstantiateWeapon(bothHands.worldObject, playerCombatAgent.rangedContainer.transform);
            }
            else{
                if(rightHand){
                    if(rightHand.meleeWeaponScriptableObject)
                        InstantiateWeapon(rightHand.worldObject, playerCombatAgent.rightHandContainer.transform);
                    else
                        InstantiateWeapon(rightHand.worldObject, playerCombatAgent.rangedContainer.transform);
                }
                if(leftHand){
                    InstantiateWeapon(leftHand.worldObject, playerCombatAgent.leftHandContainer.transform);
                }
            }
            playerCombatAgent.Setup();
        }

        PopulateWeaponContainers();
        UpdateEquipmentInstances();
    }

    #endregion

    public void EquipSlot(out Item slot, Item item){
        slot = item;
        AddToPlayerInventoryList(item);
        UpdateEquipmentInstances();
    }


    public void OffToMainHand(){
        if(leftHand.type != Item.ItemType.Shield){
            Item w = leftHand;
            UnequipSlot(ref leftHand);
            EquipSlot(out rightHand, w);

            foreach(Transform child in playerCombatAgent.leftHandContainer.transform){
                Destroy(child.gameObject);
            }

            InstantiateWeapon(w.worldObject, playerCombatAgent.rightHandContainer.transform);
            playerCombatAgent.UpdateCombatAgentVariables();

        }

        
    }

    public void InstantiateWeapon(GameObject weaponGameObject, Transform parentContainer){
        foreach(Transform child in parentContainer){
            if(child.gameObject.name != "ammunitionContainer"){
                Destroy(child.gameObject);
            }
        }

        
        GameObject g = Instantiate(weaponGameObject, parentContainer);
        g.GetComponent<WorldItem>().DisablePickup();
        g.GetComponent<WorldItem>().DisablePhysicsColliders();

        playerCombatAgent.UpdateCombatAgentVariables();

        if(g.GetComponent<WorldItem>().scriptableObject.rangedWeaponScriptableObject){
            float drawTime = g.GetComponent<WorldItem>().scriptableObject.rangedWeaponScriptableObject.drawTime;
            // Debug.Log("interaction hold time: " + drawTime);
            InputAction rangedAttackAction = FindObjectOfType<PlayerInput>().actions["Ranged Attack"];

// ******* IF RANGED WEAPON INPUT DOESN'T MATCH WHAT CONTROL MANAGER HAS, LOOK HERE
// ******* THIS IS WHERE I ATTEMPTED TO FIX THAT BUG
            rangedAttackAction.ApplyBindingOverride(new InputBinding{ overrideInteractions = "slowTap(duration=" + drawTime + ")", 
                overridePath = rangedAttackAction.bindings[0].effectivePath});


                // Debug.Log(rangedAttackAction.bindings[0].effectivePath + "; interactions: " + rangedAttackAction.bindings[0].effectiveInteractions);
        }
    }
    

    public void UnequipSlot(ref Item slot, bool dontEquipUnarmedStrike = false){
        GetComponent<PlayerInventory>().RemoveFromEquipped(slot);
        if(slot){
            if(slot.type == Item.ItemType.Weapon && !slot.twoHanded && !dontEquipUnarmedStrike){ // || slot.type == Item.ItemType.MagicWeapon
                slot = FindObjectOfType<PlayerCharacter>().unarmedStrike;
            }
            else{
                slot = null;
            }

        }
        else if(!dontEquipUnarmedStrike){
            slot = FindObjectOfType<PlayerCharacter>().unarmedStrike;
        }

        UpdateEquipmentInstances();
        
    }

    public void UnequipWeapon(Item item){

        // Unequip main hand
        if(rightHand == item && leftHand != item){
            // TODO Ranged weapons should be equipped to the both hands slot
            // if(rightHand.meleeWeaponScriptableObject)
                // RemoveWeaponFromMainHandContainer();
            // else
                // RemoveWeaponFromRangedContainer();

            UnequipSlot(ref rightHand);

            if(leftHand){
                // This function also manages combat instances of weapons
                // OffToMainHand();
            }

        }
        // Unequip off hand
        else if(rightHand != item && leftHand == item){
            UnequipSlot(ref leftHand);
            // RemoveWeaponFromOffHandContainer();
        }
        else if(bothHands == item){
            UnequipSlot(ref bothHands);
            // ClearAllWeaponContainers();
            EquipSlot(out rightHand, playerCharacter.unarmedStrike);
            EquipSlot(out leftHand, playerCharacter.unarmedStrike);
            
        }
        else{
            // This function only gets called if this weapon is equipped somewhere,
            // so if this else statement is reached then a copy of the weapon must 
            // be equipped to both hands
            
            // Unequip the weapon from both hands
            UnequipSlot(ref leftHand);
            // RemoveWeaponFromOffHandContainer();
            UnequipSlot(ref rightHand);
            // RemoveWeaponFromMainHandContainer();
        }
        
        PopulateWeaponContainers();
    }

#region Ring-related functions
    public bool FreeRingSlot(){
        if(!ringL1){
            return true;
        }
        else if(!ringL2){
            return true;
        }
        else if(!ringL3){
            return true;
        }
        else if(!ringL4){
            return true;
        }
        else if(!ringL5){
            return true;
        }
        else if(!ringR1){
            return true;
        }
        else if(!ringR2){
            return true;
        }
        else if(!ringR3){
            return true;
        }
        else if(!ringR4){
            return true;
        }
        else if(!ringR5){
            return true;
        }
        else{
            return false;
        }
    }
    
    
    public int NumFreeRingSlots(){
        int n = 0;
        if(!ringL1){
            n++;
        }
        if(!ringL2){
            n++;
        }
        if(!ringL3){
            n++;
        }
        if(!ringL4){
            n++;
        }
        if(!ringL5){
            n++;
        }
        if(!ringR1){
            n++;
        }
        if(!ringR2){
            n++;
        }
        if(!ringR3){
            n++;
        }
        if(!ringR4){
            n++;
        }
        if(!ringR5){
            n++;
        }
        return n;
    }
    

    public ref Item GetFreeRingSlot(){
        if(!ringL1){
            return ref ringL1;
        }
        else if(!ringL2){
            return ref ringL2;
        }
        else if(!ringL3){
            return ref ringL3;
        }
        else if(!ringL4){
            return ref ringL4;
        }
        else if(!ringL5){
            return ref ringL5;
        }
        else if(!ringR1){
            return ref ringR1;
        }
        else if(!ringR2){
            return ref ringR2;
        }
        else if(!ringR3){
            return ref ringR3;
        }
        else if(!ringR4){
            return ref ringR4;
        }
        else{
            return ref ringR5;
        }
    }
#endregion

    void AddToPlayerInventoryList(Item item){
        GetComponent<PlayerInventory>().AddToEquippedItemsList(item);
    }

    public Dictionary<string, Item> EquipableSlotsForItemType(Item.ItemType itemType){
        Dictionary<string, Item> slotsToReturn = new Dictionary<string, Item>();

        switch(itemType){
            case Item.ItemType.Necklace:
                slotsToReturn.Add("Necklace", necklace);
            break;

            case Item.ItemType.Bracelet:
            break;

            case Item.ItemType.Ring:
            break;

            case Item.ItemType.Weapon:
            // case Item.ItemType.MagicWeapon:
                if(rightHand){ // If right has an item equipped, left should also have an item equipped so no need to check for left hand
                    slotsToReturn.Add("Right Hand", rightHand);
                    slotsToReturn.Add("Left Hand", leftHand);
                }
                else if(bothHands){
                    slotsToReturn.Add("Both Hands", bothHands);
                }
            break;

            case Item.ItemType.Arrows:
            case Item.ItemType.Bolts:
            case Item.ItemType.Projectile:

            break;

            case Item.ItemType.Shield:

            case Item.ItemType.Clothing:
            case Item.ItemType.MagicClothing:

            case Item.ItemType.Headwear:

            case Item.ItemType.Armor:
            case Item.ItemType.MagicArmor:
            break;
        }

        return slotsToReturn;

    }

    public Item ReturnSlotFromString(string st){
        Item slot= null;

        if(st == "Headwear"){
            slot = headwear;
        }
        else if(st == "Necklace"){
            slot = necklace;
        }
        else if(st == "Left Wrist"){
            slot = braceletL;
        }
        else if(st == "Right Wrist"){
            slot = braceletR;
        }
        else if(st == "Left Thumb"){
            slot = ringL1;
        }
        else if(st == "Left Index Finger"){
            slot = ringL2;
        }
        else if(st == "Left Middle Finger"){
            slot = ringL3;
        }
        else if(st == "Left Ring Finger"){
            slot = ringL4;
        }
        else if(st == "Left Pinky Finger"){
            slot = ringL5;
        }
        else if(st == "Right Thumb"){
            slot = ringR1;
        }
        else if(st == "Right Index Finger"){
            slot = ringR2;
        }
        else if(st == "Right Middle Finger"){
            slot = ringR3;
        }
        else if(st == "Right Ring Finger"){
            slot = ringR4;
        }
        else if(st == "Right Pinky Finger"){
            slot = ringR5;
        }
        else if(st == "Left Hand"){
            slot = leftHand;
        }
        else if(st == "Right Hand"){
            slot = rightHand;
        }
        else if(st == "Both Hands"){
            slot = bothHands;
        }
        else if(st == "Ammunition"){
            slot = ammunition;
        }
        else if(st == "Shield"){
            slot = shield;
        }
        else if(st == "Clothing"){
            slot = clothing;
        }
        else if(st == "Armor"){
            slot = armor;
        }


        return slot;
    }

    public void AutoEquip(Item itemToEquip){
        switch(itemToEquip.type){
            case(Item.ItemType.Weapon):
                Item uS = playerCharacter.unarmedStrike;

                if(itemToEquip.twoHanded){
                    UnequipSlot(ref rightHand, true);
                    UnequipSlot(ref leftHand, true);
                    UnequipSlot(ref bothHands);

                    EquipSlot(out bothHands, itemToEquip);
                    PopulateWeaponContainers();
                }
                else{
                    UnequipSlot(ref rightHand);
                    UnequipSlot(ref leftHand);
                    UnequipSlot(ref bothHands, true);

                    EquipSlot(out rightHand, itemToEquip);
                    PopulateWeaponContainers();
                }
            break;
        }
    }

}
