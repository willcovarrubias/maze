using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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
    GameObject villageSceneController;
    GameObject currentSlot;

    Scene currentScene;
    string sceneName;

    private Vector2 offsetToReturnItem;
    bool beingDragged = false;

    void Start()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameController");

        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;

        if (sceneName == "VillageScene")
        {
            villageSceneController = GameObject.FindGameObjectWithTag("VillageSceneManager");
        }
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
            this.transform.SetParent(this.transform.parent.parent.parent.parent);
            this.transform.position = eventData.position - offsetToReturnItem;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!itemCameFromLoot)
        {
            this.transform.position = eventData.position - offsetToReturnItem;
            beingDragged = true;
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
        beingDragged = false;
        gameMaster.GetComponent<InventoryManager>().trash.GetComponent<OverUI>().isOver = false; gameMaster.GetComponent<InventoryManager>().removeAll.GetComponent<OverUI>().isOver = false;
        if (itemCameFromLoot)
        {
            RemoveOneItem();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!beingDragged && !itemCameFromLoot)
        {
            gameMaster.GetComponent<ItemPopUp>().ShowItemPopUp(item, slotID, gameObject);
        }
        //Testing for dumping items into VillageInventory. Super messy and inefficient!
        if (!itemCameFromLoot && villageSceneController.GetComponent<VillageInventoryManager>().addItemsToVillageInventory.GetComponent<OverUI>().isOver)
        {
            Debug.Log("Added items to Village Inventory!!!");
            //Repeated code. TODO: Clean this up and make it for efficient.


            AddThisItemToVillageInventory();

        }
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

    public void AddThisItemToVillageInventory()
    {
        //TODO: See if player has space to receive item. If they do, delete this game object. If not, trigger a warning that there's not enough space.
        if (villageSceneController.GetComponent<VillageInventoryManager>().CanFitInInventory(item.Item.Size))
        {
            item.Count--;
            villageSceneController.GetComponent<VillageInventoryManager>().AddItemToVillageInventory(item.Item);
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
