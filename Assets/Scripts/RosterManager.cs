using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RosterManager : MonoBehaviour
{
    int rosterSize;

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

    public Text nameText, levelText, jobText, hpText, mpText, attackText, specialText, defenseText, speedText, luckText, expText, inventorySizeText;
    public Image characterPortrait;

    //Building UI Stuff
    public Text buildingLevel, buildingLevelUpRequirements;


    Character currentlyClickedCharacter;

    int[] expLevels = { 0, 200, 400, 800, 1600 };

    void Start()
    {
        rosterSize = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes.Count;

        GenerateHeroSlotsBasedOnStartupRosterSize();
        ResizeSlotPanelUI();
        PopulateCurrentRoster();
    }

    public int GetMaxRosterSize()
    {
        int caravanLevel = GetComponent<BuildingsManager>().GetBarracksLevel();
        switch (caravanLevel)
        {
            case 0:
                return 2;
            case 1:
                return 3;
            case 2:
                return 4;
            case 3:
                return 5;
            case 4:
                return 6;
            case 5:
                return 7;
            default:
                return 0;
        }
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
        rosterSize = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes.Count;

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
            characterObject[i].GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Art/CharacterSprites/" + GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i].slug);
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
        levelText.text = "Lv. " + DetermineActiveCharacterCurrentLevel(character.exp);
        jobText.text = character.job;
        hpText.text = "HP: " + character.currentHP + " / " + character.maxHP;
        mpText.text = "MP: " + character.currentMP + " / " + character.maxMP;
        inventorySizeText.text = "Carry Amount: " + character.items;
        attackText.text = "Attack: " + character.attack.ToString();
        specialText.text = "Special: " + character.special.ToString();
        defenseText.text = "Defense: " + character.defense.ToString();
        speedText.text = "Speed: " + character.speed;
        luckText.text = "Luck: " + character.luck.ToString();
        characterPortrait.sprite = Resources.Load<Sprite>("Art/CharacterSprites/" + character.slug);
        //expText.text = "XP: " + (GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp - (float)expLevels[activeCharacterLevel - 1]) + "/" + (float)(expLevels[activeCharacterLevel] - expLevels[activeCharacterLevel - 1]);

    }

    private void GenerateHeroSlotsBasedOnStartupRosterSize()
    {
        for (int i = 0; i < rosterSize; i++)
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
            if (exp >= expLevels[i] && exp <= GameMaster.gameMaster.GetComponent<ActiveCharacterController>().GetExpCap())
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
