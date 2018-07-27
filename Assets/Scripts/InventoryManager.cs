using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    int maxInventorySize;
    int currentSize;
    public List<Inventory> playerItems = new List<Inventory>();

    //UI Stuff.
    public GameObject inventoryPanel;
    public GameObject slotPanel;
    public GameObject slot;
    public GameObject itemPrefab;
    public GameObject inventoryText;
    public int slotAmount;
    public List<GameObject> slots = new List<GameObject>();
    Scene currentScene;
    private string sceneName;

    public GameObject inventoryPane;
    public RectTransform slotPanelRectTransform;
    public ScrollRect scrollView;

    GameObject villageInventory;

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        maxInventorySize = 100; // set this somewhere
        currentSize = 0;
        LoadInventory();
    }

    public bool MoveItemsToPlayerInventory(Inventory items, int thisSlotId, int amount)
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
            if (SceneManager.GetActiveScene().name == "VillageScene" && villageInventory == null)
            {
                villageInventory = GameObject.FindGameObjectWithTag("VillageSceneManager");
            }
            if (IsWeapon(items.Item.ID))
            {
                CreateNewItem(items.Item, 1);
                villageInventory.GetComponent<VillageInventoryManager>().RemoveItemsFromVillageInventory(items, 1, thisSlotId);
                return true;
            }
            else
            {
                for (int i = 0; i < playerItems.Count; i++)
                {
                    if (playerItems[i].Item.ID == items.Item.ID)
                    {
                        playerItems[i].Count += amountCanFit;
                        currentSize += items.Item.Size * amountCanFit;
                        SaveInventory();
                        for (int j = 0; j < slots.Count; j++)
                        {
                            if (slots[j].GetComponentInChildren<ItemData>().GetItem().Item.ID == items.Item.ID)
                            {
                                slots[j].GetComponentInChildren<ItemData>().GetItem().Count = playerItems[i].Count;
                                slots[j].GetComponentInChildren<Text>().text = playerItems[i].Item.Title + " x" + playerItems[i].Count;
                                UpdateInventoryText();
                                villageInventory.GetComponent<VillageInventoryManager>().RemoveItemsFromVillageInventory(items, amountCanFit, thisSlotId);
                                return movedAll;
                            }
                        }
                    }
                }
                CreateNewItem(items.Item, amountCanFit);
                villageInventory.GetComponent<VillageInventoryManager>().RemoveItemsFromVillageInventory(items, amountCanFit, thisSlotId);
            }
        }
        return movedAll;
    }

    public void AddItemToInventory(Items item)
    {
        if (CanFitInInventory(item.Size))
        {
            if (IsWeapon(item.ID))
            {
                CreateNewItem(item, 1);
            }
            else
            {
                for (int i = 0; i < playerItems.Count; i++)
                {
                    if (playerItems[i].Item.ID == item.ID)
                    {
                        playerItems[i].Count++;
                        currentSize += item.Size;
                        SaveInventory();
                        for (int j = 0; j < slots.Count; j++)
                        {
                            if (slots[j].GetComponentInChildren<ItemData>().GetItem().Item.ID == item.ID)
                            {
                                slots[j].GetComponentInChildren<ItemData>().GetItem().Count = playerItems[i].Count;
                                slots[j].GetComponentInChildren<Text>().text = playerItems[i].Item.Title + " x" + playerItems[i].Count;
                                UpdateInventoryText();
                                break;
                            }
                        }
                        return;
                    }
                }
                CreateNewItem(item, 1);
            }
        }
    }

    public void RemoveItemsFromInventory(Inventory item, int count, int slotId)
    {
        if (item.Count >= count)
        {
            currentSize -= item.Item.Size * count;
            if (IsWeapon(item.Item.ID))
            {
                playerItems.Remove(item);
                item.Count = 0;
                ReorganizeSlots(slotId);
            }
            else
            {
                for (int i = 0; i < playerItems.Count; i++)
                {
                    if (playerItems[i].Item.ID == item.Item.ID)
                    {
                        playerItems[i].Count -= count;
                        if (playerItems[i].Count <= 0)
                        {
                            playerItems.Remove(item);
                            item.Count = 0;
                            ReorganizeSlots(slotId);
                        }
                        break;
                    }
                }
            }
            SaveInventory();
            UpdateInventoryText();
        }
    }

    public void RemoveWholeStackFromInventory(Inventory items)
    {
        currentSize -= items.Item.Size * items.Count;
        playerItems.Remove(items);
        items.Count = 0;
        SaveInventory();
        UpdateInventoryText();
    }

    public void PrintInventory()
    {
        for (int i = 0; i < playerItems.Count; i++)
        {
            Debug.Log("THis is the current inventory!: " + playerItems[i].Item.Title + " Count: " + playerItems[i].Count);
        }
    }

    public void UpdateInventoryText()
    {
        inventoryText.GetComponent<Text>().text = "Inventory: " + currentSize + " / " + maxInventorySize;
    }

    public bool CanFitInInventory(int itemSize)
    {
        if (currentSize + itemSize <= maxInventorySize)
        {
            return true;
        }
        return false;
    }

    public void SaveInventory()
    {
        int i;
        for (i = 0; i < playerItems.Count; i++)
        {
            PlayerPrefs.SetInt("Player Item ID" + i, playerItems[i].Item.ID);
            PlayerPrefs.SetInt("Player Item Count" + i, playerItems[i].Count);
            if (IsWeapon(playerItems[i].Item.ID))
            {
                Weapons weapon = (Weapons)playerItems[i].Item;
                PlayerPrefs.SetInt("Player Item Duribility", weapon.Durability);
            }
        }
        PlayerPrefs.SetInt("Player Item Count", i);
        PlayerPrefs.Save();
    }

    public void LoadInventory()
    {
        int itemCount = PlayerPrefs.GetInt("Player Item Count");
        playerItems.Clear();
        currentSize = 0;
        for (int i = 0; i < itemCount; i++)
        {
            int id = PlayerPrefs.GetInt("Player Item ID" + i);
            int count = PlayerPrefs.GetInt("Player Item Count" + i);
            if (IsWeapon(id))
            {
                int duribility = PlayerPrefs.GetInt("Player Item Duribility");
                Weapons weapon = GetComponent<WeaponDatabase>().FetchWeaponByID(id);
                Inventory loadedItem = new Inventory(weapon, count);
                weapon.Durability = duribility;
                playerItems.Add(loadedItem);
                currentSize += weapon.Size * count;
                AddItemToSlots(loadedItem);
            }
            else
            {
                Items item = GetComponent<ItemDatabase>().FetchItemByID(id);
                Inventory loadedItem = new Inventory(item, count);
                playerItems.Add(loadedItem);
                currentSize += item.Size * count;
                AddItemToSlots(loadedItem);
            }
        }
        UpdateInventoryText();
    }

    public bool IsWeapon(int id)
    {
        if (id >= 2000 && id < 3000)
        {
            return true;
        }
        return false;
    }

    void CreateNewItem(Items items, int count)
    {
        Inventory newItem = new Inventory(items, count);
        playerItems.Add(newItem);
        currentSize += items.Size * count;
        SaveInventory();
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
        itemObject.GetComponentInChildren<ItemData>().SetLocation(Location.WhereAmI.player);
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

    public void OpenInventoryPanelUI()
    {
        inventoryPanel.SetActive(true);
    }

    public void CloseInventoryPanelUI()
    {
        inventoryPanel.SetActive(false);
        //Closes the inventory panel no matter what scene the player is currently in.
        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
        if (sceneName == "LootScene") //Close the chest/player inventory
        {
            GameObject lootSceneController = GameObject.FindGameObjectWithTag("LootSceneManager");
            lootSceneController.GetComponent<LootGenerator>().CloseAllChestUi();
        }
        if (sceneName == "VillageScene")
        {
            GameObject.Find("VillageManager").GetComponent<VillageSceneController>().InventoryUIClose();
        }
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
        return maxInventorySize - currentSize;
    }
}
