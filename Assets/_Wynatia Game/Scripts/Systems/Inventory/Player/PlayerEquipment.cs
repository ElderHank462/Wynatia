using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    // public 
    
    public Item headwear;

    public Item necklace;

    public Item braceletL, braceletR;
    
    public Item ringL1, ringL2, ringL3, ringL4, ringL5;
    public Item ringR1, ringR2, ringR3, ringR4, ringR5;

    public Item weaponL, weaponR;

    public Item shield;

    public Item clothing;

    public Item armor;


    private PlayerCombatAgent playerCombatAgent;

    void Start(){
        playerCombatAgent = FindObjectOfType<PlayerCombatAgent>();
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
        if(ES3.KeyExists("Player_Equipment")){
            ES3.LoadInto("Player_Equipment", this);

            playerCombatAgent.mainHandContainer = playerCombatAgent.transform.Find("mainHandContainer").gameObject;
            playerCombatAgent.offHandContainer = playerCombatAgent.transform.Find("offHandContainer").gameObject;
            playerCombatAgent.rangedContainer = playerCombatAgent.transform.Find("rangedContainer").gameObject;

            if(weaponR){
                InstantiateWeapon(weaponR.worldObject, playerCombatAgent.mainHandContainer.transform);
            }
            if(weaponL){
                InstantiateWeapon(weaponL.worldObject, playerCombatAgent.offHandContainer.transform);
            }
            playerCombatAgent.Setup(this);
        }
    }

    #endregion

    public void EquipSlot(out Item slot, Item item){
        slot = item;
        AddToPlayerInventoryList(item);
    }


    public void OffToMainHand(){
        if(weaponL.type != Item.ItemType.Shield){
            Item w = weaponL;
            UnequipSlot(ref weaponL);
            EquipSlot(out weaponR, w);

            foreach(Transform child in playerCombatAgent.offHandContainer.transform){
                Destroy(child.gameObject);
            }

            // foreach(Transform child in playerCombatAgent.mainHandContainer.transform){
            //     Destroy(child.gameObject);
            // }

            InstantiateWeapon(w.worldObject, playerCombatAgent.mainHandContainer.transform);
            playerCombatAgent.UpdateWeaponInstances(this);
            // GameObject g = Instantiate(w.worldObject, FindObjectOfType<PlayerCombatAgent>().mainHandContainer.transform);
            // g.GetComponent<WorldItem>().instanceKinematic = true;
            // // Layer 2 is built-in and always equals "Ignore Raycast"
            // g.layer = 2;
            // foreach(Transform child in g.transform){
            //     child.gameObject.layer = 2;
            // }
        }

        
    }

    public void InstantiateWeapon(GameObject weaponGameObject, Transform parentContainer){
        foreach(Transform child in parentContainer){
            Destroy(child.gameObject);
        }
        
        GameObject g = Instantiate(weaponGameObject, parentContainer);
        g.GetComponent<WorldItem>().instanceKinematic = true;
        // Layer 2 is built-in and always equals "Ignore Raycast"
        g.layer = 2;
        foreach(Transform child in g.transform){
            child.gameObject.layer = 2;
            // If the collider on this transform is for rigidbody physics, disable it
            if(g.GetComponent<WorldItem>().modelColliders.Contains(child.GetComponent<Collider>())){
                child.GetComponent<Collider>().enabled = false;
            }
        }

        playerCombatAgent.UpdateWeaponInstances(this);
    }
    

    public void UnequipSlot(ref Item slot){
        GetComponent<PlayerInventory>().RemoveFromEquipped(slot);
        slot = null;
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

}
