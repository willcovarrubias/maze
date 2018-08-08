using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VillageSceneController : MonoBehaviour
{
    public static VillageSceneController villageScene;
    public GameObject mainMenu, labyrinthConfirmation, barracksMenu, blacksmithMenu, itemShopMenu, inventoryUI, villInventoryUI, recruitmentUI;
    GameObject gameMaster;
    public Canvas canvasForAllMenusInVillageScene;
    public Location.VillageMenu currentMenu;

    private void Start()
    {
        villageScene = this;
        gameMaster = GameObject.FindGameObjectWithTag("GameController");
        inventoryUI = gameMaster.transform.Find("Canvas/InventoryPanel").gameObject;
        mainMenu.SetActive(false);
        labyrinthConfirmation.SetActive(false);
        barracksMenu.SetActive(false);
        blacksmithMenu.SetActive(false);
        itemShopMenu.SetActive(false);
        currentMenu = Location.VillageMenu.mainMenu;
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
        if (gameMaster.GetComponent<InventoryManager>().GetFreeSpaceCount() >= 0 && gameMaster.GetComponent<ActiveCharacterController>().GetActiveCharacter().currentHP > 0)
        {
            gameMaster.GetComponent<InventoryManager>().CloseInventoryPanelUI();
            labyrinthConfirmation.SetActive(true);
        }
        else if (gameMaster.GetComponent<InventoryManager>().GetFreeSpaceCount() < 0)
        {
            gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Inventory Full! Discard some items before entering the maze.");
        }
        else if (gameMaster.GetComponent<ActiveCharacterController>().GetActiveCharacter().currentHP <= 0)
        {
            gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Knocked out!");
        }
    }
    public void EnterLabyrinthConfirmation()
    {
        GetComponent<CraftingDatabase>().weaponsMenu.GetComponent<CraftingMenu>().DestroyMenu();
        GetComponent<CraftingDatabase>().consumablesMenu.GetComponent<CraftingMenu>().DestroyMenu();
        GetComponent<CraftingDatabase>().armorMenu.GetComponent<CraftingMenu>().DestroyMenu();
        GetComponent<CraftingPopUp>().DestroyUI();
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
        currentMenu = Location.VillageMenu.armor;
        GetComponent<CraftingDatabase>().armorMenu.GetComponent<CraftingMenu>().OpenUI();
        //gameMaster.GetComponent<InventoryManager>().CloseInventoryPanelUI();
        //blacksmithMenu.SetActive(true);
    }

    public void BlacksmithMenuClose()
    {
        currentMenu = Location.VillageMenu.mainMenu;
        //blacksmithMenu.SetActive(false);
    }

    public void WeaponsmithMenu()
    {
        currentMenu = Location.VillageMenu.weapons;
        GetComponent<CraftingDatabase>().weaponsMenu.GetComponent<CraftingMenu>().OpenUI();
    }

    public void WeaponsmithMenuClose()
    {
        currentMenu = Location.VillageMenu.mainMenu;
    }

    public void BarracksMenu()//This'll pop up a menu that'll allow the player to upgrade the barracks but also select a character.
    {
        gameMaster.GetComponent<InventoryManager>().CloseInventoryPanelUI();
        //canvasForAllMenusInVillageScene.GetComponent<Canvas>().sortingOrder = 3;
        barracksMenu.SetActive(true);
        Debug.Log("You have accessed the Barracks Menu.");
    }

    public void BarracksMenuClose()
    {
        //canvasForAllMenusInVillageScene.GetComponent<Canvas>().sortingOrder = -1;
        barracksMenu.SetActive(false);
    }

    public void ItemShopMenu()//This'll pop up a menu that'll allow the player to upgrade the item shop but also purchase items.
    {
        currentMenu = Location.VillageMenu.pub;
        GetComponent<CraftingDatabase>().consumablesMenu.GetComponent<CraftingMenu>().OpenUI();
        //itemShopMenu.SetActive(true);
    }

    public void ItemShopMenuClose()
    {
        currentMenu = Location.VillageMenu.mainMenu;
        //itemShopMenu.SetActive(false);
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
        currentMenu = Location.VillageMenu.inventory;
        GameMaster.gameMaster.GetComponent<InventoryManager>().OpenInventoryPanelUI();
        villInventoryUI.SetActive(true);
    }

    public void InventoryUIClose()
    {
        currentMenu = Location.VillageMenu.mainMenu;
        inventoryUI.SetActive(false);
        villInventoryUI.SetActive(false);
    }
}
