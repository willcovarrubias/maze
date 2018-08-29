using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemsInventory : MonoBehaviour
{
    public Dictionary<int, Inventory> items = new Dictionary<int, Inventory>();
    public List<GameObject> slots = new List<GameObject>();
    int slotAmount;
    public GameObject slotPanel;
    public GameObject slotPrefab;
    public GameObject itemPrefab;
    public GameObject inventoryPane;
    public RectTransform slotPanelRectTransform;
    public ScrollRect scrollView;

    public void InitalizeSlots()
    {
        ClearSlots();
        Dictionary<int, Inventory> list = VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().villageItems;
        foreach (KeyValuePair<int, Inventory> keyValue in list)
        {
            int key = keyValue.Key;
            int id = list[key].Item.ID;
            if (id >= 2000 && id < 3000)
            {
                Inventory loadedItem;
                loadedItem = new Inventory(list[key].Item, list[key].Count, key);
                items.Add(loadedItem.Item.ID, loadedItem);
                AddItemToSlots(loadedItem);
            }
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
        itemObject.GetComponentInChildren<ItemData>().SetLocation(Location.WhereAmI.gems);
        if (item.Count == 1)
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
        items.Clear();
        slotAmount = 0;
        ResizeSlotPanel();
    }
}
