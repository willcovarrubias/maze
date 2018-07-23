using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    int maxInventorySize;
    int currentSize;
    public List<Inventory> playerItems = new List<Inventory>();

    void Start()
    {
        maxInventorySize = 100; // set this somewhere
        currentSize = 0;
        //LoadInventory();
    }

    /*
    private void Update()
    {
        if (Input.GetKeyDown("l"))
        {
            Debug.Log("Loading...");
            LoadInventory();
            for (int i = 0; i < playerItems.Count; i++)
            {
                Debug.Log(playerItems[i].Item.Title + " " + playerItems[i].Count);
            }
        }
        if (Input.GetKeyDown("a"))
        {
            Debug.Log("SAVING...");
            AddItemToInventory(GetComponent<ItemDatabase>().FetchItemByID(1000));
            for (int i = 0; i < playerItems.Count; i++)
            {
                Debug.Log(playerItems[i].Item.Title + " " + playerItems[i].Count);
            }
            Debug.Log("Current Size " + currentSize);
            SaveInventory();
        }
        if (Input.GetKeyDown("c"))
        {
            PlayerPrefs.DeleteAll();
        }
        if (Input.GetKeyDown("p"))
        {
            GetComponent<ItemDatabase>().DisplayAllItems();
        }
    }
    */


    public void AddItemToInventory(Items item)
    {
        if (currentSize + item.Size <= maxInventorySize)
        {
            if (IsWeapon(item.ID))
            {
                Inventory newItem = new Inventory(item, 1);
                playerItems.Add(newItem);
                currentSize += item.Size;
            }
            else
            {
                for (int i = 0; i < playerItems.Count; i++)
                {
                    if (playerItems[i].Item.ID == item.ID)
                    {
                        playerItems[i].Count++;
                        currentSize += item.Size;
                        return;
                    }
                }
                Inventory newItem = new Inventory(item, 1);
                playerItems.Add(newItem);
                currentSize += item.Size;
            }
        }
    }

    public void RemoveItemFromInventory(Inventory item)
    {
        currentSize -= item.Item.Size;
        if (IsWeapon(item.Item.ID))
        {
            playerItems.Remove(item);
        }
        else
        {
            for (int i = 0; i < playerItems.Count; i++)
            {
                if (playerItems[i].Item.ID == item.Item.ID)
                {
                    playerItems[i].Count--;
                    if (playerItems[i].Count == 0)
                    {
                        playerItems.RemoveAt(i);
                    }
                    break;
                }
            }
        }
    }

    public int GetMaxInventorySize()
    {
        return maxInventorySize;
    }

    public int GetCurrentSize()
    {
        return currentSize;
    }

    public void SaveInventory()
    {
        int i;
        for (i = 0; i < playerItems.Count; i++)
        {
            PlayerPrefs.SetInt("Player Item ID" + i, playerItems[i].Item.ID);
            PlayerPrefs.SetInt("Player Item Count" + i, playerItems[i].Count);
            if (IsWeapon(playerItems[i].Item.ID))
            {
                Weapons weapon = (Weapons)playerItems[i].Item;
                PlayerPrefs.SetInt("Player Item Duribility", weapon.Durability);
            }
        }
        PlayerPrefs.SetInt("Player Item Count", i);
        PlayerPrefs.Save();
    }

    public void LoadInventory()
    {
        int itemCount = PlayerPrefs.GetInt("Player Item Count");
        playerItems.Clear();
        currentSize = 0;
        for (int i = 0; i < itemCount; i++)
        {
            int id = PlayerPrefs.GetInt("Player Item ID" + i);
            int count = PlayerPrefs.GetInt("Player Item Count" + i);
            if (IsWeapon(id))
            {
                int duribility = PlayerPrefs.GetInt("Player Item Duribility");
                Weapons weapon = GetComponent<WeaponDatabase>().FetchWeaponByID(id);
                Inventory loadedItem = new Inventory(weapon, count);
                weapon.Durability = duribility;
                playerItems.Add(loadedItem);
                currentSize = weapon.Size * count;
            }
            else
            {
                Items item = GetComponent<ItemDatabase>().FetchItemByID(id);
                Inventory loadedItem = new Inventory(item, count);
                playerItems.Add(loadedItem);
                currentSize = item.Size * count;
            }
        }
    }

    public bool IsWeapon(int id)
    {
        if (id >= 2000 && id < 3000)
        {
            return true;
        }
        return false;
    }
}
