﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ItemPopUp : MonoBehaviour
{
    Inventory item;
    Weapons weapon;
    GameObject gameMaster, villageInventory;
    GameObject itemHolder;
    int currentSlot;
    GameObject imageOfItem, nameOfItem, statsOfItem;
    GameObject popUp, action, discard1, discardAll, move1, moveAll, exit;
    Location.WhereAmI currentLocation;

    void Start()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameController");
        popUp = transform.Find("Canvas/ItemPopUp").gameObject;
        imageOfItem = transform.Find("Canvas/ItemPopUp/Background/Image").gameObject;
        nameOfItem = transform.Find("Canvas/ItemPopUp/Background/Name").gameObject;
        statsOfItem = transform.Find("Canvas/ItemPopUp/Background/Stats").gameObject;
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
        item = i;
        currentSlot = slot;
        itemHolder = holder;
        currentLocation = location;
        discard1.GetComponentInChildren<Text>().text = "Discard";
        discardAll.GetComponentInChildren<Text>().text = "Discard All";
        if (item.Item.ID >= 1000 && item.Item.ID < 2000)
        {
            Consumable consumable = (Consumable)item.Item;
            UpdateCount();
            stats += "Consumable\nHP " + (consumable.Healing > 0 ? "+" : "") + consumable.Healing + "\nWgt " + item.Item.Size;
            statsOfItem.GetComponent<Text>().text = stats;
            action.SetActive(true);
            action.GetComponentInChildren<Text>().text = "Consume";
        }
        else if (item.Item.ID >= 3000 && item.Item.ID < 4000)
        {
            UpdateCount();
            stats += "Material" + "\nWgt " + item.Item.Size;
            statsOfItem.GetComponent<Text>().text = stats;
            action.SetActive(false);
        }
        else if (item.Item.ID >= 4000 && item.Item.ID < 5000)
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
            stats += "Armor\nDef " + armor.Defense + "\nspd " + armor.Speed + "\nApp " + armor.Appendage + "\nWgt " + item.Item.Size;
            statsOfItem.GetComponent<Text>().text = stats;
            action.SetActive(true);
        }
        else if (item.Item.ID >= 10000)
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
            stats += "Weapon\nAtk " + weap.Attack + "\nSpec " + weap.Special + "\nDu " + weap.Durability + "\nWgt " + item.Item.Size;
            statsOfItem.GetComponent<Text>().text = stats;
            action.SetActive(true);
        }
        if (currentLocation == Location.WhereAmI.player && SceneManager.GetActiveScene().name == "VillageScene")
        {
            move1.SetActive(true);
            moveAll.SetActive(true);
            move1.GetComponentInChildren<Text>().text = "Send to village";
            moveAll.GetComponentInChildren<Text>().text = "Send all to village";
        }
        else if (currentLocation == Location.WhereAmI.player && SceneManager.GetActiveScene().name == "LootScene")
        {
            move1.SetActive(true);
            moveAll.SetActive(true);
            move1.GetComponentInChildren<Text>().text = "Send to chest";
            moveAll.GetComponentInChildren<Text>().text = "Send all to chest";
        }
        else if (currentLocation == Location.WhereAmI.village || currentLocation == Location.WhereAmI.temp)
        {
            action.SetActive(false);
            move1.SetActive(true);
            moveAll.SetActive(true);
            move1.GetComponentInChildren<Text>().text = "Send to inventory";
            moveAll.GetComponentInChildren<Text>().text = "Send all to inventory";
            if (currentLocation == Location.WhereAmI.village)
            {
                villageInventory = GameObject.FindGameObjectWithTag("VillageSceneManager");
            }
        }
        else
        {
            move1.SetActive(false);
            moveAll.SetActive(false);
        }
        if (SceneManager.GetActiveScene().name == "VillageScene" && villageInventory == null)
        {
            villageInventory = GameObject.FindGameObjectWithTag("VillageSceneManager");
        }
        popUp.SetActive(true);
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
    }

    public void Close()
    {
        popUp.SetActive(false);
    }

    public void Action()
    {
        if (item.Item.ID >= 1000 && item.Item.ID < 2000)
        {
            Consumable consumable = (Consumable)item.Item;
            gameMaster.GetComponent<ActiveCharacterController>().IncreaseHP(consumable.Healing);
            ThrowAwayOne();
            if (SceneManager.GetActiveScene().name == "FightScene")
            {
                GameObject.Find("FightController").GetComponent<FightSceneController>().UseItemFromInventory();
                if (GameObject.Find("FightController").GetComponent<FightSceneController>().IsFighting())
                {
                    Close();
                    GameMaster.gameMaster.GetComponent<InventoryManager>().CloseInventoryPanelUI();
                }
            }
        }
        else if (item.Item.ID >= 4000 && item.Item.ID < 5000)
        {
            Armor piece = (Armor)item.Item;
            if (piece.Appendage == "head")
            {
                if (GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedHatID() != item.Item.ID)
                {
                    GameMaster.gameMaster.GetComponent<InventoryManager>().DisplayHatEquippedSpriteOnChange(item.Item);
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
                    GameMaster.gameMaster.GetComponent<InventoryManager>().DisplayBodyEquippedSpriteOnChange(item.Item);
                }
                else
                {
                    GameMaster.gameMaster.GetComponent<InventoryManager>().UnequipBody(item.Item);
                }
            }
            GetComponent<ActiveCharacterController>().UpdateActiveCharacterVisuals();
            Close();

        }
        else if (item.Item.ID >= 10000)
        {
            if (GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedWeaponID() != item.Item.ID)
            {
                GameMaster.gameMaster.GetComponent<InventoryManager>().DisplayWeaponEquippedSpriteOnChange(item.Item);
            }
            else
            {
                GameMaster.gameMaster.GetComponent<InventoryManager>().UnequipWeapon(item.Item);
            }
            GetComponent<ActiveCharacterController>().UpdateActiveCharacterVisuals();
            Close();
        }
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
            villageInventory.GetComponent<VillageInventoryManager>().RemoveItemsFromVillageInventory(item, 1, currentSlot);
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
            villageInventory.GetComponent<VillageInventoryManager>().RemoveWholeStackFromInventory(item, currentSlot);
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
                bool movedAll = villageInventory.GetComponent<VillageInventoryManager>().MoveItemsToVillageInventory(item, currentSlot, 1);
                villageInventory.GetComponent<VillageInventoryManager>().SaveVillageInventory();
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
                bool movedAll = villageInventory.GetComponent<VillageInventoryManager>().MoveItemsToVillageInventory(item, currentSlot, item.Count);
                villageInventory.GetComponent<VillageInventoryManager>().SaveVillageInventory();
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
