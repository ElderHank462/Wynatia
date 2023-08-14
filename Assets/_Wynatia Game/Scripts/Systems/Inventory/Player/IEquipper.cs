using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipper
{
    

    public void EquipItem(PlayerInventory.InventoryItem inventoryItemClass);

    public void UnequipItem(PlayerInventory.InventoryItem inventoryItemClass);

} 
