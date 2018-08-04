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
    List<Character> enemyDatabase = new List<Character>();
    public List<Character> listOfWanderers = new List<Character>();
    public List<Character> listOfHeroes = new List<Character>();

    private JsonData enemyData;
    public Character activeCharacter;
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

    /*
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
    */

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
                (int)json[i]["numberOfAttacks"],
                (int)json[i]["hp"],
                (int)json[i]["mp"],
                (int)json[i]["attack"],
                (int)json[i]["special"],
                (int)json[i]["defense"],
                (int)json[i]["speed"],
                (int)json[i]["luck"],
                (int)json[i]["items"],
                (int)json[i]["exp"],
                (int)json[i]["lives"],
                json[i]["slug"].ToString()));
        }
    }

    public List<Character> GetListofEnemies()
    {
        return enemyDatabase;
    }

    public void RecruitHero(Character characterToRecruit)
    {
        int slotNumber = listOfHeroes.Count;
        amountOfSavedHeroes += 1;
        amountOfSavedWanderers -= 1;

        //PlayerPrefs.SetInt("Hero Count", amountOfSavedHeroes);
        listOfHeroes.Add(characterToRecruit);
        listOfHeroes.Last().id = listOfHeroes.Count;
        DeleteWanderer(characterToRecruit);

        //SaveNewHero(characterToRecruit, slotNumber);
        villageManager.GetComponent<RosterManager>().AddACharacterSlotInBarracksUI();
        GameMaster.gameMaster.Save();
        Debug.Log("Recruited: " + characterToRecruit.name);
    }

    public Character CreateRandomWanderer()
    {
        string currentJob = GetRandomJob();
        Character newCharacter = new Character
        {
            id = GenerateWandererID(),
            name = GetRandomName(),
            job = currentJob,
            numberOfAttacks = DetermineNumberOfAttacks(currentJob),
            hp = UnityEngine.Random.Range(3, 20),
            mp = UnityEngine.Random.Range(3, 20),
            attack = UnityEngine.Random.Range(3, 20),
            special = UnityEngine.Random.Range(3, 20),
            defense = UnityEngine.Random.Range(3, 20),
            speed = UnityEngine.Random.Range(3, 20),
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

    int DetermineNumberOfAttacks(string currentJob)
    {
        if (currentJob == "Thief")
            return 2;
        else
            return 1;
    }

    void LoadHeroCharacters()
    {
        amountOfSavedHeroes = GameMaster.gameMaster.characterDB.listOfHeroes.Count();
        for (int i = 0; i < amountOfSavedHeroes; i++)
        {
            listOfHeroes[i] = GameMaster.gameMaster.characterDB.listOfHeroes[i];
        }
    }

    void LoadWandererCharacters()
    {
        amountOfSavedWanderers = GameMaster.gameMaster.characterDB.listOfWanderers.Count();
        for (int i = 0; i < amountOfSavedWanderers; i++)
        {
            listOfWanderers[i] = GameMaster.gameMaster.characterDB.listOfWanderers[i];
        }
    }

    public void DeleteWanderer(Character character)
    {
        listOfWanderers.Remove(character);
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

    public void ChangeActiveCharacter(int id)
    {
        for (int i = 0; i < listOfHeroes.Count; i++)
        {
            if (listOfHeroes[i].id == id)
            {
                activeCharacter = listOfHeroes[i];
                //GameMaster.gameMaster.activeCharacter = activeCharacter;
            }
        }
        GetComponent<ActiveCharacterController>().DetermineActiveCharacterCurrentLevel();
        GetComponent<ActiveCharacterController>().UpdateActiveCharacterVisuals();
        GameMaster.gameMaster.Save();
    }

    public void GetActiveCharacter()
    {
        // activeCharacter = GameMaster.gameMaster.activeCharacter;
    }

    void PrintCreatedCharacters()
    {
        for (int i = 0; i < listOfWanderers.Count; i++)
        {
            Debug.Log("Wanderers: " + listOfWanderers[i].name);
        }
        for (int i = 0; i < listOfHeroes.Count; i++)
        {
            Debug.Log("Heroes: " + listOfHeroes[i].name +
                "\nID: " + listOfHeroes[i].id);
        }
    }

    public List<Enemy> GetEnemiesForFightScene(/*int mazeRoomNumber*/)
    {
        List<Enemy> enemies = new List<Enemy>();
        int amountOfEnemies = UnityEngine.Random.Range(1, 6);
        for (int i = 0; i < amountOfEnemies; i++)
        {
            Enemy newEnemy = new Enemy(enemyDatabase[UnityEngine.Random.Range(0, enemyDatabase.Count)]);
            enemies.Add(newEnemy);
        }
        return enemies;
    }
}

[Serializable]
public class Character
{
    public int id { get; set; }
    public string name { get; set; }
    public string job { get; set; }
    public int numberOfAttacks { get; set; }
    public int hp { get; set; }
    public int mp { get; set; }
    public int attack { get; set; }
    public int special { get; set; }
    public int defense { get; set; }
    public int speed { get; set; }
    public int luck { get; set; }
    public int items { get; set; }
    public int exp { get; set; }
    public int lives { get; set; }
    public string slug { get; set; }
    //public Sprite sprite { get; set; }

    public Character(int id, string name, string job, int numberOfAttacks, int hp, int mp, int attack, int special,
                     int defense, int speed, int luck, int items, int exp, int lives, string slug)
    {
        this.id = id;
        this.name = name;
        this.job = job;
        this.numberOfAttacks = numberOfAttacks;
        this.hp = hp;
        this.mp = mp;
        this.attack = attack;
        this.special = special;
        this.defense = defense;
        this.speed = speed;
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

public class Enemy
{
    public Character EnemyData { get; set; }
    public int EnemyHP { get; set; }

    public Enemy(Character character)
    {
        EnemyData = character;
        EnemyHP = character.hp;
    }
}
