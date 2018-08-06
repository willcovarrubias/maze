using UnityEngine;
using UnityEngine.UI;

public class ActiveCharacterController : MonoBehaviour
{

    public GameObject activeCharacterPanel;
    public GameObject expBarFront;

    public Text nameTextObject;

    //UI stuff for MoreInfo Panel
    public Text nameText, levelText, jobText, hpText, mpText, attackText, specialText, defenseText, speedText, luckText, expText;
    public GameObject activeCharacterMoreInfoPanel;
    public GameObject expBarMoreInfo;

    Character activeCharacter;
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

        //Debug.Log("Level:" + activeCharacterLevel);

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
        attackText.text = "Attack: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.attack.ToString();
        specialText.text = "Special: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.special.ToString();
        defenseText.text = "Defense: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.defense.ToString();
        //speedText.text = GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.speed.ToString();
        luckText.text = "Luck: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.luck.ToString();
        expText.text = "XP: " + (GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp - (float)expLevels[activeCharacterLevel - 1]) + "/" + (float)(expLevels[activeCharacterLevel] - expLevels[activeCharacterLevel - 1]);


        UpdateEXPBar();
    }

    public Character GetActiveCharacter()
    {
        return GetComponent<CharacterDatabase>().activeCharacter;
    }

    public void IncreaseHP(int amout)
    {
        GetComponent<CharacterDatabase>().activeCharacter.currentHP += amout;
        if (GetComponent<CharacterDatabase>().activeCharacter.currentHP > GetComponent<CharacterDatabase>().activeCharacter.maxHP)
        {
            GetComponent<CharacterDatabase>().activeCharacter.currentHP = GetComponent<CharacterDatabase>().activeCharacter.maxHP;
        }
        UpdateActiveCharacterVisuals();
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
