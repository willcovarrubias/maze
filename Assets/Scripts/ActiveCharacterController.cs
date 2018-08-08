using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveCharacterController : MonoBehaviour
{

    public GameObject activeCharacterPanel;
    public GameObject expBarFront;

    public Text nameTextObject;

    //UI stuff for MoreInfo Panel
    public Text nameText, levelText, jobText, hpText, mpText, attackText, specialText, defenseText, speedText, luckText, expText;
    public Image activeHeroPortrait, equippedWeaponSprite, equippedHatSprite, equippedBodySprite;
    public GameObject activeCharacterMoreInfoPanel;
    public GameObject expBarMoreInfo;

    Character activeCharacter;
    Weapons currentlyEquippedWeapon;
    int activeCharacterLevel;

    int[] expLevels = new int[5] { 0, 200, 400, 800, 1600 };

    void Start()
    {

        DetermineActiveCharacterCurrentLevel();
        GameMaster.gameMaster.GetComponent<CharacterDatabase>().GetActiveCharacter();
        UpdateActiveCharacterVisuals();
        //nameTextObject.text = ;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Test();


        }

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
        float calc_Level = (GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp - (float)expLevels[activeCharacterLevel - 1]) / (float)(expLevels[activeCharacterLevel] - expLevels[activeCharacterLevel - 1]);

        //float calc_level = (expLevels[activeCharacterLevel] - GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp)

        SetXPBar(calc_Level);
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
            equippedWeaponSprite.sprite = Resources.Load<Sprite>("Art/EquipmentSprites/" + GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedWeapon().Title);
        }
        if (GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedHatID() > 0)
        {
            defense += GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedHat().Defense;
            speed += GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedHat().Speed;
            equippedHatSprite.sprite = Resources.Load<Sprite>("Art/EquipmentSprites/" + GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedHat().Title);
        }
        if (GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedBodyID() > 0)
        {
            defense += GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedBody().Defense;
            speed += GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedBody().Speed;
            equippedBodySprite.sprite = Resources.Load<Sprite>("Art/EquipmentSprites/" + GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedBody().Title);
        }

        nameTextObject.text = GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.name +
            "\n" + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.job +
            "\nLv. " + activeCharacterLevel +
            "\nHP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.currentHP + "/" + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.maxHP +
            "\nMP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.currentMP + "/" + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.maxMP;
        GetComponent<InventoryManager>().ChangeMaxInventorySize(GetComponent<CharacterDatabase>().activeCharacter.items);

        nameText.text = GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.name;
        levelText.text = "Lv. " + activeCharacterLevel.ToString();
        jobText.text = GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.job;
        hpText.text = "HP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.currentHP + "/" + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.maxHP;
        mpText.text = "MP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.currentMP + "/" + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.maxMP;
        attackText.text = "Attack: " + attack;
        defenseText.text = "Defense: " + defense;
        specialText.text = "Special: " + special;
        speedText.text = "Speed: " + speed;
        luckText.text = "Luck: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.luck.ToString();
        expText.text = "XP: " + (GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp - (float)expLevels[activeCharacterLevel - 1]) + "/" + (float)(expLevels[activeCharacterLevel] - expLevels[activeCharacterLevel - 1]);
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

    public void GiveExpForRoomClearToActiveCharacter()
    {
        GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp += 10;
        UpdateActiveCharacterVisuals();
        GameMaster.gameMaster.Save();
    }

    public void GiveExpForBattleToActiveCharacter()
    {
        GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp += 50;
        DetermineActiveCharacterCurrentLevel();
        UpdateActiveCharacterVisuals();
        GameMaster.gameMaster.Save();
    }

    //TODO: This gives me an out of bounds error if level 4 and going to level 5
    public void DetermineActiveCharacterCurrentLevel()
    {
        for (int i = 0; i < expLevels.Length; i++)
        {
            if (GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp >= expLevels[i] && GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp <= expLevels[i + 1])
            {
                activeCharacterLevel = i + 1;
            }
        }
    }
}
