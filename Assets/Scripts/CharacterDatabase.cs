using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;
using System;

public class CharacterDatabase : MonoBehaviour
{
    private List<Character> database = new List<Character>();
    private JsonData characterData;

    void Start()
    {
        characterData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Characters.json"));
        AddToDatabase();
    }

    void AddToDatabase()
    {
        for (int i = 0; i < characterData.Count; i++)
        {
            database.Add(new Character((int)characterData[i]["id"],
                characterData[i]["title"].ToString(),
                (int)characterData[i]["hp"],
                (int)characterData[i]["mp"],
                (int)characterData[i]["attack"],
                (int)characterData[i]["special"],
                (int)characterData[i]["defence"],
                (int)characterData[i]["luck"],
                (int)characterData[i]["items"],
                characterData[i]["slug"].ToString()));
        }
        Debug.Log(GetRandomName());
    }

    public void CreateHero()
    {
        Character newCharacter = new Character
        {
            ID = GetCurrentTime(),
            HP = UnityEngine.Random.Range(10, 20),
            MP = UnityEngine.Random.Range(10, 20),
            Attack = UnityEngine.Random.Range(10, 20),
            Special = UnityEngine.Random.Range(10, 20),
            Defense = UnityEngine.Random.Range(10, 20),
            Luck = UnityEngine.Random.Range(10, 20),
            Items = UnityEngine.Random.Range(10, 20)
        };
    }

    string GetRandomName()
    {
        JsonData namesData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Names.json"));
        return namesData[UnityEngine.Random.Range(0, namesData.Count)].ToString();
    }

    public static int GetCurrentTime()
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
        return currentEpochTime;
    }
}

public class Character
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int HP { get; set; }
    public int MP { get; set; }
    public int Attack { get; set; }
    public int Special { get; set; }
    public int Defense { get; set; }
    public int Luck { get; set; }
    public int Items { get; set; }
    public string Slug { get; set; }
    public Sprite Sprite { get; set; }

    public Character(int id, string name, int hp, int mp, int attack, int special, int defense, int luck, int items, string slug)
    {
        this.ID = id;
        this.Name = name;
        this.HP = hp;
        this.MP = mp;
        this.Attack = attack;
        this.Special = special;
        this.Defense = defense;
        this.Luck = luck;
        this.Items = items;
        this.Slug = slug;
        this.Sprite = Resources.Load<Sprite>("Items/" + slug);
    }

    public Character()
    {
        this.ID = -1;
    }
}
