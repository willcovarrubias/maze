using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ItemPopUp : MonoBehaviour
{
    Inventory item;
    Weapons weapon;
    GameObject gameMaster;
    GameObject itemHolder;
    int currentSlot;
    GameObject imageOfItem, nameOfItem, statsOfItem, valueOfItem;
    GameObject popUp, action, discard1, discardAll, move1, moveAll, exit;
    Location.WhereAmI currentLocation;

    void Start()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameController");
        popUp = transform.Find("Canvas/ItemPopUp").gameObject;
        imageOfItem = transform.Find("Canvas/ItemPopUp/Background/SpriteHolder/Image").gameObject;
        nameOfItem = transform.Find("Canvas/ItemPopUp/Background/Name").gameObject;
        statsOfItem = transform.Find("Canvas/ItemPopUp/Background/StatsHolder/Stats").gameObject;
        valueOfItem = transform.Find("Canvas/ItemPopUp/Background/StatsHolder/Value").gameObject;
        action = popUp.transform.Find("Background/Action").gameObject;
        discard1 = popUp.transform.Find("Background/ThrowAway1").gameObject;
        discardAll = popUp.transform.Find("Background/ThrowAwayAll").gameObject;
        move1 = popUp.transform.Find("Background/Move1").gameObject;
        moveAll = popUp.transform.Find("Background/MoveAll").gameObject;
        exit = popUp.transform.Find("Background/Exit").gameObject;
        Button actionButton = action.GetComponent<Button>();
        actionButton.onClick.AddListener(Action);
        Button discardButton = discard1.GetComponent<Button>();
        discardButton.onClick.AddListener(ThrowAwayOne);
        Button discardAllButton = discardAll.GetComponent<Button>();
        discardAllButton.onClick.AddListener(ThrowAwayAll);
        Button moveButton = move1.GetComponent<Button>();
        moveButton.onClick.AddListener(MoveOne);
        Button moveAllButton = moveAll.GetComponent<Button>();
        moveAllButton.onClick.AddListener(MoveAll);
        Button exitButton = exit.GetComponent<Button>();
        exitButton.onClick.AddListener(Close);
    }

    public void ShowItemPopUp(Inventory i, int slot, GameObject holder, Location.WhereAmI location)
    {
        string stats = "";
        string value = "";
        item = i;
        currentSlot = slot;
        itemHolder = holder;
        currentLocation = location;
        GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeSlotColor(imageOfItem.transform.parent.transform.parent.gameObject, item.Item.ID);
        imageOfItem.GetComponent<Image>().sprite = item.Item.Sprite;
        GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeWeaponColor(imageOfItem, item.Item);
        if (item.Item.ID >= 1000 && item.Item.ID < 2000)
        {
            ConsumableText(stats, value);
        }
        else if (item.Item.ID >= 2000 && item.Item.ID < 3000)
        {
            GemText(stats, value);
        }
        else if (item.Item.ID >= 3000 && item.Item.ID < 4000)
        {
            MaterialText(stats, value);
        }
        else if (item.Item.ID >= 4000 && item.Item.ID < 5000)
        {
            ArmorText(stats, value);
        }
        else if (item.Item.ID >= 10000)
        {
            WeaponText(stats, value);
        }
        UpdateButtons(i);
        popUp.SetActive(true);
    }

    void ConsumableText(string stats, string value)
    {
        Consumable consumable = (Consumable)item.Item;
        UpdateCount();
        stats += "Type\nHP\nMP\nWeight";
        value += "Consumable\n" + (consumable.Healing > 0 ? "+" : "") + consumable.Healing +
            "\n" + (consumable.MP > 0 ? "+" : "") + consumable.MP +
            "\n" + item.Item.Size;
        statsOfItem.GetComponent<Text>().text = stats;
        valueOfItem.GetComponent<Text>().text = value;
        action.SetActive(true);
        action.GetComponentInChildren<Text>().text = "Consume";
    }

    void GemText(string stats, string value)
    {
        Gem gem = (Gem)item.Item;
        UpdateCount();
        stats += "Type\nAttack\nSpecial\nSpeed\nDuribility\nWeight";
        value += "Gem\n" + (gem.Attack > 0 ? "+" : "") + gem.Attack +
            "\n" + (gem.Special > 0 ? "+" : "") + gem.Special +
            "\n" + (gem.Speed > 0 ? "+" : "") + gem.Speed +
            "\n" + (gem.Durability > 0 ? "+" : "") + gem.Durability +
            "\n" + item.Item.Size;
        statsOfItem.GetComponent<Text>().text = stats;
        valueOfItem.GetComponent<Text>().text = value;
        action.SetActive(false);
    }

    void MaterialText(string stats, string value)
    {
        UpdateCount();
        stats += "Type\nWeight";
        value += "Material\n" + item.Item.Size;
        statsOfItem.GetComponent<Text>().text = stats;
        valueOfItem.GetComponent<Text>().text = value;
        action.SetActive(false);
    }

    void ArmorText(string stats, string value)
    {
        Armor armor = (Armor)item.Item;
        if (armor.Appendage == "head")
        {
            if (GetComponent<InventoryManager>().GetEquippedHatID() != item.Item.ID)
            {
                action.GetComponentInChildren<Text>().text = "Equip";
            }
            else
            {
                action.GetComponentInChildren<Text>().text = "Unequip";
            }
        }
        else
        {
            if (GetComponent<InventoryManager>().GetEquippedBodyID() != item.Item.ID)
            {
                action.GetComponentInChildren<Text>().text = "Equip";
            }
            else
            {
                action.GetComponentInChildren<Text>().text = "Unequip";
            }
        }
        UpdateCount();
        stats += "Type\nApendage\nDefense\nSpeed\nWeight";
        value += "Armor\n" + armor.Appendage +
            "\n" + (armor.Defense > 0 ? "+" : "") + armor.Defense +
            "\n" + (armor.Speed > 0 ? "+" : "") + armor.Speed +
            "\n" + item.Item.Size;
        statsOfItem.GetComponent<Text>().text = stats;
        valueOfItem.GetComponent<Text>().text = value;
        action.SetActive(true);
    }

    void WeaponText(string stats, string value)
    {
        Weapons weap = (Weapons)item.Item;
        if (GetComponent<InventoryManager>().GetEquippedWeaponID() != item.Item.ID)
        {
            action.GetComponentInChildren<Text>().text = "Equip";
        }
        else
        {
            action.GetComponentInChildren<Text>().text = "Unequip";
        }
        UpdateCount();
        stats += "Type\nAttack\nSpecial\nSpeed\nDuribilty\nWeight";
        value += "Weapon\n" + (weap.Attack > 0 ? "+" : "") + weap.Attack +
            "\n" + (weap.Special > 0 ? "+" : "") + weap.Special +
            "\n" + (weap.Speed > 0 ? "+" : "") + weap.Speed +
            "\n" + weap.Durability +
            "\n" + item.Item.Size;
        statsOfItem.GetComponent<Text>().text = stats;
        valueOfItem.GetComponent<Text>().text = value;
        action.SetActive(true);
    }

    void UpdateButtons(Inventory inventory)
    {
        discard1.GetComponentInChildren<Text>().text = "Discard";
        discardAll.GetComponentInChildren<Text>().text = "Discard All";
        if (inventory.Count > 1)
        {
            discard1.SetActive(true);
            discardAll.SetActive(true);
            move1.SetActive(true);
            moveAll.SetActive(true);
        }
        else
        {
            discardAll.SetActive(false);
            moveAll.SetActive(false);
        }
        if (currentLocation == Location.WhereAmI.player && SceneManager.GetActiveScene().name == "VillageScene")
        {
            if (VillageSceneController.villageScene.currentMenu == Location.VillageMenu.inventory)
            {
                move1.GetComponentInChildren<Text>().text = "Send to village";
                moveAll.GetComponentInChildren<Text>().text = "Send all to village";
            }
            else
            {
                move1.SetActive(false);
                moveAll.SetActive(false);
            }
        }
        else if (currentLocation == Location.WhereAmI.player && SceneManager.GetActiveScene().name == "LootScene")
        {
            move1.GetComponentInChildren<Text>().text = "Send to chest";
            moveAll.GetComponentInChildren<Text>().text = "Send all to chest";
        }
        else if (currentLocation == Location.WhereAmI.village || currentLocation == Location.WhereAmI.temp)
        {
            action.SetActive(false);
            move1.GetComponentInChildren<Text>().text = "Send to inventory";
            moveAll.GetComponentInChildren<Text>().text = "Send all to inventory";
        }
        else
        {
            move1.SetActive(false);
            moveAll.SetActive(false);
        }
    }

    void UpdateCount()
    {
        if (item.Count > 1)
        {
            nameOfItem.GetComponent<Text>().text = item.Item.Title + " x" + item.Count;
            itemHolder.GetComponentInParent<Text>().text = item.Item.Title + " x" + item.Count;
        }
        else
        {
            nameOfItem.GetComponent<Text>().text = item.Item.Title;
            itemHolder.GetComponentInParent<Text>().text = item.Item.Title;
        }
        UpdateButtons(item);
    }

    public void Close()
    {
        popUp.SetActive(false);
    }

    public void Action()
    {
        if (item.Item.ID >= 1000 && item.Item.ID < 2000)
        {
            ConsumableAction();
        }
        else if (item.Item.ID >= 4000 && item.Item.ID < 5000)
        {
            ArmorAction();
        }
        else if (item.Item.ID >= 10000)
        {
            WeaponAction();
        }
    }

    void ConsumableAction()
    {
        string dialog = "";
        Consumable consumable = (Consumable)item.Item;
        gameMaster.GetComponent<ActiveCharacterController>().IncreaseHP(consumable.Healing);
        gameMaster.GetComponent<ActiveCharacterController>().IncreaseHP(consumable.MP);
        if (consumable.Healing > 0)
        {
            dialog += "Recovered " + consumable.Healing + " HP.";
        }
        else if (consumable.Healing < 0)
        {
            dialog += "Lost " + consumable.Healing + " HP. ";
        }
        if (consumable.MP > 0)
        {
            dialog += "Recovered " + consumable.MP + " MP.";
        }
        else if (consumable.MP < 0)
        {
            dialog += "Lost " + consumable.MP + " MP.";
        }
        gameMaster.GetComponent<InventoryManager>().ChangeDialogBox(dialog);
        ThrowAwayOne();
        CheckIfUsedItemDuringFight();
    }

    void ArmorAction()
    {
        Armor piece = (Armor)item.Item;
        if (piece.Appendage == "head")
        {
            if (GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedHatID() != item.Item.ID)
            {
                GameMaster.gameMaster.GetComponent<InventoryManager>().SetEquippedHat(item.Item);
            }
            else
            {
                GameMaster.gameMaster.GetComponent<InventoryManager>().UnequipHat(item.Item);
            }
        }
        else if (piece.Appendage == "chest")
        {
            if (GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedBodyID() != item.Item.ID)
            {
                GameMaster.gameMaster.GetComponent<InventoryManager>().SetEquippedBody(item.Item);
            }
            else
            {
                GameMaster.gameMaster.GetComponent<InventoryManager>().UnequipBody(item.Item);
            }
        }
        CheckIfUsedItemDuringFight();
        Close();
    }

    void WeaponAction()
    {
        if (GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedWeaponID() != item.Item.ID)
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().SetEquippedWeapon(item.Item);
        }
        else
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().UnequipWeapon(item.Item);
        }
        CheckIfUsedItemDuringFight();
        Close();
    }

    public void ThrowAwayOne()
    {
        if (currentLocation == Location.WhereAmI.player)
        {
            gameMaster.GetComponent<InventoryManager>().RemoveItemsFromInventory(item, 1, currentSlot);
            if (item.Count > 0)
            {
                UpdateCount();
            }
            else
            {
                Destroy(itemHolder);
                Close();
            }
        }
        else if (currentLocation == Location.WhereAmI.village)
        {
            VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().RemoveItemsFromVillageInventory(item, 1, currentSlot);
            if (item.Count > 0)
            {
                UpdateCount();
            }
            else
            {
                Destroy(itemHolder);
                Close();
            }
        }
        else if (currentLocation == Location.WhereAmI.temp)
        {
            itemHolder.transform.parent.parent.parent.parent.gameObject.GetComponent<DynamicInventory>().RemoveItemsFromInventory(item, 1, currentSlot);
            if (item.Count > 0)
            {
                UpdateCount();
            }
            else
            {
                Destroy(itemHolder);
                Close();
            }
        }
    }

    public void ThrowAwayAll()
    {
        if (currentLocation == Location.WhereAmI.player)
        {
            gameMaster.GetComponent<InventoryManager>().RemoveWholeStackFromInventory(item, currentSlot);
            Destroy(itemHolder);
            Close();
        }
        else if (currentLocation == Location.WhereAmI.village)
        {
            VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().RemoveWholeStackFromInventory(item, currentSlot);
            Destroy(itemHolder);
            Close();
        }
        else if (currentLocation == Location.WhereAmI.temp)
        {
            itemHolder.transform.parent.parent.parent.parent.gameObject.GetComponent<DynamicInventory>().RemoveItemsFromInventory(item, 1, currentSlot);
            itemHolder.transform.parent.parent.parent.parent.gameObject.GetComponent<DynamicInventory>().ReorganizeSlots(currentSlot);
            Destroy(itemHolder);
            Close();
        }
    }

    public void MoveOne()
    {
        if (currentLocation == Location.WhereAmI.player)
        {
            if (SceneManager.GetActiveScene().name == "VillageScene")
            {
                bool movedAll = VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().MoveItemsToVillageInventory(item, currentSlot, 1);
                VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().SaveVillageInventory();
                CloseOrUpdate(movedAll);
            }
            else if (SceneManager.GetActiveScene().name == "LootScene")
            {
                GetActivePanel().GetComponent<DynamicInventory>().MoveItemsToHere(item, currentSlot, 1);
                if (item.Count > 0)
                {
                    CloseOrUpdate(false);
                }
                else
                {
                    CloseOrUpdate(true);
                }
            }
        }
        else if (currentLocation == Location.WhereAmI.village)
        {
            bool movedAll = gameMaster.GetComponent<InventoryManager>().MoveItemsToPlayerInventory(item, currentSlot, 1, true, null);
            CloseOrUpdate(movedAll);
        }
        else if (currentLocation == Location.WhereAmI.temp)
        {
            bool movedAll = gameMaster.GetComponent<InventoryManager>().MoveItemsToPlayerInventory(item, currentSlot, 1, false, itemHolder.transform.parent.parent.parent.parent.gameObject);
            CloseOrUpdate(movedAll);
        }
    }

    public void MoveAll()
    {
        if (currentLocation == Location.WhereAmI.player)
        {
            if (SceneManager.GetActiveScene().name == "VillageScene")
            {
                bool movedAll = VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().MoveItemsToVillageInventory(item, currentSlot, item.Count);
                VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().SaveVillageInventory();
                CloseOrUpdate(movedAll);
            }
            else if (SceneManager.GetActiveScene().name == "LootScene")
            {
                GetActivePanel().GetComponent<DynamicInventory>().MoveItemsToHere(item, currentSlot, item.Count);
                CloseOrUpdate(true);
            }
        }
        else if (currentLocation == Location.WhereAmI.village)
        {
            bool movedAll = gameMaster.GetComponent<InventoryManager>().MoveItemsToPlayerInventory(item, currentSlot, item.Count, true, null);
            CloseOrUpdate(movedAll);
        }
        else if (currentLocation == Location.WhereAmI.temp)
        {
            bool movedAll = gameMaster.GetComponent<InventoryManager>().MoveItemsToPlayerInventory(item, currentSlot, item.Count, false, itemHolder.transform.parent.parent.parent.parent.gameObject);
            CloseOrUpdate(movedAll);
        }
    }

    void CheckIfUsedItemDuringFight()
    {
        if (SceneManager.GetActiveScene().name == "FightScene")
        {
            GameObject.Find("FightController").GetComponent<FightSceneController>().UseItemFromInventory();
            GameObject.Find("FightController").GetComponent<FightSceneController>().UpdatePlayerStats();
            if (GameObject.Find("FightController").GetComponent<FightSceneController>().IsFighting())
            {
                Close();
                GameMaster.gameMaster.GetComponent<InventoryManager>().CloseInventoryPanelUI();
            }
        }
    }

    void CloseOrUpdate(bool movedAll)
    {
        if (movedAll)
        {
            Close();
        }
        else
        {
            UpdateCount();
        }
    }

    GameObject GetActivePanel()
    {
        GameObject panel = gameMaster.transform.GetChild(0).gameObject;
        for (int i = 0; i < panel.transform.childCount; i++)
        {
            if (panel.transform.GetChild(i).gameObject.activeInHierarchy == true && panel.transform.GetChild(i).gameObject.name == "InventoryPanel(Clone)")
            {
                panel = panel.transform.GetChild(i).gameObject;
                return panel;
            }
        }
        return null;
    }
}
