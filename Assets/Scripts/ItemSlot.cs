using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public int id;
    GameObject gameMaster;
    GameObject villageSceneController;

    void Start()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameController");
        if (SceneManager.GetActiveScene().name == "VillageScene")
        {
            villageSceneController = GameObject.FindGameObjectWithTag("VillageSceneManager");
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();
        if (!droppedItem) // Check if it's in the box.
        {
            //Do nothing
        }
        else if (droppedItem && this.transform.childCount > 0 &&
                 droppedItem.GetComponent<ItemData>().GetCurrentLocation() == Location.WhereAmI.player &&
                 droppedItem.GetComponent<ItemData>().GetGoingToLocation() == Location.WhereAmI.player)
        {
            Transform oldWeapon = this.transform.GetChild(0);
            oldWeapon.GetComponent<ItemData>().slotID = droppedItem.slotID;
            oldWeapon.transform.SetParent(gameMaster.GetComponent<InventoryManager>().slots[droppedItem.slotID].transform);
            oldWeapon.transform.position = gameMaster.GetComponent<InventoryManager>().slots[droppedItem.slotID].transform.position;

            droppedItem.slotID = id;
            droppedItem.transform.SetParent(this.transform);
            droppedItem.transform.position = this.transform.position;
        }
        else if (droppedItem && this.transform.childCount > 0 &&
                 droppedItem.GetComponent<ItemData>().GetCurrentLocation() == Location.WhereAmI.village &&
                 droppedItem.GetComponent<ItemData>().GetGoingToLocation() == Location.WhereAmI.village)
        {
            Transform oldWeapon = this.transform.GetChild(0);
            oldWeapon.GetComponent<ItemData>().slotID = droppedItem.slotID;
            oldWeapon.transform.SetParent(villageSceneController.GetComponent<VillageInventoryManager>().slots[droppedItem.slotID].transform);
            oldWeapon.transform.position = villageSceneController.GetComponent<VillageInventoryManager>().slots[droppedItem.slotID].transform.position;

            droppedItem.slotID = id;
            droppedItem.transform.SetParent(this.transform);
            droppedItem.transform.position = this.transform.position;
        }
        else if (droppedItem && this.transform.childCount > 0 &&
                 droppedItem.GetComponent<ItemData>().GetCurrentLocation() == Location.WhereAmI.temp &&
                 droppedItem.GetComponent<ItemData>().GetGoingToLocation() == Location.WhereAmI.temp)
        {
            Transform oldWeapon = this.transform.GetChild(0);
            oldWeapon.GetComponent<ItemData>().slotID = droppedItem.slotID;
            GameObject panel = oldWeapon.transform.parent.parent.parent.parent.gameObject;
            oldWeapon.transform.SetParent(panel.GetComponent<DynamicInventory>().slots[droppedItem.slotID].transform);
            oldWeapon.transform.position = panel.GetComponent<DynamicInventory>().slots[droppedItem.slotID].transform.position;
            droppedItem.slotID = id;
            droppedItem.transform.SetParent(this.transform);
            droppedItem.transform.position = this.transform.position;
        }
    }
}
