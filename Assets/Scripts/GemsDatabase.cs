using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using System.IO;

public class GemsDatabase : MonoBehaviour
{
    List<List<int>> gemsID = new List<List<int>>();
    JsonData gemsData;
    static int maxAmountOfRooms = 12;

    void Start()
    {
        gemsData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Gems.json"));
        for (int i = 0; i < maxAmountOfRooms; i++)
        {
            List<int> newList = new List<int>();
            gemsID.Add(newList);
        }
        AddToDatabase();
    }

    void AddToDatabase()
    {
        for (int i = 0; i < gemsData.Count; i++)
        {
            Gem gem = new Gem();
            gem.ID = (int)gemsData[i]["id"];
            gem.Attack = (int)gemsData[i]["attack"];
            gem.Special = (int)gemsData[i]["special"];
            gem.Speed = (int)gemsData[i]["speed"];
            gem.Durability = (int)gemsData[i]["duribility"];
            gem.Size = (int)gemsData[i]["size"];
            gem.Title = gemsData[i]["title"].ToString();
            gem.Slug = gemsData[i]["slug"].ToString();
            gem.AddedTitle = gemsData[i]["addedTitle"].ToString();
            gem.Sprite = Resources.Load<Sprite>("Sprites/Gems/" + gem.ID);
            List<int> newList = new List<int>();
            for (int j = 0; j < gemsData[i]["rarity"].Count; j++)
            {
                newList.Add((int)gemsData[i]["rarity"][j]);
            }
            gem.Rarity = newList;
            GetComponent<ItemDatabase>().AddToDatabase(gem);
            for (int j = 0; j < gem.Rarity.Count; j++)
            {
                gemsID[gem.Rarity[j]].Add(gem.ID);
            }
        }
    }

    public KeyValuePair<int, int> GetRandomGemID(int mazeRoomNumber)
    {
        KeyValuePair<int, int> amountAndID = new KeyValuePair<int, int>();
        int rarity = Mathf.FloorToInt((float)mazeRoomNumber / 10);
        int amount = Random.Range(1, 3);
        float randomValue = Random.value;
        if (randomValue >= 0.9f && randomValue < 0.95f)
        {
            amount = Random.Range(1, 3);
            rarity = IncreaseOrDecreaseRarity(rarity, 1);
        }
        else if (randomValue >= 0.95f && randomValue < 0.95f)
        {
            amount = 1;
            rarity = IncreaseOrDecreaseRarity(rarity, 2);
        }
        else if (randomValue >= 0.98f && randomValue < 0.99)
        {
            amount = 1;
            rarity = IncreaseOrDecreaseRarity(rarity, 3);
        }
        else if (randomValue >= 0.99f && randomValue < 1)
        {
            amount = 1;
            rarity = IncreaseOrDecreaseRarity(rarity, 4);
        }
        amountAndID = new KeyValuePair<int, int>(gemsID[rarity][Random.Range(0, gemsID[rarity].Count)], amount);
        return amountAndID;
    }

    int IncreaseOrDecreaseRarity(int rarity, int amount)
    {
        if (Random.value > 0.5f)
        {
            rarity += amount;
            if (rarity > gemsID.Count - 1)
            {
                rarity = gemsID.Count - 1;
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

public class Gem : Items
{
    public int Attack { get; set; }
    public int Special { get; set; }
    public int Speed { get; set; }
    public int Durability { get; set; }
    public string AddedTitle { get; set; }
}