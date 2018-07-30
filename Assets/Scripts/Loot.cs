using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loot : MonoBehaviour
{
    GameObject gameMaster;
    List<Inventory> chestItems = new List<Inventory>();

    int slotAmount;
    public List<GameObject> slots = new List<GameObject>();

    GameObject chestPanel;
    GameObject slotPanel;
    GameObject slot;
    GameObject item;

    void Awake()
    {
        Button open = transform.Find("ChestButton").gameObject.GetComponent<Button>();
        open.onClick.AddListener(OpenChestUI);
        Button collectAll = transform.Find("ChestPanel/CollectAll").gameObject.GetComponent<Button>();
        collectAll.onClick.AddListener(CollectAllLoot);
    }

    public void RunRandom(GameObject slotPrefab, GameObject itemPrefab)
    {
        gameObject.SetActive(true);
        slot = slotPrefab;
        item = itemPrefab;
        chestPanel = transform.Find("ChestPanel").gameObject;
        slotPanel = transform.Find("ChestPanel/SlotPanel").gameObject;
        gameMaster = GameObject.FindGameObjectWithTag("GameController");
        chestItems = gameMaster.GetComponent<ItemDatabase>().GetRandomItemsForChest();
        slotAmount = chestItems.Count;
        for (int i = 0; i < slotAmount; i++)
        {
            slots.Add(Instantiate(slot));
            slots[i].transform.SetParent(slotPanel.transform);
        }
        AddListOfItemsToChest();
    }

    public void AddListOfItemsToChest()
    {
        for (int i = 0; i < chestItems.Count; i++)
        {
            GameObject itemObject = Instantiate(item);
            itemObject.transform.SetParent(slots[i].transform);
            itemObject.transform.localPosition = Vector2.zero;
            itemObject.name = chestItems[i].Item.Title;
            itemObject.GetComponent<ItemData>().SetItem(chestItems[i]);
            itemObject.GetComponent<ItemData>().SetLocation(Location.WhereAmI.temp);
            if (chestItems[i].Count > 1)
            {
                itemObject.GetComponent<Text>().text = chestItems[i].Item.Title + " x" + chestItems[i].Count;
            }
            else
            {
                itemObject.GetComponent<Text>().text = chestItems[i].Item.Title;
            }
        }
    }

    public void CollectAllLoot()
    {
        for (int i = 0; i < chestItems.Count; i++)
        {
            int count = chestItems[i].Count;
            for (int j = 0; j < count; j++)
            {
                //slots[i].GetComponentInChildren<ItemData>().RemoveOneItemFromChest();
            }
        }
    }

    public void OpenChestUI()
    {
        chestPanel.SetActive(true);
        gameMaster.GetComponent<InventoryManager>().OpenInventoryPanelUI();
    }

    public void CloseChestUI()
    {
        chestPanel.SetActive(false);
    }
}
