using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    /*
     * For swapping slots
     */

    public int id;
    GameObject gameMaster;
    GameObject villageSceneController;

    public void OnDrop(PointerEventData eventData)
    {
        ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();
        if (droppedItem && transform.childCount > 0)
        {
            if (droppedItem.GetComponent<ItemData>().GetCurrentLocation() == Location.WhereAmI.player &&
                droppedItem.GetComponent<ItemData>().GetGoingToLocation() == Location.WhereAmI.player)
            {
                if (gameMaster == null)
                {
                    gameMaster = GameObject.FindGameObjectWithTag("GameController");
                }
                Transform oldWeapon = this.transform.GetChild(0);
                if (oldWeapon.GetComponent<ItemData>().GetItem() != null)
                {
                    int temp = oldWeapon.GetComponent<ItemData>().GetItem().SlotNum;
                    GameMaster.gameMaster.GetComponent<InventoryManager>().playerItems[oldWeapon.GetComponent<ItemData>().GetItem().Item.ID].SlotNum = droppedItem.GetComponent<ItemData>().GetItem().SlotNum;
                    GameMaster.gameMaster.GetComponent<InventoryManager>().playerItems[droppedItem.GetComponent<ItemData>().GetItem().Item.ID].SlotNum = temp;
                    GameMaster.gameMaster.GetComponent<InventoryManager>().SaveInventory("Player Item");
                }
                oldWeapon.GetComponent<ItemData>().slotID = droppedItem.slotID;
                oldWeapon.transform.SetParent(gameMaster.GetComponent<InventoryManager>().slots[droppedItem.slotID].transform);
                oldWeapon.transform.position = gameMaster.GetComponent<InventoryManager>().slots[droppedItem.slotID].transform.position;
                droppedItem.slotID = id;
                droppedItem.transform.SetParent(this.transform);
                droppedItem.transform.position = this.transform.position;
                GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeSlotColor(oldWeapon.transform.parent.gameObject, oldWeapon.GetComponent<ItemData>().GetItem().Item.ID);
                GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeSlotColor(droppedItem.transform.parent.gameObject, droppedItem.GetComponent<ItemData>().GetItem().Item.ID);
            }
            else if (droppedItem.GetComponent<ItemData>().GetCurrentLocation() == Location.WhereAmI.village &&
                     droppedItem.GetComponent<ItemData>().GetGoingToLocation() == Location.WhereAmI.village)
            {
                villageSceneController = GameObject.FindGameObjectWithTag("VillageSceneManager");
                Transform oldWeapon = this.transform.GetChild(0);
                if (oldWeapon.GetComponent<ItemData>().GetItem() != null)
                {
                    int temp = oldWeapon.GetComponent<ItemData>().GetItem().SlotNum;
                    villageSceneController.GetComponent<VillageInventoryManager>().villageItems[oldWeapon.GetComponent<ItemData>().GetItem().Item.ID].SlotNum = droppedItem.GetComponent<ItemData>().GetItem().SlotNum;
                    villageSceneController.GetComponent<VillageInventoryManager>().villageItems[droppedItem.GetComponent<ItemData>().GetItem().Item.ID].SlotNum = temp;
                    villageSceneController.GetComponent<VillageInventoryManager>().SaveVillageInventory();
                }
                oldWeapon.GetComponent<ItemData>().slotID = droppedItem.slotID;
                oldWeapon.transform.SetParent(villageSceneController.GetComponent<VillageInventoryManager>().slots[droppedItem.slotID].transform);
                oldWeapon.transform.position = villageSceneController.GetComponent<VillageInventoryManager>().slots[droppedItem.slotID].transform.position;
                droppedItem.slotID = id;
                droppedItem.transform.SetParent(this.transform);
                droppedItem.transform.position = this.transform.position;
                GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeSlotColor(oldWeapon.transform.parent.gameObject, oldWeapon.GetComponent<ItemData>().GetItem().Item.ID);
                GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeSlotColor(droppedItem.transform.parent.gameObject, droppedItem.GetComponent<ItemData>().GetItem().Item.ID);
            }
            else if (droppedItem.GetComponent<ItemData>().GetCurrentLocation() == Location.WhereAmI.temp &&
                     droppedItem.GetComponent<ItemData>().GetGoingToLocation() == Location.WhereAmI.temp)
            {
                Transform oldWeapon = this.transform.GetChild(0);
                GameObject panel = oldWeapon.transform.parent.parent.parent.parent.gameObject;
                oldWeapon.GetComponent<ItemData>().slotID = droppedItem.slotID;
                if (oldWeapon.GetComponent<ItemData>().GetItem() != null)
                {
                    int temp = oldWeapon.GetComponent<ItemData>().GetItem().SlotNum;
                    panel.GetComponent<DynamicInventory>().items[oldWeapon.GetComponent<ItemData>().GetItem().Item.ID].SlotNum = droppedItem.GetComponent<ItemData>().GetItem().SlotNum;
                    panel.GetComponent<DynamicInventory>().items[droppedItem.GetComponent<ItemData>().GetItem().Item.ID].SlotNum = temp;
                }
                oldWeapon.transform.SetParent(panel.GetComponent<DynamicInventory>().slots[droppedItem.slotID].transform);
                oldWeapon.transform.position = panel.GetComponent<DynamicInventory>().slots[droppedItem.slotID].transform.position;
                droppedItem.slotID = id;
                droppedItem.transform.SetParent(this.transform);
                droppedItem.transform.position = this.transform.position;
                GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeSlotColor(oldWeapon.transform.parent.gameObject, oldWeapon.GetComponent<ItemData>().GetItem().Item.ID);
                GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeSlotColor(droppedItem.transform.parent.gameObject, droppedItem.GetComponent<ItemData>().GetItem().Item.ID);
            }
        }
    }
}
