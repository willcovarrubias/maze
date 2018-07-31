using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    int maxInventorySize;
    int currentSize;
    public Dictionary<int, Inventory> playerItems = new Dictionary<int, Inventory>();

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

    /*
    private void Update()
    {
        if (Input.GetKeyUp("p"))
        {
            PrintInventory();
        }
    }
    */

    public bool MoveItemsToPlayerInventory(Inventory items, int thisSlotId, int amount, bool fromVillage, GameObject panel)
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
            if (SceneManager.GetActiveScene().name == "VillageScene")
            {
                villageInventory = GameObject.FindGameObjectWithTag("VillageSceneManager");
            }
            if (IsWeapon(items.Item.ID))
            {
                CreateNewItem(items.Item, 1);
                if (fromVillage)
                {
                    villageInventory.GetComponent<VillageInventoryManager>().RemoveItemsFromVillageInventory(items, 1, thisSlotId);
                }
                else
                {
                    panel.GetComponent<DynamicInventory>().RemoveItemsFromInventory(items, 1, thisSlotId);
                }
                return true;
            }
            else
            {
                Inventory temp;
                if (playerItems.TryGetValue(items.Item.ID, out temp))
                {
                    playerItems[items.Item.ID].Count += amountCanFit;
                    currentSize += items.Item.Size * amountCanFit;
                    SaveInventory();
                    slots[playerItems[items.Item.ID].SlotNum].GetComponentInChildren<ItemData>().GetItem().Count = playerItems[items.Item.ID].Count;
                    slots[playerItems[items.Item.ID].SlotNum].GetComponentInChildren<Text>().text = playerItems[items.Item.ID].Item.Title + " x" + playerItems[items.Item.ID].Count;
                    UpdateInventoryText();
                    if (fromVillage)
                    {
                        villageInventory.GetComponent<VillageInventoryManager>().RemoveItemsFromVillageInventory(items, amountCanFit, thisSlotId);
                    }
                    else
                    {
                        panel.GetComponent<DynamicInventory>().RemoveItemsFromInventory(items, amountCanFit, thisSlotId);
                    }
                    return movedAll;
                }
                CreateNewItem(items.Item, amountCanFit);
                if (fromVillage)
                {
                    villageInventory.GetComponent<VillageInventoryManager>().RemoveItemsFromVillageInventory(items, amountCanFit, thisSlotId);
                }
                else
                {
                    panel.GetComponent<DynamicInventory>().RemoveItemsFromInventory(items, amountCanFit, thisSlotId);
                }
            }
        }
        //TODO if reached here the item cannot fit!
        return movedAll;
    }

    public void RemoveItemsFromInventory(Inventory item, int count, int slotId)
    {
        if (item.Count >= count)
        {
            currentSize -= item.Item.Size * count;
            if (IsWeapon(item.Item.ID))
            {
                playerItems.Remove(item.Item.ID);
                item.Count = 0;
                ReorganizeSlots(slotId);
            }
            else
            {
                playerItems[item.Item.ID].Count -= count;
                if (playerItems[item.Item.ID].Count <= 0)
                {
                    playerItems.Remove(item.Item.ID);
                    item.Count = 0;
                    ReorganizeSlots(slotId);
                }
            }
            SaveInventory();
            UpdateInventoryText();
        }
    }

    public void RemoveWholeStackFromInventory(Inventory items)
    {
        currentSize -= items.Item.Size * items.Count;
        playerItems.Remove(items.Item.ID);
        items.Count = 0;
        SaveInventory();
        UpdateInventoryText();
    }

    public void MoveWholeInventoryToVillage()
    {
        if (sceneName == "VillageScene")
        {
            villageInventory = GameObject.FindGameObjectWithTag("VillageSceneManager");
        }
        for (int i = 0; i < slots.Count; i += 0)
        {
            slots[i].GetComponentInChildren<ItemData>().GetItem();
            bool complete = villageInventory.GetComponent<VillageInventoryManager>().MoveItemsToVillageInventory(
                            slots[i].GetComponentInChildren<ItemData>().GetItem(),
                            slots[i].GetComponentInChildren<ItemData>().slotID,
                            slots[i].GetComponentInChildren<ItemData>().GetItem().Count);
            if (!complete)
            {
                i++;
            }
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
        int i = 0;
        foreach (KeyValuePair<int, Inventory> keyValue in playerItems)
        {
            int key = keyValue.Key;
            PlayerPrefs.SetInt("Player Item ID" + i, playerItems[key].Item.ID);
            PlayerPrefs.SetInt("Player Item Count" + i, playerItems[key].Count);
            if (IsWeapon(playerItems[key].Item.ID))
            {
                Weapons weapon = (Weapons)playerItems[key].Item;
                PlayerPrefs.SetInt("Player Item Duribility", weapon.Durability);
            }
            i++;
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
            Inventory loadedItem;
            if (IsWeapon(id))
            {
                int duribility = PlayerPrefs.GetInt("Player Item Duribility");
                Weapons weapon = GetComponent<WeaponDatabase>().FetchWeaponByID(id);
                weapon.Durability = duribility;
                loadedItem = new Inventory(weapon, count, slotAmount);
            }
            else
            {
                Items item = GetComponent<ItemDatabase>().FetchItemByID(id);
                loadedItem = new Inventory(item, count, slotAmount);
            }
            playerItems.Add(loadedItem.Item.ID, loadedItem);
            currentSize += loadedItem.Item.Size * count;
            AddItemToSlots(loadedItem);
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
        Inventory newItem;
        if (IsWeapon(items.ID))
        {
            Weapons weapon = GetComponent<WeaponDatabase>().FetchWeaponByID(items.ID);
            newItem = new Inventory(weapon, count, slotAmount);
        }
        else
        {
            newItem = new Inventory(items, count, slotAmount);
        }
        playerItems.Add(newItem.Item.ID, newItem);
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
        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
        if (sceneName == "VillageScene")
        {
            inventoryPanel.transform.Find("SendToVillage").gameObject.SetActive(true);
        }
        else
        {
            inventoryPanel.transform.Find("SendToVillage").gameObject.SetActive(false);
        }
    }

    public void CloseInventoryPanelUI()
    {
        inventoryPanel.SetActive(false);
        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
        //Closes the inventory panel no matter what scene the player is currently in.
        if (sceneName == "LootScene" || sceneName == "BrandonTest") //Close the chest/player inventory
        {
            GameObject.Find("Manager").gameObject.GetComponent<CreateDynamicInventory>().CloseUi();
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
            playerItems[slots[i].GetComponentInChildren<ItemData>().GetItem().Item.ID].SlotNum = i;
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

    public void PrintInventory()
    {
        foreach (KeyValuePair<int, Inventory> keyValue in playerItems)
        {
            int key = keyValue.Key;
            Debug.Log(playerItems[key].Item.Title + ".....Slot Num: " + playerItems[key].SlotNum);
        }
    }
}
