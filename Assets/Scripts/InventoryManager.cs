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
    public GameObject dialogBox;
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
        if (Input.GetKeyUp("s"))
        {
            SortInventory();
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
        if (amountCanFit <= 0)
        {
            ChangeDialogBox("Inventory full!");
        }
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
        villageInventory.GetComponent<VillageInventoryManager>().SaveVillageInventory();
        SaveInventory();
    }

    public void SortInventory()
    {
        //TODO: everytime you press the button it will sort a differnt way
        //first by consumables, materials, armor, weapons
        //then it will sort by size
        int itemCount = PlayerPrefs.GetInt("Player Item Count");
        List<KeyValuePair<int, Inventory>> temp = new List<KeyValuePair<int, Inventory>>();
        foreach (KeyValuePair<int, Inventory> keyValue in playerItems)
        {
            int key = keyValue.Key;
            int size = playerItems[key].Item.Size;
            int id = playerItems[key].Item.ID;
            KeyValuePair<int, Inventory> item = new KeyValuePair<int, Inventory>(playerItems[key].SlotNum, playerItems[key]);
            temp.Add(item);
        }
        temp.Sort(delegate (KeyValuePair<int, Inventory> x, KeyValuePair<int, Inventory> y)
        {
            return x.Value.Item.Size.CompareTo(y.Value.Item.Size);
        });
        for (int i = 0; i < temp.Count; i++)
        {
            Debug.Log(temp[i].Value.Item.Title + temp[i].Value.Item.Size);
        }
        ClearSlots();
        playerItems.Clear();
        currentSize = 0;
        for (int i = 0; i < temp.Count; i++)
        {
            temp[i].Value.SlotNum = i;
            playerItems.Add(temp[i].Value.Item.ID, temp[i].Value);
            AddItemToSlots(temp[i].Value);
            //CreateNewItem(temp[i].Value.Item, temp[i].Value.Count);
        }
        ChangeDialogBox("Sorted by Size");
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
            PlayerPrefs.SetInt("Player Item Slot" + i, playerItems[key].SlotNum);
            if (IsWeapon(playerItems[key].Item.ID))
            {
                Weapons weapon = (Weapons)playerItems[key].Item;
                PlayerPrefs.SetString("Player Item Name" + i, weapon.Title);
                PlayerPrefs.SetInt("Player Item Rarity" + i, weapon.Rarity);
                PlayerPrefs.SetInt("Player Item Attack" + i, weapon.Attack);
                PlayerPrefs.SetInt("Player Item Special" + i, weapon.Special);
                PlayerPrefs.SetInt("Player Item Duribility" + i, weapon.Durability);
                PlayerPrefs.SetInt("Player Item Size" + i, weapon.Size);
            }
            i++;
        }
        PlayerPrefs.SetInt("Player Item Count", i);
        PlayerPrefs.Save();
    }

    public void LoadInventory()
    {
        Dictionary<int, Inventory> tempList = new Dictionary<int, Inventory>();
        int itemCount = PlayerPrefs.GetInt("Player Item Count");
        playerItems.Clear();
        currentSize = 0;
        for (int i = 0; i < itemCount; i++)
        {
            int id = PlayerPrefs.GetInt("Player Item ID" + i);
            int count = PlayerPrefs.GetInt("Player Item Count" + i);
            int slotNum = PlayerPrefs.GetInt("Player Item Slot" + i);
            Inventory loadedItem;
            if (IsWeapon(id))
            {
                string title = PlayerPrefs.GetString("Player Item Name" + i);
                int rarity = PlayerPrefs.GetInt("Player Item Rarity" + i);
                int attack = PlayerPrefs.GetInt("Player Item Attack" + i);
                int special = PlayerPrefs.GetInt("Player Item Special" + i);
                int duribility = PlayerPrefs.GetInt("Player Item Duribility" + i);
                int size = PlayerPrefs.GetInt("Player Item Size" + i);
                Weapons weapon = new Weapons(id, title, rarity, attack, special, duribility, size, "");
                loadedItem = new Inventory(weapon, count, slotNum);
            }
            else
            {
                Items item = GetComponent<ItemDatabase>().FetchItemByID(id);
                loadedItem = new Inventory(item, count, slotNum);
            }
            tempList.Add(loadedItem.SlotNum, loadedItem);
        }
        for (int i = 0; i < tempList.Count; i++)
        {
            playerItems.Add(tempList[i].Item.ID, tempList[i]);
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

    public void ClearSlots()
    {
        int i = 0;
        while (i < slotPanel.transform.childCount)
        {
            Destroy(slotPanel.transform.GetChild(i).gameObject);
            i++;
        }
        slotAmount = 0;
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

    public void ChangeDialogBox(string text)
    {
        if (text != "")
        {
            dialogBox.SetActive(true);
            if (dialogBox.GetComponent<DialogTimer>() != null)
            {
                dialogBox.GetComponent<DialogTimer>().ResetCurrentTime();
            }
            else
            {
                dialogBox.AddComponent<DialogTimer>();
            }
        }
        else
        {
            dialogBox.SetActive(false);
            Destroy(dialogBox.GetComponent<DialogTimer>());
        }
        dialogBox.GetComponentInChildren<Text>().text = text;
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
