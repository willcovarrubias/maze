using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RosterManager : MonoBehaviour
{

    private int maxRosterSize;

    //UI Stuff.
    public GameObject rosterPanel;
    public GameObject characterSlotPanel;
    public GameObject characterSlot;
    public GameObject characterObjectPrefab;
    private List<GameObject> characterObject = new List<GameObject>();
    public int characterSlotAmount;
    public List<GameObject> characterSlots = new List<GameObject>();

    public RectTransform slotPanelRectTransform;
    public GameObject barracksPopUp;

    public Text nameText, levelText, jobText, hpText, mpText, attackText, specialText, defenseText, speedText, luckText, expText;

    Character currentlyClickedCharacter;

    int[] expLevels = new int[5] { 0, 200, 400, 800, 1600 };

    void Start()
    {
        maxRosterSize = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes.Count;

        GenerateHeroSlotsBasedOnStartupRosterSize();
        ResizeSlotPanelUI();
        PopulateCurrentRoster();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("List of heros count: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes.Count);
        }
    }

    public void PopulateCurrentRoster()
    {
        //This should update every time the roster is populated so that the newest size of the roster is reflected.
        maxRosterSize = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes.Count;

        //DetermineActiveCharacterCurrentLevel();

        for (int i = 0; i < GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes.Count; i++)
        {
            characterObject[i].GetComponent<CharacterData>().character = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i];
            characterObject[i].GetComponent<CharacterData>().thisObjectsID = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i].id;

            characterObject[i].name = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i].name;
            characterObject[i].GetComponentInChildren<Text>().text = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i].name +
                                                            "\nLv. " + DetermineActiveCharacterCurrentLevel(GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i].exp) +
                                                            "\n" + GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i].job +
                                                            "\nLives: 1/3 (use sprites)";
        }
    }

    internal void RosterAdvancedUIOpen()
    {
        barracksPopUp.SetActive(true);
    }

    public void RosterAdvancedUIClose()
    {
        barracksPopUp.SetActive(false);
    }

    public void SetActiveCharacter()
    {
        GameMaster.gameMaster.GetComponent<CharacterDatabase>().ChangeActiveCharacter(currentlyClickedCharacter.id);
    }

    public void SetCurrentlyClickedCharacter(Character characterClicked)
    {
        currentlyClickedCharacter = characterClicked;
    }

    public void PopulateBarracksPopUp(Character character)
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
        //expText.text = "XP: " + (GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp - (float)expLevels[activeCharacterLevel - 1]) + "/" + (float)(expLevels[activeCharacterLevel] - expLevels[activeCharacterLevel - 1]);

    }

    private void GenerateHeroSlotsBasedOnStartupRosterSize()
    {
        for (int i = 0; i < maxRosterSize; i++)
        {
            GameObject slot = Instantiate(characterSlot);
            slot.transform.SetParent(characterSlotPanel.transform);
            characterSlots.Add(slot);
            //Adds an ID to each slot when it generates the slots. Used for drag/drop.
            //characterSlots[characterSlotAmount - 1].GetComponent<ItemSlot>().id = characterSlotAmount - 1;
            //characterSlots[characterSlotAmount - 1].name = "Slot" + (characterSlotAmount - 1);
            //characterSlots[i].transform.SetParent(characterSlotPanel.transform);

            GameObject characterObj = Instantiate(characterObjectPrefab);
            characterObj.transform.SetParent(slot.transform);
            characterObj.transform.localPosition = Vector2.zero;
            characterObj.GetComponent<CharacterData>().characterIsAlreadyRecruited = true;
            characterObj.GetComponent<CharacterData>().character = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i];


            characterObject.Add(characterObj);

            characterSlotAmount++;

        }

    }

    private int DetermineActiveCharacterCurrentLevel(int exp)
    {
        int charactersLevel = 0;

        for (int i = 0; i < expLevels.Length; i++)
        {
            if (exp >= expLevels[i] && exp <= expLevels[i + 1])
            {
                charactersLevel = i + 1;
            }
        }

        return charactersLevel;
    }

    public void RemoveACharacterSlotInBarracksUI()
    {
        characterSlotAmount--;
        Destroy(characterSlots[characterSlotAmount]);

        ResizeSlotPanelUI();
    }

    public void AddACharacterSlotInBarracksUI()
    {
        GameObject slot = Instantiate(characterSlot);
        slot.transform.transform.SetParent(characterSlotPanel.transform);
        characterSlots.Add(slot);
        characterSlotAmount++;

        GameObject characterObj = Instantiate(characterObjectPrefab);
        characterObj.transform.SetParent(slot.transform);
        characterObj.transform.localPosition = Vector2.zero;
        characterObj.GetComponent<CharacterData>().characterIsAlreadyRecruited = true;
        characterObject.Add(characterObj);

        ResizeSlotPanelUI();
    }

    void ResizeSlotPanelUI()
    {
        //Sets the slot panel RectTransform's size dependent on how many slots there are. This allows for the scrolling logic to work.
        //TODO: Maybe figure out a way to not hard code these values below?
        slotPanelRectTransform.Translate(0, (int)(Mathf.Ceil((float)characterSlotAmount / 2) * -250), 0);
        slotPanelRectTransform.sizeDelta = new Vector2(590, (Mathf.Ceil((float)characterSlotAmount / 2) * 155));
    }
}
