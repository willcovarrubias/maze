using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VillageSceneController : MonoBehaviour
{

    public GameObject mainMenu, labyrinthConfirmation, barracksMenu, blacksmithMenu, itemShopMenu, inventoryUI, villInventoryUI, recruitmentUI;

    GameObject gameMaster;

    private void Start()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameController");
        inventoryUI = gameMaster.transform.Find("Canvas/InventoryPanel").gameObject;
        mainMenu.SetActive(false);
        labyrinthConfirmation.SetActive(false);
        barracksMenu.SetActive(false);
        blacksmithMenu.SetActive(false);
        itemShopMenu.SetActive(false);
    }

    public void MainMenu()
    {
        gameMaster.GetComponent<InventoryManager>().CloseInventoryPanelUI();

        mainMenu.SetActive(true);
    }

    public void MainMenuClose()
    {
        mainMenu.SetActive(false);
    }

    //This function will ask the player if they're sure they want to enter the maze. If so, the next scene will load.
    public void EnterLabyrinth() //TODO: Need to add a confirmation to this, so players don't accidentally enter the labyrinth  when they didn't mean to.
    {
        if (gameMaster.GetComponent<InventoryManager>().GetFreeSpaceCount() >= 0)
        {
            gameMaster.GetComponent<InventoryManager>().CloseInventoryPanelUI();
            labyrinthConfirmation.SetActive(true);
        }
        else
        {
            gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Inventory Full! Discard some items before entering the maze.");
        }
    }
    public void EnterLabyrinthConfirmation()
    {
        GetComponent<VillageInventoryManager>().DestroyPanel();
        GameMaster.gameMaster.roomCount = -1; //Resets the room counter each time the hero starts a new adventure.
        SceneManager.LoadScene("PathScene");

    }
    public void EnterLabyrinthCancel()
    {
        labyrinthConfirmation.SetActive(false);
    }

    public void BlacksmithMenu()//This'll pop up a menu that'll allow the player to upgrade the smith but also purchase weapons.
    {
        gameMaster.GetComponent<InventoryManager>().CloseInventoryPanelUI();
        blacksmithMenu.SetActive(true);
    }

    public void BlacksmithMenuClose()
    {
        blacksmithMenu.SetActive(false);
    }

    public void BarracksMenu()//This'll pop up a menu that'll allow the player to upgrade the barracks but also select a character.
    {
        gameMaster.GetComponent<InventoryManager>().CloseInventoryPanelUI();

        barracksMenu.SetActive(true);
        Debug.Log("You have accessed the Barracks Menu.");
    }

    public void BarracksMenuClose()
    {
        barracksMenu.SetActive(false);
    }

    public void ItemShopMenu()//This'll pop up a menu that'll allow the player to upgrade the item shop but also purchase items.
    {
        gameMaster.GetComponent<InventoryManager>().CloseInventoryPanelUI();
        itemShopMenu.SetActive(true);
    }

    public void ItemShopMenuClose()
    {
        itemShopMenu.SetActive(false);
    }

    public void RecruitmentUIOpen()
    {
        recruitmentUI.SetActive(true);
    }

    public void RecruitmentUIClose()
    {
        recruitmentUI.SetActive(false);
    }

    public void InventoryUIOpen()
    {
        GameMaster.gameMaster.GetComponent<InventoryManager>().OpenInventoryPanelUI();
        villInventoryUI.SetActive(true);
    }

    public void InventoryUIClose()
    {
        inventoryUI.SetActive(false);
        villInventoryUI.SetActive(false);
    }
}
