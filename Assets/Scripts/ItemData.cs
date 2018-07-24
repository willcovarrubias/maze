using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler {

    //This script will contain the data of each individual item so that when we drag and drop, the system will know what this item containts.
    //It will be included in the prefrab of the blank, generic item that we'll use to interface items from the DB into the actual game. It'll make more
    //sense once the inventory  drag and drop functionalitiy is implemented.

    //public Weapons weapons;
    
    public int amount;
    public int slotID;
    public bool itemCameFromLoot;

    GameObject gameMaster;

    private int thisWeaponsID;
    private Vector2 offsetToReturnItem;

    void Start()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameController");
        //inventoryManager = GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<WeaponManager>();
        IdentifyThisWeapon();
    }
        
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (weapons != null && !itemCameFromLoot)
        {
            offsetToReturnItem = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
            this.transform.SetParent(this.transform.parent.parent);
            this.transform.position = eventData.position - offsetToReturnItem;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        else
        {
            Debug.Log("Unable to find Weapons DB!");
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
            this.transform.SetParent(inventoryManager.slots[slotID].transform);
            this.transform.position = inventoryManager.slots[slotID].transform.position;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Is this being called?");
        if (itemCameFromLoot)
        {           
            //Add item to player's inventory if possible.
            //gameMaster.GetComponent<InventoryManager>().AddItemToInventory(thisWeaponsID);
        }
    }

    public void IdentifyThisWeapon()
    {
        
        //thisWeaponsID = gameMaster.GetComponent<WeaponDatabase>().FetchWeaponByID()
    }
}
