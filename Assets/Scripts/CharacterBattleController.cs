using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBattleController : MonoBehaviour {

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
        Application.LoadLevel("VillageScene");
    }

    public void GoToPathRoom()
    {
        Application.LoadLevel("PathScene");
    }
}
