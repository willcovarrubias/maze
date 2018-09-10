using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VillageSceneController : MonoBehaviour
{
    public static VillageSceneController villageScene;
    public GameObject mainMenu, labyrinthConfirmation, barracksMenu;
    public GameObject inventoryUI, villInventoryUI, recruitmentUI, upgradeMenu;
    GameObject gameMaster;
    public Canvas canvasForAllMenusInVillageScene;
    public Location.VillageMenu currentMenu;

    private void Start()
    {
        GameMaster.gameMaster.currentArea = Location.Area.village;
        villageScene = this;
        gameMaster = GameObject.FindGameObjectWithTag("GameController");
        inventoryUI = gameMaster.transform.Find("Canvas/InventoryPanel").gameObject;
        mainMenu.SetActive(false);
        labyrinthConfirmation.SetActive(false);
        barracksMenu.SetActive(false);
        currentMenu = Location.VillageMenu.mainMenu;
        GameMaster.gameMaster.PlayerPrefsSave();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            CheckWhichObjectPressed();
        }
    }

    void CheckWhichObjectPressed()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 20))
        {
            switch (hit.collider.name)
            {
                case "Maze":
                    EnterLabyrinth();
                    break;
                case "Barracks":
                    BarracksMenu();
                    break;
                case "Caravan":
                    RecruitmentUIOpen();
                    break;
                case "Inventory":
                    InventoryUIOpen();
                    break;
                case "Armor":
                    BlacksmithMenu();
                    break;
                case "Weapon":
                    WeaponsmithMenu();
                    break;
                case "Item":
                    ItemShopMenu();
                    break;
            }
        }
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
    public void EnterLabyrinth()
    {
        if (gameMaster.GetComponent<InventoryManager>().GetFreeSpaceCount() >= 0 &&
            gameMaster.GetComponent<ActiveCharacterController>().GetActiveCharacter().currentHP > 0 &&
            gameMaster.GetComponent<ActiveCharacterController>().GetActiveCharacter().id != -1
           )
        {
            gameMaster.GetComponent<InventoryManager>().CloseInventoryPanelUI();
            labyrinthConfirmation.SetActive(true);
        }
        else if (gameMaster.GetComponent<ActiveCharacterController>().GetActiveCharacter().id == -1)
        {
            gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Please set an active character from the Barracks");
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
        GameMaster.gameMaster.GetComponent<InventoryManager>().SaveInventory("Temp");
        PlayerPrefs.SetInt("Exit Maze", 1);
        GameMaster.gameMaster.roomCount = -1; //Resets the room counter each time the hero starts a new adventure.
        SceneManager.LoadScene("PathScene");
    }

    public void EnterLabyrinthCancel()
    {
        labyrinthConfirmation.SetActive(false);
    }

    public void BlacksmithMenu()//This'll pop up a menu that'll allow the player to upgrade the smith but also purchase weapons.
    {
        if (gameMaster.GetComponent<ActiveCharacterController>().GetActiveCharacter().id != -1)
        {
            currentMenu = Location.VillageMenu.armor;
            GetComponent<CraftingDatabase>().armorMenu.GetComponent<CraftingMenu>().OpenUI();
        }
        else
        {
            gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Please set an active character from the Barracks");
        }
    }

    public void BlacksmithMenuClose()
    {
        currentMenu = Location.VillageMenu.mainMenu;
    }

    public void WeaponsmithMenu()
    {
        if (gameMaster.GetComponent<ActiveCharacterController>().GetActiveCharacter().id != -1)
        {
            currentMenu = Location.VillageMenu.weapons;
            GetComponent<CraftingDatabase>().weaponsMenu.GetComponent<CraftingMenu>().OpenUI();
        }
        else
        {
            gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Please set an active character from the Barracks");
        }
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
    }

    public void BarracksMenuClose()
    {
        //canvasForAllMenusInVillageScene.GetComponent<Canvas>().sortingOrder = -1;
        barracksMenu.SetActive(false);
    }

    public void ItemShopMenu()//This'll pop up a menu that'll allow the player to upgrade the item shop but also purchase items.
    {
        if (gameMaster.GetComponent<ActiveCharacterController>().GetActiveCharacter().id != -1)
        {
            currentMenu = Location.VillageMenu.pub;
            GetComponent<CraftingDatabase>().consumablesMenu.GetComponent<CraftingMenu>().OpenUI();
        }
        else
        {
            gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Please set an active character from the Barracks");
        }
    }

    public void ItemShopMenuClose()
    {
        currentMenu = Location.VillageMenu.mainMenu;
    }

    public void RecruitmentUIOpen()
    {
        if (!gameObject.GetComponent<WanderersRefreshTime>())
        {
            gameObject.AddComponent<WanderersRefreshTime>();
        }
        recruitmentUI.SetActive(true);
    }

    public void RecruitmentUIClose()
    {
        if (gameObject.GetComponent<WanderersRefreshTime>())
        {
            Destroy(gameObject.GetComponent<WanderersRefreshTime>());
        }
        recruitmentUI.SetActive(false);
    }

    public void InventoryUIOpen()
    {
        if (gameMaster.GetComponent<ActiveCharacterController>().GetActiveCharacter().id != -1)
        {
            currentMenu = Location.VillageMenu.inventory;
            GameMaster.gameMaster.GetComponent<InventoryManager>().OpenInventoryPanelUI();
            villInventoryUI.SetActive(true);
        }
        else if (gameMaster.GetComponent<ActiveCharacterController>().GetActiveCharacter().id == -1)
        {
            gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Please set an active character from the Barracks");
        }
    }

    public void InventoryUIClose()
    {
        currentMenu = Location.VillageMenu.mainMenu;
        inventoryUI.SetActive(false);
        villInventoryUI.SetActive(false);
    }

    public void UpgradeUIOpen()
    {
        currentMenu = Location.VillageMenu.upgrade;
        upgradeMenu.SetActive(true);
    }

    public void UpgradeUIClose()
    {
        currentMenu = Location.VillageMenu.mainMenu;
        upgradeMenu.SetActive(false);
    }
}
