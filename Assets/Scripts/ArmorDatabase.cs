using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class ArmorDatabase : MonoBehaviour
{
    List<List<int>> armorID = new List<List<int>>();
    JsonData itemsData;
    static int maxAmountOfRooms = 12;

    void Start()
    {
        itemsData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Armor.json"));
        for (int i = 0; i < maxAmountOfRooms; i++)
        {
            List<int> newList = new List<int>();
            armorID.Add(newList);
        }
        AddToDatabase();
    }

    void AddToDatabase()
    {
        for (int i = 0; i < itemsData.Count; i++)
        {
            Armor armor = new Armor();
            armor.ID = (int)itemsData[i]["id"];
            armor.Title = itemsData[i]["title"].ToString();
            armor.Size = (int)itemsData[i]["size"];
            armor.Slug = itemsData[i]["slug"].ToString();
            armor.Speed = (int)itemsData[i]["speed"];
            armor.Appendage = itemsData[i]["appendage"].ToString();
            armor.Defense = (int)itemsData[i]["defense"];
            List<int> newList = new List<int>();
            for (int j = 0; j < itemsData[i]["rarity"].Count; j++)
            {
                newList.Add((int)itemsData[i]["rarity"][j]);
            }
            armor.Rarity = newList;
            GetComponent<ItemDatabase>().AddToDatabase(armor);
            for (int j = 0; j < itemsData[i]["rarity"].Count; j++)
            {
                armorID[(int)itemsData[i]["rarity"][j]].Add(armor.ID);
            }
        }
    }

    //TODO: use mazeRoomNumber and rarity in the future
    public int GetRandomArmorID(/*int mazeRoomNumber*/)
    {
        return 4000;
        //return armorID[Random.Range(0, armorID.Count)];
    }
}

public class Armor : Items
{
    public int Defense { get; set; }
    public int Speed { get; set; }
    public string Appendage { get; set; }

    public Armor(int id, string title, int size, int defense, int speed, string appendage, string slug)
    {
        this.ID = id;
        this.Title = title;
        this.Size = size;
        this.Slug = slug;
        this.Defense = defense;
        this.Speed = speed;
        this.Appendage = appendage;
        this.Sprite = Resources.Load<Sprite>("Items/" + slug);
    }

    public Armor() {}
}