using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageInventoryManager : MonoBehaviour
{
    List<Inventory> villageItems = new List<Inventory>();

    /*
    private void Update()
    {
        if (Input.GetKeyDown("l"))
        {
            Debug.Log("Loading...");
            LoadFromPlayerPrefs();
            for (int i = 0; i < villageItems.Count; i++)
            {
                Debug.Log(villageItems[i].Item.Title + " " + villageItems[i].Count);
            }
        }
        if (Input.GetKeyDown("a"))
        {
            Debug.Log("SAVING...");
            AddItem(GetComponent<ItemDatabase>().FetchItemByID(1000));
            AddItem(GetComponent<ItemDatabase>().FetchItemByID(2000));
            AddItem(GetComponent<ItemDatabase>().FetchItemByID(2000));
            AddItem(GetComponent<ItemDatabase>().FetchItemByID(3000));
            AddItem(GetComponent<ItemDatabase>().FetchItemByID(3000));
            for (int i = 0; i < villageItems.Count; i++)
            {
                Debug.Log(villageItems[i].Item.Title + " " + villageItems[i].Count);
            }
            SaveToPlayerPrefs();
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

    public void AddItem(Items item)
    {
        if (IsWeapon(item.ID))
        {
            Inventory newItem = new Inventory(item, 1);
            villageItems.Add(newItem);
        }
        else
        {
            for (int i = 0; i < villageItems.Count; i++)
            {
                if (villageItems[i].Item.ID == item.ID)
                {
                    villageItems[i].Count += 1;
                    return;
                }
            }
            Inventory newItem = new Inventory(item, 1);
            villageItems.Add(newItem);
        }
    }

    public void MoveItemToInventory(Items item)
    {
        for (int i = 0; i < villageItems.Count; i++)
        {
            if (villageItems[i].Item.ID == item.ID)
            {
                if (IsWeapon(item.ID))
                {
                    villageItems.RemoveAt(i);
                }
                else
                {
                    villageItems[i].Count--;
                    if (villageItems[i].Count == 0)
                    {
                        villageItems.RemoveAt(i);
                    }
                }
                break;
            }
        }
    }

    public void SaveToPlayerPrefs()
    {
        int i;
        for (i = 0; i < villageItems.Count; i++)
        {
            PlayerPrefs.SetInt("Item ID" + i, villageItems[i].Item.ID);
            PlayerPrefs.SetInt("Item Count" + i, villageItems[i].Count);
            if (IsWeapon(villageItems[i].Item.ID))
            {
                Weapons weapon = (Weapons)villageItems[i].Item;
                PlayerPrefs.SetInt("Item Duribility", weapon.Durability);
            }
        }
        PlayerPrefs.SetInt("Item Count", i);
        PlayerPrefs.Save();
    }

    public void LoadFromPlayerPrefs()
    {
        int itemCount = PlayerPrefs.GetInt("Item Count");
        villageItems.Clear();
        for (int i = 0; i < itemCount; i++)
        {
            int id = PlayerPrefs.GetInt("Item ID" + i);
            int count = PlayerPrefs.GetInt("Item Count" + i);
            if (IsWeapon(id))
            {
                int duribility = PlayerPrefs.GetInt("Item Duribility");
                Weapons weapon = GetComponent<WeaponDatabase>().FetchWeaponByID(id);
                Inventory loadedItem = new Inventory(weapon, count);
                weapon.Durability = duribility;
                villageItems.Add(loadedItem);
            }
            else
            {
                Items item = GetComponent<ItemDatabase>().FetchItemByID(id);
                Inventory loadedItem = new Inventory(item, count);
                villageItems.Add(loadedItem);
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

public class Inventory
{
    public Items Item { get; set; }
    public int Count { get; set; }

    public Inventory(Items item, int count)
    {
        this.Item = item;
        this.Count = count;
    }
}
