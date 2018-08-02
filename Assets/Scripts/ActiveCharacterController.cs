using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveCharacterController : MonoBehaviour {

    public GameObject activeCharacterPanel;
    public Text nameTextObject;
    Character activeCharacter;


    void Start () {

        GameMaster.gameMaster.GetComponent<CharacterDatabase>().GetActiveCharacter();
        
        UpdateActiveCharacterVisuals();
        //nameTextObject.text = ;
	}

    public void UpdateActiveCharacterVisuals()
    {
        nameTextObject.text = GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.name +
                                "\n" + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.job +
                                "\nXP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp +
                                "\nHP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.hp +
                                "\nMP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.mp;

    }

    public void GiveExpForRoomClearToActiveCharacter()
    {
        GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp += 100;
        UpdateActiveCharacterVisuals();
        GameMaster.gameMaster.Save();
    }

    public void GiveExpForBattleToActiveCharacter()
    {
        GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.exp += 200;
        UpdateActiveCharacterVisuals();
        GameMaster.gameMaster.Save();
    }

}
