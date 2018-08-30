using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingsManager : MonoBehaviour
{
    int barracksLevel, caravanLevel, armorSmithLevel, weaponSmithLevel;
    int itemShopLevel, villageInventoryLevel;
    public GameObject upgradePopUp;
    public GameObject barracksButton, caravanButton, armorSmithButton, weaponSmithButton;
    public GameObject itemShopButton, villageInventoryButton;
    public GameObject upgradePopUpTitle, upgradePopUpText;
    Dictionary<int, int> materials;
    int currentID;

    void Awake()
    {
        barracksLevel = PlayerPrefs.GetInt("barracks", 0);
        caravanLevel = PlayerPrefs.GetInt("caravan", 0);
        armorSmithLevel = PlayerPrefs.GetInt("armorSmith", 0);
        weaponSmithLevel = PlayerPrefs.GetInt("weaponSmith", 0);
        itemShopLevel = PlayerPrefs.GetInt("itemShop", 0);
        villageInventoryLevel = PlayerPrefs.GetInt("villageInvetory", 0);
        SetBuildingsText();
    }

    void SetBuildingsText()
    {
        barracksButton.GetComponentInChildren<Text>().text = "<b>Upgrade</b>\nBarracks\nLevel " + GetBarracksLevel();
        caravanButton.GetComponentInChildren<Text>().text = "<b>Upgrade</b>\nWanderers Caravan\nLevel " + GetCaravanLevel();
        armorSmithButton.GetComponentInChildren<Text>().text = "<b>Upgrade</b>\nArmor Smith\nLevel " + GetArmorSmithLevel();
        weaponSmithButton.GetComponentInChildren<Text>().text = "<b>Upgrade</b>\nWeapon Smith\nLevel " + GetWeaponSmithLevel();
        itemShopButton.GetComponentInChildren<Text>().text = "<b>Upgrade</b>\nItem Shop\nLevel " + GetItemShopLevel();
        villageInventoryButton.GetComponentInChildren<Text>().text = "<b>Upgrade</b>\nVillage Inventory\nLevel " + GetVillageInventoryLevel();
    }

    int GetBarracksLevel()
    {
        return barracksLevel;
    }

    int GetCaravanLevel()
    {
        return caravanLevel;
    }

    int GetArmorSmithLevel()
    {
        return armorSmithLevel;
    }

    int GetWeaponSmithLevel()
    {
        return armorSmithLevel;
    }

    int GetItemShopLevel()
    {
        return itemShopLevel;
    }

    int GetVillageInventoryLevel()
    {
        return villageInventoryLevel;
    }

    void LevelUpBarracks()
    {
        GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Upgraded Barracks");
        PlayerPrefs.SetInt("barracks", barracksLevel + 1);
        barracksLevel = PlayerPrefs.GetInt("barracks");
        PlayerPrefs.Save();
        SetBuildingsText();
    }

    void LevelUpCaravan()
    {
        GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Upgraded Wanderers Caravan");
        PlayerPrefs.SetInt("caravan", caravanLevel + 1);
        caravanLevel = PlayerPrefs.GetInt("caravan");
        PlayerPrefs.Save();
        SetBuildingsText();
    }

    void LevelUpArmorSmith()
    {
        GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Upgraded Armor Smith");
        PlayerPrefs.SetInt("armorSmith", armorSmithLevel + 1);
        armorSmithLevel = PlayerPrefs.GetInt("armorSmith");
        PlayerPrefs.Save();
        SetBuildingsText();
    }

    void LevelUpWeaponSmith()
    {
        GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Upgraded Weapon Smith");
        PlayerPrefs.SetInt("weaponSmith", weaponSmithLevel + 1);
        weaponSmithLevel = PlayerPrefs.GetInt("weaponSmith");
        PlayerPrefs.Save();
        SetBuildingsText();
    }

    void LevelUpItemShop()
    {
        GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Upgraded Item Shop");
        PlayerPrefs.SetInt("itemShop", itemShopLevel + 1);
        itemShopLevel = PlayerPrefs.GetInt("itemShop");
        PlayerPrefs.Save();
        SetBuildingsText();
    }

    void LevelUpVillageInventory()
    {
        GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Upgraded Village Inventory");
        PlayerPrefs.SetInt("villageInvetory", villageInventoryLevel + 1);
        villageInventoryLevel = PlayerPrefs.GetInt("villageInvetory");
        PlayerPrefs.Save();
        SetBuildingsText();
    }

    public void BarracksUpgradePopUp()
    {
        UpgradePopUp(0, "barracks");
    }

    public void CaravanUpgradePopUp()
    {
        UpgradePopUp(1, "caravan");
    }

    public void ArmorSmithUpgradePopUp()
    {
        UpgradePopUp(2, "armorSmith");
    }

    public void WeaponSmithUpgradePopUp()
    {
        UpgradePopUp(3, "weaponSmith");
    }

    public void ItemShopUpgradePopUp()
    {
        UpgradePopUp(4, "itemShop");
    }

    public void VillageInventoryUpgradePopUp()
    {
        UpgradePopUp(5, "villageInvetory");
    }

    void UpgradeBasedOnID()
    {
        switch (currentID)
        {
            case 0:
                LevelUpBarracks();
                break;
            case 1:
                LevelUpCaravan();
                break;
            case 2:
                LevelUpArmorSmith();
                break;
            case 3:
                LevelUpWeaponSmith();
                break;
            case 4:
                LevelUpItemShop();
                break;
            case 5:
                LevelUpVillageInventory();
                break;
        }
    }

    void UpgradePopUp(int id, string buildingName)
    {
        if (GetComponent<BuildingDatabase>().GetBuildingsData()[id].materials.Count > PlayerPrefs.GetInt(buildingName))
        {
            string text = "";
            currentID = id;
            materials = GetComponent<BuildingDatabase>().GetBuildingsData()[id].materials[PlayerPrefs.GetInt(buildingName)];
            upgradePopUpTitle.GetComponent<Text>().text = "Upgrade " + GetComponent<BuildingDatabase>().GetBuildingsData()[id].title + "?";
            text += "<b>" + GetComponent<BuildingDatabase>().GetBuildingsData()[id].levelsDescription[PlayerPrefs.GetInt(buildingName)] + "</b>\n\n";
            text += "<b>Materials Needed</b>";
            foreach (KeyValuePair<int, int> keyValue in materials)
            {
                text += "\n";
                text += GameMaster.gameMaster.GetComponent<ItemDatabase>().FetchItemByID(keyValue.Key).Title;
                text += " x" + keyValue.Value;
                if (VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().villageItems.ContainsKey(keyValue.Key))
                {
                    text += "<i> (Village has " + VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().villageItems[keyValue.Key].Count + ")</i>";
                }
                else
                {
                    text += "<i> (Village has 0)</i>";
                }
            }
            upgradePopUpText.GetComponent<Text>().text = text;
            upgradePopUp.SetActive(true);
        }
    }

    public void CloseUpgradePopUp()
    {
        upgradePopUp.SetActive(false);
    }

    public void UpgradeButton()
    {
        if (CheckIfHaveMaterials())
        {
            foreach (KeyValuePair<int, int> keyValue in materials)
            {
                VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().RemoveItemsFromVillageInventory(
                    VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().villageItems[keyValue.Key],
                    keyValue.Value,
                    VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().villageItems[keyValue.Key].SlotNum);
            }
            UpgradeBasedOnID();
            CloseUpgradePopUp();
        }
    }

    bool CheckIfHaveMaterials()
    {
        foreach (KeyValuePair<int, int> keyValue in materials)
        {
            if (VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().villageItems.ContainsKey(keyValue.Key))
            {
                if (!(VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().villageItems[keyValue.Key].Count >= keyValue.Value))
                {
                    GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Not enough materials");
                    return false;
                }
            }
            else
            {
                GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Not enough materials");
                return false;
            }
        }
        return true;
    }
}
