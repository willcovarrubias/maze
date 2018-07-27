using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VillageInventoryManager : MonoBehaviour
{
    int maxVillageInventorySize;
    int currentSize;
    public List<Inventory> villageItems = new List<Inventory>();

    public GameObject slotPanel;
    public GameObject slot;
    public GameObject itemPrefab;
    public GameObject villageInventoryText;
    public int slotAmount;
    public List<GameObject> slots = new List<GameObject>();

    GameObject gameMaster;
    public GameObject addItemsToVillageInventory;
    //public GameObject add_ALL_ItemsToVillageInventory;

    public RectTransform slotPanelRectTransform;
    public ScrollRect scrollViewVillage;

    private void Start()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameController");
        maxVillageInventorySize = 100;
        currentSize = 0;
        LoadVillageInventory();
        ResizeSlotPanel();
    }

    public void AddItemToVillageInventory(Items item)
    {
        if (CanFitInInventory(item.Size))
        {
            if (IsWeapon(item.ID))
            {
                Inventory newItem = new Inventory(item, 1);
                villageItems.Add(newItem);
                currentSize += item.Size;
                SaveVillageInventory();
                AddItemToSlots(newItem);
                UpdateInventoryText();
            }
            else
            {
                for (int i = 0; i < villageItems.Count; i++)
                {
                    if (villageItems[i].Item.ID == item.ID)
                    {
                        villageItems[i].Count++;
                        currentSize += item.Size;
                        SaveVillageInventory();
                        for (int j = 0; j < slots.Count; j++)
                        {
                            if (slots[j].GetComponentInChildren<ItemData>().GetItem().Item.ID == item.ID)
                            {
                                slots[j].GetComponentInChildren<ItemData>().GetItem().Count = villageItems[i].Count;
                                slots[j].GetComponentInChildren<Text>().text = villageItems[i].Item.Title + " x" + villageItems[i].Count;
                                UpdateInventoryText();
                                break;
                            }
                        }
                        return;
                    }
                }
                Inventory newItem = new Inventory(item, 1);
                villageItems.Add(newItem);
                currentSize += item.Size;
                SaveVillageInventory();
                AddItemToSlots(newItem);
                UpdateInventoryText();
            }
        }
    }

    public void RemoveItemFromVillageInventory(Inventory item)
    {
        currentSize -= item.Item.Size;
        if (IsWeapon(item.Item.ID))
        {
            villageItems.Remove(item);
            item.Count = 0;
        }
        else
        {
            for (int i = 0; i < villageItems.Count; i++)
            {
                if (villageItems[i].Item.ID == item.Item.ID)
                {
                    villageItems[i].Count--;
                    if (villageItems[i].Count == 0)
                    {
                        villageItems.RemoveAt(i);
                    }
                    break;
                }
            }
        }
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

    void ResizeSlotPanel()
    {
        //Sets the slot panel RectTransform's size dependent on how many slots there are. This allows for the scrolling logic to work.
        slotPanelRectTransform.Translate(0, (slotAmount * -35), 0);
        slotPanelRectTransform.sizeDelta = new Vector2(407.4f, (slotAmount * 70));
    }

    public void SaveVillageInventory()
    {
        int i;
        for (i = 0; i < villageItems.Count; i++)
        {
            PlayerPrefs.SetInt("Village Item ID" + i, villageItems[i].Item.ID);
            PlayerPrefs.SetInt("Village Item Count" + i, villageItems[i].Count);
            if (IsWeapon(villageItems[i].Item.ID))
            {
                Weapons weapon = (Weapons)villageItems[i].Item;
                PlayerPrefs.SetInt("Village Item Duribility", weapon.Durability);
            }
        }
        PlayerPrefs.SetInt("Village Item Count", i);
        PlayerPrefs.Save();
    }

    public void LoadVillageInventory()
    {
        int itemCount = PlayerPrefs.GetInt("Village Item Count");
        villageItems.Clear();
        currentSize = 0;
        for (int i = 0; i < itemCount; i++)
        {
            int id = PlayerPrefs.GetInt("Village Item ID" + i);
            int count = PlayerPrefs.GetInt("Village Item Count" + i);
            if (IsWeapon(id))
            {
                int duribility = PlayerPrefs.GetInt("Village Item Duribility");
                Weapons weapon = gameMaster.GetComponent<WeaponDatabase>().FetchWeaponByID(id);
                Inventory loadedItem = new Inventory(weapon, count);
                weapon.Durability = duribility;
                villageItems.Add(loadedItem);
                currentSize += weapon.Size * count;
                AddItemToSlots(loadedItem);
            }
            else
            {
                Items item = gameMaster.GetComponent<ItemDatabase>().FetchItemByID(id);
                Inventory loadedItem = new Inventory(item, count);
                villageItems.Add(loadedItem);
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
        }
        Destroy(currentSlot);
        ResizeSlotPanel();
    }
}

public class Inventory
{
    public Items Item { get; set; }
    public int Count { get; set; }

    public Inventory(Items item, int count)
    {
        this.Item = item;
        this.Count = count;
    }
}
