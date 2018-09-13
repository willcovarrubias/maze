using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecruitmentManager : MonoBehaviour
{
    int maxAmountOfHeroesToRecruit;
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
    public GameObject refreshTimeText;

    public int previousTime;
    public int refreshTime;

    void Start()
    {
        refreshTime = 14400; //4 hours in seconds
        SetMaxAmountOfWanderers();
        if (PlayerPrefs.GetInt("Previous Time") == 0)
        {
            UpdateListOfHeroes();
            PlayerPrefs.SetInt("Previous Time", GetTimeInSeconds());
        }
        previousTime = PlayerPrefs.GetInt("Previous Time");
        GameMaster.gameMaster.PlayerPrefsSave();
        DisplayCurrentListOfWanderers();
    }

    public void SetMaxAmountOfWanderers()
    {
        int caravanLevel = GetComponent<BuildingsManager>().GetCaravanLevel();
        switch (caravanLevel)
        {
            case 0:
                maxAmountOfHeroesToRecruit = 2;
                break;
            case 1:
                maxAmountOfHeroesToRecruit = 3;
                break;
            case 2:
                maxAmountOfHeroesToRecruit = 4;
                break;
            case 3:
                maxAmountOfHeroesToRecruit = 5;
                break;
            case 4:
                maxAmountOfHeroesToRecruit = 6;
                break;
            case 5:
                maxAmountOfHeroesToRecruit = 7;
                break;
        }
    }

    public void RefreshIfEnoughTimeHasPassed()
    {
        int currTime = GetTimeInSeconds();
        if (currTime > previousTime + refreshTime)
        {
            int timePassed = currTime - previousTime;
            int remainder = timePassed % refreshTime;
            previousTime = currTime - remainder;
            PlayerPrefs.SetInt("Previous Time", previousTime);
            GameMaster.gameMaster.PlayerPrefsSave();
            UpdateListOfHeroes();
            DisplayCurrentListOfWanderers();
        }
    }

    int GetTimeInSeconds()
    {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        return (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
    }

    void UpdateListOfHeroes()
    {
        GameMaster.gameMaster.GetComponent<CharacterDatabase>().DeleteAllWanderers();
        RemoveWanderers();

        for (int i = 0; i < maxAmountOfHeroesToRecruit; i++)
        {
            //characterSlots.RemoveAt(i);
            //Destroy(characterSlots[i]);
            Character newWanderer = GameMaster.gameMaster.GetComponent<CharacterDatabase>().CreateRandomWanderer();
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
            slot.transform.SetParent(characterSlotPanel.transform, false);
            slot.GetComponent<ItemSlot>().id = i;
            characterSlots.Add(slot);

            GameObject characterObj = Instantiate(characterObjectPrefab);
            characterObj.transform.SetParent(slot.transform, false);
            characterObj.transform.localPosition = Vector2.zero;
            characterObject.Add(characterObj);
        }
    }

    void DisplayCurrentListOfWanderers()
    {
        RemoveWanderers();
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
            characterObject[i].name = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].name;
            characterObject[i].GetComponentInChildren<Text>().text = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].name +
                "\n: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].job +
                "\nHP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].maxHP +
                "\nMP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].maxMP;

            characterObject[i].GetComponent<CharacterData>().character = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i];
            characterObject[i].GetComponent<CharacterData>().thisObjectsID = GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].id;
            characterObject[i].GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Art/CharacterSprites/" + GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].slug);

            //Debug.Log("Wanderer: " + gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].name +
            //          "\nID: " + gameMaster.GetComponent<CharacterDatabase>().listOfWanderers[i].id);
        }
    }

    public void PopulateCaravanPopUp(Character character)
    {
        nameText.text = character.name;
        //levelText.text = "Lv. " + activeCharacterLevel.ToString();
        jobText.text = character.job;
        hpText.text = "HP: " + character.maxHP.ToString();
        mpText.text = "MP: " + character.maxMP.ToString();
        attackText.text = "Attack: " + character.attack.ToString();
        specialText.text = "Special: " + character.special.ToString();
        defenseText.text = "Defense: " + character.defense.ToString();
        speedText.text = "Speed: " + character.speed;
        luckText.text = "Luck: " + character.luck.ToString();
        wandererPortrait.sprite = Resources.Load<Sprite>("Art/CharacterSprites/" + character.slug);
        //expText.text = "XP: " + (GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp - (float)expLevels[activeCharacterLevel - 1]) + "/" + (float)(expLevels[activeCharacterLevel] - expLevels[activeCharacterLevel - 1]);
    }

    public void FinalizeRecruitment()
    {
        if (GetComponent<RosterManager>().GetMaxRosterSize() > GameMaster.gameMaster.GetComponent<CharacterDatabase>().listOfHeroes.Count)
        {
            characterObject.Remove(objectsToDestroyWhenWandererIsRecruited);
            characterSlots.Remove(objectsToDestroyWhenWandererIsRecruited.transform.parent.gameObject);
            Destroy(objectsToDestroyWhenWandererIsRecruited.transform.parent.gameObject);
            GameMaster.gameMaster.GetComponent<CharacterDatabase>().RecruitHero(currentlyClickedCharacter);
            CaravanAdvancedUIClose();
        }
        else
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Barracks Full");
        }
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
