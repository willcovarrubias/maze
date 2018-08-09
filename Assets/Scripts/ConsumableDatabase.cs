using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class ConsumableDatabase : MonoBehaviour
{
    List<List<int>> consumablesID = new List<List<int>>();
    JsonData itemsData;
    static int maxAmountOfRooms = 12;

    void Start()
    {
        itemsData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Consumable.json"));
        for (int i = 0; i < maxAmountOfRooms; i++)
        {
            List<int> newList = new List<int>();
            consumablesID.Add(newList);
        }
        AddToDatabase();
    }

    void AddToDatabase()
    {
        for (int i = 0; i < itemsData.Count; i++)
        {
            Consumable item = new Consumable();
            item.ID = (int)itemsData[i]["id"];
            item.Title = itemsData[i]["title"].ToString();
            item.Size = (int)itemsData[i]["size"];
            item.Slug = itemsData[i]["slug"].ToString();
            item.Healing = (int)itemsData[i]["healing"];
            List<int> newList = new List<int>();
            for (int j = 0; j < itemsData[i]["rarity"].Count; j++)
            {
                newList.Add((int)itemsData[i]["rarity"][j]);
            }
            item.Rarity = newList;
            GetComponent<ItemDatabase>().AddToDatabase(item);
            for (int j = 0; j < itemsData[i]["rarity"].Count; j++)
            {
                consumablesID[(int)itemsData[i]["rarity"][j]].Add(item.ID);
            }
        }
    }

    public KeyValuePair<int, int> GetRandomConsumableID(int mazeRoomNumber)
    {
        KeyValuePair<int, int> amountAndID = new KeyValuePair<int, int>();
        int rarity = Mathf.FloorToInt((float)mazeRoomNumber / 10);
        int amount = Random.Range(1, 6);
        float randomValue = Random.value;
        if (randomValue >= 0.9f && randomValue < 0.95f)
        {
            amount = Random.Range(1, 5);
            rarity = IncreaseOrDecreaseRarity(rarity, 1);
        }
        else if (randomValue >= 0.95f && randomValue < 0.95f)
        {
            amount = Random.Range(1, 4);
            rarity = IncreaseOrDecreaseRarity(rarity, 2);
        }
        else if (randomValue >= 0.98f && randomValue < 0.99)
        {
            amount = Random.Range(1, 3);
            rarity = IncreaseOrDecreaseRarity(rarity, 3);
        }
        else if (randomValue >= 0.99f && randomValue < 1)
        {
            amount = 1;
            rarity = IncreaseOrDecreaseRarity(rarity, 4);
        }
        amountAndID = new KeyValuePair<int, int>(consumablesID[rarity][Random.Range(0, consumablesID[rarity].Count)], amount);
        return amountAndID;
    }

    int IncreaseOrDecreaseRarity(int rarity, int amount)
    {
        if (Random.value > 0.5f)
        {
            rarity += amount;
            if (rarity > consumablesID.Count - 1)
            {
                rarity = consumablesID.Count - 1;
            }
            return rarity;
        }
        else
        {
            rarity += amount;
            if (rarity < 0)
            {
                rarity = 0;
            }
            return rarity;
        }
    }
}

public class Consumable : Items
{
    public int Healing { get; set; }

    public Consumable(int id, string title, int size, int healing, string slug)
    {
        this.ID = id;
        this.Title = title;
        this.Size = size;
        this.Healing = healing;
        this.Slug = slug;
        this.Sprite = Resources.Load<Sprite>("Items/" + slug);
    }

    public Consumable() {}
}
