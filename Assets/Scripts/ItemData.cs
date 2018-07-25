using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemData : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {

    //This script will contain the data of each individual item so that when we drag and drop, the system will know what this item containts.
    //It will be included in the prefrab of the blank, generic item that we'll use to interface items from the DB into the actual game. It'll make more
    //sense once the inventory  drag and drop functionalitiy is implemented.
       
    public int amount;
    public int slotID;
    public bool itemCameFromLoot;

    GameObject gameMaster;

    public int thisItemsID;
    private Vector2 offsetToReturnItem;

    void Start()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameController");
    }
        
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gameMaster != null && !itemCameFromLoot)
        {
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
        if (itemCameFromLoot)
        {
            //if(gameMaster.GetComponent<InventoryManager>().GetMaxInventorySize().)
            //TODO: See if player has space to receive item. If they do, delete this game object. If not, trigger a warning that there's not enough space.
            gameMaster.GetComponent<InventoryManager>().AddItemToInventory(thisItemsID);
            //gameMaster.GetComponent<InventoryManager>().PrintInventory(); //TODO: Remove this once done testing.
        }
    }
}
