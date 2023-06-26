using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    public bool active = true;
    public bool deactivateOnHit = true;
    public int damageAmount = 0;
    
    void OnTriggerEnter(Collider other){
        if(active && other.GetComponent<IDamageable>() != null){
            other.GetComponent<IDamageable>().Damage(damageAmount, other.ClosestPointOnBounds(transform.position));

            if(deactivateOnHit)
                active = false;
        }
    }

}
