﻿using UnityEngine;
using UnityEditor;
using LitJson;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class CharacterDatabase : MonoBehaviour
{
    private List<Character> enemyDatabase = new List<Character>();
    public List<Character> listOfHeroes = new List<Character>();
    private JsonData enemyData;
    private JsonData heroData;
    Character currentCharacter;
    static int maxAmountOfHeroes = 4;
    int currentAmountOfHeroes = 0;

    


    void Start()
    {
        enemyData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Characters.json"));
        heroData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Heroes.json"));
        AddToDatabase(enemyData, enemyDatabase);
        AddToDatabase(heroData, listOfHeroes); //NOTE: Make sure heroes.json has at least []
        LoadCharacters();
    }

    /*
    private void Update()
    {
        if (Input.GetKeyDown("c"))
        {
            CreateHero();
        }
        if (Input.GetKeyDown("d"))
        {
            //DeleteHero();
        }
    }
    */

    public Character FetchCharacterByID(int id)
    {
        for (int i = 0; i < GetComponent<CharacterDatabase>().GetListOfHeroes().Count; i++)
        {
            if (GetComponent<CharacterDatabase>().GetListOfHeroes()[i].id == id)
                return (Character)GetComponent<CharacterDatabase>().GetListOfHeroes()[i];
        }
        return null;
    }

    public List<Character> GetListOfHeroes()
    {
        return listOfHeroes;
    }

    void AddToDatabase(JsonData json, List<Character> characters)
    {
        for (int i = 0; i < json.Count; i++)
        {
            characters.Add(new Character((int)json[i]["id"],
                json[i]["name"].ToString(),
                json[i]["job"].ToString(),
                (int)json[i]["hp"],
                (int)json[i]["mp"],
                (int)json[i]["attack"],
                (int)json[i]["special"],
                (int)json[i]["defense"],
                (int)json[i]["luck"],
                (int)json[i]["items"],
                (int)json[i]["exp"],
                (int)json[i]["lives"],
                json[i]["slug"].ToString()));
        }
    }

    public Character CreateHero()
    {
        Character newCharacter = new Character
        {
            id = GetCurrentTime(),
            name = GetRandomName(),
            job = GetRandomJob(),
            hp = UnityEngine.Random.Range(3, 20),
            mp = UnityEngine.Random.Range(3, 20),
            attack = UnityEngine.Random.Range(3, 20),
            special = UnityEngine.Random.Range(3, 20),
            defense = UnityEngine.Random.Range(3, 20),
            luck = UnityEngine.Random.Range(3, 20),
            items = UnityEngine.Random.Range(3, 20),
            exp = 0,
            lives = 3,
            slug = "00"
        };
        listOfHeroes.Add(newCharacter);
        SaveCharacters(newCharacter);
        return newCharacter;
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

    string GetRandomJob()
    {
        string[] jobs = new string[] { "Warrior", "Knight", "Thief" };
        int pickAJob = UnityEngine.Random.Range(0, 3);

        return jobs[pickAJob];
    }

    public void SaveCharacters(Character hero)
    {
        int i;
        for (i = 0; i < listOfHeroes.Count; i++)
        {
            PlayerPrefs.SetInt("Character ID" + i, listOfHeroes[i].id);
        }

        PlayerPrefs.Save();

        //PlayerPrefs.SetInt("Hero " + heroNum + " ID", hero.id);
        //PlayerPrefs.SetString("Hero " + heroNum + " Name", hero.name);
        //PlayerPrefs.SetString("Hero " + heroNum + " Name", hero.name);

        /*
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
        else if (lines.Length == 1)
        {
            string[] newArray = new string[0];
            File.WriteAllLines(path, newArray);
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
            if (!newArray[newArray.Length - 1].EndsWith(",", StringComparison.Ordinal))
            {
                writer.Write(",");
            }
            writer.WriteLine(json_hero);
            writer.WriteLine("]");
            writer.Close();
        }
        */
    }

    public void LoadCharacters()
    {
        int characterCount = PlayerPrefs.GetInt("Character Count");
        listOfHeroes.Clear();
        for (int i = 0; i < characterCount; i++)
        {
            int id = PlayerPrefs.GetInt("Character ID" + i);
            Character loadCharacters = GetComponent<CharacterDatabase>().FetchCharacterByID(id);
            listOfHeroes.Add(loadCharacters);
        }
    }

    public void DeleteHero(Character character)
    {
        string path = "Assets/StreamingAssets/Heroes.json";
        for (int i = 0; i < listOfHeroes.Count; i++)
        {
            if (listOfHeroes[i].id == character.id)
            {
                listOfHeroes.RemoveAt(i);
                break;
            }
        }
        string[] newJsonArray = new string[listOfHeroes.Count + 1];
        newJsonArray[0] = "[";
        for (int i = 0; i < listOfHeroes.Count - 1; i++)
        {
            if (listOfHeroes.Count > 0 && i != listOfHeroes.Count - 1 && i != 0)
            {
                newJsonArray[i + 1] = ",";
            }
            newJsonArray[i + 1] += JsonMapper.ToJson(listOfHeroes[i]);
        }
        newJsonArray[listOfHeroes.Count] = "]";
        File.WriteAllLines(path, newJsonArray);
    }

    public void ChangeCurrentCharacter(int id)
    {
        for (int i = 0; i < listOfHeroes.Count; i++)
        {
            if (listOfHeroes[i].id == id)
            {
                currentCharacter = listOfHeroes[i];
            }
        }
    }
}

public class Character
{
    public int id { get; set; }
    public string name { get; set; }
    public string job { get; set; }
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

    public Character(int id, string name, string job, int hp, int mp, int attack, int special,
                     int defense, int luck, int items, int exp, int lives, string slug)
    {
        this.id = id;
        this.name = name;
        this.job = job;
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
