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
    public GameObject trash;

    void Start()
    {
        maxInventorySize = 100; // set this somewhere
        currentSize = 0;
        LoadInventory();
    }

    public void AddItemToInventory(Items item)
    {
        if (CanFitInInventory(item.Size))
        {
            if (IsWeapon(item.ID))
            {
                Inventory newItem = new Inventory(item, 1);
                playerItems.Add(newItem);
                currentSize += item.Size;
                AddItemToSlots(newItem);
                UpdateInventoryText();
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
                Inventory newItem = new Inventory(item, 1);
                playerItems.Add(newItem);
                currentSize += item.Size;
                SaveInventory();
                AddItemToSlots(newItem);
                UpdateInventoryText();
            }
        }
    }

    //TODO: Also remove slot when removing items. But also make sure that the slot IDs play nice.
    public void RemoveItemFromInventory(Inventory item)
    {
        currentSize -= item.Item.Size;
        if (IsWeapon(item.Item.ID))
        {
            playerItems.Remove(item);
            item.Count--;
        }
        else
        {
            for (int i = 0; i < playerItems.Count; i++)
            {
                if (playerItems[i].Item.ID == item.Item.ID)
                {
                    playerItems[i].Count--;
                    if (playerItems[i].Count == 0)
                    {
                        playerItems.RemoveAt(i);
                    }
                    break;
                }
            }
        }
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

    void AddItemToSlots(Inventory item)
    {
        GameObject itemObject = Instantiate(itemPrefab);
        AddDynamicSlot();
        itemObject.transform.SetParent(slots[slotAmount - 1].transform);
        itemObject.transform.localPosition = Vector2.zero;
        itemObject.name = item.Item.Title;
        itemObject.GetComponentInChildren<ItemData>().slotID = slotAmount - 1;
        itemObject.GetComponentInChildren<ItemData>().SetItem(item);
        if (IsWeapon(item.Item.ID) || item.Count == 1)
        {
            itemObject.GetComponent<Text>().text = item.Item.Title;
        }
        else
        {
            itemObject.GetComponent<Text>().text = item.Item.Title + " x" + item.Count;
        }
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
        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
        if (sceneName == "LootScene") //Close the chest/player inventory
        {
            GameObject lootSceneController = GameObject.FindGameObjectWithTag("LootSceneManager");
            lootSceneController.GetComponent<LootSceneController>().CloseChestUI();
            lootSceneController.GetComponent<LootGenerator>().CloseAllChestUi();
        }
    }

    public void ReorganizeSlots(GameObject slot)
    {
        slotAmount--;
        slots.Remove(slot);
        for (int i = 0; i < slotAmount; i++)
        {
            slots[i].GetComponent<ItemSlot>().id = i;
            slots[i].GetComponentInChildren<ItemData>().slotID = i;
        }
        Destroy(slot);
    }
}
