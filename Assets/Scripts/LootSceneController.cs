﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSceneController : MonoBehaviour {

    public GameObject chest;

    private void Start()
    {
        Debug.Log("You're in the loot scene!!!");
    }

    public void GoToPathScene()
    {
        Application.LoadLevel("PathScene");
    }

    public void OpenChestUI()
    {
        chest.SetActive(true);
    }

    public void CloseChestUI()
    {
        chest.SetActive(false);
    }

   
}
