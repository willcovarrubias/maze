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
        
        
    }

    private void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.G))
        {
            UpdateListOfHeroes();
        }
    }

    void GenerateListOfHeroesObjects()
    {
        gameMaster.GetComponent<CharacterDatabase>().DeleteAllWanderers();
        for (int i = 0; i < maxAmountOfHeroesToRecruit; i++)
        {
            characterObject.Add(Instantiate(characterObjectPrefab));

            characterObject[i].transform.SetParent(characterSlots[i].transform);
            characterObject[i].transform.localPosition = Vector2.zero;

            Character newWander = gameMaster.GetComponent<CharacterDatabase>().CreateRandomWanderer();
            characterObject[i].name = newWander.name;
            characterObject[i].GetComponent<Text>().text = newWander.name;
            characterObject[i].GetComponent<CharacterData>().character = newWander;
        }

    }

    void UpdateListOfHeroes()
    {
        GameMaster.gameMaster.GetComponent<CharacterDatabase>().DeleteAllWanderers();

       
        for (int i = 0; i < maxAmountOfHeroesToRecruit; i++)
        {
            gameMaster.GetComponent<CharacterDatabase>().CreateRandomWanderer();
            characterObject[i].name = gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].name;
            characterObject[i].GetComponent<Text>().text = gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].name;
            characterObject[i].GetComponent<CharacterData>().character = gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i];
            //characterObject.GetComponent<Image>().sprite = gameMaster.GetComponent<CharacterDatabase>().CreateHero().sprite;
        }
    }

    private void AddHeroSlot()
    {
        for (int i = 0; i < maxAmountOfHeroesToRecruit; i++)
        {
            characterSlots.Add(Instantiate(characterSlot));
            //characterSlotAmount++;
            //Adds an ID to each slot when it generates the slots. Used for drag/drop.
            //characterSlots[characterSlotAmount - 1].GetComponent<ItemSlot>().id = characterSlotAmount - 1;
            //characterSlots[characterSlotAmount - 1].name = "Slot" + (characterSlotAmount - 1);
            characterSlots[i].transform.SetParent(characterSlotPanel.transform);

            
            
        }
            
    }

   

}
