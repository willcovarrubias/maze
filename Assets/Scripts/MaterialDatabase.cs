using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class MaterialDatabase : MonoBehaviour
{
    List<List<int>> materialsID = new List<List<int>>();
    JsonData itemsData;
    static int maxAmountOfRooms = 12;

    void Start()
    {
        itemsData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Materials.json"));
        for (int i = 0; i < maxAmountOfRooms; i++)
        {
            List<int> newList = new List<int>();
            materialsID.Add(newList);
        }
        AddToDatabase();
    }

    void AddToDatabase()
    {
        for (int i = 0; i < itemsData.Count; i++)
        {
            Material material = new Material();
            material.ID = (int)itemsData[i]["id"];
            material.Size = (int)itemsData[i]["size"];
            material.Title = itemsData[i]["title"].ToString();
            material.Slug = itemsData[i]["slug"].ToString();
            material.Sprite = Resources.Load<Sprite>("Sprites/Materials/" + material.ID);
            List<int> newList = new List<int>();
            for (int j = 0; j < itemsData[i]["rarity"].Count; j++)
            {
                newList.Add((int)itemsData[i]["rarity"][j]);
            }
            material.Rarity = newList;
            GetComponent<ItemDatabase>().AddToDatabase(material);
            for (int j = 0; j < material.Rarity.Count; j++)
            {
                materialsID[material.Rarity[j]].Add(material.ID);
            }
        }
    }

    public KeyValuePair<int, int> GetRandomMaterialID(int mazeRoomNumber)
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
        amountAndID = new KeyValuePair<int, int>(materialsID[rarity][Random.Range(0, materialsID[rarity].Count)], amount);
        return amountAndID;
    }

    int IncreaseOrDecreaseRarity(int rarity, int amount)
    {
        if (Random.value > 0.5f)
        {
            rarity += amount;
            if (rarity > materialsID.Count - 1)
            {
                rarity = materialsID.Count - 1;
            }
            return rarity;
        }
        else
        {
            rarity -= amount;
            if (rarity < 0)
            {
                rarity = 0;
            }
            return rarity;
        }
    }
}

public class Material : Items
{
    public Material(int id, string title, int size, string slug)
    {
        this.ID = id;
        this.Title = title;
        this.Size = size;
        this.Slug = slug;
        this.Sprite = Resources.Load<Sprite>("Items/" + slug);
    }

    public Material() { }
}