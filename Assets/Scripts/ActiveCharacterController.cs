using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveCharacterController : MonoBehaviour
{

    public GameObject activeCharacterPanel;
    public GameObject expBarFront;

    public Text nameTextObject;

    //UI stuff for MoreInfo Panel
    public Text nameText, levelText, jobText, hpText, mpText, attackText, specialText, defenseText, speedText, luckText, expText, inventorySizeText;
    public Image activeHeroPortrait, equippedWeaponSprite, equippedHatSprite, equippedBodySprite;
    public GameObject activeCharacterMoreInfoPanel;
    public GameObject expBarMoreInfo;

    Character activeCharacter;
    Weapons currentlyEquippedWeapon;
    public int expCap = 1600;
    int levelCap = 5;

    int[] expLevels = new int[5] { 0, 200, 400, 800, 1600 };

    void Start()
    {

        GameMaster.gameMaster.GetComponent<CharacterDatabase>().GetActiveCharacter();
        UpdateActiveCharacterVisuals();
        //nameTextObject.text = ;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Test();

            GiveExpToActiveCharacter(100);
        }

    }

    public int GetExpCap()
    {
        return expCap;
    }

    public void SetXPBar(float exp)
    {
        expBarFront.transform.localScale = new Vector3(exp, expBarFront.transform.localScale.y, expBarFront.transform.localScale.z);
        expBarFront.transform.localScale = new Vector3(Mathf.Clamp(exp, 0f, 1f), expBarFront.transform.localScale.y, expBarFront.transform.localScale.z);

        expBarMoreInfo.transform.localScale = new Vector3(exp, expBarMoreInfo.transform.localScale.y, expBarMoreInfo.transform.localScale.z);
        expBarMoreInfo.transform.localScale = new Vector3(Mathf.Clamp(exp, 0f, 1f), expBarMoreInfo.transform.localScale.y, expBarMoreInfo.transform.localScale.z);
    }

    void UpdateEXPBar()
    {
        if (GetActiveCharacterCurrentLevel() < levelCap)
        {
            float calc_Level = (GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp - (float)expLevels[GetActiveCharacterCurrentLevel() - 1]) / (float)(expLevels[GetActiveCharacterCurrentLevel()] - expLevels[GetActiveCharacterCurrentLevel() - 1]);
            SetXPBar(calc_Level);
        }
        else
        {
            SetXPBar(.95f);
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
        int attack = GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.attack;
        int special = GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.special;
        int defense = GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.defense;
        int speed = GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.speed;
        if (GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedWeaponID() > 0)
        {
            attack += GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedWeapon().Attack;
            special += GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedWeapon().Special;
            speed += GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedWeapon().Speed;


            equippedWeaponSprite.enabled = true;
            equippedWeaponSprite.sprite = Resources.Load<Sprite>("Art/EquipmentSprites/" + GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedWeapon().Title);
        }
        else
        {
            equippedWeaponSprite.enabled = false;
        }

        if (GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedHatID() > 0)
        {
            defense += GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedHat().Defense;
            speed += GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedHat().Speed;

            equippedHatSprite.enabled = true;
            equippedHatSprite.sprite = Resources.Load<Sprite>("Art/EquipmentSprites/" + GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedHat().Title);
        }
        else
        {
            equippedHatSprite.enabled = false;
        }

        if (GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedBodyID() > 0)
        {
            defense += GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedBody().Defense;
            speed += GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedBody().Speed;

            equippedBodySprite.enabled = true;
            equippedBodySprite.sprite = Resources.Load<Sprite>("Art/EquipmentSprites/" + GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedBody().Title);
        }
        else
        {
            equippedBodySprite.enabled = false;
        }

        nameTextObject.text = GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.name +
            "\n" + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.job +
            "\nLv. " + GetActiveCharacterCurrentLevel() +
            "\nHP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.currentHP + "/" + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.maxHP +
            "\nMP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.currentMP + "/" + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.maxMP;
        GetComponent<InventoryManager>().ChangeMaxInventorySize(GetComponent<CharacterDatabase>().activeCharacter.items);

        nameText.text = GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.name;
        levelText.text = "Lv. " + GetActiveCharacterCurrentLevel().ToString();
        jobText.text = GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.job;
        hpText.text = "HP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.currentHP + "/" + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.maxHP;
        mpText.text = "MP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.currentMP + "/" + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.maxMP;
        inventorySizeText.text = "Carry Amount: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.items;
        attackText.text = "Attack: " + attack;
        defenseText.text = "Defense: " + defense;
        specialText.text = "Special: " + special;
        speedText.text = "Speed: " + speed;
        luckText.text = "Luck: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.luck.ToString();

        if (GetActiveCharacterCurrentLevel() < levelCap)
        {
            expText.text = "XP: " + (GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp - (float)expLevels[GetActiveCharacterCurrentLevel() - 1]) + "/" + (float)(expLevels[GetActiveCharacterCurrentLevel()] - expLevels[GetActiveCharacterCurrentLevel() - 1]);

        }
        else
        {
            expText.text = "XP: MAX"; 

        }
        activeHeroPortrait.sprite = Resources.Load<Sprite>("Art/CharacterSprites/" + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.slug);
        UpdateEXPBar();
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

    void DestroyActiveWeapon()
    {
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

    //TODO: This gives me an out of bounds error if level 4 and going to level 5
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
                character.maxHP += Mathf.CeilToInt( 2 * (float)GameMaster.gameMaster.GetComponent<ProfessionDatabase>().FetchProfessionByID(i).hpMod + hpRandom);
                character.currentHP += Mathf.CeilToInt(2 * (float)GameMaster.gameMaster.GetComponent<ProfessionDatabase>().FetchProfessionByID(i).hpMod +  hpRandom);

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
