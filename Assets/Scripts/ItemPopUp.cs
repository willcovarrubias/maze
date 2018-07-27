using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopUp : MonoBehaviour
{
    Inventory item;
    GameObject gameMaster;
    GameObject itemHolder;
    int currentSlot;
    GameObject imageOfItem, nameOfItem, statsOfItem;
    GameObject popUp, action, discard1, discardAll, exit;
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
        exit = popUp.transform.Find("Background/Exit").gameObject;
        Button actionButton = action.GetComponent<Button>();
        actionButton.onClick.AddListener(Action);
        Button discardButton = discard1.GetComponent<Button>();
        discardButton.onClick.AddListener(ThrowAwayOne);
        Button discardAllButton = discardAll.GetComponent<Button>();
        discardAllButton.onClick.AddListener(ThrowAwayAll);
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
        if (item.Item.ID >= 1000 && item.Item.ID < 2000)
        {
            Consumable consumable = (Consumable)item.Item;
            UpdateCount();
            stats += "Consumable\nHP " + (consumable.Healing > 0 ? "+" : "") + consumable.Healing + "\nWgt " + item.Item.Size;
            statsOfItem.GetComponent<Text>().text = stats;
            action.SetActive(true);
            action.GetComponentInChildren<Text>().text = "Consume";
        }
        else if (item.Item.ID >= 2000 && item.Item.ID < 3000)
        {
            Weapons weapon = (Weapons)item.Item;
            UpdateCount();
            stats += "Weapon\nAtk " + weapon.Attack + "\nSpec " + weapon.Special + "\nDu " + weapon.Durability + "\nWgt " + item.Item.Size;
            statsOfItem.GetComponent<Text>().text = stats;
            action.SetActive(true);
            action.GetComponentInChildren<Text>().text = "Equip";
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
            UpdateCount();
            stats += "Armor\nDef " + armor.Defense + "\nApp " + armor.Appendage + "\nWgt " + item.Item.Size;
            statsOfItem.GetComponent<Text>().text = stats;
            action.SetActive(true);
            action.GetComponentInChildren<Text>().text = "Equip";
        }
        if (currentLocation == Location.WhereAmI.player)
        {
            discard1.GetComponentInChildren<Text>().text = "Discard 1";
            discardAll.GetComponentInChildren<Text>().text = "Discard All";
        }
        else if (currentLocation == Location.WhereAmI.village)
        {
            action.SetActive(false);
            discard1.GetComponentInChildren<Text>().text = "Move 1 to inventory";
            discardAll.GetComponentInChildren<Text>().text = "Move all to inventory";
        }
        popUp.SetActive(true);
    }

    void UpdateCount()
    {
        if (item.Count > 1)
        {
            nameOfItem.GetComponent<Text>().text = item.Item.Title + " x" + item.Count;
        }
        else
        {
            nameOfItem.GetComponent<Text>().text = item.Item.Title;
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
            //ThrowAwayOne();
            //change HP or MP
        }
        else if (item.Item.ID >= 2000 && item.Item.ID < 3000)
        {
            //equip weapon
        }
        else if (item.Item.ID >= 4000 && item.Item.ID < 5000)
        {
            //equip armor
        }
    }

    public void ThrowAwayOne()
    {
        if (currentLocation == Location.WhereAmI.player)
        {
            gameMaster.GetComponent<InventoryManager>().RemoveItemFromInventory(item);
            if (item.Count == 1)
            {
                itemHolder.GetComponentInParent<Text>().text = item.Item.Title;
                UpdateCount();
            }
            else if (item.Count > 0)
            {
                itemHolder.GetComponentInParent<Text>().text = item.Item.Title + " x" + item.Count;
                UpdateCount();
            }
            else
            {
                gameMaster.GetComponent<InventoryManager>().ReorganizeSlots(currentSlot);
                Destroy(itemHolder);
                Close();
            }
        }
        else if (currentLocation == Location.WhereAmI.village)
        {
            itemHolder.GetComponent<ItemData>().AddThisItemToPlayerInventory();
            if (item.Count > 0)
            {
                UpdateCount();
            }
            else
            {
                Close();
            }
        }
    }

    public void ThrowAwayAll()
    {
        if (currentLocation == Location.WhereAmI.player)
        {
            gameMaster.GetComponent<InventoryManager>().RemoveWholeStackFromInventory(item);
            gameMaster.GetComponent<InventoryManager>().ReorganizeSlots(currentSlot);
            Destroy(itemHolder);
            Close();
        }
        else if (currentLocation == Location.WhereAmI.village)
        {
            int itemCount = item.Count;
            for (int i = 0; i < itemCount; i++)
            {
                bool completed = itemHolder.GetComponent<ItemData>().AddThisItemToPlayerInventory();
                if (item.Count > 0)
                {
                    UpdateCount();
                }
                else
                {
                    Close();
                } 
                if (!completed)
                {
                    return;
                }
            }
        }
    }
}
