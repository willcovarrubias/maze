using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleSceneController : MonoBehaviour {

    private void Start()
    {
        Debug.Log("You're in the Puzzle Scene!");
    }

    public void GoToPathScene()
    {
        SceneManager.LoadScene("PathScene");
    }
}
