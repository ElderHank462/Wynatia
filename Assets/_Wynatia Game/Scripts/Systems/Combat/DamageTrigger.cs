using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    
    public bool active = true;
    public bool deactivateOnHit = true;
    public int damageAmount = 0;

    public bool disableParentItemPickupOnHit = false;
    [Header("parentItem only needs to be assigned if disablePickupOnHit is set to true.")]
    public WorldItem parentItem;
    

    void OnTriggerEnter(Collider other){
        if(active && other.GetComponent<IDamageable>() != null){
            other.GetComponent<IDamageable>().Damage(damageAmount, other.ClosestPointOnBounds(transform.position));

            if(deactivateOnHit)
                active = false;

            if(disableParentItemPickupOnHit){
                parentItem.DisablePickup();
            }
        }
    }

}
