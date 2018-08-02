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
        DestroyDynamicPanels();
        SceneManager.LoadScene("PathScene");
    }

    void DestroyDynamicPanels()
    {
        GameObject panel = GameMaster.gameMaster.transform.GetChild(0).gameObject;
        for (int i = 0; i < panel.transform.childCount; i++)
        {
            if (panel.transform.GetChild(i).gameObject.name == "InventoryPanel(Clone)")
            {
                Destroy(panel.transform.GetChild(i).gameObject);
            }
        }
    }
}