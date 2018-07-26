using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler {

    public int id;
    GameObject gameMaster;

    void Start()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameController");
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();
        if (gameMaster.GetComponent<InventoryManager>().playerItems[id].Item.ID == -1)
        {
            //This is for empty slots.
        }
        else if (droppedItem && !droppedItem.itemCameFromLoot)
        {
            Transform oldWeapon = this.transform.GetChild(0);
            oldWeapon.GetComponent<ItemData>().slotID = droppedItem.slotID;
            oldWeapon.transform.SetParent(gameMaster.GetComponent<InventoryManager>().slots[droppedItem.slotID].transform);
            oldWeapon.transform.position = gameMaster.GetComponent<InventoryManager>().slots[droppedItem.slotID].transform.position;

            droppedItem.slotID = id;
            droppedItem.transform.SetParent(this.transform);
            droppedItem.transform.position = this.transform.position;
            
        }
    }

}
