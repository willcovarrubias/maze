using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecruitmentManager : MonoBehaviour {

    GameObject gameMaster;
    private int maxAmountOfHeroesToRecruit;

    //UI Stuff.
    public GameObject recruitmentPanel;
    public GameObject characterSlotPanel;
    public GameObject characterSlot;
    public GameObject characterObjectPrefab;
    private List<GameObject> characterObject = new List<GameObject>();
    public int characterSlotAmount;
    public List<GameObject> characterSlots = new List<GameObject>();

    // Use this for initialization
    void Start () {
        gameMaster = GameObject.FindGameObjectWithTag("GameController");

        maxAmountOfHeroesToRecruit = 5; //Set this somewhere, possibly from a village upgrade milestone. Goes up with game progress.

        
        AddHeroSlot();
        GenerateListOfHeroesObjects();
        DisplayCurrentListOfWanderers();
        
        
    }

    private void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.G))
        {
            //AddHeroSlot();
            UpdateListOfHeroes();
            DisplayCurrentListOfWanderers();
        }
    }

    void GenerateListOfHeroesObjects()
    {
        //gameMaster.GetComponent<CharacterDatabase>().DeleteAllWanderers();
        for (int i = 0; i < GameMaster.gameMaster.characterDB.listOfWanderers.Count; i++)
        {
            

            
        }
        
    }

    void UpdateListOfHeroes()//TODO: Add some logic here that updates the list after a certain amount of time.
    {
        GameMaster.gameMaster.GetComponent<CharacterDatabase>().DeleteAllWanderers();


        for (int i = 0; i < maxAmountOfHeroesToRecruit; i++)
        {
            Character newWanderer = gameMaster.GetComponent<CharacterDatabase>().CreateRandomWanderer();
            //characterObject[i].name = newWanderer.name;
            //characterObject[i].GetComponent<Text>().text = newWanderer.name;
            //characterObject[i].GetComponent<CharacterData>().character = newWanderer;
            //characterObject[i].GetComponent<CharacterData>().thisCharactersID = newWanderer.id;

            //gameMaster.GetComponent<CharacterDatabase>().CreateRandomWanderer();
            //characterObject[i].name = gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].name;
            //characterObject[i].GetComponent<Text>().text = gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].name;
            //characterObject[i].GetComponent<CharacterData>().character = gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i];
            //characterObject.GetComponent<Image>().sprite = gameMaster.GetComponent<CharacterDatabase>().CreateHero().sprite;
        }
        GameMaster.gameMaster.Save();
    }

    void DisplayCurrentListOfWanderers()
    {
        for (int i = 0; i < GameMaster.gameMaster.characterDB.listOfWanderers.Count; i++)
        {
            characterSlots.Add(Instantiate(characterSlot));
            characterSlots[i].transform.SetParent(characterSlotPanel.transform);

            characterObject.Add(Instantiate(characterObjectPrefab));
            characterObject[i].transform.SetParent(characterSlots[i].transform);
            characterObject[i].transform.localPosition = Vector2.zero;


            Debug.Log("Wanderer: " + gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].name +
                "\nID: " + gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].id);

                characterObject[i].name = gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].name;
                characterObject[i].GetComponent<Text>().text =
                    gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].name +
                    "\nClass: " + gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].job +
                    "\nHP: " + gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].hp +
                    "\nMP: " + gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].mp;

                characterObject[i].GetComponent<CharacterData>().character = gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i];
                characterObject[i].GetComponent<CharacterData>().thisCharactersID = gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].id;

            


        }
    }

    private void AddHeroSlot()
    {
        for (int i = 0; i < maxAmountOfHeroesToRecruit; i++)
        {
            //characterSlotAmount++;
            //Adds an ID to each slot when it generates the slots. Used for drag/drop.
            //characterSlots[characterSlotAmount - 1].GetComponent<ItemSlot>().id = characterSlotAmount - 1;
            //characterSlots[characterSlotAmount - 1].name = "Slot" + (characterSlotAmount - 1);

            
            
        }
            
    }

   

}
