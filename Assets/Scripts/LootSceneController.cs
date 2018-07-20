using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSceneController : MonoBehaviour {

    public GameObject lootRoom1ChestLayout, lootRoom2ChestLayout;
    public GameObject chest;

    private void Start()
    {
        Debug.Log("You're in the loot scene!!!");
        RandomizeLootRoomLayout();
    }

    private void RandomizeLootRoomLayout()
    {
        int lootRoomType = Random.Range(0, 3);

        if (lootRoomType == 2)
        {
            lootRoom1ChestLayout.SetActive(true);
        }
        else
        {
            lootRoom2ChestLayout.SetActive(true);
        }
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
