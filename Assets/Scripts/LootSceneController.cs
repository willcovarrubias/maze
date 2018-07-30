using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LootSceneController : MonoBehaviour
{
    GameObject gameMaster;

    private void Start()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameController");
        Debug.Log("You're in the loot scene!!!");
    }

    public void GoToPathScene()
    {
        SceneManager.LoadScene("PathScene");
    }
}