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
                return database[i];
        }
        return null;
    }

    public void AddToDatabase(Items item)
    {
        database.Add(item);
    }

    public List<Items> GetDatabase()
    {
        return database;
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
