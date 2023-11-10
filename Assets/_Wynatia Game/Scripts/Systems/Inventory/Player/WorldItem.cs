using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class WorldItem : MonoBehaviour
{
    public Item scriptableObject;
    public int quantity = 1;
    public Vector3 positionToSave = Vector3.zero;
    public bool instanceKinematic = false;
    public List<Collider> modelColliders = new List<Collider>();

    
    void Start(){
        positionToSave = transform.position;
        StartCoroutine(InitializeRigidbody());
        // Debug.Log("spawned: " + gameObject.name);
    }

    IEnumerator InitializeRigidbody(){
        if(!GetComponent<Rigidbody>().isKinematic){
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        GetComponent<Rigidbody>().isKinematic = true;
        yield return new WaitForEndOfFrame();
        GetComponent<Rigidbody>().isKinematic = instanceKinematic;
    }

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

    void OnCollisionStay(Collision collision){
        List<ContactPoint> contactPoints = new List<ContactPoint>();
        collision.GetContacts(contactPoints);
        positionToSave = transform.position;
        for (int i = 0; i < contactPoints.Count; i++)
        {
            if(contactPoints[i].point.y < positionToSave.y){
                positionToSave = transform.position + Vector3.up * (transform.position.y - contactPoints[i].point.y);
            }
        }
    }

    void OnCollisionExit(){
        // Used to signify that we should save transform position, not lowest contact point
        positionToSave = Vector3.up * -100;
    }

    public void UpdateSavePosition(){
        if(positionToSave == Vector3.up * -100){
            positionToSave = transform.position;
        }
        // Debug.Log(gameObject.name + " position to be saved: " + positionToSave);
    }

    public void AddToInventory(PlayerInventory inventory){
        inventory.AddItem(new PlayerInventory.InventoryItem(scriptableObject, scriptableObject.worldObject, quantity));

        WorldItemManager manager = FindObjectOfType<WorldItemManager>();

        if(manager.worldItems.Contains(gameObject))
            manager.worldItems.Remove(gameObject);

        Destroy(gameObject);
    }

    public void DisablePickup(bool kinematic = true){
        GetComponent<Rigidbody>().isKinematic = kinematic;
        instanceKinematic = kinematic;

        // Layer 2 is built-in and always equals "Ignore Raycast"
        gameObject.layer = 2;

        foreach(Transform child in transform){
            child.gameObject.layer = 2;
        }
    }

    public void EnablePickup(bool kinematic = false){
        GetComponent<Rigidbody>().isKinematic = kinematic;
        instanceKinematic = kinematic;

        // Layer 3 is user defined and equals "Inventory Item"
        gameObject.layer = 3;
        
        foreach(Transform child in transform){
            child.gameObject.layer = 3;
        }
    }

    public void DisablePhysicsColliders(){
        StartCoroutine(WaitThenDisableColliders());
    }

    IEnumerator WaitThenDisableColliders(){
        yield return new WaitForSeconds(Time.fixedDeltaTime);

        foreach(Collider col in modelColliders){
            // If the collider on this transform is for rigidbody physics, enable it
                col.enabled = false;
        }
    }

    public void EnablePhysicsColliders(){
        // foreach(Transform child in transform){
        //     // If the collider on this transform is for rigidbody physics, enable it
        //     if(modelColliders.Contains(child.GetComponent<Collider>())){
        //         child.GetComponent<Collider>().enabled = true;
        //     }
        // }

        foreach(Collider col in modelColliders){
            col.enabled = true;
        }
    }

}
