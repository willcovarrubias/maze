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
    public GameObject inventoryName;
    public GameObject dialogBox;
    public GameObject otherSortButton, sendToPlayerButton, sendToVillage;
    public int slotAmount;
    public List<GameObject> slots = new List<GameObject>();
    public GameObject equippedCheckMark;
    Scene currentScene;
    private string sceneName;

    public GameObject inventoryPane;
    public RectTransform slotPanelRectTransform;
    public ScrollRect scrollView;

    GameObject villageInventory;
    int sorting;
    Vector3 originalPosition;

    void Start()
    {
        maxInventorySize = GetComponent<ActiveCharacterController>().GetActiveCharacter().items;
        currentSize = 0;
        if (PlayerPrefs.GetInt("Exit Maze") == 0)
        {
            LoadInventory("Player Item");
        }
        else
        {
            LoadInventory("Temp");
            PlayerPrefs.SetInt("Exit Maze", 0);
        }
        SaveInventory("Player Item");
        Button actionButton = otherSortButton.GetComponent<Button>();
        actionButton.onClick.AddListener(OtherSortButtonAction);
        Button discardButton = sendToPlayerButton.GetComponent<Button>();
        discardButton.onClick.AddListener(SendToPlayerAction);
        originalPosition = scrollView.transform.position;
        ShowEquippedOnStartUp();
    }

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
                SaveInventory("Player Item");
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

    public void AddBoughtItem(Inventory item)
    {
        if (IsWeapon(item.Item.ID))
        {
            CreateNewItem(item.Item, 1);
            return;
        }
        Inventory temp;
        if (playerItems.TryGetValue(item.Item.ID, out temp))
        {
            playerItems[item.Item.ID].Count += item.Count;
            currentSize += item.Item.Size * item.Count;
            SaveInventory("Player Item");
            slots[playerItems[item.Item.ID].SlotNum].GetComponentInChildren<ItemData>().GetItem().Count = playerItems[item.Item.ID].Count;
            slots[playerItems[item.Item.ID].SlotNum].GetComponentInChildren<Text>().text = playerItems[item.Item.ID].Item.Title + " x" + playerItems[item.Item.ID].Count;
            UpdateInventoryText();
        }
        else
        {
            CreateNewItem(item.Item, item.Count);
        }
    }

    public void RemoveItemsFromInventory(Inventory item, int count, int slotId)
    {
        if (item.Count >= count)
        {
            currentSize -= item.Item.Size * count;
            if (IsWeapon(item.Item.ID))
            {
                CheckItemToUnequip(item.Item);
                playerItems.Remove(item.Item.ID);
                item.Count = 0;
                ReorganizeSlots(slotId);
            }
            else
            {
                CheckItemToUnequip(item.Item);
                playerItems[item.Item.ID].Count -= count;
                if (playerItems[item.Item.ID].Count <= 0)
                {
                    playerItems.Remove(item.Item.ID);
                    item.Count = 0;
                    ReorganizeSlots(slotId);
                }
                else
                {
                    UpdateSlotText(slotId, item);
                }
            }
            SaveInventory("Player Item");
            UpdateInventoryText();
        }
    }

    public void RemoveWholeStackFromInventory(Inventory items, int slot)
    {
        currentSize -= items.Item.Size * items.Count;
        CheckItemToUnequip(items.Item);
        playerItems.Remove(items.Item.ID);
        items.Count = 0;
        ReorganizeSlots(slot);
        SaveInventory("Player Item");
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
        SaveInventory("Player Item");
    }

    public void SortInventory()
    {
        List<KeyValuePair<int, Inventory>> temp = new List<KeyValuePair<int, Inventory>>();
        foreach (KeyValuePair<int, Inventory> keyValue in playerItems)
        {
            int key = keyValue.Key;
            int size = playerItems[key].Item.Size;
            int id = playerItems[key].Item.ID;
            KeyValuePair<int, Inventory> item = new KeyValuePair<int, Inventory>(playerItems[key].SlotNum, playerItems[key]);
            temp.Add(item);
        }
        if (sorting > 2)
        {
            sorting = 0;
        }
        if (sorting == 0)
        {
            temp.Sort(delegate (KeyValuePair<int, Inventory> x, KeyValuePair<int, Inventory> y)
            {
                if (x.Value.Item.Size == y.Value.Item.Size)
                {
                    return x.Value.Item.ID.CompareTo(y.Value.Item.ID);
                }
                return x.Value.Item.Size.CompareTo(y.Value.Item.Size);
            });
            ChangeDialogBox("Sorted by size");
        }
        else if (sorting == 1)
        {
            temp.Sort(delegate (KeyValuePair<int, Inventory> x, KeyValuePair<int, Inventory> y)
            {
                return x.Value.Item.ID.CompareTo(y.Value.Item.ID);
            });
            ChangeDialogBox("Sorted by type");
        }
        else if (sorting == 2)
        {
            temp.Sort(delegate (KeyValuePair<int, Inventory> x, KeyValuePair<int, Inventory> y)
            {
                return string.Compare(x.Value.Item.Title, y.Value.Item.Title, System.StringComparison.Ordinal);
            });
            ChangeDialogBox("Sorted by name");
        }
        ClearSlots();
        playerItems.Clear();
        for (int i = 0; i < temp.Count; i++)
        {
            temp[i].Value.SlotNum = i;
            playerItems.Add(temp[i].Value.Item.ID, temp[i].Value);
            AddItemToSlots(temp[i].Value);
        }
        sorting++;
        ShowEquippedOnStartUp();
        SaveInventory("Player Item");
    }

    public void UpdateInventoryText()
    {
        inventoryText.GetComponent<Text>().text = "Limit: " + currentSize + " / " + maxInventorySize;
        GetComponent<ActiveCharacterController>().UpdateStats();
    }

    public int GetCurrentSize()
    {
        return currentSize;
    }

    public bool CanFitInInventory(int itemSize)
    {
        if (currentSize + itemSize <= maxInventorySize)
        {
            return true;
        }
        return false;
    }

    public void SaveInventory(string item)
    {
        int i = 0;
        foreach (KeyValuePair<int, Inventory> keyValue in playerItems)
        {
            int key = keyValue.Key;
            PlayerPrefs.SetInt(item + " ID" + i, playerItems[key].Item.ID);
            PlayerPrefs.SetInt(item + " Count" + i, playerItems[key].Count);
            PlayerPrefs.SetInt(item + " Slot" + i, playerItems[key].SlotNum);
            if (IsWeapon(playerItems[key].Item.ID))
            {
                Weapons weapon = (Weapons)playerItems[key].Item;
                PlayerPrefs.SetString(item + " Name" + i, weapon.Title);
                PlayerPrefs.SetInt(item + " Attack" + i, weapon.Attack);
                PlayerPrefs.SetInt(item + " Special" + i, weapon.Special);
                PlayerPrefs.SetInt(item + " Speed" + i, weapon.Speed);
                PlayerPrefs.SetInt(item + " Duribility" + i, weapon.Durability);
                PlayerPrefs.SetInt(item + " Size" + i, weapon.Size);
                PlayerPrefs.SetString(item + " Slug" + i, weapon.Slug);
            }
            i++;
        }
        PlayerPrefs.SetInt(item + " Count", i);
        GameMaster.gameMaster.PlayerPrefsSave();
    }

    public void LoadInventory(string item)
    {
        Dictionary<int, Inventory> tempList = new Dictionary<int, Inventory>();
        int itemCount = PlayerPrefs.GetInt(item + " Count");
        playerItems.Clear();
        currentSize = 0;
        for (int i = 0; i < itemCount; i++)
        {
            int id = PlayerPrefs.GetInt(item + " ID" + i);
            int count = PlayerPrefs.GetInt(item + " Count" + i);
            int slotNum = PlayerPrefs.GetInt(item + " Slot" + i);
            Inventory loadedItem;
            if (IsWeapon(id))
            {
                string title = PlayerPrefs.GetString(item + " Name" + i);
                int attack = PlayerPrefs.GetInt(item + " Attack" + i);
                int special = PlayerPrefs.GetInt(item + " Special" + i);
                int speed = PlayerPrefs.GetInt(item + " Speed" + i);
                int durability = PlayerPrefs.GetInt(item + " Duribility" + i);
                int size = PlayerPrefs.GetInt(item + " Size" + i);
                string slug = PlayerPrefs.GetString(item + " Slug" + i);
                Weapons weapon = new Weapons(id, title, attack, special, speed, durability, size, slug);
                loadedItem = new Inventory(weapon, count, slotNum);
            }
            else
            {
                Items newItem = GetComponent<ItemDatabase>().FetchItemByID(id);
                loadedItem = new Inventory(newItem, count, slotNum);
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
        playerItems.Add(items.ID, newItem);
        currentSize += items.Size * count;
        AddItemToSlots(newItem);
        SaveInventory("Player Item");
        UpdateInventoryText();
    }

    public void SetEquippedWeapon(Items item)
    {
        if (IsWeapon(item.ID))
        {
            int weaponID = GetEquippedWeaponID();
            if (weaponID != 0)
            {
                slots[playerItems[weaponID].SlotNum].GetComponentInChildren<ItemData>().UnequipItem();
            }
            slots[playerItems[item.ID].SlotNum].GetComponentInChildren<ItemData>().EquipItem();
            PlayerPrefs.SetInt("Equipped Weapon", item.ID);
            GameMaster.gameMaster.PlayerPrefsSave();
            GetComponent<ActiveCharacterController>().UpdateActiveCharacterVisuals();
        }
    }

    public Weapons GetEquippedWeapon()
    {
        if (GetEquippedWeaponID() >= 0)
        {
            return (Weapons)playerItems[GetEquippedWeaponID()].Item;
        }
        return null;
    }

    public int GetEquippedWeaponID()
    {
        return PlayerPrefs.GetInt("Equipped Weapon");
    }

    public void SetEquippedHat(Items item)
    {
        int hatID = GetEquippedHatID();
        if (hatID != 0)
        {
            slots[playerItems[hatID].SlotNum].GetComponentInChildren<ItemData>().UnequipItem();
        }
        slots[playerItems[item.ID].SlotNum].GetComponentInChildren<ItemData>().EquipItem();
        PlayerPrefs.SetInt("Equipped Hat", item.ID);
        GameMaster.gameMaster.PlayerPrefsSave();
        GetComponent<ActiveCharacterController>().UpdateActiveCharacterVisuals();
    }

    public Armor GetEquippedHat()
    {
        if (GetEquippedHatID() != 0)
        {
            return (Armor)playerItems[GetEquippedHatID()].Item;
        }
        return null;
    }

    public int GetEquippedHatID()
    {
        return PlayerPrefs.GetInt("Equipped Hat");
    }

    public void SetEquippedBody(Items item)
    {
        int bodyID = GetEquippedBodyID();
        if (bodyID != 0)
        {
            slots[playerItems[bodyID].SlotNum].GetComponentInChildren<ItemData>().UnequipItem();
        }
        slots[playerItems[item.ID].SlotNum].GetComponentInChildren<ItemData>().EquipItem();
        PlayerPrefs.SetInt("Equipped Body", item.ID);
        GameMaster.gameMaster.PlayerPrefsSave();
        GetComponent<ActiveCharacterController>().UpdateActiveCharacterVisuals();
    }

    public Armor GetEquippedBody()
    {
        if (GetEquippedBodyID() != 0)
        {
            return (Armor)playerItems[GetEquippedBodyID()].Item;
        }
        return null;
    }

    public int GetEquippedBodyID()
    {
        return PlayerPrefs.GetInt("Equipped Body");
    }

    public void UnequipHat(Items item)
    {
        PlayerPrefs.SetInt("Equipped Hat", 0);
        GameMaster.gameMaster.PlayerPrefsSave();
        if (slots[playerItems[item.ID].SlotNum].GetComponentInChildren<ItemData>())
        {
            slots[playerItems[item.ID].SlotNum].GetComponentInChildren<ItemData>().UnequipItem();
        }
        GetComponent<ActiveCharacterController>().UpdateActiveCharacterVisuals();
    }

    public void UnequipBody(Items item)
    {
        PlayerPrefs.SetInt("Equipped Body", 0);
        GameMaster.gameMaster.PlayerPrefsSave();
        if (slots[playerItems[item.ID].SlotNum].GetComponentInChildren<ItemData>())
        {
            slots[playerItems[item.ID].SlotNum].GetComponentInChildren<ItemData>().UnequipItem();
        }
        GetComponent<ActiveCharacterController>().UpdateActiveCharacterVisuals();
    }

    public void UnequipWeapon(Items item)
    {
        PlayerPrefs.SetInt("Equipped Weapon", 0);
        GameMaster.gameMaster.PlayerPrefsSave();
        if (slots[playerItems[item.ID].SlotNum].GetComponentInChildren<ItemData>())
        {
            slots[playerItems[item.ID].SlotNum].GetComponentInChildren<ItemData>().UnequipItem();
        }
        GetComponent<ActiveCharacterController>().UpdateActiveCharacterVisuals();
    }

    void CheckItemToUnequip(Items item)
    {
        if (item.ID == GetEquippedWeaponID())
        {
            UnequipWeapon(item);
        }
        else if (item.ID == GetEquippedHatID())
        {
            UnequipHat(item);
        }
        else if (item.ID == GetEquippedBodyID())
        {
            UnequipBody(item);
        }
    }

    void AddItemToSlots(Inventory item)
    {
        GameObject itemObject = Instantiate(itemPrefab);
        AddDynamicSlot();
        itemObject.transform.SetParent(slots[slotAmount - 1].transform, false);
        itemObject.transform.localPosition = Vector2.zero;
        itemObject.name = item.Item.Title;
        itemObject.GetComponentInChildren<ItemData>().slotID = slotAmount - 1;
        itemObject.GetComponentInChildren<ItemData>().SetItem(item);
        itemObject.GetComponentInChildren<ItemData>().SetLocation(Location.WhereAmI.player);
        itemObject.GetComponentInChildren<Image>().sprite = item.Item.Sprite;
        ChangeSlotColor(itemObject.transform.parent.gameObject, item.Item.ID);
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
        slots[slotAmount - 1].transform.SetParent(slotPanel.transform, false);
    }

    public void ReorganizeSlots(int slotID)
    {
        GameObject currentSlot = slots[slotID];
        slotAmount--;
        slots.RemoveAt(slotID);
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].GetComponent<ItemSlot>().id = i;
            slots[i].GetComponentInChildren<ItemData>().slotID = i;
            playerItems[slots[i].GetComponentInChildren<ItemData>().GetItem().Item.ID].SlotNum = i;
        }
        Destroy(currentSlot);
        ResizeSlotPanel();
    }

    public void ChangeSlotColor(GameObject slot, int id)
    {
        if (id >= 1000 && id < 2000)
        {
            slot.GetComponent<Image>().color = new Color(1, 0.5f, 0.5f);
        }
        else if (id >= 2000 && id < 3000)
        {
            slot.GetComponent<Image>().color = new Color(0.5f, 1, 0.5f);
        }
        else if (id >= 3000 && id < 4000)
        {
            slot.GetComponent<Image>().color = new Color(1, 0.75f, 0.5f);
        }
        else if (id >= 4000 && id < 5000)
        {
            slot.GetComponent<Image>().color = new Color(0.5f, 0.5f, 1);
        }
        else if (id >= 10000)
        {
            slot.GetComponent<Image>().color = new Color(0.75f, 0.5f, 1);
        }
    }

    public void ClearSlots()
    {
        int i = 0;
        while (i < slotPanel.transform.childCount)
        {
            Destroy(slotPanel.transform.GetChild(i).gameObject);
            i++;
        }
        slots.Clear();
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

    public void UpdateSlotText(int slotID, Inventory item)
    {
        if (item.Count > 1)
        {
            slots[slotID].GetComponentInChildren<Text>().text = item.Item.Title + " x" + item.Count;
        }
        else
        {
            slots[slotID].GetComponentInChildren<Text>().text = item.Item.Title;
        }
    }

    public void OpenInventoryPanelUI()
    {
        inventoryPanel.SetActive(true);
        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
        MoveInventory();
        if (sceneName == "VillageScene")
        {
            if (VillageSceneController.villageScene.GetComponent<VillageSceneController>().currentMenu == Location.VillageMenu.inventory)
            {
                otherSortButton.SetActive(true);
                sendToVillage.SetActive(true);
                sendToPlayerButton.SetActive(false);
            }
            else
            {
                otherSortButton.SetActive(false);
                sendToVillage.SetActive(false);
                sendToPlayerButton.SetActive(false);
            }
        }
        else if (sceneName == "LootScene")
        {
            otherSortButton.SetActive(false);
            sendToVillage.SetActive(false);
            sendToPlayerButton.SetActive(false);
        }
        else
        {
            otherSortButton.SetActive(false);
            sendToVillage.SetActive(false);
            sendToPlayerButton.SetActive(false);
        }
    }

    public void CloseInventoryPanelUI()
    {
        inventoryPanel.SetActive(false);
        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
        //Closes the inventory panel no matter what scene the player is currently in.
        if (sceneName == "LootScene" || sceneName == "BrandonTest" || sceneName == "FightScene") //Close the chest/player inventory
        {
            GameObject.Find("Manager").gameObject.GetComponent<CreateDynamicInventory>().CloseUi();
        }
        if (sceneName == "VillageScene")
        {
            switch (VillageSceneController.villageScene.GetComponent<VillageSceneController>().currentMenu)
            {
                case Location.VillageMenu.inventory:
                    VillageSceneController.villageScene.GetComponent<VillageSceneController>().InventoryUIClose();
                    break;
                case Location.VillageMenu.armor:
                    VillageSceneController.villageScene.GetComponent<CraftingDatabase>().armorMenu.GetComponent<CraftingMenu>().CloseUI();
                    break;
                case Location.VillageMenu.pub:
                    VillageSceneController.villageScene.GetComponent<CraftingDatabase>().consumablesMenu.GetComponent<CraftingMenu>().CloseUI();
                    break;
                case Location.VillageMenu.weapons:
                    VillageSceneController.villageScene.GetComponent<CraftingDatabase>().weaponsMenu.GetComponent<CraftingMenu>().CloseUI();
                    break;
            }
        }
    }

    void OtherSortButtonAction()
    {
        if (SceneManager.GetActiveScene().name == "VillageScene")
        {
            villageInventory = GameObject.FindGameObjectWithTag("VillageSceneManager");
            villageInventory.GetComponent<VillageInventoryManager>().SortInventory();
        }
        else if (SceneManager.GetActiveScene().name == "LootScene")
        {

        }
    }

    void SendToPlayerAction()
    {
        if (SceneManager.GetActiveScene().name == "LootScene")
        {

        }
    }

    void MoveInventory()
    {
        if (SceneManager.GetActiveScene().name == "LootScene" || SceneManager.GetActiveScene().name == "VillageScene" || SceneManager.GetActiveScene().name == "FightScene")
        {
            inventoryName.transform.parent.transform.position = new Vector3(originalPosition.x, inventoryName.transform.position.y, originalPosition.z);
            inventoryText.transform.parent.transform.position = new Vector3(originalPosition.x, inventoryText.transform.position.y, originalPosition.z);
            scrollView.transform.position = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z);
        }
        else
        {
            inventoryName.transform.parent.transform.position = new Vector3(Screen.width / 2, inventoryName.transform.position.y, originalPosition.z);
            inventoryText.transform.parent.transform.position = new Vector3(Screen.width / 2, inventoryText.transform.position.y, originalPosition.z);
            scrollView.transform.position = new Vector3(Screen.width / 2, originalPosition.y, originalPosition.z);
        }
    }

    public void ChangeMaxInventorySize(int amount)
    {
        maxInventorySize = amount;
        UpdateInventoryText();
    }

    void ShowEquippedOnStartUp()
    {
        int bodyID = GetEquippedBodyID();
        int hatID = GetEquippedHatID();
        int weaponID = GetEquippedWeaponID();
        if (GetEquippedBodyID() != 0)
        {
            slots[playerItems[bodyID].SlotNum].GetComponentInChildren<ItemData>().EquipItem();
        }
        if (GetEquippedHatID() != 0)
        {
            slots[playerItems[hatID].SlotNum].GetComponentInChildren<ItemData>().EquipItem();
        }
        if (GetEquippedWeaponID() != 0)
        {
            slots[playerItems[weaponID].SlotNum].GetComponentInChildren<ItemData>().EquipItem();
        }
    }

    public void LoseRandomAmountOfItems()
    {
        if (playerItems.Count > 0)
        {
            int amount = Random.Range(1, playerItems.Count);
            for (int i = 0; i < amount; i++)
            {
                List<int> keyList = new List<int>(playerItems.Keys);
                int randomKey = keyList[Random.Range(0, keyList.Count)];
                int randomAmount = Random.Range(1, playerItems[randomKey].Count);
                RemoveItemsFromInventory(playerItems[randomKey], randomAmount, playerItems[randomKey].SlotNum);
            }
        }
    }

    public void LoseAllItems()
    {
        List<int> keyList = new List<int>(playerItems.Keys);
        for (int i = 0; i < keyList.Count; i++)
        {
            int key = keyList[i];
            RemoveWholeStackFromInventory(playerItems[key], playerItems[key].SlotNum);
        }
    }
}
