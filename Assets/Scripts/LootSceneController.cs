using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LootSceneController : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("You're in the loot scene!!!");
    }

    public void GoToPathScene()
    {
        GameObject.Find("Manager").GetComponent<CreateDynamicInventory>().DestroyDynamicPanels();
        SceneManager.LoadScene("PathScene");
    }
}