using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ItemPopUp : MonoBehaviour
{
    Inventory item;
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
        if (currentLocation == Location.WhereAmI.player && SceneManager.GetActiveScene().name == "VillageScene")
        {
            move1.GetComponentInChildren<Text>().text = "Send to village";
            moveAll.GetComponentInChildren<Text>().text = "Send all to village";
        }
        else if (currentLocation == Location.WhereAmI.village)
        {
            action.SetActive(false);
            move1.GetComponentInChildren<Text>().text = "Send to inventory";
            moveAll.GetComponentInChildren<Text>().text = "Send all to inventory";
            villageInventory = GameObject.FindGameObjectWithTag("VillageSceneManager");
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
            gameMaster.GetComponent<InventoryManager>().RemoveItemsFromInventory(item, 1, currentSlot);
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
                Destroy(itemHolder);
                Close();
            }
        }
        else if (currentLocation == Location.WhereAmI.village)
        {
            villageInventory.GetComponent<VillageInventoryManager>().RemoveItemsFromVillageInventory(item, 1, currentSlot);
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
                Destroy(itemHolder);
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
            villageInventory.GetComponent<VillageInventoryManager>().RemoveWholeStackFromInventory(item);
            villageInventory.GetComponent<VillageInventoryManager>().ReorganizeSlots(currentSlot);
            Destroy(itemHolder);
            Close();
        }
    }

    public void MoveOne()
    {
        if (currentLocation == Location.WhereAmI.player)
        {
            itemHolder.GetComponent<ItemData>().AddThisItemToVillageInventory();
            if (item.Count > 0)
            {
                UpdateCount();
            }
            else
            {
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

    //TODO: Make this more efficent
    public void MoveAll()
    {
        if (currentLocation == Location.WhereAmI.player)
        {
            bool movedAll = villageInventory.GetComponent<VillageInventoryManager>().MoveItemsToVillageInventory(item, currentSlot);
            if (movedAll)
            {
                Close();
            }
        }
        else if (currentLocation == Location.WhereAmI.village)
        {
            bool movedAll = gameMaster.GetComponent<InventoryManager>().MoveItemsToPlayerInventory(item, currentSlot);
            if (movedAll)
            {
                Close();
            }
        }
    }
}
