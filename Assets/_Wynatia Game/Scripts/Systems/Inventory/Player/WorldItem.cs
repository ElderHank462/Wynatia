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
    }

    IEnumerator InitializeRigidbody(){
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
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
    }

    public void AddToInventory(PlayerInventory inventory){
        inventory.AddItem(new PlayerInventory.InventoryItem(scriptableObject, scriptableObject.worldObject, quantity));
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(positionToSave, 0.2f);
    }
}
