using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System;

public class GameMaster : MonoBehaviour {

    public static GameMaster gameMaster;

    public int roomCount = -1;

    public CharacterDatabase characterDB;
    
    public void Awake()
    {
        if (gameMaster == null)
        {
            DontDestroyOnLoad(gameObject);
            gameMaster = this;
        }
        else if (gameMaster != this)
        {
            Destroy(gameObject);
        }

        //PlayerPrefs.DeleteAll();
    }

    private void Start()
    {
        characterDB = GetComponent<CharacterDatabase>();

        Load();
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        SavedPlayerData data = new SavedPlayerData();
        data.savedListOfHeroes = characterDB.listOfHeroes;
        data.savedListOfWanderers = characterDB.listOfWanderers;
        data.savedActiveCharacter = characterDB.activeCharacter;
        
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Saving to: " + Application.persistentDataPath + "/playerInfo.dat");
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            SavedPlayerData data = (SavedPlayerData)bf.Deserialize(file);

            file.Close();

            characterDB.listOfHeroes = data.savedListOfHeroes;
            characterDB.listOfWanderers = data.savedListOfWanderers;
            characterDB.activeCharacter = data.savedActiveCharacter;

            Debug.Log("Stats loaded!");

        }
    }


}

[Serializable]
class SavedPlayerData
{
    public List<Character> savedListOfHeroes;
    public List<Character> savedListOfWanderers;
    public Character savedActiveCharacter;

}
