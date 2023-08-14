using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddItem : MonoBehaviour
{
    PlayerInventory pI;
    public bool randomlySelect = false;
    public PlayerInventory.InventoryItem[] items;


    void Start(){
        pI = FindObjectOfType<PlayerInventory>();
    }

    private void OnTriggerEnter(Collider other){
        if(randomlySelect && other.CompareTag("Player")){
            int i = Random.Range(0, items.Length);
            pI.AddItem(new PlayerInventory.InventoryItem(items[i].sObj, items[i].gObj, items[i].count));
        }
    }
}
