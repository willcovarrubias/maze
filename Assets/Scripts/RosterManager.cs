﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RosterManager : MonoBehaviour {

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

    void Start () {
        maxRosterSize = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes.Count;

        GenerateHeroSlotsBasedOnStartupRosterSize();
        ResizeSlotPanelUI();
        PopulateCurrentRoster();
    }

    public void PopulateCurrentRoster()
    {
        //This should update every time the roster is populated so that the newest size of the roster is reflected.
        maxRosterSize = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes.Count;

        for (int i = 0; i < maxRosterSize; i++)
        {
            characterObject[i].GetComponent<CharacterData>().character = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i];
            characterObject[i].GetComponent<CharacterData>().thisCharactersID = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i].id;

            characterObject[i].name = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i].name;
            characterObject[i].GetComponent<Text>().text = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i].name +
                                                            "\nClass: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i].job +
                                                            "\nHP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i].hp +
                                                            "\nMP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes[i].mp;
        }
    }

    private void GenerateHeroSlotsBasedOnStartupRosterSize()
    {
        for (int i = 0; i < maxRosterSize; i++)
        {
            characterSlots.Add(Instantiate(characterSlot));
            characterSlotAmount++;
            //Adds an ID to each slot when it generates the slots. Used for drag/drop.
            //characterSlots[characterSlotAmount - 1].GetComponent<ItemSlot>().id = characterSlotAmount - 1;
            //characterSlots[characterSlotAmount - 1].name = "Slot" + (characterSlotAmount - 1);
            characterSlots[i].transform.SetParent(characterSlotPanel.transform);

            characterObject.Add(Instantiate(characterObjectPrefab));
            characterObject[i].transform.SetParent(characterSlots[i].transform);
            characterObject[i].transform.localPosition = Vector2.zero;
            characterObject[i].GetComponent<CharacterData>().characterIsAlreadyRecruited = true;

        }

    }

    public void RemoveACharacterSlotInBarracksUI()
    {
        characterSlotAmount--;
        Destroy(characterSlots[characterSlotAmount]);

        ResizeSlotPanelUI();
    }

    public void AddACharacterSlotInBarracksUI()
    {
        characterSlots.Add(Instantiate(characterSlot));
        characterSlots[maxRosterSize].transform.SetParent(characterSlotPanel.transform);
        characterSlotAmount++;

        characterObject.Add(Instantiate(characterObjectPrefab));
        characterObject[maxRosterSize].transform.SetParent(characterSlots[maxRosterSize].transform);
        characterObject[maxRosterSize].transform.localPosition = Vector2.zero;
        characterObject[maxRosterSize].GetComponent<CharacterData>().characterIsAlreadyRecruited = true;

        ResizeSlotPanelUI();
    }

    void ResizeSlotPanelUI()
    {
        //Sets the slot panel RectTransform's size dependent on how many slots there are. This allows for the scrolling logic to work.
        //TODO: Maybe figure out a way to not hard code these values below?
        slotPanelRectTransform.Translate(0, (characterSlotAmount * -250), 0);
        slotPanelRectTransform.sizeDelta = new Vector2(590, (characterSlotAmount * 72));
    }
}
