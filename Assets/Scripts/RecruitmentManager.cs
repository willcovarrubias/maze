using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecruitmentManager : MonoBehaviour
{

    GameObject gameMaster;
    private int maxAmountOfHeroesToRecruit;

    //UI Stuff.
    public GameObject recruitmentPanel;
    public GameObject characterSlotPanel;
    public GameObject characterSlot;
    public GameObject characterObjectPrefab;
    public List<GameObject> characterObject = new List<GameObject>();
    public int characterSlotAmount;
    public List<GameObject> characterSlots = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameController");

        maxAmountOfHeroesToRecruit = 5; //Set this somewhere, possibly from a village upgrade milestone. Goes up with game progress.

        AddUIStuffFirst();
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
        if (Input.GetKeyDown(KeyCode.R))
        {

            RemoveWanderers();
        }

    }


    void UpdateListOfHeroes()//TODO: Add some logic here that updates the list after a certain amount of time.
    {
        GameMaster.gameMaster.GetComponent<CharacterDatabase>().DeleteAllWanderers();
        RemoveWanderers();

        for (int i = 0; i < maxAmountOfHeroesToRecruit; i++)
        {


            /*
            
            */
            //characterSlots.RemoveAt(i);
            //Destroy(characterSlots[i]);
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


    void AddUIStuffFirst()
    {
        for (int i = 0; i < GameMaster.gameMaster.characterDB.listOfWanderers.Count; i++)
        {
            GameObject slot = Instantiate(characterSlot);
            slot.transform.SetParent(characterSlotPanel.transform);
            slot.GetComponent<ItemSlot>().id = i;
            characterSlots.Add(slot);

            GameObject characterObj = Instantiate(characterObjectPrefab);
            characterObj.transform.SetParent(slot.transform);
            characterObj.transform.localPosition = Vector2.zero;
            characterObject.Add(characterObj);
        }

    }
    void DisplayCurrentListOfWanderers()
    {
        AddUIStuffFirst();
        for (int i = 0; i < GameMaster.gameMaster.characterDB.listOfWanderers.Count; i++)
        {
            /*
            Debug.Log("This is null");
            characterSlots.Add(Instantiate(characterSlot));
            characterObject.Add(Instantiate(characterObjectPrefab));

            characterSlots[i].transform.SetParent(characterSlotPanel.transform);
            characterObject[i].transform.SetParent(characterSlots[i].transform);
            characterObject[i].transform.localPosition = Vector2.zero;
            */

            //Debug.Log("This is NOT null");
            characterObject[i].name = gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].name;
            characterObject[i].GetComponent<Text>().text = gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].name +
                        "\nClass: " + gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].job +
                        "\nHP: " + gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].hp +
                        "\nMP: " + gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].mp;

            characterObject[i].GetComponent<CharacterData>().character = gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i];
            characterObject[i].GetComponent<CharacterData>().thisObjectsID = gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].id;


            //Debug.Log("Wanderer: " + gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].name +
            //          "\nID: " + gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].id);


        }
    }

    public void RemoveWanderers()
    {
        for (int i = 0; i < characterSlots.Count; i++)
        {

            Destroy(characterSlots[i].gameObject);

        }

        for (int i = 0; i < characterObject.Count; i++)
        {

            Destroy(characterObject[i].gameObject);
        }
        characterSlots.Clear();
        characterObject.Clear();
    }

}
