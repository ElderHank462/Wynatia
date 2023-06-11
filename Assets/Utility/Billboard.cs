using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Transform cam;

    void Start(){
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }
    
    void Update(){
        transform.LookAt(cam);
    }
}
