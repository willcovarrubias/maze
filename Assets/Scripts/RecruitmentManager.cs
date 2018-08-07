using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecruitmentManager : MonoBehaviour
{

    GameObject gameMaster;
    private int maxAmountOfHeroesToRecruit;
    Character currentlyClickedCharacter;
    GameObject objectsToDestroyWhenWandererIsRecruited;

    //UI Stuff.
    public GameObject recruitmentPanel;
    public GameObject characterSlotPanel;
    public GameObject characterSlot;
    public GameObject characterObjectPrefab;
    public List<GameObject> characterObject = new List<GameObject>();
    public int characterSlotAmount;
    public List<GameObject> characterSlots = new List<GameObject>();
    public Text nameText, levelText, jobText, hpText, mpText, attackText, specialText, defenseText, speedText, luckText, expText;
    public Image wandererPortrait;
    public GameObject caravanPopUPObject;


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
        CaravanAdvancedUIClose();
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
            characterObject[i].GetComponentInChildren<Text>().text = gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].name +
                        "\n: " + gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].job +
                        "\nHP: " + gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].hp +
                        "\nMP: " + gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].mp;

            characterObject[i].GetComponent<CharacterData>().character = gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i];
            characterObject[i].GetComponent<CharacterData>().thisObjectsID = gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].id;
            characterObject[i].GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Art/CharacterSprites/" + gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].slug);


            //Debug.Log("Wanderer: " + gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].name +
            //          "\nID: " + gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].id);


        }
    }

    public void PopulateCaravanPopUp(Character character)
    {
        nameText.text = character.name;
        //levelText.text = "Lv. " + activeCharacterLevel.ToString();
        jobText.text = character.job;
        hpText.text = "HP: " + character.hp.ToString();
        mpText.text = "MP: " + character.mp.ToString();
        attackText.text = "Attack: " + character.attack.ToString();
        specialText.text = "Special: " + character.special.ToString();
        defenseText.text = "Defense: " + character.defense.ToString();
        //speedText.text = GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.speed.ToString();
        luckText.text = "Luck: " + character.luck.ToString();
        wandererPortrait.sprite = Resources.Load<Sprite>("Art/CharacterSprites/" + character.slug);
        //expText.text = "XP: " + (GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp - (float)expLevels[activeCharacterLevel - 1]) + "/" + (float)(expLevels[activeCharacterLevel] - expLevels[activeCharacterLevel - 1]);

    }

    public void FinalizeRecruitment()
    {
        characterObject.Remove(objectsToDestroyWhenWandererIsRecruited);
        characterSlots.Remove(objectsToDestroyWhenWandererIsRecruited.transform.parent.gameObject);
        Destroy(objectsToDestroyWhenWandererIsRecruited.transform.parent.gameObject);
        GameMaster.gameMaster.GetComponent<CharacterDatabase>().RecruitHero(currentlyClickedCharacter);
        CaravanAdvancedUIClose();
    }

    public void SetCurrentlyClickedCharacter(Character characterClicked, GameObject objectToDestroy)
    {
        currentlyClickedCharacter = characterClicked;
        objectsToDestroyWhenWandererIsRecruited = objectToDestroy;
    }

    internal void CaravanAdvancedUIOpen()
    {
        caravanPopUPObject.SetActive(true);
    }

    public void CaravanAdvancedUIClose()
    {
        caravanPopUPObject.SetActive(false);
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
