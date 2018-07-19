using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class ConsumableDatabase : MonoBehaviour
{
    ItemDatabase itemDatabase = new ItemDatabase();
    private JsonData itemsData;

    void Start()
    {
        itemsData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Consumable.json"));
        ConstructItemsDatabase();
    }

    void ConstructItemsDatabase()
    {
        for (int i = 0; i < itemsData.Count; i++)
        {
            itemDatabase.AddToDatabase(new Consumable((int)itemsData[i]["id"],
                itemsData[i]["title"].ToString(),
                (int)itemsData[i]["rarity"],
                (int)itemsData[i]["size"],
                (int)itemsData[i]["healing"],
                itemsData[i]["slug"].ToString()));
        }
    }
}

public class Consumable : Items
{
    public int Healing { get; set; }

    public Consumable(int id, string title, int rarity, int size, int healing, string slug)
    {
        this.ID = id;
        this.Title = title;
        this.Rarity = rarity;
        this.Size = size;
        this.Healing = healing;
        this.Slug = slug;
        this.Sprite = Resources.Load<Sprite>("Items/" + slug);
    }
}
