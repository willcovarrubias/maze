﻿using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class WeaponDatabase : MonoBehaviour
{
    List<int> weaponIDs = new List<int>();
    private JsonData itemsData;

    void Start()
    {
        itemsData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Weapons.json"));
        ConstructWeaponDatabase();

        //Debug.Log (FetchWeaponByID(1).Title);
    }

    public Weapons FetchWeaponByID(int id)
    {
        for (int i = 0; i < GetComponent<ItemDatabase>().GetDatabase().Count; i++)
        {
            if (GetComponent<ItemDatabase>().GetDatabase()[i].ID == id)
                return (Weapons)GetComponent<ItemDatabase>().GetDatabase()[i];
        }
        return null;
    }

    void ConstructWeaponDatabase()
    {
        for (int i = 0; i < itemsData.Count; i++)
        {
            Weapons weapon = new Weapons((int)itemsData[i]["id"],
                itemsData[i]["title"].ToString(),
                (int)itemsData[i]["rarity"],
                (int)itemsData[i]["attack"],
                (int)itemsData[i]["special"],
                (int)itemsData[i]["durability"],
                (int)itemsData[i]["size"],
                itemsData[i]["slug"].ToString(), 
                0);
            GetComponent<ItemDatabase>().AddToDatabase(weapon);
            weaponIDs.Add(weapon.ID);
        }
    }

    //TODO: use mazeRoomNumber and rarity in the future
    public int GetRandomWeaponID(/*int mazeRoomNumber*/)
    {
        return weaponIDs[Random.Range(0, weaponIDs.Count)];
    }
}

public class Weapons : Items
{
    public int Attack { get; set; }
    public int Special { get; set; }
    public int Durability { get; set; }
    public int Num { get; set; }

    public Weapons(int id, string title, int rarity, int attack, int special, int durability, int size, string slug, int num)
    {
        this.ID = id;
        this.Title = title;
        this.Rarity = rarity;
        this.Attack = attack;
        this.Special = special;
        this.Durability = durability;
        this.Size = size;
        this.Slug = slug;
        this.Sprite = Resources.Load<Sprite>("Items/" + slug);
        this.Num = num;
    }

    public Weapons()
    {
        this.ID = -1;
    }
}
