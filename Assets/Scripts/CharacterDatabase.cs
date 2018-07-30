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
    public List<Character> listOfHeroes = new List<Character>();

    private JsonData enemyData;
    Character currentCharacter;
    static int maxAmountOfHeroes = 4;
    int currentAmountOfHeroes = 0;
    int amountOfSavedHeroes;

    void Start()
    {
        enemyData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Characters.json"));
        AddToDatabase(enemyData, enemyDatabase);
        LoadCharacters();
    }

    /*
    private void Update()
    {
        if (Input.GetKeyUp("c"))
        {
            CreateRandomHero();
        }
        if (Input.GetKeyUp("p"))
        {
            PrintCreatedCharacters();
        }
        if (Input.GetKeyUp("d"))
        {
            DeleteHero(listOfHeroes[UnityEngine.Random.Range(0, listOfHeroes.Count)]);
        }
        if (Input.GetKeyUp("e"))
        {
            PlayerPrefs.DeleteAll();
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

    public Character CreateRandomHero()
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
        bool newSlot = true;
        int slotNumber = listOfHeroes.Count;
        for (int i = 0; i < listOfHeroes.Count; i++)
        {
            if (PlayerPrefs.GetInt("Hero Num " + i) == 0)
            {
                slotNumber = i;
                newSlot = false;
                break;
            }
        }
        if (newSlot)
        {
            amountOfSavedHeroes += 1;
            PlayerPrefs.SetInt("Character Count", amountOfSavedHeroes);
        }
        listOfHeroes.Add(newCharacter);
        SaveNewCharacter(newCharacter, slotNumber);
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
        string[] jobs = { "Warrior", "Knight", "Thief" };
        int pickAJob = UnityEngine.Random.Range(0, 3);
        return jobs[pickAJob];
    }

    /*
     * save new character
     */

    void SaveNewCharacter(Character hero, int index)
    {
        PlayerPrefs.SetInt("Hero Num " + index, hero.id);
        PlayerPrefs.SetInt("Hero " + index + " ID", hero.id);
        PlayerPrefs.SetString("Hero " + index + " Name", hero.name);
        PlayerPrefs.SetString("Hero " + index + " Job", hero.job);
        PlayerPrefs.SetInt("Hero " + index + " HP", hero.hp);
        PlayerPrefs.SetInt("Hero " + index + " MP", hero.mp);
        PlayerPrefs.SetInt("Hero " + index + " Attack", hero.attack);
        PlayerPrefs.SetInt("Hero " + index + " Special", hero.special);
        PlayerPrefs.SetInt("Hero " + index + " Defense", hero.defense);
        PlayerPrefs.SetInt("Hero " + index + " Luck", hero.luck);
        PlayerPrefs.SetInt("Hero " + index + " Items", hero.items);
        PlayerPrefs.SetInt("Hero " + index + " Exp", hero.exp);
        PlayerPrefs.SetInt("Hero " + index + " Lives", hero.lives);
        PlayerPrefs.SetString("Hero " + index + " Slug", hero.slug);
        PlayerPrefs.Save();
        Debug.Log("Added " + hero.name);
    }

    /*
     * update stats of a character
     */

    public void SaveCharacter(Character hero)
    {
        for (int i = 0; i < amountOfSavedHeroes; i++)
        {
            if (PlayerPrefs.GetInt("Hero Num " + i) == hero.id)
            {
                PlayerPrefs.SetInt("Hero " + i + " ID", hero.id);
                PlayerPrefs.SetString("Hero " + i + " Name", hero.name);
                PlayerPrefs.SetString("Hero " + i + " Job", hero.job);
                PlayerPrefs.SetInt("Hero " + i + " HP", hero.hp);
                PlayerPrefs.SetInt("Hero " + i + " MP", hero.mp);
                PlayerPrefs.SetInt("Hero " + i + " Attack", hero.attack);
                PlayerPrefs.SetInt("Hero " + i + " Special", hero.special);
                PlayerPrefs.SetInt("Hero " + i + " Defense", hero.defense);
                PlayerPrefs.SetInt("Hero " + i + " Luck", hero.luck);
                PlayerPrefs.SetInt("Hero " + i + " Items", hero.items);
                PlayerPrefs.SetInt("Hero " + i + " Exp", hero.exp);
                PlayerPrefs.SetInt("Hero " + i + " Lives", hero.lives);
                PlayerPrefs.SetString("Hero " + i + " Slug", hero.slug);
                return;
            }
        }
        PlayerPrefs.Save();
    }

    void LoadCharacters()
    {
        amountOfSavedHeroes = PlayerPrefs.GetInt("Character Count", 0);
        listOfHeroes.Clear();
        for (int i = 0; i < amountOfSavedHeroes; i++)
        {
            int characterNum = PlayerPrefs.GetInt("Hero Num " + i);
            if (characterNum != 0)
            {
                int id = PlayerPrefs.GetInt("Hero " + i + " ID");
                string heroName = PlayerPrefs.GetString("Hero " + i + " Name");
                string job = PlayerPrefs.GetString("Hero " + i + " Job");
                int hp = PlayerPrefs.GetInt("Hero " + i + " HP");
                int mp = PlayerPrefs.GetInt("Hero " + i + " MP");
                int att = PlayerPrefs.GetInt("Hero " + i + " Attack");
                int spec = PlayerPrefs.GetInt("Hero " + i + " Special");
                int def = PlayerPrefs.GetInt("Hero " + i + " Defense");
                int luck = PlayerPrefs.GetInt("Hero " + i + " Luck");
                int item = PlayerPrefs.GetInt("Hero " + i + " Items");
                int exp = PlayerPrefs.GetInt("Hero " + i + " Exp");
                int lives = PlayerPrefs.GetInt("Hero " + i + " Lives");
                string slug = PlayerPrefs.GetString("Hero " + i + " Slug");
                Character character = new Character(id, heroName, job, hp, mp, att, spec, def, luck, item, exp, lives, slug);
                listOfHeroes.Add(character);
            }
        }
    }

    public void DeleteHero(Character character)
    {
        for (int i = 0; i < listOfHeroes.Count; i++)
        {
            if (listOfHeroes[i] == character)
            {
                PlayerPrefs.SetInt("Hero Num " + i, 0);
                PlayerPrefs.SetInt("Hero " + i + " ID", 0);
                PlayerPrefs.SetString("Hero " + i + " Name", "");
                PlayerPrefs.SetString("Hero " + i + " Job", "");
                PlayerPrefs.SetInt("Hero " + i + " HP", 0);
                PlayerPrefs.SetInt("Hero " + i + " MP", 0);
                PlayerPrefs.SetInt("Hero " + i + " Attack", 0);
                PlayerPrefs.SetInt("Hero " + i + " Special", 0);
                PlayerPrefs.SetInt("Hero " + i + " Defense", 0);
                PlayerPrefs.SetInt("Hero " + i + " Luck", 0);
                PlayerPrefs.SetInt("Hero " + i + " Items", 0);
                PlayerPrefs.SetInt("Hero " + i + " Exp", 0);
                PlayerPrefs.SetInt("Hero " + i + " Lives", 0);
                PlayerPrefs.SetString("Hero " + i + " Slug", "");
                if (i == (listOfHeroes.Count - 1))
                {
                    PlayerPrefs.SetInt("Character Count", (PlayerPrefs.GetInt("Character Count") - 1));
                }
                Debug.Log("Deleted " + listOfHeroes[i].name);
                listOfHeroes.RemoveAt(i);
                return;
            }
        }
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

    void PrintCreatedCharacters()
    {
        for (int i = 0; i < listOfHeroes.Count; i++)
        {
            Debug.Log(listOfHeroes[i].name);
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
