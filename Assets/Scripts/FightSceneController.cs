using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightSceneController : MonoBehaviour {

    private void Start()
    {
        Debug.Log("You're in the Fight scene!");
    }

    public void Attack()
    {
    }

    public void Guard()
    {
    }

    public void Inventory()
    {
    }

    public void Special()
    {
    }

    public void GoToVillage()
    {
        SceneManager.LoadScene("VillageScene");
    }

    public void GoToPathRoom()
    {
        GameMaster.gameMaster.GetComponent<ActiveCharacterController>().GiveExpForBattleToActiveCharacter();
        SceneManager.LoadScene("PathScene");
    }
}
