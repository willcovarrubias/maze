using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        //Testing - set to only spawn chest rooms for now
        int roomType = Random.Range(1, 3);


        if (roomType == 0)
        {
            SceneManager.LoadScene("PuzzleScene");
        }
        else if (roomType == 1)
        {
            SceneManager.LoadScene("FightScene");
        }
        else
        {
            SceneManager.LoadScene("LootScene");
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
        SceneManager.LoadScene("VillageScene");
    }

    public void InventoryUIOpen()
    {
        GameMaster.gameMaster.GetComponent<InventoryManager>().OpenInventoryPanelUI();
    }
}
