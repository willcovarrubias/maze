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

    public GameObject message, amount;
    public Text nameText, statsText, statsValueText;
    public Image characterPortrait;
    public GameObject actionButton1, actionButton2;

    //Building UI Stuff
    int currentSlotId;

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

    public void PopulateCurrentRoster()
    {
        //This should update every time the roster is populated so that the newest size of the roster is reflected.
        rosterSize = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes.Count;
        amount.GetComponent<Text>().text = "Availability: " + rosterSize + "/" + GetMaxRosterSize();
        if (rosterSize == 0)
        {
            message.GetComponent<Text>().text = "Barracks empty. Recruit wanderers from Caravan.";
        }
        else
        {
            message.GetComponent<Text>().text = "";
        }
        //DetermineActiveCharacterCurrentLevel();
        for (int i = 0; i < GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes.Count; i++)
        {
            characterObject[i].GetComponent<CharacterData>().character = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i];
            characterObject[i].GetComponent<CharacterData>().thisObjectsID = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i].id;

            characterObject[i].name = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i].name;
            characterObject[i].GetComponentInChildren<Text>().text = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i].name +
                "\n" + GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i].job +
                "\nLvl: " + DetermineActiveCharacterCurrentLevel(GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i].exp) +
                "\nLives: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i].lives;
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
        GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox(currentlyClickedCharacter.name + " is now active");
        RosterAdvancedUIClose();
    }

    public void DismissCharacter()
    {
        RemoveACharacterSlotInBarracksUIByID(currentSlotId);
        GameMaster.gameMaster.GetComponent<CharacterDatabase>().DeleteHero(currentlyClickedCharacter);
        GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Dismissed " + currentlyClickedCharacter.name);
        RosterAdvancedUIClose();
    }

    public void SetCurrentlyClickedCharacter(Character characterClicked)
    {
        currentlyClickedCharacter = characterClicked;
    }

    public void PopulateBarracksPopUp(Character character, int slotID)
    {
        currentSlotId = slotID;
        nameText.text = character.name;
        statsText.text = "<b>Stats</b>\nLevel\nEXP\nJob\nHP\nMP\nAttack\nSpecial\nDefense\nSpeed\nLuck\nInventory\nLives";
        statsValueText.text = "\n" + DetermineActiveCharacterCurrentLevel(character.exp) +
            "\n" + character.exp +
            "\n " + character.job +
            "\n" + character.currentHP + "/" + character.maxHP +
            "\n" + character.currentMP + "/" + character.maxMP +
            "\n" + character.attack +
            "\n" + character.special +
            "\n" + character.defense +
            "\n" + character.speed +
            "\n" + character.luck +
            "\n" + character.items +
            "\n" + character.lives;
        characterPortrait.sprite = Resources.Load<Sprite>("Art/CharacterSprites/" + character.slug);
        actionButton1.GetComponentInChildren<Text>().text = "Set Active";
        actionButton1.GetComponent<Button>().onClick.RemoveAllListeners();
        actionButton1.GetComponent<Button>().onClick.AddListener(SetActiveCharacter);
        actionButton2.SetActive(true);
        //expText.text = "XP: " + (GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp - (float)expLevels[activeCharacterLevel - 1]) + "/" + (float)(expLevels[activeCharacterLevel] - expLevels[activeCharacterLevel - 1]);
    }

    private void GenerateHeroSlotsBasedOnStartupRosterSize()
    {
        for (int i = 0; i < characterSlots.Count; i++)
        {
            Destroy(characterSlots[i].gameObject);
        }
        characterSlots.Clear();
        characterObject.Clear();
        for (int i = 0; i < rosterSize; i++)
        {
            GameObject slot = Instantiate(characterSlot);
            slot.transform.SetParent(characterSlotPanel.transform, false);
            slot.GetComponent<ItemSlot>().id = i;
            characterSlots.Add(slot);
            //Adds an ID to each slot when it generates the slots. Used for drag/drop.
            //characterSlots[characterSlotAmount - 1].GetComponent<ItemSlot>().id = characterSlotAmount - 1;
            //characterSlots[characterSlotAmount - 1].name = "Slot" + (characterSlotAmount - 1);
            //characterSlots[i].transform.SetParent(characterSlotPanel.transform);

            GameObject characterObj = Instantiate(characterObjectPrefab);
            characterObj.transform.SetParent(slot.transform, false);
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

    public void RemoveACharacterSlotInBarracksUIByID(int slot)
    {
        //GameObject charObject = characterObject[slot];
        characterSlotAmount--;
        Destroy(characterSlots[slot]);
        characterSlots.RemoveAt(slot);
        Destroy(characterObject[slot]);
        characterObject.RemoveAt(slot);
        ResizeSlotPanelUI();
        for (int i = 0; i < characterSlots.Count; i++)
        {
            characterSlots[i].GetComponent<ItemSlot>().id = i;
        }
        if (characterSlots.Count == 0)
        {
            message.GetComponent<Text>().text = "Barracks empty. Recruit wanderers from Caravan.";
        }
        else
        {
            message.GetComponent<Text>().text = "";
        }
        amount.GetComponent<Text>().text = "Availability: " + characterSlots.Count + "/" + GetMaxRosterSize();
    }

    public void AddACharacterSlotInBarracksUI(Character newHero)
    {
        GameObject slot = Instantiate(characterSlot);
        slot.transform.transform.SetParent(characterSlotPanel.transform, false);
        slot.GetComponent<ItemSlot>().id = characterSlotAmount;
        characterSlots.Add(slot);
        characterSlotAmount++;

        GameObject characterObj = Instantiate(characterObjectPrefab);
        characterObj.transform.SetParent(slot.transform, false);
        characterObj.transform.localPosition = Vector2.zero;
        characterObj.GetComponent<CharacterData>().characterIsAlreadyRecruited = true;
        characterObj.GetComponent<CharacterData>().character = newHero;
        characterObj.GetComponent<CharacterData>().thisObjectsID = newHero.id;
        characterObj.name = newHero.name;
        characterObj.GetComponentInChildren<Text>().text = newHero.name +
            "\n" + newHero.job +
            "\nLvl: " + DetermineActiveCharacterCurrentLevel(newHero.exp) +
            "\nLives: " + newHero.lives;
        characterObj.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Art/CharacterSprites/" + newHero.slug);
        characterObject.Add(characterObj);

        //GenerateHeroSlotsBasedOnStartupRosterSize();
        //PopulateCurrentRoster();
        ResizeSlotPanelUI();
        rosterSize = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes.Count;
        amount.GetComponent<Text>().text = "Availability: " + rosterSize + "/" + GetMaxRosterSize();
    }

    void ResizeSlotPanelUI()
    {
        //Sets the slot panel RectTransform's size dependent on how many slots there are. This allows for the scrolling logic to work.
        //TODO: Maybe figure out a way to not hard code these values below?
        //slotPanelRectTransform.Translate(0, (int)(Mathf.Ceil((float)characterSlotAmount / 2) * -250), 0);
        //slotPanelRectTransform.sizeDelta = new Vector2(slotPanelRectTransform.sizeDelta.x, (Mathf.Ceil((float)characterSlotAmount / 2) * slotPanelRectTransform.sizeDelta.y));
    }
}
