using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleController : MonoBehaviour
{
    public enum Reticle{
        Dot,
        X,
        Circle,
        Triangles
    }
    
    [SerializeField] GameObject[] reticles;
    
    void Start(){
        SetReticle((int)Reticle.Dot);
    }

    public void SetReticle(int ret){
        for(int i = 0; i < reticles.Length; i++){
            if(i == ret){
                reticles[i].SetActive(true);
            }
            else{
                reticles[i].SetActive(false);
            }
        }
    }
}
