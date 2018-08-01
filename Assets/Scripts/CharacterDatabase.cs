using UnityEngine;
using UnityEditor;
using LitJson;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Runtime.Serialization;

public class CharacterDatabase : MonoBehaviour
{
    private List<Character> enemyDatabase = new List<Character>();
    public List<Character> listOfWanderers = new List<Character>();
    public List<Character> listOfHeroes = new List<Character>();

    private JsonData enemyData;
    Character currentCharacter;
    static int maxAmountOfHeroes = 4;
    int currentAmountOfHeroes;
    int amountOfSavedHeroes;
    int amountOfSavedWanderers;

    GameObject villageManager;

    void Start()
    {
        enemyData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Characters.json"));
        AddToDatabase(enemyData, enemyDatabase);
        LoadHeroCharacters();
        LoadWandererCharacters();

        villageManager = GameObject.FindGameObjectWithTag("VillageSceneManager");

        PrintCreatedCharacters();
    }

    private void Update()
    {
        if (Input.GetKeyUp("c"))
        {
            //CreateRandomHero();
        }
        if (Input.GetKeyUp("p"))
        {
            PrintCreatedCharacters();
        }
        if (Input.GetKeyUp("d"))
        {
            //DeleteWanderer(listOfWanderers[UnityEngine.Random.Range(0, listOfWanderers.Count)]);
        }
        if (Input.GetKeyUp("e"))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public Character FetchHeroByID(int id)
    {
        for (int i = 0; i < GetComponent<CharacterDatabase>().GetListOfHeroes().Count; i++)
        {
            if (GetComponent<CharacterDatabase>().GetListOfHeroes()[i].id == id)
                return GetComponent<CharacterDatabase>().GetListOfHeroes()[i];
        }
        return null;
    }

    public List<Character> GetListOfHeroes()
    {
        return listOfHeroes;
    }

    public List<Character> GetListOfWanderers()
    {
        return listOfWanderers;
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

    public void RecruitHero(Character characterToRecruit)
    {
        int slotNumber = listOfHeroes.Count;
        amountOfSavedHeroes += 1;
        amountOfSavedWanderers -= 1;
        
        //PlayerPrefs.SetInt("Hero Count", amountOfSavedHeroes);
        listOfHeroes.Add(characterToRecruit);
        DeleteWanderer(characterToRecruit);

        //SaveNewHero(characterToRecruit, slotNumber);
        villageManager.GetComponent<RosterManager>().AddACharacterSlotInBarracksUI();
        GameMaster.gameMaster.Save();
        Debug.Log("Recruited: " + characterToRecruit.name);
    }

    public Character CreateRandomWanderer()
    {
        Character newCharacter = new Character
        {
            id = GenerateWandererID(),
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
        int slotNumber = listOfWanderers.Count;
        amountOfSavedWanderers += 1;
        //PlayerPrefs.SetInt("Wanderer Count", amountOfSavedWanderers);
        listOfWanderers.Add(newCharacter);
        //GameMaster.gameMaster.Save();
        //SaveNewWanderer(newCharacter, slotNumber);
        return newCharacter;
    }

    static int GetCurrentTime()
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
        return currentEpochTime;
    }

    int GenerateWandererID()
    {
        int newWandererID = listOfWanderers.Count + 1;
        return newWandererID;
    }


    int GenerateHeroID()
    {
        int newHeroID = listOfHeroes.Count + 1;
        return newHeroID;
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

    void SaveNewHero(Character hero, int index)
    {


        /*
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
        //Debug.Log("Added " + hero.name);*/
    }

    /*
    * save new wanderer
    */

    void SaveNewWanderer(Character hero, int index)
    {/*
        PlayerPrefs.SetInt("Wanderer Num " + index, hero.id);
        PlayerPrefs.SetInt("Wanderer " + index + " ID", hero.id);
        PlayerPrefs.SetString("Wanderer " + index + " Name", hero.name);
        PlayerPrefs.SetString("Wanderer " + index + " Job", hero.job);
        PlayerPrefs.SetInt("Wanderer " + index + " HP", hero.hp);
        PlayerPrefs.SetInt("Wanderer " + index + " MP", hero.mp);
        PlayerPrefs.SetInt("Wanderer " + index + " Attack", hero.attack);
        PlayerPrefs.SetInt("Wanderer " + index + " Special", hero.special);
        PlayerPrefs.SetInt("Wanderer " + index + " Defense", hero.defense);
        PlayerPrefs.SetInt("Wanderer " + index + " Luck", hero.luck);
        PlayerPrefs.SetInt("Wanderer " + index + " Items", hero.items);
        PlayerPrefs.SetInt("Wanderer " + index + " Exp", hero.exp);
        PlayerPrefs.SetInt("Wanderer " + index + " Lives", hero.lives);
        PlayerPrefs.SetString("Wanderer " + index + " Slug", hero.slug);
        PlayerPrefs.Save();*/
    }

    /*
     * update stats of a hero
     */

    public void SaveCharacter(Character hero)
    {/*
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
        PlayerPrefs.Save();*/
    }

    void LoadHeroCharacters()
    {
        amountOfSavedHeroes = GameMaster.gameMaster.characterDB.listOfHeroes.Count();

        for (int i = 0; i < amountOfSavedHeroes; i++)
        {
            listOfHeroes[i] = GameMaster.gameMaster.characterDB.listOfHeroes[i];
        }
        /*
        amountOfSavedHeroes = PlayerPrefs.GetInt("Hero Count", 0);
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
        }*/
    }

    void LoadWandererCharacters()
    {
        amountOfSavedWanderers = GameMaster.gameMaster.characterDB.listOfWanderers.Count();

        for (int i = 0; i < amountOfSavedWanderers; i++)
        {
            listOfWanderers[i] = GameMaster.gameMaster.characterDB.listOfWanderers[i];            
        }

        /*
        amountOfSavedWanderers = PlayerPrefs.GetInt("Wanderer Count", 0);
        listOfWanderers.Clear();
        for (int i = 0; i < amountOfSavedWanderers; i++)
        {
            int characterNum = PlayerPrefs.GetInt("Wanderer Num " + i);
            if (characterNum != 0)
            {
                int id = PlayerPrefs.GetInt("Wanderer " + i + " ID");
                string heroName = PlayerPrefs.GetString("Wanderer " + i + " Name");
                string job = PlayerPrefs.GetString("Wanderer " + i + " Job");
                int hp = PlayerPrefs.GetInt("Wanderer " + i + " HP");
                int mp = PlayerPrefs.GetInt("Wanderer " + i + " MP");
                int att = PlayerPrefs.GetInt("Wanderer " + i + " Attack");
                int spec = PlayerPrefs.GetInt("Wanderer " + i + " Special");
                int def = PlayerPrefs.GetInt("Wanderer " + i + " Defense");
                int luck = PlayerPrefs.GetInt("Wanderer " + i + " Luck");
                int item = PlayerPrefs.GetInt("Wanderer " + i + " Items");
                int exp = PlayerPrefs.GetInt("Wanderer " + i + " Exp");
                int lives = PlayerPrefs.GetInt("Wanderer " + i + " Lives");
                string slug = PlayerPrefs.GetString("Wanderer " + i + " Slug");
                Character character = new Character(id, heroName, job, hp, mp, att, spec, def, luck, item, exp, lives, slug);
                listOfWanderers.Add(character);
            }
        }*/
    }

    public void DeleteWanderer(Character character)
    {
        listOfWanderers.Remove(character);
        /*
        PlayerPrefs.DeleteKey("Hero Num " + character.id);
        PlayerPrefs.DeleteKey("Hero " + character.id + " ID");
        PlayerPrefs.DeleteKey("Hero " + character.id + " Name");
        PlayerPrefs.DeleteKey("Hero " + character.id + " Job");
        PlayerPrefs.DeleteKey("Hero " + character.id + " HP");
        PlayerPrefs.DeleteKey("Hero " + character.id + " MP");
        PlayerPrefs.DeleteKey("Hero " + character.id + " Attack");
        PlayerPrefs.DeleteKey("Hero " + character.id + " Special");
        PlayerPrefs.DeleteKey("Hero " + character.id + " Defense");
        PlayerPrefs.DeleteKey("Hero " + character.id + " Luck");
        PlayerPrefs.DeleteKey("Hero " + character.id + " Items");
        PlayerPrefs.DeleteKey("Hero " + character.id + " Exp");
        PlayerPrefs.DeleteKey("Hero " + character.id + " Lives");
        PlayerPrefs.DeleteKey("Hero " + character.id + " Slug");*/
        Debug.Log("Deleted " + character.name);
                
        
        //PlayerPrefs.Save();
    }

    public void DeleteAllWanderers()
    {
        for (int i = 0; i < listOfWanderers.Count; i++)
        {
            DeleteWanderer(listOfWanderers[i]);
            //PlayerPrefs.SetInt("Character Count", 0);
            listOfWanderers.Clear();
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
        for (int i = 0; i < listOfWanderers.Count; i++)
        {
            Debug.Log("Wanderers: " + listOfWanderers[i].name);

        }

        for (int i = 0; i < listOfHeroes.Count; i++)
        {
            Debug.Log("Heroes: " + listOfHeroes[i].name);

        }
    }

   
}

[Serializable]
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
    //public Sprite sprite { get; set; }

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
        //this.sprite = Resources.Load<Sprite>("Items/" + slug);
    }

    public Character()
    {
        this.id = -1;
    }
}
