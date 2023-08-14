using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class WorldItem : MonoBehaviour
{
    public Item scriptableObject;
    public int quantity = 1;

    void OnTriggerEnter(Collider other){
        if(other.CompareTag("Player")){
            other.GetComponent<PlayerInventory>().StartItemRaycast(this);
        }
    }
    void OnTriggerExit(Collider other){
        if(other.CompareTag("Player")){
            other.GetComponent<PlayerInventory>().StopItemRaycast(this);
        }
    }

    public void AddToInventory(PlayerInventory inventory){
        inventory.AddItem(new PlayerInventory.InventoryItem(scriptableObject, scriptableObject.worldObject, quantity));
        Destroy(gameObject);
    }
}
