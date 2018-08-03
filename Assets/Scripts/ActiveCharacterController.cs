using UnityEngine;
using UnityEngine.UI;

public class ActiveCharacterController : MonoBehaviour
{

    public GameObject activeCharacterPanel;
    public GameObject expBarFront;
    public Text nameTextObject;
    Character activeCharacter;
    int activeCharacterLevel;

    int[] expLevels = new int[5] {0, 200, 400, 800, 1600 };

    void Start () {

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
    }

    void UpdateEXPBar()
    {
        float calc_Level = (GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp - (float)expLevels[activeCharacterLevel - 1]) / (float)(expLevels[activeCharacterLevel] - expLevels[activeCharacterLevel - 1]);

        //float calc_level = (expLevels[activeCharacterLevel] - GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp)

        SetXPBar(calc_Level);
    }

    public void UpdateActiveCharacterVisuals()
    {
        nameTextObject.text = GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.name +
                                "\n" + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.job +
                                "\nXP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp +
                                "\nLevel: " + activeCharacterLevel + 
                                "\nHP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.hp +
                                "\nMP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.mp;
        GetComponent<InventoryManager>().ChangeMaxInventorySize(GetComponent<CharacterDatabase>().activeCharacter.items);

        UpdateEXPBar();
    }

    public Character GetActiveCharacter()
    {
        return GetComponent<CharacterDatabase>().activeCharacter;
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
