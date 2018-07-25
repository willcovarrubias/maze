﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler {

    public int id;
    GameObject gameMaster;

    void Start()
    {
        //For the sake of testing, I created a tag in the Editor called WeaponsManager. Probably should find a better way to reference later.
        //wm = GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<WeaponManager>();
        gameMaster = GameObject.FindGameObjectWithTag("GameController");
    }
    
    public void OnDrop(PointerEventData eventData)
    {

        //*******Old working code when I used WeaponManager********
        //Keeping this for reference.

        /*
        ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();
        if (wm.weapons[id].ID == -1)
        {
            wm.weapons[droppedItem.slotID] = new Weapons();
            wm.weapons[id] = droppedItem.weapons;
            droppedItem.slotID = id;
        }
        else
        {
            Transform oldWeapon = this.transform.GetChild(0);
            oldWeapon.GetComponent<ItemData>().slotID = droppedItem.slotID;
            oldWeapon.transform.SetParent(wm.slots[droppedItem.slotID].transform);
            oldWeapon.transform.position = wm.slots[droppedItem.slotID].transform.position;

            droppedItem.slotID = id;
            droppedItem.transform.SetParent(this.transform);
            droppedItem.transform.position = this.transform.position;

            wm.weapons[droppedItem.slotID] = oldWeapon.GetComponent<ItemData>().weapons;
            wm.weapons[id] = droppedItem.weapons;
        }*/


        
        //New, NOT working code when I'm trying to use Inventory Manager;
        ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();
        if (gameMaster.GetComponent<InventoryManager>().playerItems[id].Item.ID == -1)
        {
            //This is for empty slots.
        }
        else if (!droppedItem.itemCameFromLoot)
        {
            //ALSO STUCK HERE!!

            Transform oldWeapon = this.transform.GetChild(0);
            oldWeapon.GetComponent<ItemData>().slotID = droppedItem.slotID;
            oldWeapon.transform.SetParent(gameMaster.GetComponent<InventoryManager>().slots[droppedItem.slotID].transform);
            oldWeapon.transform.position = gameMaster.GetComponent<InventoryManager>().slots[droppedItem.slotID].transform.position;

            droppedItem.slotID = id;
            droppedItem.transform.SetParent(this.transform);
            droppedItem.transform.position = this.transform.position;
            Debug.Log("This is End DROP");

            //gameMaster.GetComponent<InventoryManager>().playerItems[droppedItem.slotID] = oldWeapon.GetComponent<ItemData>().playerItems;
            //gameMaster.GetComponent<InventoryManager>().playerItems[id] = droppedItem.slotID;
        }
    }

}
