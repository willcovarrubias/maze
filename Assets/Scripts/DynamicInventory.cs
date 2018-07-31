using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    ScrollRect scrollView;
    public GameObject openButton;
    GameObject sendAllButton;

    GameObject gameMaster;
    Location.WhereAmI location;

    void Start()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameController");
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
        slotPanelRectTransform = slotPanel.GetComponent<RectTransform>();
        scrollView = inventoryPane.GetComponent<ScrollRect>();
        Button open = button.GetComponent<Button>();
        open.onClick.AddListener(OpenUI);
        Button sendAll = sendAllButton.GetComponent<Button>();
        sendAll.onClick.AddListener(MoveWholeInventoryToPlayer);
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

    void CreateNewItem(Items item, int count)
    {
        Inventory newItem;
        if (IsWeapon(item.ID))
        {
            Weapons weapon = GetComponent<WeaponDatabase>().FetchWeaponByID(item.ID);
            weapon.Num = Random.Range(0, 10000);
            newItem = new Inventory(weapon, count, slotAmount);
        }
        else
        {
            newItem = new Inventory(item, count, slotAmount);
        }
        items.Add(newItem.Item.ID, newItem);
        AddItemToSlots(newItem);
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
        itemObject.GetComponentInChildren<ItemData>().SetLocation(location);
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

    void ResizeSlotPanel()
    {
        slotPanelRectTransform.Translate(0, (slotAmount * -35), 0);
        slotPanelRectTransform.sizeDelta = new Vector2(407.4f, (slotAmount * 70));
    }

    public bool IsWeapon(int id)
    {
        if (id >= 2000 && id < 3000)
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
            if (IsWeapon(list[i].Item.ID))
            {
                Weapons weapon = GameMaster.gameMaster.GetComponent<WeaponDatabase>().FetchWeaponByID(list[i].Item.ID);
                weapon.Num = Random.Range(0, 10000);
                loadedItem = new Inventory(weapon, 1, i);
            }
            else
            {
                loadedItem = new Inventory(list[i].Item, list[i].Count, i);
            }
            items.Add(loadedItem.Item.ID, loadedItem);
            AddItemToSlots(loadedItem);
        }
    }

    public void OpenUI()
    {
        gameObject.SetActive(true);
        GameMaster.gameMaster.GetComponent<InventoryManager>().OpenInventoryPanelUI();
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }
}
