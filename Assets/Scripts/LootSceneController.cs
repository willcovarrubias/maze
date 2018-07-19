using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSceneController : MonoBehaviour {

    private void Start()
    {
        Debug.Log("You're in the loot scene!!!");
    }

    public void GoToPathScene()
    {
        Application.LoadLevel("PathScene");
    }
}
