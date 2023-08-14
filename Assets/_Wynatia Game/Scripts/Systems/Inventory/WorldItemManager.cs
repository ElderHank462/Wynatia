using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemManager : MonoBehaviour
{
    public List<GameObject> worldItems = new List<GameObject>();
    [Tooltip("This is used to prevent objects from being spawned partway into the ground.")]
    public float spawnHeightOffset = 0.1f;
    
    void Start(){
        if(ES3.KeyExists("World Items List"))
            Load();
        else
            Save();
    }

    void OnApplicationQuit(){
        Save();
    }

    void Save(){
        worldItems.Clear();
        foreach(Transform child in transform){
            worldItems.Add(child.gameObject);
        }
        ES3.Save("World Items List", worldItems);
    }

    void Load(){
        if(ES3.KeyExists("World Items List")){
            worldItems = ES3.Load<List<GameObject>>("World Items List");

            List<Vector3> pos = new List<Vector3>();
            List<Quaternion> rot = new List<Quaternion>();

            for (int i = 0; i < worldItems.Count; i++)
            {
                var prefab = worldItems[i].GetComponent<WorldItem>().scriptableObject.worldObject;
                pos.Add(worldItems[i].transform.position + Vector3.up * spawnHeightOffset);
                rot.Add(worldItems[i].transform.rotation);
                worldItems[i] = prefab;
            }
            foreach(Transform child in transform){
                Destroy(child.gameObject);
            }
            for(int i = 0; i < worldItems.Count; i++){
                Instantiate(worldItems[i], pos[i], rot[i], transform);
            }
        }
    }
    
 
}
