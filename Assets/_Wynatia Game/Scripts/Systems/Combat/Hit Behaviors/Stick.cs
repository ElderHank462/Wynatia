using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour, ILaunchable
{
    public void Hit(Collider hit){

        if(transform.parent){
            if(!transform.parent.CompareTag("Item Container"))
            {
                GetComponent<WorldItem>().EnablePickup(true);
                GetComponent<WorldItem>().DisablePhysicsColliders();
                transform.SetParent(hit.transform);
            }
        }
        else{
            GetComponent<WorldItem>().EnablePickup(true);
            GetComponent<WorldItem>().DisablePhysicsColliders();
            transform.SetParent(hit.transform);
        }
    }
}
