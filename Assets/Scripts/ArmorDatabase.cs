using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class ArmorDatabase : MonoBehaviour
{
    List<int> armorIDs = new List<int>();
    private JsonData itemsData;

    void Start()
    {
        itemsData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Armor.json"));
        AddToDatabase();
    }

    void AddToDatabase()
    {
        for (int i = 0; i < itemsData.Count; i++)
        {
            Armor armor = new Armor((int)itemsData[i]["id"],
                itemsData[i]["title"].ToString(),
                (int)itemsData[i]["rarity"],
                (int)itemsData[i]["size"],
                (int)itemsData[i]["defense"],
                itemsData[i]["appendage"].ToString(),
                itemsData[i]["slug"].ToString());
            GetComponent<ItemDatabase>().AddToDatabase(armor);
            armorIDs.Add(armor.ID);
        }
    }

    //TODO: use mazeRoomNumber and rarity in the future
    public int GetRandomArmorID(/*int mazeRoomNumber*/)
    {
        return armorIDs[Random.Range(0, armorIDs.Count)];
    }
}

public class Armor : Items
{
    public int Defense { get; set; }
    public string Appendage { get; set; }

    public Armor(int id, string title, int rarity, int size, int defense, string appendage, string slug)
    {
        this.ID = id;
        this.Title = title;
        this.Rarity = rarity;
        this.Size = size;
        this.Slug = slug;
        this.Defense = defense;
        this.Appendage = appendage;
        this.Sprite = Resources.Load<Sprite>("Items/" + slug);
    }
}