using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class ConsumableDatabase : MonoBehaviour
{
    List<int> consumablesIDs = new List<int>();
    private JsonData itemsData;

    void Start()
    {
        itemsData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Consumable.json"));
        AddToDatabase();
    }

    void AddToDatabase()
    {
        for (int i = 0; i < itemsData.Count; i++)
        {
            Consumable item = new Consumable((int)itemsData[i]["id"],
                itemsData[i]["title"].ToString(),
                (int)itemsData[i]["rarity"],
                (int)itemsData[i]["size"],
                (int)itemsData[i]["healing"],
                itemsData[i]["slug"].ToString());

            GetComponent<ItemDatabase>().AddToDatabase(item);
            consumablesIDs.Add(item.ID);
        }
    }

    //TODO: use mazeRoomNumber and rarity in the future
    public int GetRandomConsumableID(/*int mazeRoomNumber*/)
    {
        return consumablesIDs[Random.Range(0, consumablesIDs.Count)];
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
