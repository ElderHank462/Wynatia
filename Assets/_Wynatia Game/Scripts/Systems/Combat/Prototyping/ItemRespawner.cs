using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRespawner : MonoBehaviour
{
    public GameObject item;
    public Transform itemContainer;

    public float respawnTime = 2;

    public Vector3 position = Vector3.zero;

    bool respawning = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(item, position, Quaternion.identity, itemContainer);
    }

    // Update is called once per frame
    void Update()
    {
        if(itemContainer.childCount == 0 && !respawning){
            respawning = true;
            StartCoroutine(RespawnAfterWait());
        }
    }

    IEnumerator RespawnAfterWait(){
        yield return new WaitForSeconds(respawnTime);
        Instantiate(item, position, Quaternion.identity, itemContainer);
        respawning = false;
    }
}
