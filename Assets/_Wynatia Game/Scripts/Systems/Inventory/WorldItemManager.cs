using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemManager : MonoBehaviour
{
    public List<GameObject> worldItems = new List<GameObject>();
    [Tooltip("This is used to prevent objects from being spawned partway into the ground.")]
    public float spawnHeightOffset = 0.1f;



    void Start(){
        if(ES3.KeyExists("World Items List")){
            Load();
        }
        // if(ES3.KeyExists("World Items Container"))
        //     AltLoad();
    }

    void OnApplicationQuit(){
        Save();
        // AltSave();

    }


    void AltSave(){
        if(ES3.KeyExists("World Items Container")){
            ES3.DeleteKey("World Items Container");
        }
        ES3.Save("World Items Container", transform.GetChild(0).gameObject);
    }

    void AltLoad(){
        foreach(Transform child in transform){
            Destroy(child.gameObject);
        }

        Instantiate(ES3.Load<GameObject>("World Items Container"), Vector3.zero, Quaternion.identity, transform);
    }








    void Save(){
        worldItems.Clear();
        
        WorldItem[] scripts = FindObjectsOfType<WorldItem>();
        foreach (var script in scripts)
        {
            if(script.scriptableObject.type == Item.ItemType.Arrows || script.scriptableObject.type == Item.ItemType.Bolts || script.scriptableObject.type == Item.ItemType.Projectile)
            // If the player saves while drawing their bow, I think this will save the arrow being drawn as a world item...
                script.transform.parent = transform;
        }

        List<bool> kinematicValues = new List<bool>();
        List<Vector3> positions = new List<Vector3>();
        List<Quaternion> quaternions = new List<Quaternion>();

        foreach(Transform child in transform){
            // Created a function in WorldItem that gets the contact point with the lowest y position.
            // Need to save that as the gameobject position instead of the gameobjects actual position.
            // Maybe have it allocated a new gamobject with identical properties to the real one and set
            // the position of that one?
            child.GetComponent<WorldItem>().UpdateSavePosition();
    
            GameObject temp = child.gameObject;

            temp.transform.position = child.GetComponent<WorldItem>().positionToSave;
            temp.transform.rotation = child.rotation;
// Previously this line added "temp" to worldItems
            worldItems.Add(temp);
            positions.Add(temp.transform.position);
            quaternions.Add(temp.transform.rotation);

            kinematicValues.Add(temp.GetComponent<WorldItem>().instanceKinematic);
        }

        // Is world items length correct upon saving?
        // Yes
        // Debug.Log("World Items Length: " + worldItems.Count);

        // Armor is going in here with the correct save position
        // Put breakpoint at loading from ES3 to see if it's correct there
        ES3.Save("World Items List", worldItems);
        ES3.Save("World Items Kinematic Values", kinematicValues);
        ES3.Save("World Item Positions", positions);
        ES3.Save("World Item Quaternions", quaternions);
    }

    void Load(){
        if(ES3.KeyExists("World Items List")){
            // On the third play mode entry, kinematic object (armor) spawns twice even though only one instance was supposed to be saved
            // One spawns kinematic (as intended) but at player's position
            // The other spawns near the bow's position and is not kinematic


            worldItems = ES3.Load<List<GameObject>>("World Items List");


            // Is worldItems the correct length (48 items long) when it's loaded the first and second times? Does it's length change across the loads?
            // Yes, it is correct both times (note that first load is the second play, second load is the third play)
            // No, it's length remains the same (48) for both loads
            // The length remains the same for the third and fourth loads as well
            // Perhaps some hidden error is causing the armor to spawn twice?

            List<bool> kinematicValues = ES3.Load<List<bool>>("World Items Kinematic Values");
            // Is kinematicValues the correct length when it's loaded the first and second times? Does it's length change across the loads?
            // Yes, the length is correct
            // No, it does not change, it remains the correct value (48)

            List<Vector3> pos = ES3.Load<List<Vector3>>("World Item Positions");
            List<Quaternion> rot = ES3.Load<List<Quaternion>>("World Item Quaternions");


            for (int i = 0; i < worldItems.Count; i++)
            {
                var prefab = worldItems[i].GetComponent<WorldItem>().scriptableObject.worldObject;

                // pos.Add(worldItems[i].transform.position + Vector3.up * spawnHeightOffset);
                // rot.Add(worldItems[i].transform.rotation);
                worldItems[i] = prefab;
            }
            // After adding all of the saved worldItems positions and rotations to lists, are the lists the correct length? Do they change across loads?
            // Yes, the lists are the correct length across all loads

            foreach(Transform child in transform){
                Destroy(child.gameObject);
            }
            // Is worldItems count still correct?
            // Yes
            // Debug.Log("worldItems Length: " + worldItems.Count);
            
            for(int i = 0; i < worldItems.Count; i++){
                GameObject g = Instantiate(worldItems[i], pos[i], rot[i], transform);
                // if(worldItems[i].GetComponent<WorldItem>().scriptableObject.type == Item.ItemType.Armor)
                // Debug.Log("Instantiated: " + worldItems[i].name);
                WorldItem wI = g.GetComponent<WorldItem>();
                wI.instanceKinematic = kinematicValues[i];
                // Don't want to enable physics colliders for projectiles
                if(wI.scriptableObject.type != Item.ItemType.Arrows || wI.scriptableObject.type != Item.ItemType.Bolts || wI.scriptableObject.type != Item.ItemType.Projectile){
                    wI.EnablePhysicsColliders();
                }
                wI.EnablePickup(kinematicValues[i]);


                // Debug.Log(wI.scriptableObject.itemName + " kinematic: " + wI.instanceKinematic);
            }

            worldItems.Clear();
        }
    }

    public void AddMe(GameObject gObj){
        if(!worldItems.Contains(gObj))
            worldItems.Add(gObj);
    }
    
 
}
