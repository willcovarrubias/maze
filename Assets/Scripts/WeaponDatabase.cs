using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class WeaponDatabase : MonoBehaviour
{
    List<List<int>> weaponTypeIndexNumber = new List<List<int>>();
    List<List<int>> weaponMaterialIndexNumber = new List<List<int>>();
    Dictionary<int, TypeAndMaterial> materialAndTypes = new Dictionary<int, TypeAndMaterial>();

    JsonData weaponMaterialData, weaponTypeData;
    int weaponCount;

    static int maxAmountOfRooms = 12;

    void Start()
    {
        weaponMaterialData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Weapon Materials.json"));
        weaponTypeData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Weapon Types.json"));
        weaponCount = PlayerPrefs.GetInt("Weapon Count", 10000);
        AddToLists();
    }

    void AddToLists()
    {
        for (int i = 0; i < maxAmountOfRooms; i++)
        {
            List<int> newList = new List<int>();
            weaponTypeIndexNumber.Add(newList);
            weaponMaterialIndexNumber.Add(newList);
        }
        for (int i = 0; i < weaponTypeData.Count; i++)
        {
            TypeAndMaterial typeAndMaterial = new TypeAndMaterial();
            typeAndMaterial.Name = weaponTypeData[i]["name"].ToString();
            typeAndMaterial.Attack = (int)weaponTypeData[i]["attack"];
            typeAndMaterial.Durability = (int)weaponTypeData[i]["duribility"];
            typeAndMaterial.Special = (int)weaponTypeData[i]["special"];
            typeAndMaterial.Speed = (int)weaponTypeData[i]["speed"];
            typeAndMaterial.Size = (int)weaponTypeData[i]["size"];
            typeAndMaterial.Id = (int)weaponTypeData[i]["id"];
            materialAndTypes.Add((int)weaponTypeData[i]["id"], typeAndMaterial);
            for (int j = 0; j < weaponTypeData[i]["rarity"].Count; j++)
            {
                weaponTypeIndexNumber[(int)weaponTypeData[i]["rarity"][j]].Add(i);
            }
        }
        for (int i = 0; i < weaponMaterialData.Count; i++)
        {
            TypeAndMaterial typeAndMaterial = new TypeAndMaterial();
            typeAndMaterial.Name = weaponMaterialData[i]["name"].ToString();
            typeAndMaterial.Attack = (int)weaponMaterialData[i]["attack"];
            typeAndMaterial.Durability = (int)weaponMaterialData[i]["duribility"];
            typeAndMaterial.Special = (int)weaponMaterialData[i]["special"];
            typeAndMaterial.Speed = (int)weaponMaterialData[i]["speed"];
            typeAndMaterial.Size = (int)weaponMaterialData[i]["size"];
            typeAndMaterial.Id = (int)weaponMaterialData[i]["id"];
            materialAndTypes.Add((int)weaponMaterialData[i]["id"], typeAndMaterial);
            for (int j = 0; j < weaponMaterialData[i]["rarity"].Count; j++)
            {
                weaponMaterialIndexNumber[(int)weaponMaterialData[i]["rarity"][j]].Add(i);
            }
        }
    }

    public Weapons CreateWeapon(int mazeRoomNumber)
    {
        int material, type;
        int materialRarity = Mathf.FloorToInt(mazeRoomNumber / 10);
        int typeRarity = Mathf.FloorToInt(mazeRoomNumber / 10);
        materialRarity = ChangeRarity(materialRarity);
        typeRarity = ChangeRarity(typeRarity);
        material = weaponMaterialIndexNumber[materialRarity][Random.Range(0, weaponMaterialIndexNumber[materialRarity].Count)];
        type = weaponTypeIndexNumber[typeRarity][Random.Range(0, weaponTypeIndexNumber[typeRarity].Count)];
        Weapons weapon = new Weapons
        {
            ID = GetNewWeaponsCount(),
            Title = weaponMaterialData[material]["name"] + " " + weaponTypeData[type]["name"],
            Attack = (int)weaponMaterialData[material]["attack"] + (int)weaponTypeData[type]["attack"],
            Special = (int)weaponMaterialData[material]["special"] + (int)weaponTypeData[type]["special"],
            Speed = (int)weaponMaterialData[material]["speed"] + (int)weaponTypeData[type]["speed"],
            Durability = (int)weaponMaterialData[material]["duribility"] + (int)weaponTypeData[type]["duribility"],
            Size = (int)weaponMaterialData[material]["size"] + (int)weaponTypeData[type]["size"],
            TypeID = (int)weaponTypeData[type]["id"],
            MaterialID = (int)weaponMaterialData[material]["id"],
            Sprite = Resources.Load<Sprite>("Sprites/Weapons/" + weaponTypeData[type]["id"]),
            Slug = weaponTypeData[type]["id"].ToString()
        };
        return weapon;
    }

    public Weapons GetWeaponFromMaterialAndType(int materialID, int typeID)
    {
        Weapons weapon = new Weapons
        {
            ID = GetNewWeaponsCount(),
            Title = materialAndTypes[materialID].Name + " " + materialAndTypes[typeID].Name,
            Attack = materialAndTypes[materialID].Attack + materialAndTypes[typeID].Attack,
            Special = materialAndTypes[materialID].Special + materialAndTypes[typeID].Special,
            Speed = materialAndTypes[materialID].Speed + materialAndTypes[typeID].Speed,
            Durability = materialAndTypes[materialID].Durability + materialAndTypes[typeID].Durability,
            Size = materialAndTypes[materialID].Size + materialAndTypes[typeID].Size,
            TypeID = materialAndTypes[typeID].Id,
            MaterialID = materialAndTypes[materialID].Id,
            Sprite = Resources.Load<Sprite>("Sprites/Weapons/" + materialAndTypes[typeID].Id),
            Slug = materialAndTypes[typeID].Id.ToString()
        };
        return weapon;
    }

    public int GetNewWeaponsCount()
    {
        int count = weaponCount;
        weaponCount++;
        PlayerPrefs.SetInt("Weapon Count", weaponCount);
        GameMaster.gameMaster.PlayerPrefsSave();
        return count;
    }

    int ChangeRarity(int rarity)
    {
        int amount = rarity;
        float randomValue = Random.value;
        if (randomValue >= 0.9f && randomValue < 0.95f)
        {
            amount = IncreaseOrDecreaseRarity(rarity, 1);
        }
        else if (randomValue >= 0.95f && randomValue < 0.95f)
        {
            amount = IncreaseOrDecreaseRarity(rarity, 2);
        }
        else if (randomValue >= 0.98f && randomValue < 0.99)
        {
            amount = IncreaseOrDecreaseRarity(rarity, 3);
        }
        else if (randomValue >= 0.99f && randomValue < 1)
        {
            amount = IncreaseOrDecreaseRarity(rarity, 4);
        }
        return amount;
    }

    int IncreaseOrDecreaseRarity(int rarity, int amount)
    {
        if (Random.value > 0.5f)
        {
            rarity += amount;
            if (rarity > maxAmountOfRooms - 1)
            {
                rarity = maxAmountOfRooms - 1;
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

public class Weapons : Items
{
    public int Attack { get; set; }
    public int Special { get; set; }
    public int Speed { get; set; }
    public int Durability { get; set; }
    public int TypeID { get; set; }
    public int MaterialID { get; set; }
    public int GemID { get; set; }

    public Weapons(int id, string title, int attack, int special, int speed, int durability, int size, string slug, int materialID, int gemID)
    {
        this.ID = id;
        this.Title = title;
        this.Attack = attack;
        this.Special = special;
        this.Speed = speed;
        this.Durability = durability;
        this.Size = size;
        this.Slug = slug;
        this.MaterialID = materialID;
        this.GemID = gemID;
        this.Sprite = Resources.Load<Sprite>("Sprites/Weapons/" + slug);
    }

    public Weapons()
    {
        this.ID = -1;
    }
}

public class TypeAndMaterial
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Attack { get; set; }
    public int Special { get; set; }
    public int Speed { get; set; }
    public int Durability { get; set; }
    public int Size { get; set; }
}
