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
    public GameObject characaterSlot;
    public GameObject characterObjectPrefab;
    public int characterSlotAmount;
    public List<GameObject> characterSlots = new List<GameObject>();

    // Use this for initialization
    void Start () {
        gameMaster = GameObject.FindGameObjectWithTag("GameController");

        maxAmountOfHeroesToRecruit = 5; //Set this somewhere, possibly from a village upgrade milestone. Goes up with game progress.

        GenerateListOfHeroes();
	}

    void GenerateListOfHeroes()
    {
        for (int i = 0; i < maxAmountOfHeroesToRecruit; i++)
        {
            AddHeroSlot();
            GameObject characterObject = Instantiate(characterObjectPrefab);

            characterObject.transform.SetParent(characterSlots[i].transform);
            characterObject.transform.localPosition = Vector2.zero;
            //characterObject.GetComponent<CharacterData>().character = gameMaster.GetComponent<CharacterDatabase>().CreateRandomHero();
            //characterObject.name = characterObject.GetComponent<CharacterData>().character.name;
            //characterObject.GetComponent<Text>().text = characterObject.GetComponent<CharacterData>().character.name;

            //characterObject.GetComponent<Image>().sprite = gameMaster.GetComponent<CharacterDatabase>().CreateHero().sprite;
        }

    }

    private void AddHeroSlot()
    {
        characterSlots.Add(Instantiate(characaterSlot));
        characterSlotAmount++;
        //Adds an ID to each slot when it generates the slots. Used for drag/drop.
        characterSlots[characterSlotAmount - 1].GetComponent<ItemSlot>().id = characterSlotAmount - 1;
        characterSlots[characterSlotAmount - 1].name = "Slot" + (characterSlotAmount - 1);
        characterSlots[characterSlotAmount - 1].transform.SetParent(characterSlotPanel.transform);
    }
}
