using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CraftingMenu : MonoBehaviour
{
    public List<CraftableItem> items = new List<CraftableItem>();
    List<GameObject> slots = new List<GameObject>();
    int slotAmount;

    public GameObject slotPanel;
    public GameObject itemPrefab;
    public GameObject slotPrefab;
    public RectTransform slotPanelRectTransform;
    public Location.VillageMenu currentLocation;

    GameObject gameMaster;

    void Start()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameController");
        transform.SetParent(gameMaster.transform.Find("Canvas").transform, true);
        transform.SetSiblingIndex(1);
    }

    public void CreateNewItem(CraftableItem item, bool isWeapon)
    {
        items.Add(item);
        if (isWeapon)
        {
            AddItemToSlots(item, item.Weapon.Title, true);
        }
        else
        {
            AddItemToSlots(item, GameMaster.gameMaster.GetComponent<ItemDatabase>().FetchItemByID(item.CraftedItemID).Title, false);
        }
    }

    void AddItemToSlots(CraftableItem item, string itemName, bool isWeapon)
    {
        GameObject itemObject = Instantiate(itemPrefab);
        AddDynamicSlot();
        itemObject.transform.SetParent(slots[slotAmount - 1].transform, false);
        itemObject.transform.localPosition = Vector2.zero;
        itemObject.name = itemName;
        Destroy(itemObject.GetComponent<ItemData>());
        Destroy(itemObject.GetComponentInChildren<ItemSlot>());
        itemObject.AddComponent<CraftableItemData>();
        itemObject.GetComponentInChildren<CraftableItemData>().slotID = slotAmount - 1;
        itemObject.GetComponentInChildren<CraftableItemData>().SetItem(item);
        itemObject.GetComponent<Text>().text = itemName;
        if (isWeapon)
        {
            itemObject.GetComponentInChildren<Image>().sprite = item.Weapon.Sprite;
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeSlotColor(itemObject.transform.parent.gameObject, 10000);
        }
        else
        {
            itemObject.GetComponentInChildren<Image>().sprite = GameMaster.gameMaster.GetComponent<ItemDatabase>().FetchItemByID(item.CraftedItemID).Sprite;
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeSlotColor(itemObject.transform.parent.gameObject, item.CraftedItemID);
        }
        ResizeSlotPanel();
    }

    void AddDynamicSlot()
    {
        slots.Add(Instantiate(slotPrefab));
        slotAmount++;
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

    public void OpenUI()
    {
        gameObject.SetActive(true);
        GameMaster.gameMaster.GetComponent<InventoryManager>().OpenInventoryPanelUI();
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
        VillageSceneController.villageScene.GetComponent<VillageSceneController>().ChangeCurrentMenu();
    }

    public void DestroyMenu()
    {
        Destroy(gameObject);
    }
}
