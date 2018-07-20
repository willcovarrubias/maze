using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    int maxInventorySize;
    int currentSize;
    public List<Items> items = new List<Items>();

    void Start()
    {
        maxInventorySize = 100; // set this somewhere
        currentSize = 0;
        SetInventory(); // load inventory from player prefs
    }

    void SetInventory()
    {

    }

    public void AddItemToInventory(Items item)
    {
        if (currentSize + item.Size <= maxInventorySize)
        {
            currentSize += item.Size;
            items.Add(item);
        }
    }

    public void RemoveItemFromInventory(Items item)
    {
        currentSize -= item.Size;
        items.Remove(item);
    }

    public int GetMaxInventorySize()
    {
        return maxInventorySize;
    }

    public int GetCurrentSize()
    {
        return currentSize;
    }
}
