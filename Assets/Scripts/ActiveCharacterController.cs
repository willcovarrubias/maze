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
                                "\nHP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.hp +
                                "\nMP: " + GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter.mp;

    }
	
}
