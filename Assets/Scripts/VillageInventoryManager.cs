﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VillageInventoryManager : MonoBehaviour
{
    int maxVillageInventorySize;
    int currentSize;
    public Dictionary<int, Inventory> villageItems = new Dictionary<int, Inventory>();

    public GameObject slotPanel;
    public GameObject slot;
    public GameObject itemPrefab;
    public GameObject villageInventoryText;
    public int slotAmount;
    public List<GameObject> slots = new List<GameObject>();

    GameObject gameMaster;
    public GameObject addItemsToVillageInventory;

    public RectTransform slotPanelRectTransform;
    public ScrollRect scrollViewVillage;

    /*
    private void Update()
    {
        if (Input.GetKeyUp("p"))
        {
            PrintInventory();
        }
    }
    */

    private void Start()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameController");
        maxVillageInventorySize = 1000;
        currentSize = 0;
        LoadVillageInventory();
        ResizeSlotPanel();
    }

    public bool MoveItemsToVillageInventory(Inventory items, int thisSlotId, int amount)
    {
        bool movedAll = false;
        int amountCanFit = amount;
        if (amountCanFit >= Mathf.FloorToInt(GetFreeSpaceCount() / items.Item.Size))
        {
            amountCanFit = Mathf.FloorToInt(GetFreeSpaceCount() / items.Item.Size);
        }
        if (amountCanFit >= items.Count)
        {
            amountCanFit = items.Count;
            movedAll = true;
        }
        if (amountCanFit > 0)
        {
            if (IsWeapon(items.Item.ID))
            {
                CreateNewItem(items.Item, 1);
                gameMaster.GetComponent<InventoryManager>().RemoveItemsFromInventory(items, 1, thisSlotId);
                return true;
            }
            Inventory temp;
            if (villageItems.TryGetValue(items.Item.ID, out temp))
            {
                villageItems[items.Item.ID].Count += amountCanFit;
                currentSize += items.Item.Size * amountCanFit;
                slots[villageItems[items.Item.ID].SlotNum].GetComponentInChildren<ItemData>().GetItem().Count = villageItems[items.Item.ID].Count;
                slots[villageItems[items.Item.ID].SlotNum].GetComponentInChildren<Text>().text = villageItems[items.Item.ID].Item.Title + " x" + villageItems[items.Item.ID].Count;
                UpdateInventoryText();
                gameMaster.GetComponent<InventoryManager>().RemoveItemsFromInventory(items, amountCanFit, thisSlotId);
                return movedAll;
            }
            CreateNewItem(items.Item, amountCanFit);
            gameMaster.GetComponent<InventoryManager>().RemoveItemsFromInventory(items, amountCanFit, thisSlotId);
        }
        return movedAll;
    }

    public void RemoveItemsFromVillageInventory(Inventory item, int count, int slotId)
    {
        if (item.Count >= count)
        {
            currentSize -= item.Item.Size * count;
            if (IsWeapon(item.Item.ID))
            {
                villageItems.Remove(item.Item.ID);
                item.Count = 0;
                ReorganizeSlots(slotId);
            }
            else
            {
                villageItems[item.Item.ID].Count -= count;
                if (villageItems[item.Item.ID].Count <= 0)
                {
                    villageItems.Remove(item.Item.ID);
                    item.Count = 0;
                    ReorganizeSlots(slotId);
                }
            }
            SaveVillageInventory();
            UpdateInventoryText();
        }
    }

    public void RemoveWholeStackFromInventory(Inventory items)
    {
        currentSize -= items.Item.Size * items.Count;
        villageItems.Remove(items.Item.ID);
        items.Count = 0;
        SaveVillageInventory();
        UpdateInventoryText();
    }

    public void UpdateInventoryText()
    {
        villageInventoryText.GetComponent<Text>().text = "Inventory: " + currentSize + " / " + maxVillageInventorySize;
    }

    public bool CanFitInInventory(int itemSize)
    {
        if (currentSize + itemSize <= maxVillageInventorySize)
        {
            return true;
        }
        return false;
    }

    public void SaveVillageInventory()
    {
        int i = 0;
        foreach (KeyValuePair<int, Inventory> keyValue in villageItems)
        {
            int key = keyValue.Key;
            PlayerPrefs.SetInt("Village Item ID" + i, villageItems[key].Item.ID);
            PlayerPrefs.SetInt("Village Item Count" + i, villageItems[key].Count);
            PlayerPrefs.SetInt("Village Item Slot" + i, villageItems[key].SlotNum);
            if (IsWeapon(villageItems[key].Item.ID))
            {
                Weapons weapon = (Weapons)villageItems[key].Item;
                PlayerPrefs.SetString("Village Item Name" + i, weapon.Title);
                PlayerPrefs.SetInt("Village Item Rarity" + i, weapon.Rarity);
                PlayerPrefs.SetInt("Village Item Attack" + i, weapon.Attack);
                PlayerPrefs.SetInt("Village Item Special" + i, weapon.Special);
                PlayerPrefs.SetInt("Village Item Duribility" + i, weapon.Durability);
                PlayerPrefs.SetInt("Village Item Size" + i, weapon.Size);
            }
            i++;
        }
        PlayerPrefs.SetInt("Village Item Count", i);
        PlayerPrefs.Save();
    }

    public void LoadVillageInventory()
    {
        Dictionary<int, Inventory> tempList = new Dictionary<int, Inventory>();
        int itemCount = PlayerPrefs.GetInt("Village Item Count");
        villageItems.Clear();
        currentSize = 0;
        for (int i = 0; i < itemCount; i++)
        {
            int id = PlayerPrefs.GetInt("Village Item ID" + i);
            int count = PlayerPrefs.GetInt("Village Item Count" + i);
            int slotNum = PlayerPrefs.GetInt("Village Item Slot" + i);
            Inventory loadedItem;
            if (IsWeapon(id))
            {
                string title = PlayerPrefs.GetString("Village Item Name" + i);
                int rarity = PlayerPrefs.GetInt("Village Item Rarity" + i);
                int attack = PlayerPrefs.GetInt("Village Item Attack" + i);
                int special = PlayerPrefs.GetInt("Village Item Special" + i);
                int duribility = PlayerPrefs.GetInt("Village Item Duribility" + i);
                int size = PlayerPrefs.GetInt("Village Item Size" + i);
                Weapons weapon = new Weapons(id, title, rarity, attack, special, duribility, size, "");
                loadedItem = new Inventory(weapon, count, slotNum);
            }
            else
            {
                Items item = gameMaster.GetComponent<ItemDatabase>().FetchItemByID(id);
                loadedItem = new Inventory(item, count, slotNum);
            }
            tempList.Add(loadedItem.SlotNum, loadedItem);
        }
        for (int i = 0; i < tempList.Count; i++)
        {
            villageItems.Add(tempList[i].Item.ID, tempList[i]);
            currentSize += tempList[i].Item.Size * tempList[i].Count;
            AddItemToSlots(tempList[i]);
        }
        UpdateInventoryText();
    }

    public bool IsWeapon(int id)
    {
        if (id >= 10000)
        {
            return true;
        }
        return false;
    }

    void CreateNewItem(Items items, int count)
    {
        Inventory newItem;
        newItem = new Inventory(items, count, slotAmount);
        villageItems.Add(newItem.Item.ID, newItem);
        currentSize += items.Size * count;
        SaveVillageInventory();
        AddItemToSlots(newItem);
        UpdateInventoryText();
    }

    void AddItemToSlots(Inventory item)
    {
        GameObject itemObject = Instantiate(itemPrefab);
        AddDynamicSlot();
        itemObject.transform.SetParent(slots[slotAmount - 1].transform);
        itemObject.transform.localPosition = Vector2.zero;
        itemObject.name = item.Item.Title;
        itemObject.GetComponentInChildren<ItemData>().slotID = slotAmount - 1;
        itemObject.GetComponentInChildren<ItemData>().SetItem(item);
        itemObject.GetComponentInChildren<ItemData>().SetLocation(Location.WhereAmI.village);
        if (IsWeapon(item.Item.ID) || item.Count == 1)
        {
            itemObject.GetComponent<Text>().text = item.Item.Title;
        }
        else
        {
            itemObject.GetComponent<Text>().text = item.Item.Title + " x" + item.Count;
        }

        ResizeSlotPanel();
    }

    public void AddDynamicSlot()
    {
        slots.Add(Instantiate(slot));
        slotAmount++;
        //Adds an ID to each slot when it generates the slots. Used for drag/drop.
        slots[slotAmount - 1].GetComponent<ItemSlot>().id = slotAmount - 1;
        slots[slotAmount - 1].name = "Slot" + (slotAmount - 1);
        slots[slotAmount - 1].transform.SetParent(slotPanel.transform);
    }

    public void ReorganizeSlots(int slotID)
    {
        GameObject currentSlot = slots[slotID];
        slotAmount--;
        slots.RemoveAt(slotID);
        for (int i = 0; i < slotAmount; i++)
        {
            slots[i].GetComponent<ItemSlot>().id = i;
            slots[i].GetComponentInChildren<ItemData>().slotID = i;
            villageItems[slots[i].GetComponentInChildren<ItemData>().GetItem().Item.ID].SlotNum = i;
        }
        Destroy(currentSlot);
        ResizeSlotPanel();
    }

    void ResizeSlotPanel()
    {
        //Sets the slot panel RectTransform's size dependent on how many slots there are. This allows for the scrolling logic to work.
        slotPanelRectTransform.Translate(0, (slotAmount * -35), 0);
        slotPanelRectTransform.sizeDelta = new Vector2(407.4f, (slotAmount * 70));
    }

    public int GetFreeSpaceCount()
    {
        return maxVillageInventorySize - currentSize;
    }

    public void PrintInventory()
    {
        foreach (KeyValuePair<int, Inventory> keyValue in villageItems)
        {
            int key = keyValue.Key;
            UnityEngine.Debug.Log(villageItems[key].Item.Title + ".....Slot Num: " + villageItems[key].SlotNum);
        }
    }
}

public class Inventory
{
    public Items Item { get; set; }
    public int Count { get; set; }
    public int SlotNum { get; set; }

    public Inventory(Items item, int count, int slot)
    {
        this.Item = item;
        this.Count = count;
        this.SlotNum = slot;
    }
}
