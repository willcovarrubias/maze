using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class ItemDatabase : MonoBehaviour
{
    private List<Items> database = new List<Items>();
    private JsonData itemsData;

    public Items FetchItemByID(int id)
    {
        for (int i = 0; i < database.Count; i++)
        {
            if (database[i].ID == id)
            {
                return database[i];
            }
        }
        return null;
    }

    public void AddToDatabase(Items item)
    {
        database.Add(item);
        //Debug.Log(item.ID + " " + database.Count);
    }

    public List<Items> GetDatabase()
    {
        return database;
    }

    public void DisplayAllItems()
    {
        for (int i = 0; i < database.Count; i++)
        {
            Debug.Log(database[i].Title);
        }
    }

    //TODO: Make sure to use mazeRoomNumber and rarity someway in the furture
    public List<Items> GetRandomItemsForChest(/*int mazeRoomNumber*/)
    {
        List<Items> chestItems = new List<Items>();
        int numberOfItems = Random.Range(1, 11);
        for (int i = 0; i < numberOfItems; i++)
        {
            float randomValue = Random.value;
            if (randomValue >= 0.6f)
            {
                chestItems.Add(FetchItemByID(GetComponent<ConsumableDatabase>().GetRandomConsumableID()));
            }
            else if (randomValue >= 0.2f && randomValue < 0.6f)
            {
                chestItems.Add(FetchItemByID(GetComponent<MaterialDatabase>().GetRandomMaterialID()));
            }
            else if (randomValue >= 0.1f && randomValue < 0.2f)
            {
                chestItems.Add(FetchItemByID(GetComponent<WeaponDatabase>().GetRandomWeaponID()));
            }
            else
            {
                chestItems.Add(FetchItemByID(GetComponent<ArmorDatabase>().GetRandomArmorID()));
            }
        }
        /*
        for (int i = 0; i < chestItems.Count; i++)
        {
            Debug.Log(chestItems[i].Title);
        }
        */
        return chestItems;
    }

    /*
    private void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            GetRandomItemsForChest();
        }
    }
    */
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
