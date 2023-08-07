using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string type;
    public int value = 0;
}
