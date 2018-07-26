using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemData : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    //This script will contain the data of each individual item so that when we drag and drop, the system will know what this item containts.
    //It will be included in the prefrab of the blank, generic item that we'll use to interface items from the DB into the actual game. It'll make more
    //sense once the inventory  drag and drop functionalitiy is implemented.


    public int amount;
    public int slotID;
    public bool itemCameFromLoot;
    Inventory item;

    GameObject gameMaster;
    GameObject currentSlot;

    //public int thisItemsID;
    private Vector2 offsetToReturnItem;

    void Start()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameController");
    }

    public Inventory GetItem()
    {
        return item;
    }

    public void SetItem(Inventory itemToBeSet)
    {
        item = itemToBeSet;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gameMaster != null && !itemCameFromLoot)
        {
            currentSlot = transform.parent.gameObject;
            offsetToReturnItem = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
            this.transform.SetParent(this.transform.parent.parent);
            this.transform.position = eventData.position - offsetToReturnItem;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!itemCameFromLoot)
        {
            this.transform.position = eventData.position - offsetToReturnItem;

        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!itemCameFromLoot)
        {
            this.transform.SetParent(gameMaster.GetComponent<InventoryManager>().slots[slotID].transform);
            this.transform.position = gameMaster.GetComponent<InventoryManager>().slots[slotID].transform.position;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        gameMaster.GetComponent<InventoryManager>().trash.GetComponent<OverUI>().isOver = false;
        gameMaster.GetComponent<InventoryManager>().removeAll.GetComponent<OverUI>().isOver = false;
        if (!itemCameFromLoot)
        {
            gameMaster.GetComponent<InventoryManager>().trash.gameObject.SetActive(true);
            gameMaster.GetComponent<InventoryManager>().removeAll.gameObject.SetActive(true);
        }
        if (itemCameFromLoot)
        {
            RemoveOneItem();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!itemCameFromLoot && gameMaster.GetComponent<InventoryManager>().trash.GetComponent<OverUI>().isOver)
        {
            gameMaster.GetComponent<InventoryManager>().RemoveItemFromInventory(item);
            if (item.Count == 1)
            {
                GetComponentInParent<Text>().text = item.Item.Title;
            }
            else if (item.Count > 0)
            {
                GetComponentInParent<Text>().text = item.Item.Title + " x" + item.Count;
            }
            else
            {
                gameMaster.GetComponent<InventoryManager>().ReorganizeSlots(currentSlot);
                Destroy(gameObject);
            }
        }

        //FOR REMOVE ALL

        if (!itemCameFromLoot && gameMaster.GetComponent<InventoryManager>().removeAll.GetComponent<OverUI>().isOver)
        {
            gameMaster.GetComponent<InventoryManager>().RemoveWholeStackFromInventory(item);
            gameMaster.GetComponent<InventoryManager>().ReorganizeSlots(currentSlot);
            Destroy(gameObject);
        }

        gameMaster.GetComponent<InventoryManager>().removeAll.gameObject.SetActive(false);
        gameMaster.GetComponent<InventoryManager>().trash.gameObject.SetActive(false);
    }

    public void RemoveOneItem()
    {
        //TODO: See if player has space to receive item. If they do, delete this game object. If not, trigger a warning that there's not enough space.
        if (gameMaster.GetComponent<InventoryManager>().CanFitInInventory(item.Item.Size))
        {
            item.Count--;
            gameMaster.GetComponent<InventoryManager>().AddItemToInventory(item.Item);
            //gameMaster.GetComponent<InventoryManager>().PrintInventory(); //TODO: Remove this once done testing.
            if (item.Count == 1)
            {
                GetComponentInParent<Text>().text = item.Item.Title;
            }
            else if (item.Count > 0)
            {
                GetComponentInParent<Text>().text = item.Item.Title + " x" + item.Count;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            
        }
    }
}
