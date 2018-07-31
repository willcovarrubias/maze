﻿using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class ItemDatabase : MonoBehaviour
{
    Dictionary<int, Items> database = new Dictionary<int, Items>();
    private JsonData itemsData;

    public Items FetchItemByID(int id)
    {
        return database[id];
    }

    public void AddToDatabase(Items item)
    {
        database.Add(item.ID, item);
    }

    public Dictionary<int, Items> GetDatabase()
    {
        return database;
    }

    public void DisplayAllItems()
    {
        foreach (KeyValuePair<int, Items> keyValue in database)
        {
            Debug.Log(database[keyValue.Key].Title);
        }
    }

    //TODO: Make sure to use mazeRoomNumber and rarity someway in the furture
    public List<Inventory> GetRandomItemsForChest(/*int mazeRoomNumber*/)
    {
        List<Inventory> chestItems = new List<Inventory>();
        int numberOfItems = Random.Range(1, 11);
        int i = 0;
        while (i < numberOfItems)
        {
            float randomValue = Random.value;
            int chestItemID;
            int amount;
            if (randomValue >= 0.6f)
            {
                int max = numberOfItems - i;
                if (max > 6)
                {
                    max = 6;
                }
                amount = Random.Range(1, max);
                i += amount;
                chestItemID = GetComponent<ConsumableDatabase>().GetRandomConsumableID();
                AddToChestList(chestItems, chestItemID, amount);
            }
            else if (randomValue >= 0.2f && randomValue < 0.6f)
            {
                int max = numberOfItems - i;
                if (max > 6)
                {
                    max = 6;
                }
                amount = Random.Range(1, max);
                i += amount;
                chestItemID = GetComponent<MaterialDatabase>().GetRandomMaterialID();
                AddToChestList(chestItems, chestItemID, amount);
            }
            else if (randomValue >= 0.1f && randomValue < 0.2f)
            {
                amount = 1;
                Items weapon = GetComponent<WeaponDatabase>().CreateWeapon();
                chestItems.Add(new Inventory(weapon, 1, chestItems.Count));
                i++;
            }
            else
            {
                amount = 1;
                chestItemID = GetComponent<ArmorDatabase>().GetRandomArmorID();
                i++;
                AddToChestList(chestItems, chestItemID, amount);
            }
        }
        return chestItems;
    }

    void AddToChestList(List<Inventory> chestItems, int id, int count)
    {
        if (count > 0)
        {
            for (int i = 0; i < chestItems.Count; i++)
            {
                if (chestItems[i].Item.ID == id)
                {
                    chestItems[i].Count += count;
                    return;
                }
            }
            Items chestItem = FetchItemByID(id);
            chestItems.Add(new Inventory(chestItem, count, chestItems.Count));
        }
    }
}

public class Items
{
    public int ID { get; set; }
    public string Title { get; set; }
    public int Rarity { get; set; }
    public int Size { get; set; }
    public string Slug { get; set; }
    public Sprite Sprite { get; set; }

    public Items(int id, string title, int rarity, int size, string slug)
    {
        this.ID = id;
        this.Title = title;
        this.Slug = slug;
        this.Rarity = rarity;
        this.Size = size;
        this.Sprite = Resources.Load<Sprite>("Items/" + slug);
    }

    public Items()
    {
        this.ID = -1;
    }
}
