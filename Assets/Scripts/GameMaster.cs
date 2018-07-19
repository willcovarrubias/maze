using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    public static GameMaster gameMaster;

    public void Awake()
    {
        if (gameMaster == null)
        {
            DontDestroyOnLoad(gameObject);
            gameMaster = this;
        }
        else if (gameMaster != this)
        {
            Destroy(gameObject);
        }
    }

    public void MainMenu()
    {
        Debug.Log("You're in the Main Menu.");
    }


    //This function will ask the player if they're sure they want to enter the maze. If so, the next scene will load.
    public void EnterLabyrinth() //TODO: Need to add a confirmation to this, so players don't accidentally enter the labyrinth  when they didn't mean to.
    {
        Debug.Log("Are you sure you want to enter the Labyrinth?");
    }

    public void BlacksmithMenu()//This'll pop up a menu that'll allow the player to upgrade the smith but also purchase weapons.
    {
        Debug.Log("You have access the Blacksmith's Menu.");

    }

    public void BarracksMenu()//This'll pop up a menu that'll allow the player to upgrade the barracks but also select a character.
    {
        Debug.Log("You have accessed the Barracks Menu.");

    }

    public void ItemShopMenu()//This'll pop up a menu that'll allow the player to upgrade the item shop but also purchase items.
    {
        Debug.Log("You  have accessed the Item Shop Menu.");

    }


}
