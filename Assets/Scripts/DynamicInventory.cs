using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicInventory : MonoBehaviour
{
    List<Inventory> items = new List<Inventory>();

    public List<GameObject> slots = new List<GameObject>();
    int slotAmount;
    GameObject slotPanel;
    GameObject slotPrefab;
    GameObject itemPrefab;
    GameObject inventoryPane;
    RectTransform slotPanelRectTransform;
    ScrollRect scrollView;
    public GameObject openButton;

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
        Button open = button.GetComponent<Button>();
        open.onClick.AddListener(OpenUI);
        inventoryPane = gameObject.transform.Find("ScrollView").gameObject;
        slotPanel = inventoryPane.transform.Find("SlotPanel").gameObject;
        slotPanelRectTransform = slotPanel.GetComponent<RectTransform>();
        scrollView = inventoryPane.GetComponent<ScrollRect>();
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
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Item.ID == item.Item.ID)
                {
                    items[i].Count += amount;
                    for (int j = 0; j < slots.Count; j++)
                    {
                        if (slots[j].GetComponentInChildren<ItemData>().GetItem().Item.ID == item.Item.ID)
                        {
                            slots[j].GetComponentInChildren<ItemData>().GetItem().Count = items[i].Count;
                            slots[j].GetComponentInChildren<Text>().text = items[i].Item.Title + " x" + items[i].Count;
                            gameMaster.GetComponent<InventoryManager>().RemoveItemsFromInventory(item, amount, thisSlotId);
                            return;
                        }
                    }
                }
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
                items.Remove(item);
                item.Count = 0;
                ReorganizeSlots(slotId);
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Item.ID == item.Item.ID)
                    {
                        items[i].Count -= count;
                        if (items[i].Count <= 0)
                        {
                            items.Remove(item);
                            item.Count = 0;
                            ReorganizeSlots(slotId);
                        }
                        break;
                    }
                }
            }
        }
    }

    public void RemoveWholeStackFromInventory(Inventory item)
    {
        items.Remove(item);
        item.Count = 0;
    }

    void CreateNewItem(Items item, int count)
    {
        Inventory newItem = new Inventory(item, count);
        items.Add(newItem);
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
            Inventory loadedItem = new Inventory(list[i].Item, list[i].Count);
            items.Add(loadedItem);
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
