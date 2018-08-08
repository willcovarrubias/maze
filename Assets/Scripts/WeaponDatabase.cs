using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class WeaponDatabase : MonoBehaviour
{
    List<int> weaponIDs = new List<int>();
    JsonData itemsData, weaponMaterialData, weaponTypeData;
    int weaponCount;

    void Start()
    {
        weaponMaterialData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Weapon Materials.json"));
        weaponTypeData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Weapon Types.json"));
        weaponCount = PlayerPrefs.GetInt("Weapon Count", 10000);
    }

    public Weapons CreateWeapon(/* int rarityNumber */)
    {
        Weapons weapon = new Weapons(
            weaponCount,
            weaponMaterialData[Random.Range(0, weaponMaterialData.Count)] + " " + weaponTypeData[Random.Range(0, weaponTypeData.Count)],
            0,
            Random.Range(1, 10),
            Random.Range(1, 10),
            Random.Range(-2, 2),
            Random.Range(1, 10),
            Random.Range(1, 10),
            "");
        weaponCount++;
        PlayerPrefs.SetInt("Weapon Count", weaponCount);
        PlayerPrefs.Save();
        return weapon;
    }

    public int GetNewWeaponsCount()
    {
        int count = weaponCount;
        weaponCount++;
        PlayerPrefs.SetInt("Weapon Count", weaponCount);
        PlayerPrefs.Save();
        return count;
    }
}

public class Weapons : Items
{
    public int Attack { get; set; }
    public int Special { get; set; }
    public int Speed { get; set; }
    public int Durability { get; set; }

    public Weapons(int id, string title, int rarity, int attack, int special, int speed, int durability, int size, string slug)
    {
        this.ID = id;
        this.Title = title;
        this.Rarity = rarity;
        this.Attack = attack;
        this.Special = special;
        this.Speed = speed;
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
