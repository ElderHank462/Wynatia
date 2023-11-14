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

    // [Tooltip("These layers will cause the projectile to carry out its hitBehavior. If the projectile hits any other layer it will return to being an item.")]
    // public List<string> targetTag;
    // public HitBehavior hitBehavior;
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
            // This is undone in OnTriggerExit below
        }
        else{
            // Don't know how to setup different hit behaviors depending on whether target was hit or not
            // 11.10.23: Maybe just call ILaunchable.Hit whenever and handle different hit behaviors in the ILaunchable
            // make hitEffect variable in IDamageable
            // for example, for a "stick" behavior the "Stick" script could check if collision has IDamageable script
                // if so
                    // if it has a hit effect instantiate it
                    // if the surface is hard don't reparent to it and keep physics colliders enabled and instanceKinematic equal to false
                    // damage the object (should take care of itself with damage trigger)
                    // disable pickup
                // if not
                    // reparent to object and set instanceKinematic to true
                    // enable pickup

            
            arrow = false;
            // if(targetTag.Cont collision.gameObject.CompareTag(targetTag)){
                GetComponent<ILaunchable>().Hit(collision.collider);
                // A little more assertive, perhaps?
                Destroy(this);
                // enabled = false;
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
