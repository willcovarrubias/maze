using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveCharacterController : MonoBehaviour
{
    public GameObject activeCharacterPanel;
    public Text nameTextObject, statsValueText, equippedText;

    //UI stuff for MoreInfo Panel
    public Text nameText, moreStatsText, moreEquipped, moreEquippedText;
    public Image activeHeroPortrait;
    public GameObject activeCharacterMoreInfoPanel;
    public GameObject expBarMoreInfo;

    Character activeCharacter;
    Weapons currentlyEquippedWeapon;
    static int expCap = 1600;
    int levelCap = 5;

    int[] expLevels = { 0, 200, 400, 800, 1600 };

    void Start()
    {

        GameMaster.gameMaster.GetComponent<CharacterDatabase>().GetActiveCharacter();
        UpdateActiveCharacterVisuals();
        //nameTextObject.text = ;
    }

    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Test();

            GiveExpToActiveCharacter(100);
        }
    }
    */

    public int GetExpCap()
    {
        return expCap;
    }

    public void SetXPBar(float exp)
    {
        expBarMoreInfo.transform.localScale = new Vector3(exp, expBarMoreInfo.transform.localScale.y, expBarMoreInfo.transform.localScale.z);
        expBarMoreInfo.transform.localScale = new Vector3(Mathf.Clamp(exp, 0f, 1f), expBarMoreInfo.transform.localScale.y, expBarMoreInfo.transform.localScale.z);
    }

    void UpdateEXPBar()
    {
        if (GetActiveCharacterCurrentLevel() < levelCap)
        {
            float calc_Level = (GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp - (float)expLevels[GetActiveCharacterCurrentLevel() - 1]) / (expLevels[GetActiveCharacterCurrentLevel()] - expLevels[GetActiveCharacterCurrentLevel() - 1]);
            SetXPBar(calc_Level);
        }
        else
        {
            SetXPBar(1);
        }
        //float calc_level = (expLevels[activeCharacterLevel] - GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp)
    }

    public void MoreInfoUIOpen()
    {
        activeCharacterMoreInfoPanel.SetActive(true);
    }

    public void MoreInfoUIClose()
    {
        activeCharacterMoreInfoPanel.SetActive(false);
    }

    public void UpdateActiveCharacterVisuals()
    {
        if (GetComponent<CharacterDatabase>().activeCharacter.id == -1)
        {
            activeCharacterPanel.SetActive(false);
        }
        else
        {
            activeCharacterPanel.SetActive(true);
            int attack = GetComponent<CharacterDatabase>().activeCharacter.attack;
            int special = GetComponent<CharacterDatabase>().activeCharacter.special;
            int defense = GetComponent<CharacterDatabase>().activeCharacter.defense;
            int speed = GetComponent<CharacterDatabase>().activeCharacter.speed;
            if (GetComponent<InventoryManager>().GetEquippedWeaponID() > 0)
            {
                attack += GetComponent<InventoryManager>().GetEquippedWeapon().Attack;
                special += GetComponent<InventoryManager>().GetEquippedWeapon().Special;
                speed += GetComponent<InventoryManager>().GetEquippedWeapon().Speed;
            }
            if (GetComponent<InventoryManager>().GetEquippedHatID() > 0)
            {
                defense += GetComponent<InventoryManager>().GetEquippedHat().Defense;
                speed += GetComponent<InventoryManager>().GetEquippedHat().Speed;
            }
            if (GetComponent<InventoryManager>().GetEquippedBodyID() > 0)
            {
                defense += GetComponent<InventoryManager>().GetEquippedBody().Defense;
                speed += GetComponent<InventoryManager>().GetEquippedBody().Speed;
            }

            nameTextObject.text = GetComponent<CharacterDatabase>().activeCharacter.name +
                "\n" + GetComponent<CharacterDatabase>().activeCharacter.job +
                "\nLevel " + GetActiveCharacterCurrentLevel();
            UpdateStats();
            equippedText.text = "";
            if (GetComponent<InventoryManager>().GetEquippedHatID() > 0)
            {
                equippedText.text += GetComponent<InventoryManager>().GetEquippedHat().Title;
            }
            equippedText.text += "\n";
            if (GetComponent<InventoryManager>().GetEquippedBodyID() > 0)
            {
                equippedText.text += GetComponent<InventoryManager>().GetEquippedBody().Title;
            }
            equippedText.text += "\n";
            if (GetComponent<InventoryManager>().GetEquippedWeaponID() > 0)
            {
                string text = GetComponent<InventoryManager>().GetEquippedWeapon().Title +
                    " (" + GetComponent<InventoryManager>().GetEquippedWeapon().Durability + ")";
                if (text.Length > 23)
                {
                    equippedText.text += "<size=" + (23- (text.Length - 23)) + ">" + text + "</size>";
                }
                else
                {
                    equippedText.text += text;
                }
            }

            if (GetActiveCharacterCurrentLevel() < levelCap)
            {
                nameText.text = nameTextObject.text + "\nEXP " + (GetComponent<CharacterDatabase>().activeCharacter.exp - (float)expLevels[GetActiveCharacterCurrentLevel() - 1]) +
                    "/" + (float)(expLevels[GetActiveCharacterCurrentLevel()] - expLevels[GetActiveCharacterCurrentLevel() - 1]);
            }
            else
            {
                nameText.text = nameTextObject.text + "\nEXP: MAX";
            }
            moreStatsText.text = GetComponent<CharacterDatabase>().activeCharacter.currentHP + "/" + GetComponent<CharacterDatabase>().activeCharacter.maxHP +
                "\n" + GetComponent<CharacterDatabase>().activeCharacter.currentMP + "/" + GetComponent<CharacterDatabase>().activeCharacter.maxMP +
                "\n" + GetComponent<InventoryManager>().GetCurrentSize() + "/" + GetComponent<CharacterDatabase>().activeCharacter.items +
                "\n" + attack + "\n" + defense + "\n" + special + "\n" + speed +
                "\n" + GetComponent<CharacterDatabase>().activeCharacter.luck +
                "\n" + GetComponent<CharacterDatabase>().activeCharacter.lives;
            moreEquippedText.text = equippedText.text;
            activeHeroPortrait.sprite = Resources.Load<Sprite>("Art/CharacterSprites/" + GetComponent<CharacterDatabase>().activeCharacter.slug);
            activeHeroPortrait.preserveAspect = true;
            UpdateEXPBar();
        }
        GetComponent<InventoryManager>().ChangeMaxInventorySize(GetComponent<CharacterDatabase>().activeCharacter.items);
    }

    public void UpdateStats()
    {
        statsValueText.text = GetComponent<CharacterDatabase>().activeCharacter.currentHP +
            "/" + GetComponent<CharacterDatabase>().activeCharacter.maxHP +
            "\n" + GetComponent<CharacterDatabase>().activeCharacter.currentMP +
            "/" + GetComponent<CharacterDatabase>().activeCharacter.maxMP +
            "\n" + GetComponent<InventoryManager>().GetCurrentSize() +
            "/" + GetComponent<CharacterDatabase>().activeCharacter.items;
    }

    public Character GetActiveCharacter()
    {
        return GetComponent<CharacterDatabase>().activeCharacter;
    }

    public void Test()
    {
        int weaponID = GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedWeaponID();
        foreach (KeyValuePair<int, Inventory> keyValue in GameMaster.gameMaster.GetComponent<InventoryManager>().playerItems)
        {
            int key = keyValue.Key;
            if (GameMaster.gameMaster.GetComponent<InventoryManager>().playerItems[key].Item.ID == weaponID)
            {
                Debug.Log("Currently equipped weapon is: " + GameMaster.gameMaster.GetComponent<InventoryManager>().playerItems[key].Item.Title);
            }
        }
    }

    public void DecreaseHP(int amount)
    {
        if (amount != 0)
        {
            GetComponent<CharacterDatabase>().activeCharacter.currentHP -= amount;
            if (GetComponent<CharacterDatabase>().activeCharacter.currentHP < 0)
            {
                GetComponent<CharacterDatabase>().activeCharacter.currentHP = 0;
            }
            UpdateActiveCharacterVisuals();
            GameMaster.gameMaster.Save();
        }
    }

    public void IncreaseHP(int amount)
    {
        GetComponent<CharacterDatabase>().activeCharacter.currentHP += amount;
        if (GetComponent<CharacterDatabase>().activeCharacter.currentHP > GetComponent<CharacterDatabase>().activeCharacter.maxHP)
        {
            GetComponent<CharacterDatabase>().activeCharacter.currentHP = GetComponent<CharacterDatabase>().activeCharacter.maxHP;
        }
        UpdateActiveCharacterVisuals();
        GameMaster.gameMaster.Save();
    }

    public void IncreaseMP(int amount)
    {
        GetComponent<CharacterDatabase>().activeCharacter.currentMP += amount;
        if (GetComponent<CharacterDatabase>().activeCharacter.currentMP > GetComponent<CharacterDatabase>().activeCharacter.maxMP)
        {
            GetComponent<CharacterDatabase>().activeCharacter.currentMP = GetComponent<CharacterDatabase>().activeCharacter.maxMP;
        }
        UpdateActiveCharacterVisuals();
        GameMaster.gameMaster.Save();
    }

    public void DecreaseLives()
    {
        GetComponent<CharacterDatabase>().activeCharacter.lives--;
        UpdateActiveCharacterVisuals();
        GameMaster.gameMaster.Save();
    }

    public void GiveExpToActiveCharacter(int amount)
    {
        if (GetActiveCharacterCurrentLevel() == levelCap)
        {
            //Don't give exp since character is capped.

        }
        else
        {
            int currentLevel = GetActiveCharacterCurrentLevel();
            GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp += amount;

            int newLevel = GetActiveCharacterCurrentLevel();

            if (currentLevel != newLevel)
            {
                LevelUpLogic(GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter);
            }

        }
        UpdateActiveCharacterVisuals();
        GameMaster.gameMaster.Save();
    }

    public int GetActiveCharacterCurrentLevel()
    {
        int charactersLevel = 0;
        for (int i = 0; i < expLevels.Length; i++)
        {
            if (GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp >= expLevels[i] && GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp <= expCap)
            {
                charactersLevel = i + 1;
            }
            else if (GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp >= expCap)
            {
                charactersLevel = levelCap;
            }
        }
        return charactersLevel;
    }

    public void LevelUpLogic(Character character)
    {
        string charactersJob = GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.job;
        for (int i = 0; i < GameMaster.gameMaster.GetComponent<ProfessionDatabase>().GetCountOfProfessionsList(); i++)
        {
            if (GameMaster.gameMaster.GetComponent<ProfessionDatabase>().FetchProfessionByID(i).title == charactersJob)
            {
                //HP
                int hpRandom = Random.Range(0, 4);
                character.maxHP += Mathf.CeilToInt(2 * (float)GameMaster.gameMaster.GetComponent<ProfessionDatabase>().FetchProfessionByID(i).hpMod + hpRandom);
                character.currentHP += Mathf.CeilToInt(2 * (float)GameMaster.gameMaster.GetComponent<ProfessionDatabase>().FetchProfessionByID(i).hpMod + hpRandom);

                //MP
                int mpRandom = Random.Range(0, 4);
                character.maxMP += Mathf.CeilToInt(2 * (float)GameMaster.gameMaster.GetComponent<ProfessionDatabase>().FetchProfessionByID(i).mpMod + mpRandom);
                character.currentMP += Mathf.CeilToInt(2 * (float)GameMaster.gameMaster.GetComponent<ProfessionDatabase>().FetchProfessionByID(i).mpMod + mpRandom);

                //Attack
                int attackRandom = Random.Range(0, 4);
                character.attack += Mathf.CeilToInt(2 * (float)GameMaster.gameMaster.GetComponent<ProfessionDatabase>().FetchProfessionByID(i).attackMod + attackRandom);

                //Special
                int specialRandom = Random.Range(0, 4);
                character.special += Mathf.CeilToInt(2 * (float)GameMaster.gameMaster.GetComponent<ProfessionDatabase>().FetchProfessionByID(i).specialMod + specialRandom);

                //Defense
                int defenseRandom = Random.Range(0, 4);
                character.defense += Mathf.CeilToInt(2 * (float)GameMaster.gameMaster.GetComponent<ProfessionDatabase>().FetchProfessionByID(i).defenseMod + defenseRandom);

                //Speed
                int speedRandom = Random.Range(0, 4);
                character.speed += Mathf.CeilToInt(2 * (float)GameMaster.gameMaster.GetComponent<ProfessionDatabase>().FetchProfessionByID(i).speedMod + speedRandom);

                //Luck
                int luckRandom = Random.Range(0, 4);
                character.luck += Mathf.CeilToInt(2 * (float)GameMaster.gameMaster.GetComponent<ProfessionDatabase>().FetchProfessionByID(i).luckMod + luckRandom);

                //Size
                int sizeRandom = Random.Range(0, 4);
                character.items += Mathf.CeilToInt(2 * (float)GameMaster.gameMaster.GetComponent<ProfessionDatabase>().FetchProfessionByID(i).itemsMod + sizeRandom);
            }
        }
    }
}
