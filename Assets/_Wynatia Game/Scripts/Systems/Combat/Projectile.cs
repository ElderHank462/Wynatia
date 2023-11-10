using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum HitBehavior{
        Destroy,
        DestroyWithEffect,
        Stick,
        ReturnToItem
    }
    
    [Tooltip("Setting this to true will cause the projectile to turn to face its trajectory as it flies.")]
    public bool arrow = true;

    [Tooltip("These layers will cause the projectile to carry out its hitBehavior. If the projectile hits any other layer it will return to being an item.")]
    // public List<string> targetTag;
    public HitBehavior hitBehavior;
    public List<Collider> collidersToIgnore = new List<Collider>();

    private Rigidbody rb;
    [SerializeField] private DamageTrigger damageTrigger;
    
    void Start(){
        rb = GetComponent<Rigidbody>();
    }

    public void Setup(List<Collider> cols, int damage){
        collidersToIgnore = cols;
        damageTrigger.active = true;
        damageTrigger.damageAmount = damage;
    }

    public void Deactivate(){

        damageTrigger.active = false;
        enabled = false;
    }

    void FixedUpdate(){
        if(arrow){
            transform.LookAt(transform.position + rb.velocity.normalized);
        }
    }

    void OnCollisionEnter(Collision collision){

        if(collidersToIgnore.Contains(collision.collider)){
            // Disable colliders
            GetComponent<WorldItem>().DisablePhysicsColliders();
        }
        else{
            // Don't know how to setup different hit behaviors depending on whether target was hit or not
            
            arrow = false;
            // if(targetTag.Cont collision.gameObject.CompareTag(targetTag)){
                GetComponent<ILaunchable>().Hit(collision.collider);
                enabled = false;
            // }
            // else{
            //     // Do the hit behavior
            // }
        }
    }

    void OnTriggerExit(Collider other){
        if(collidersToIgnore.Contains(other) && damageTrigger.active){
            // Enable colliders
            GetComponent<WorldItem>().EnablePhysicsColliders();
        }
    }
}
