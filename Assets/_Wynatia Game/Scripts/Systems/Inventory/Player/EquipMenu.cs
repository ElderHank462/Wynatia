using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipMenu : MonoBehaviour
{
    // Equipment display prefab (contains name and type texts)
        // When selected, updates the button palette
        // Setup function instantiates necessary text objects and populates them based on the item type
        // Should also link up to the item highlighting for the render texture
    [SerializeField] GameObject equipmentDisplayPrefab;
    [SerializeField] ToggleGroup equipmentDisplayToggleGroup;
    [SerializeField] GameObject statDisplayPrefab;
    [SerializeField] Transform oldItemStatPanel;
    [SerializeField] Transform newItemStatPanel;
    [SerializeField] GameObject panel;
    [SerializeField] Transform scrollViewContent;
    [SerializeField] Item itemToEquip;

    [SerializeField] Color betterStatsColor = Color.green;
    [SerializeField] Color worseStatsColor = Color.red;

    [SerializeField] Button confirmEquipButton;

    [SerializeField] TextMeshProUGUI oldItemText;
    [SerializeField] TextMeshProUGUI newItemText;

    private string slotToEquipTo;
    private PlayerEquipment playerEquipment;


    // Setup function
        // takes in slots corresponding to selected item
        // creates equipment displays for each slot
            // get populated with the item (if that slot currently has an item equipped)
        
// TODO disable inventory closing while menu is open


    void Start(){
        panel.SetActive(false);
    }

    public void Setup(Item iTE, Dictionary<string, Item> slots){
        itemToEquip = iTE;
        
        foreach(Transform child in scrollViewContent){
            Destroy(child.gameObject);
        }
        
        foreach (var item in slots)
        {
            if(item.Value != null){
                GameObject d = Instantiate(equipmentDisplayPrefab, scrollViewContent);
                // Set equipment display's item
                d.GetComponent<EquipmentDisplay>().Setup(item, this);
                d.GetComponent<Toggle>().group = equipmentDisplayToggleGroup;
            }
        }

        ResetComparisonWindow();

        panel.SetActive(true);
    }

    public void CloseMenu(){
        panel.SetActive(false);
    }

#region Comparison Window

    class ItemData{
        public float fl = -1f;
        public int i = -1;
        public string st = "null";

        public ItemData(float d){
            fl = d;
        }
        public ItemData(int d){
            i = d;
        }
        public ItemData(string d){
            st = d;
        }

    }

    class ItemComparison{
        public ItemData oldItem;
        public ItemData newItem;

        public ItemComparison(ItemData o, ItemData n){
            oldItem = o;
            newItem = n;
        }
    }

    public void ResetComparisonWindow(){
        foreach (Transform obj in oldItemStatPanel)
        {
            Destroy(obj.gameObject);
        }

        GameObject infoText = Instantiate(statDisplayPrefab, oldItemStatPanel);
        infoText.GetComponent<TextAssigner>().SetText(0, "No item selected");
        infoText.GetComponent<TextAssigner>().SetText(1, "");

        Dictionary<string, ItemData> newItemStats = PopulateItemStats(itemToEquip);
        CreateStatTextObjectsStandalone(newItemStats, newItemStatPanel);

        oldItemText.SetText("");
        newItemText.SetText(itemToEquip.itemName);

        confirmEquipButton.interactable = false;
    }

    public void SetupComparisonWindow(Item oldItem, string iTESlot){
        bool typesMatch;
        if(oldItem.type == itemToEquip.type)
            typesMatch = true;
        else
            typesMatch = false;
        
        oldItemText.SetText(oldItem.itemName);
        newItemText.SetText("(replace with) " + itemToEquip.itemName);

        slotToEquipTo = iTESlot;

        // Store as ints or floats ToStrings() and then try to parse it back out
        // Hopefully this will allow for multiple item types with less duplication of effort

        // Have value be class that can return the appropriate data type
        Dictionary<string, ItemData> oldItemStats = PopulateItemStats(oldItem);
        Dictionary<string, ItemData> newItemStats = PopulateItemStats(itemToEquip);


        // Have dictionary with keys being strings ("base damage", "power attack damage") and values being Vector2s (x = oldItem value, y = new item value)
        // (For matching item types) Have a prefab with three text objects: far left being simple text that has the key string, middle being text that displays x, far right being text displaying y
        // (For non-matching item types) Have a prefab with two text objects: left being stats for old item and right being stats for new item 

        // If item types match (could be shield and weapon, which wouldn't make sense to color code)
        // For strings "Damage:", "Power Attack Damage:", "Weight:", "Sharpness:"
            // Compare the values of x and y: if x is greater, color x text green and y text red; if y is greater, color y text green and x text red
        // For strings "Cooldown Time:", "Power Attack Cooldown Time:"
            // Compare the values of x and y: if x is greater, color x text red and y text green; if y is greater, color y text red and x text green

        // Comparison window will have two panels as children (one for each item)
        // Text object titling each of the items

        if(typesMatch){
            // Dictionary<string, Vector2> itemStats = new Dictionary<string, Vector2>();
            Dictionary<string, ItemComparison> itemStats = new Dictionary<string, ItemComparison>();
            
            switch(oldItem.type){
                case(Item.ItemType.Weapon):
                    if(oldItem.meleeWeaponScriptableObject && itemToEquip.meleeWeaponScriptableObject){
                        itemStats["Damage"] = new ItemComparison(oldItemStats["Damage"], newItemStats["Damage"]);
                        itemStats["Power Attack Damage"] = new ItemComparison(oldItemStats["Power Attack Damage"], newItemStats["Power Attack Damage"]);
                        itemStats["Cooldown Time"] = new ItemComparison(oldItemStats["Cooldown Time"], newItemStats["Cooldown Time"]);
                        itemStats["Power Attack Cooldown Time"] = new ItemComparison(oldItemStats["Power Attack Cooldown Time"], newItemStats["Power Attack Cooldown Time"]);
                        itemStats["Weight"] = new ItemComparison(oldItemStats["Weight"], newItemStats["Weight"]);
                        itemStats["Sharpness"] = new ItemComparison(oldItemStats["Sharpness"], newItemStats["Sharpness"]);
                        itemStats["Traits"] = new ItemComparison(oldItemStats["Traits"], newItemStats["Traits"]);

                        CreateStatTextObjectsComparative(itemStats);
                    }
                    else if (oldItem.rangedWeaponScriptableObject && itemToEquip.rangedWeaponScriptableObject){
                        itemStats["Damage"] = new ItemComparison(oldItemStats["Damage"], newItemStats["Damage"]);
                        itemStats["Draw Time"] = new ItemComparison(oldItemStats["Draw Time"], newItemStats["Draw Time"]);
                        itemStats["Cooldown Time"] = new ItemComparison(oldItemStats["Cooldown Time"], newItemStats["Cooldown Time"]);
                        itemStats["Weight"] = new ItemComparison(oldItemStats["Weight"], newItemStats["Weight"]);
                        itemStats["Sharpness"] = new ItemComparison(oldItemStats["Sharpness"], newItemStats["Sharpness"]);
                        itemStats["Ammunition Type"] = new ItemComparison(oldItemStats["Ammunition Type"], newItemStats["Ammunition Type"]);
                        itemStats["Traits"] = new ItemComparison(oldItemStats["Traits"], newItemStats["Traits"]);

                        CreateStatTextObjectsComparative(itemStats);
                    }
                    else{
                        // comparing a melee and a ranged weapon, can't compare stats
                        SetupNoncomparativeDisplay(oldItemStats, newItemStats);
                    }
                    
                break;
            }

            
        }
        else{
            SetupNoncomparativeDisplay(oldItemStats, newItemStats);
        }
        confirmEquipButton.onClick.RemoveAllListeners();
        confirmEquipButton.onClick.AddListener(delegate { Equip(oldItem); CloseMenu();});

        confirmEquipButton.interactable = true;
        
    }

    void SetupNoncomparativeDisplay(Dictionary<string, ItemData> oldIS, Dictionary<string, ItemData> newIS){
        // Dictionary<string, ItemData> oldIS = new Dictionary<string, ItemData>();
        // Dictionary<string, ItemData> newIS = new Dictionary<string, ItemData>();
        
        // if(!oldMeleeRanged){
        //     MeleeWeapon m = oldItem.meleeWeaponScriptableObject;

        //     // Melee
        //     oldIS["Damage"] = new ItemData(m.baseDamage);
        //     oldIS["Power Attack Damage"] = new ItemData(Mathf.RoundToInt(m.powerAttackDamageMultiplier * m.baseDamage));
        //     oldIS["Cooldown Time"] = new ItemData(m.cooldownTime);
        //     oldIS["Power Attack Cooldown Time"] = new ItemData(m.cooldownTime * m.powerAttackCooldownMultiplier);
        //     oldIS["Weight"] = new ItemData(m.weight);
        //     oldIS["Sharpness"] = new ItemData(m.sharpness);
        //     oldIS["Traits"] = new ItemData(m.baseDamage);
        // }
        // else{
        //     // Ranged
        // }
        
        CreateStatTextObjectsStandalone(oldIS, oldItemStatPanel);
        CreateStatTextObjectsStandalone(newIS, newItemStatPanel);
    }

    Dictionary<string, ItemData> PopulateItemStats(Item itemToReadFrom){
        Dictionary<string, ItemData> dict = new Dictionary<string, ItemData>();
        
        switch(itemToReadFrom.type){
            case(Item.ItemType.Weapon):
            // case(Item.ItemType.MagicWeapon):

                if(itemToReadFrom.meleeWeaponScriptableObject){
                    MeleeWeapon mWSobj = itemToReadFrom.meleeWeaponScriptableObject;
                    dict["Damage"] = new ItemData(mWSobj.baseDamage);
                    dict["Power Attack Damage"] = new ItemData(Mathf.RoundToInt(mWSobj.powerAttackDamageMultiplier * mWSobj.baseDamage));
                    dict["Cooldown Time"] = new ItemData(mWSobj.cooldownTime);
                    dict["Power Attack Cooldown Time"] = new ItemData(mWSobj.cooldownTime * mWSobj.powerAttackCooldownMultiplier);
                    dict["Weight"] = new ItemData(mWSobj.weight);
                    dict["Sharpness"] = new ItemData(mWSobj.sharpness);
                    string weaponTraits = "";
                    if(mWSobj.traits.Length == 0)
                        weaponTraits = "None";
                    else{
                        for (int i = 0; i < mWSobj.traits.Length; i++)
                        {
                            if(i != mWSobj.traits.Length - 1){
                                weaponTraits += mWSobj.traits[i].ToString() + ", ";
                            }
                            else{
                                weaponTraits += mWSobj.traits[i].ToString();
                            }
                        }
                    }
                    dict["Traits"] = new ItemData(weaponTraits);
                }
                else if(itemToReadFrom.rangedWeaponScriptableObject){
                    RangedWeapon rWSobj = itemToReadFrom.rangedWeaponScriptableObject;
                    dict["Damage"] = new ItemData(rWSobj.baseDamage);
                    dict["Draw Time"] = new ItemData(rWSobj.drawTime);
                    dict["Cooldown Time"] = new ItemData(rWSobj.cooldownTime);
                    dict["Weight"] = new ItemData(rWSobj.weight);
                    dict["Sharpness"] = new ItemData(rWSobj.sharpness);
                    dict["Ammunition Type"] = new ItemData(rWSobj.ammunitionType.ToString());
                    string weaponTraits = "";
                    if(rWSobj.traits.Length == 0)
                        weaponTraits = "None";
                    else{
                        for (int i = 0; i < rWSobj.traits.Length; i++)
                        {
                            if(i != rWSobj.traits.Length - 1){
                                weaponTraits += rWSobj.traits[i].ToString() + ", ";
                            }
                            else{
                                weaponTraits += rWSobj.traits[i].ToString();
                            }
                        }
                    }
                    dict["Traits"] = new ItemData(weaponTraits);
                }

            break;
        }


        return dict;
    }

    void CreateStatTextObjectsComparative(Dictionary<string, ItemComparison> comparativeArray){
        foreach (Transform obj in oldItemStatPanel)
        {
            Destroy(obj.gameObject);
        }
        foreach (Transform obj in newItemStatPanel)
        {
            Destroy(obj.gameObject);
        }
        
        foreach (var kVPair in comparativeArray)
        {
            GameObject sO = Instantiate(statDisplayPrefab, oldItemStatPanel);
            GameObject sN = Instantiate(statDisplayPrefab, newItemStatPanel);
            sO.GetComponent<TextAssigner>().SetText(0, kVPair.Key + ":");
            sN.GetComponent<TextAssigner>().SetText(0, kVPair.Key + ":");
            
            if(kVPair.Value.oldItem.fl != -1f){
                sO.GetComponent<TextAssigner>().SetText(1, kVPair.Value.oldItem.fl.ToString());
                sN.GetComponent<TextAssigner>().SetText(1, kVPair.Value.newItem.fl.ToString());
            }
            else if(kVPair.Value.oldItem.i != -1){
                sO.GetComponent<TextAssigner>().SetText(1, kVPair.Value.oldItem.i.ToString());
                sN.GetComponent<TextAssigner>().SetText(1, kVPair.Value.newItem.i.ToString());
            }
            else if(kVPair.Value.oldItem.st != "null"){
                sO.GetComponent<TextAssigner>().SetText(1, kVPair.Value.oldItem.st);
                sN.GetComponent<TextAssigner>().SetText(1, kVPair.Value.newItem.st);
            }


            switch(kVPair.Key){
                        // If item types match (could be shield and weapon, which wouldn't make sense to color code)
                case"Damage":
                case"Power Attack Damage":
                case"Weight":
                case"Sharpness":
                    
                    if(kVPair.Value.oldItem.i > kVPair.Value.newItem.i){
                        sO.GetComponent<TextAssigner>().SetColor(-1, betterStatsColor);
                        sN.GetComponent<TextAssigner>().SetColor(-1, worseStatsColor);
                    }
                    if(kVPair.Value.newItem.i > kVPair.Value.oldItem.i){
                        sN.GetComponent<TextAssigner>().SetColor(-1, betterStatsColor);
                        sO.GetComponent<TextAssigner>().SetColor(-1, worseStatsColor);
                    }

                break;
        // For strings "Damage:", "Power Attack Damage:", "Weight:", "Sharpness:"
            // Compare the values of x and y: if x is greater, color x text green and y text red; if y is greater, color y text green and x text red
        // For strings "Cooldown Time:", "Power Attack Cooldown Time:"
            // Compare the values of x and y: if x is greater, color x text red and y text green; if y is greater, color y text red and x text green

                case"Cooldown Time":
                case"Power Attack Cooldown Time":

                    if(kVPair.Value.oldItem.fl < kVPair.Value.newItem.fl){
                        sO.GetComponent<TextAssigner>().SetColor(-1, betterStatsColor);
                        sN.GetComponent<TextAssigner>().SetColor(-1, worseStatsColor);
                    }
                    if(kVPair.Value.newItem.fl < kVPair.Value.oldItem.fl){
                        sN.GetComponent<TextAssigner>().SetColor(-1, betterStatsColor);
                        sO.GetComponent<TextAssigner>().SetColor(-1, worseStatsColor);
                    }

                break;
            }
        }
    }

    void CreateStatTextObjectsStandalone(Dictionary<string, ItemData> itemStats, Transform container){
        foreach (Transform obj in container)
        {
            Destroy(obj.gameObject);
        }

        foreach (var kVPair in itemStats)
        {
            GameObject statText = Instantiate(statDisplayPrefab, container);
            statText.GetComponent<TextAssigner>().SetText(0, kVPair.Key + ":");
            
            if(kVPair.Value.fl != -1f){
                statText.GetComponent<TextAssigner>().SetText(1, kVPair.Value.fl.ToString());
            }
            else if(kVPair.Value.i != -1){
                statText.GetComponent<TextAssigner>().SetText(1, kVPair.Value.i.ToString());
            }
            else if(kVPair.Value.st != "null"){
                statText.GetComponent<TextAssigner>().SetText(1, kVPair.Value.st);
            }
        }
    }

#endregion


    // TODO: setup weapon equipping with support for one handed, two handed, and ranged weapons
    public void Equip(Item itemToReplace){
        playerEquipment = FindObjectOfType<PlayerEquipment>();
        
        switch (itemToEquip.type)
        {
            case(Item.ItemType.Weapon):
                if(itemToEquip.twoHanded){
                    // Unequip right and left hand slots
                    playerEquipment.UnequipSlot(ref playerEquipment.rightHand, true);
                    playerEquipment.UnequipSlot(ref playerEquipment.leftHand, true);
                    playerEquipment.UnequipSlot(ref playerEquipment.bothHands, true);
                    // Equip item to both hands slot
                    playerEquipment.EquipSlot(out playerEquipment.bothHands, itemToEquip);

                    playerEquipment.PopulateWeaponContainers();
                }
                else{
                    // Unequip slot for itemToReplace (accessed as a string through equipmentDisplay, use if statements/switch blocks to convert)
                    UnequipSlotFromString(slotToEquipTo);

                    if(!itemToReplace.twoHanded){
                        // Equip item to slot found above
                        EquipSlotFromString(slotToEquipTo, itemToEquip);
                        
                    }
                    else{
                        // Equip item to main hand
                        playerEquipment.EquipSlot(out playerEquipment.rightHand, itemToEquip);
                    }

                    if(!playerEquipment.rightHand){
                        playerEquipment.EquipSlot(out playerEquipment.rightHand, FindObjectOfType<PlayerCharacter>().unarmedStrike);
                    }
                    if(!playerEquipment.leftHand){
                        playerEquipment.EquipSlot(out playerEquipment.leftHand, FindObjectOfType<PlayerCharacter>().unarmedStrike);
                    }

                    playerEquipment.PopulateWeaponContainers();
                }
                
            break;
            
        }

    }

#region PlayerEquipment slot management

    void EquipSlotFromString(string st, Item i){
        if(st == "Headwear"){
            playerEquipment.EquipSlot(out playerEquipment.headwear, i);
        }
        else if(st == "Necklace"){
            playerEquipment.EquipSlot(out playerEquipment.necklace, i);
        }
        else if(st == "Left Wrist"){
            playerEquipment.EquipSlot(out playerEquipment.braceletL, i);
        }
        else if(st == "Right Wrist"){
            playerEquipment.EquipSlot(out playerEquipment.braceletR, i);
        }
        else if(st == "Left Thumb"){
            playerEquipment.EquipSlot(out playerEquipment.ringL1, i);
        }
        else if(st == "Left Index Finger"){
            playerEquipment.EquipSlot(out playerEquipment.ringL2, i);
        }
        else if(st == "Left Middle Finger"){
            playerEquipment.EquipSlot(out playerEquipment.ringL3, i);
        }
        else if(st == "Left Ring Finger"){
            playerEquipment.EquipSlot(out playerEquipment.ringL4, i);
        }
        else if(st == "Left Pinky Finger"){
            playerEquipment.EquipSlot(out playerEquipment.ringL5, i);
        }
        else if(st == "Right Thumb"){
            playerEquipment.EquipSlot(out playerEquipment.ringR1, i);
        }
        else if(st == "Right Index Finger"){
            playerEquipment.EquipSlot(out playerEquipment.ringR2, i);
        }
        else if(st == "Right Middle Finger"){
            playerEquipment.EquipSlot(out playerEquipment.ringR3, i);
        }
        else if(st == "Right Ring Finger"){
            playerEquipment.EquipSlot(out playerEquipment.ringR4, i);
        }
        else if(st == "Right Pinky Finger"){
            playerEquipment.EquipSlot(out playerEquipment.ringR5, i);
        }
        else if(st == "Left Hand"){
            if(playerEquipment.rightHand != FindObjectOfType<PlayerCharacter>().unarmedStrike){
                playerEquipment.EquipSlot(out playerEquipment.leftHand, i);
            }
            else{
                playerEquipment.EquipSlot(out playerEquipment.rightHand, i);
            }
        }
        else if(st == "Right Hand"){
            playerEquipment.EquipSlot(out playerEquipment.rightHand, i);
        }
        else if(st == "Both Hands"){
            playerEquipment.EquipSlot(out playerEquipment.bothHands, i);
        }
        else if(st == "Ammunition"){
            playerEquipment.EquipSlot(out playerEquipment.ammunition, i);
        }
        else if(st == "Shield"){
            playerEquipment.EquipSlot(out playerEquipment.shield, i);
        }
        else if(st == "Clothing"){
            playerEquipment.EquipSlot(out playerEquipment.clothing, i);
        }
        else if(st == "Armor"){
            playerEquipment.EquipSlot(out playerEquipment.armor, i);
        }
    }

    void UnequipSlotFromString(string st){
        if(st == "Headwear"){
            playerEquipment.UnequipSlot(ref playerEquipment.headwear);
        }
        else if(st == "Necklace"){
            playerEquipment.UnequipSlot(ref playerEquipment.necklace);
        }
        else if(st == "Left Wrist"){
            playerEquipment.UnequipSlot(ref playerEquipment.braceletL);
        }
        else if(st == "Right Wrist"){
            playerEquipment.UnequipSlot(ref playerEquipment.braceletR);
        }
        else if(st == "Left Thumb"){
            playerEquipment.UnequipSlot(ref playerEquipment.ringL1);
        }
        else if(st == "Left Index Finger"){
            playerEquipment.UnequipSlot(ref playerEquipment.ringL2);
        }
        else if(st == "Left Middle Finger"){
            playerEquipment.UnequipSlot(ref playerEquipment.ringL3);
        }
        else if(st == "Left Ring Finger"){
            playerEquipment.UnequipSlot(ref playerEquipment.ringL4);
        }
        else if(st == "Left Pinky Finger"){
            playerEquipment.UnequipSlot(ref playerEquipment.ringL5);
        }
        else if(st == "Right Thumb"){
            playerEquipment.UnequipSlot(ref playerEquipment.ringR1);
        }
        else if(st == "Right Index Finger"){
            playerEquipment.UnequipSlot(ref playerEquipment.ringR2);
        }
        else if(st == "Right Middle Finger"){
            playerEquipment.UnequipSlot(ref playerEquipment.ringR3);
        }
        else if(st == "Right Ring Finger"){
            playerEquipment.UnequipSlot(ref playerEquipment.ringR4);
        }
        else if(st == "Right Pinky Finger"){
            playerEquipment.UnequipSlot(ref playerEquipment.ringR5);
        }
        else if(st == "Left Hand"){
            playerEquipment.UnequipSlot(ref playerEquipment.leftHand);
        }
        else if(st == "Right Hand"){
            playerEquipment.UnequipSlot(ref playerEquipment.rightHand);
            if(playerEquipment.leftHand != FindObjectOfType<PlayerCharacter>().unarmedStrike &&
                playerEquipment.leftHand.type == Item.ItemType.Weapon){
                playerEquipment.EquipSlot(out playerEquipment.rightHand, playerEquipment.leftHand);
                playerEquipment.UnequipSlot(ref playerEquipment.leftHand);
            }
        }
        else if(st == "Both Hands"){
            playerEquipment.UnequipSlot(ref playerEquipment.bothHands);
        }
        else if(st == "Ammunition"){
            playerEquipment.UnequipSlot(ref playerEquipment.ammunition);
        }
        else if(st == "Shield"){
            playerEquipment.UnequipSlot(ref playerEquipment.shield);
        }
        else if(st == "Clothing"){
            playerEquipment.UnequipSlot(ref playerEquipment.clothing);
        }
        else if(st == "Armor"){
            playerEquipment.UnequipSlot(ref playerEquipment.armor);
        }
    }

    

#endregion



    

}
