using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSceneManager : MonoBehaviour {

    private void Start () {

        Debug.Log("You're in the Path Scene!");

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
}
