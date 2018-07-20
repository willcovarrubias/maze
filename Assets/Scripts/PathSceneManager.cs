﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSceneManager : MonoBehaviour {

    public GameObject pathRoom2Doors, pathRoom3Doors, pathRoomEscape;
    public GameObject mainSubMenu;

    private void Start () {

        GameMaster.gameMaster.roomCount++;

        Debug.Log("You're in the Path Scene!");
        Debug.Log("Rooms cleared so far: " + GameMaster.gameMaster.roomCount.ToString());

        if (GameMaster.gameMaster.roomCount != 0 && GameMaster.gameMaster.roomCount % 10 == 0)
        {
            LoadEscapePath();
        }
        else
        {
            RandomizePathRoomType();
        }

	}

    private void RandomizePathRoomType()
    {
        int roomType = Random.Range(0, 2);

        if (roomType == 0)
        {
            pathRoom2Doors.SetActive(true);
        }
        else
        {
            pathRoom3Doors.SetActive(true);
        }
    }

    private void LoadEscapePath()
    {
        pathRoomEscape.SetActive(true);
    }

    //This function will randomize the next room that will spawn. For now, I'll use a simple random.range to give you a 1/3 chance at the different rooms.
    //We'll adjust this later to create better balance.
    public void SelectPath()
    {
        int roomType = Random.Range(0, 4);


        if (roomType == 0)
        {
            Application.LoadLevel("FightScene");
        }
        else if (roomType == 1)
        {
            Application.LoadLevel("PuzzleScene");
        }
        else
        {
            Application.LoadLevel("LootScene");
        }
    }

    public void MainSubMenuOpen()
    {
        mainSubMenu.SetActive(true);
    }
    public void MainSubMenuClose()
    {
        mainSubMenu.SetActive(false);
    }


    public void ReturnToVillage()
    {
        Application.LoadLevel("VillageScene");
    }
}
