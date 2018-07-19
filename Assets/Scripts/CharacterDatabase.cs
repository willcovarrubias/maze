using UnityEngine;
using UnityEditor;
using LitJson;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class CharacterDatabase : MonoBehaviour
{
    private List<Character> enemyDatabase = new List<Character>();
    private List<Character> heroDatabase = new List<Character>();
    private JsonData characterData;

    void Start()
    {
        characterData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Characters.json"));
        AddToDatabase();
        //load heros here from editable JSON 
        CreateHero();
    }

    void AddToDatabase()
    {
        for (int i = 0; i < characterData.Count; i++)
        {
            enemyDatabase.Add(new Character((int)characterData[i]["id"],
                characterData[i]["name"].ToString(),
                (int)characterData[i]["hp"],
                (int)characterData[i]["mp"],
                (int)characterData[i]["attack"],
                (int)characterData[i]["special"],
                (int)characterData[i]["defense"],
                (int)characterData[i]["luck"],
                (int)characterData[i]["items"],
                (int)characterData[i]["exp"],
                (int)characterData[i]["lives"],
                characterData[i]["slug"].ToString()));
        }
    }

    public void CreateHero()
    {
        Character newCharacter = new Character
        {
            id = GetCurrentTime(),
            name = GetRandomName(),
            hp = UnityEngine.Random.Range(10, 20),
            mp = UnityEngine.Random.Range(10, 20),
            attack = UnityEngine.Random.Range(10, 20),
            special = UnityEngine.Random.Range(10, 20),
            defense = UnityEngine.Random.Range(10, 20),
            luck = UnityEngine.Random.Range(10, 20),
            items = UnityEngine.Random.Range(10, 20),
            exp = 0,
            lives = 3
        };
        heroDatabase.Add(newCharacter);
        HeroToJson(newCharacter);
    }

    static int GetCurrentTime()
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
        return currentEpochTime;
    }

    string GetRandomName()
    {
        JsonData namesData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Names.json"));
        return namesData[UnityEngine.Random.Range(0, namesData.Count)].ToString();
    }

    public void HeroToJson(Character hero)
    {
        string path = "Assets/StreamingAssets/Heroes.json";
        string json_hero = JsonMapper.ToJson(hero);
        string[] lines = File.ReadAllLines(path);
        if (lines.Length == 0)
        {
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine("[");
            writer.WriteLine(json_hero);
            writer.WriteLine("]");
            writer.Close();
        }
        else
        {
            string[] newArray = new string[lines.Length - 1];
            for (int i = 0; i < newArray.Length; i++)
            {
                newArray[i] = lines[i];
            }
            File.WriteAllLines(path, newArray);
            StreamWriter writer = new StreamWriter(path, true);
            writer.Write(",");
            writer.WriteLine(json_hero);
            writer.WriteLine("]");
            writer.Close();
        }

    }
}

public class Character
{
    public int id { get; set; }
    public string name { get; set; }
    public int hp { get; set; }
    public int mp { get; set; }
    public int attack { get; set; }
    public int special { get; set; }
    public int defense { get; set; }
    public int luck { get; set; }
    public int items { get; set; }
    public int exp { get; set; }
    public int lives { get; set; }
    public string slug { get; set; }
    public Sprite sprite { get; set; }

    public Character(int id, string name, int hp, int mp, int attack, int special,
                     int defense, int luck, int items, int exp, int lives, string slug)
    {
        this.id = id;
        this.name = name;
        this.hp = hp;
        this.mp = mp;
        this.attack = attack;
        this.special = special;
        this.defense = defense;
        this.luck = luck;
        this.items = items;
        this.exp = exp;
        this.lives = lives;
        this.slug = slug;
        this.sprite = Resources.Load<Sprite>("Items/" + slug);
    }

    public Character()
    {
        this.id = -1;
    }
}
