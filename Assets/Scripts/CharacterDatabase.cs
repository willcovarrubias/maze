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
    List<List<int>> enemyID = new List<List<int>>();
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
    static int maxAmountOfRooms = 12;

    void Start()
    {
        enemyData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Characters.json"));
        for (int i = 0; i < maxAmountOfRooms; i++)
        {
            List<int> newList = new List<int>();
            enemyID.Add(newList);
        }
        AddToDatabase(enemyData, enemyDatabase);
        LoadHeroCharacters();
        LoadWandererCharacters();
        villageManager = GameObject.FindGameObjectWithTag("VillageSceneManager");
        PrintCreatedCharacters();
    }

    public Character FetchHeroByID(int id)
    {
        for (int i = 0; i < GetListOfHeroes().Count; i++)
        {
            if (GetListOfHeroes()[i].id == id)
                return GetListOfHeroes()[i];
        }
        return null;
    }

    public Enemy FetchEnemyByID(int id)
    {
        for (int i = 0; i < enemyDatabase.Count; i++)
        {
            if (enemyDatabase[i].id == id)
            {
                Enemy returnedEnemy = new Enemy(enemyDatabase[i]);
                return returnedEnemy;
            }
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
            Character character = new Character();
            character.id = (int)json[i]["id"];
            character.name = json[i]["name"].ToString();
            character.job = json[i]["job"].ToString();
            character.numberOfAttacks = (int)json[i]["numberOfAttacks"];
            character.maxHP = (int)json[i]["hp"];
            character.maxMP = (int)json[i]["mp"];
            character.attack = (int)json[i]["attack"];
            character.special = (int)json[i]["special"];
            character.defense = (int)json[i]["defense"];
            character.speed = (int)json[i]["speed"];
            character.luck = (int)json[i]["luck"];
            character.items = (int)json[i]["items"];
            character.exp = (int)json[i]["exp"];
            character.lives = (int)json[i]["lives"];
            character.slug = json[i]["slug"].ToString();
            List<int> rarityList = new List<int>();
            for (int j = 0; j < json[i]["rarity"].Count; j++)
            {
                rarityList.Add((int)json[i]["rarity"][j]);
            }
            List<int> itemIDs = new List<int>();
            for (int j = 0; j < json[i]["itemsHolding"].Count; j++)
            {
                itemIDs.Add((int)json[i]["itemsHolding"][j]);
            }
            character.itemsHolding = itemIDs;
            character.rarity = rarityList;
            characters.Add(character);
            for (int j = 0; j < json[i]["rarity"].Count; j++)
            {
                enemyID[(int)json[i]["rarity"][j]].Add(character.id);
            }
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
        int maxHP = UnityEngine.Random.Range(3, 20);
        int maxMP = UnityEngine.Random.Range(3, 20);
        Character newCharacter = new Character
        {
            id = GenerateWandererID(),
            name = GetRandomName(),
            job = currentJob,
            numberOfAttacks = DetermineNumberOfAttacks(currentJob),
            maxHP = maxHP,
            maxMP = maxMP,
            currentHP = maxHP,
            currentMP = maxMP,
            attack = UnityEngine.Random.Range(3, 20),
            special = UnityEngine.Random.Range(3, 20),
            defense = UnityEngine.Random.Range(3, 20),
            speed = UnityEngine.Random.Range(3, 20),
            luck = UnityEngine.Random.Range(3, 20),
            items = UnityEngine.Random.Range(3, 20),
            exp = 0,
            lives = 3,
            slug = currentJob
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
        //GetComponent<ActiveCharacterController>().DetermineActiveCharacterCurrentLevel();
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

    public List<Enemy> GetEnemiesForFightScene(int mazeRoomNumber)
    {
        List<Enemy> listOfEnemies = new List<Enemy>();
        int mod = mazeRoomNumber % 20;
        int max = (mod / 4) + 1;
        int amountOfEnemiesForList = UnityEngine.Random.Range(1, max);
        int rarity = Mathf.FloorToInt((float)mazeRoomNumber / 10);
        for (int i = 0; i < amountOfEnemiesForList; i++)
        {
            listOfEnemies.Add(GetRandomEnemy(rarity));
        }
        return listOfEnemies;
    }

    Enemy GetRandomEnemy(int rarity)
    {
        float randomValue = UnityEngine.Random.value;
        if (randomValue >= 0.9f && randomValue < 0.95f)
        {
            rarity = IncreaseOrDecreaseRarity(rarity, 1);
        }
        else if (randomValue >= 0.95f && randomValue < 0.95f)
        {
            rarity = IncreaseOrDecreaseRarity(rarity, 2);
        }
        else if (randomValue >= 0.98f && randomValue < 0.99)
        {
            rarity = IncreaseOrDecreaseRarity(rarity, 3);
        }
        else if (randomValue >= 0.99f && randomValue < 1)
        {
            rarity = IncreaseOrDecreaseRarity(rarity, 4);
        }
        return FetchEnemyByID(enemyID[rarity][UnityEngine.Random.Range(0, enemyID[rarity].Count)]);
    }

    int IncreaseOrDecreaseRarity(int rarity, int amount)
    {
        if (UnityEngine.Random.value > 0.5f)
        {
            rarity += amount;
            if (rarity > enemyID.Count - 1)
            {
                rarity = enemyID.Count - 1;
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

[Serializable]
public class Character
{
    public int id { get; set; }
    public string name { get; set; }
    public string job { get; set; }
    public int numberOfAttacks { get; set; }
    public int maxHP { get; set; }
    public int currentHP { get; set; }
    public int maxMP { get; set; }
    public int currentMP { get; set; }
    public int attack { get; set; }
    public int special { get; set; }
    public int defense { get; set; }
    public int speed { get; set; }
    public int luck { get; set; }
    public int items { get; set; }
    public int exp { get; set; }
    public int lives { get; set; }
    public string slug { get; set; }
    public List<int> itemsHolding { get; set; }
    public List<int> rarity { get; set; }
    //public Sprite sprite { get; set; }

    public Character(int id, string name, string job, int numberOfAttacks, int hp, int mp, int attack, int special,
                     int defense, int speed, int luck, int items, int exp, int lives, string slug)
    {
        this.id = id;
        this.name = name;
        this.job = job;
        this.numberOfAttacks = numberOfAttacks;
        this.maxHP = hp;
        this.maxMP = mp;
        this.currentHP = hp;
        this.currentMP = mp;
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
        EnemyHP = character.maxHP;
    }
}
