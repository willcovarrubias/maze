using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DynamicInventory : MonoBehaviour
{
    public Dictionary<int, Inventory> items = new Dictionary<int, Inventory>();

    public List<GameObject> slots = new List<GameObject>();
    int slotAmount;
    GameObject slotPanel;
    GameObject slotPrefab;
    GameObject itemPrefab;
    GameObject inventoryPane;
    RectTransform slotPanelRectTransform;
    public GameObject openButton;
    GameObject sendAllButton;
    GameObject sortButton;

    GameObject gameMaster;
    Location.WhereAmI location;
    int sorting;

    void Start()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameController");
        gameObject.transform.SetParent(gameMaster.transform.Find("Canvas").transform, true);
        gameObject.transform.SetSiblingIndex(1);
    }

    public void Initialize(Location.WhereAmI loc, List<Inventory> createdItems, GameObject slotPre, GameObject itemPre, GameObject button)
    {
        location = loc;
        slotPrefab = slotPre;
        itemPrefab = itemPre;
        openButton = button;
        inventoryPane = gameObject.transform.Find("ScrollView").gameObject;
        slotPanel = inventoryPane.transform.Find("SlotPanel").gameObject;
        sendAllButton = gameObject.transform.Find("SendAll").gameObject;
        sortButton = gameObject.transform.Find("Sort").gameObject;
        slotPanelRectTransform = slotPanel.GetComponent<RectTransform>();
        Button open = button.GetComponent<Button>();
        open.onClick.AddListener(OpenUI);
        Button sendAll = sendAllButton.GetComponent<Button>();
        sendAll.onClick.AddListener(MoveWholeInventoryToPlayer);
        Button sort = sortButton.GetComponent<Button>();
        sort.onClick.AddListener(SortInventory);
        InitalizeSlots(createdItems);
    }

    public void MoveItemsToHere(Inventory item, int thisSlotId, int amount)
    {
        if (IsWeapon(item.Item.ID))
        {
            CreateNewItem(item.Item, 1);
            gameMaster.GetComponent<InventoryManager>().RemoveItemsFromInventory(item, 1, thisSlotId);
        }
        else
        {
            Inventory temp;
            if (items.TryGetValue(item.Item.ID, out temp))
            {
                items[item.Item.ID].Count += amount;
                slots[items[item.Item.ID].SlotNum].GetComponentInChildren<ItemData>().GetItem().Count = items[item.Item.ID].Count;
                slots[items[item.Item.ID].SlotNum].GetComponentInChildren<Text>().text = items[item.Item.ID].Item.Title + " x" + items[item.Item.ID].Count;
                gameMaster.GetComponent<InventoryManager>().RemoveItemsFromInventory(item, amount, thisSlotId);
                return;
            }
            CreateNewItem(item.Item, amount);
            gameMaster.GetComponent<InventoryManager>().RemoveItemsFromInventory(item, amount, thisSlotId);
        }
    }

    public void RemoveItemsFromInventory(Inventory item, int count, int slotId)
    {
        if (item.Count >= count)
        {
            if (IsWeapon(item.Item.ID))
            {
                items.Remove(item.Item.ID);
                item.Count = 0;
                ReorganizeSlots(slotId);
            }
            else
            {
                items[item.Item.ID].Count -= count;
                if (items[item.Item.ID].Count <= 0)
                {
                    items.Remove(item.Item.ID);
                    item.Count = 0;
                    ReorganizeSlots(slotId);
                }
                else
                {
                    UpdateSlotText(slotId, item.Item.ID);
                }
            }
        }
    }

    public void RemoveWholeStackFromInventory(Inventory item)
    {
        items.Remove(item.Item.ID);
        item.Count = 0;
    }

    public void MoveWholeInventoryToPlayer()
    {
        for (int i = 0; i < slots.Count; i += 0)
        {
            slots[i].GetComponentInChildren<ItemData>().GetItem();
            bool complete = gameMaster.GetComponent<InventoryManager>().MoveItemsToPlayerInventory(
                            slots[i].GetComponentInChildren<ItemData>().GetItem(),
                            slots[i].GetComponentInChildren<ItemData>().slotID,
                            slots[i].GetComponentInChildren<ItemData>().GetItem().Count,
                            false,
                            gameObject);
            if (!complete)
            {
                i++;
            }
        }
    }

    public void SortInventory()
    {
        List<KeyValuePair<int, Inventory>> temp = new List<KeyValuePair<int, Inventory>>();
        foreach (KeyValuePair<int, Inventory> keyValue in items)
        {
            int key = keyValue.Key;
            int size = items[key].Item.Size;
            int id = items[key].Item.ID;
            KeyValuePair<int, Inventory> item = new KeyValuePair<int, Inventory>(items[key].SlotNum, items[key]);
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
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Sorted by size");
        }
        else if (sorting == 1)
        {
            temp.Sort(delegate (KeyValuePair<int, Inventory> x, KeyValuePair<int, Inventory> y)
            {
                return x.Value.Item.ID.CompareTo(y.Value.Item.ID);
            });
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Sorted by type");
        }
        else if (sorting == 2)
        {
            temp.Sort(delegate (KeyValuePair<int, Inventory> x, KeyValuePair<int, Inventory> y)
            {
                return string.Compare(x.Value.Item.Title, y.Value.Item.Title, System.StringComparison.Ordinal);
            });
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Sorted by name");
        }
        ClearSlots();
        items.Clear();
        for (int i = 0; i < temp.Count; i++)
        {
            temp[i].Value.SlotNum = i;
            items.Add(temp[i].Value.Item.ID, temp[i].Value);
            AddItemToSlots(temp[i].Value);
        }
        sorting++;
    }

    void CreateNewItem(Items item, int count)
    {
        Inventory newItem;
        newItem = new Inventory(item, count, slotAmount);
        items.Add(newItem.Item.ID, newItem);
        AddItemToSlots(newItem);
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
        itemObject.GetComponentInChildren<ItemData>().SetLocation(location);
        itemObject.GetComponentInChildren<Image>().sprite = item.Item.Sprite;
        GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeSlotColor(itemObject.transform.parent.gameObject, item.Item.ID);
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
        slots.Add(Instantiate(slotPrefab));
        slotAmount++;
        slots[slotAmount - 1].GetComponent<ItemSlot>().id = slotAmount - 1;
        slots[slotAmount - 1].name = "Slot" + (slotAmount - 1);
        slots[slotAmount - 1].transform.SetParent(slotPanel.transform, false);
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
            items[slots[i].GetComponentInChildren<ItemData>().GetItem().Item.ID].SlotNum = i;
        }
        Destroy(currentSlot);
        ResizeSlotPanel();
    }

    void ResizeSlotPanel()
    {
        slotPanelRectTransform.Translate(0, (slotAmount * -35), 0);
        slotPanelRectTransform.sizeDelta = new Vector2(407.4f, (slotAmount * 70));
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

    public bool IsWeapon(int id)
    {
        if (id >= 10000)
        {
            return true;
        }
        return false;
    }

    void InitalizeSlots(List<Inventory> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Inventory loadedItem;
            loadedItem = new Inventory(list[i].Item, list[i].Count, i);
            items.Add(loadedItem.Item.ID, loadedItem);
            AddItemToSlots(loadedItem);
        }
    }

    public void UpdateSlotText(int slotID, int itemID)
    {
        if (items[itemID].Count > 1)
        {
            slots[slotID].GetComponentInChildren<Text>().text = items[itemID].Item.Title + " x" + items[itemID].Count;
        }
        else
        {
            slots[slotID].GetComponentInChildren<Text>().text = items[itemID].Item.Title;
        }
    }

    public void OpenUI()
    {
        if (SceneManager.GetActiveScene().name == "LootScene")
        {
            gameObject.SetActive(true);
            GameMaster.gameMaster.GetComponent<InventoryManager>().OpenInventoryPanelUI();
        }
        else if (SceneManager.GetActiveScene().name == "FightScene")
        {
            if (!GameObject.Find("FightController").GetComponent<FightSceneController>().IsFighting() &&
                !GameObject.Find("FightController").GetComponent<FightSceneController>().IsPickingOption())
            {
                gameObject.SetActive(true);
                GameMaster.gameMaster.GetComponent<InventoryManager>().OpenInventoryPanelUI();
            }
        }
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }
}
