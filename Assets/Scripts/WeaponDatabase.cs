using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class WeaponDatabase : MonoBehaviour
{
    private List<Weapons> database = new List<Weapons>();
    private JsonData weaponData;

    void Start()
    {
        weaponData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Weapons.json"));
        ConstructWeaponDatabase();

        //Debug.Log (FetchWeaponByID(1).Title);
    }

    public Weapons FetchWeaponByID(int id)
    {
        for (int i = 0; i < database.Count; i++)
        {
            if (database[i].ID == id)
                return database[i];
        }
        return null;

    }

    void ConstructWeaponDatabase()
    {
        for (int i = 0; i < weaponData.Count; i++)
        {
                database.Add(new Weapons((int)weaponData[i]["id"],      
                weaponData[i]["title"].ToString(),
                (int)weaponData[i]["rarity"],
                (int)weaponData[i]["attack"],
                (int)weaponData[i]["special"],
                (int)weaponData[i]["durability"],
                (int)weaponData[i]["size"],
                weaponData[i]["slug"].ToString()));
        }
    }
}

public class Weapons
{
    public int ID { get; set; }   
    public string Title { get; set; }
    public int Rarity { get; set; }
    public int Attack { get; set; }
    public int Special { get; set; }
    public int Durability { get; set; }
    public int Size { get; set; }
    public string Slug { get; set; }
    public Sprite Sprite { get; set; }



    public Weapons(int id, string title, int rarity, int attack, int special, int durability, int size, string slug)
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

    }

    public Weapons()
    {
        this.ID = -1;
    }
}
