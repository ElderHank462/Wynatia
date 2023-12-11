using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipMenu : MonoBehaviour
{
    // Equipment display prefab (contains name and type texts)
        // When selected, updates the button palette
        // Setup function instantiates necessary text objects and populates them based on the item type
        // Should also link up to the item highlighting for the render texture
    [SerializeField] GameObject equipmentDisplayPrefab;
    [SerializeField] Transform scrollViewContent;


    // Setup function
        // takes in slots corresponding to selected item
        // creates equipment displays for each slot
            // get populated with the item (if that slot currently has an item equipped)
        



    void Start(){
        gameObject.SetActive(false);
    }

    public void Setup(Item[] slots){
        foreach (var item in slots)
        {
            GameObject d = Instantiate(equipmentDisplayPrefab, scrollViewContent);
            // Set equipment display's item
        }


    }


}
