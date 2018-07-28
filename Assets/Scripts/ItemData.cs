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
    Inventory item;

    GameObject gameMaster;
    GameObject villageSceneController;
    GameObject currentSlot;

    Scene currentScene;
    string sceneName;

    private Vector2 offsetToReturnItem;
    bool beingDragged = false;

    Location.WhereAmI currentLocation;
    Location.WhereAmI goingToLocation;

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

    public void SetItem(Inventory itemToBeSet)
    {
        item = itemToBeSet;
    }

    public void SetLocation(Location.WhereAmI location)
    {
        currentLocation = location;
    }

    public Location.WhereAmI GetCurrentLocation()
    {
        return currentLocation;
    }

    public Location.WhereAmI GetGoingToLocation()
    {
        return goingToLocation;
    }

    public Inventory GetItem()
    {
        return item;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gameMaster != null && currentLocation != Location.WhereAmI.chest)
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
        this.transform.root.GetComponentInChildren<Canvas>().sortingOrder = 2;

        if (currentLocation != Location.WhereAmI.chest)
        {
            this.transform.position = eventData.position - offsetToReturnItem;
            beingDragged = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentLocation == Location.WhereAmI.player && goingToLocation == Location.WhereAmI.village)
        {
            if (item.Count > 0)
            {
                this.transform.SetParent(currentSlot.transform);
                this.transform.position = currentSlot.transform.position;
                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }
        else if (currentLocation == Location.WhereAmI.player && goingToLocation == Location.WhereAmI.player)
        {
            this.transform.SetParent(gameMaster.GetComponent<InventoryManager>().slots[slotID].transform);
            this.transform.position = gameMaster.GetComponent<InventoryManager>().slots[slotID].transform.position;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else if (currentLocation == Location.WhereAmI.village && goingToLocation == Location.WhereAmI.player)
        {
            if (item.Count > 0)
            {
                this.transform.SetParent(currentSlot.transform);
                this.transform.position = currentSlot.transform.position;
                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }
        else if (currentLocation == Location.WhereAmI.village && goingToLocation == Location.WhereAmI.village)
        {
            this.transform.SetParent(villageSceneController.GetComponent<VillageInventoryManager>().slots[slotID].transform);
            this.transform.position = villageSceneController.GetComponent<VillageInventoryManager>().slots[slotID].transform.position;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else if (goingToLocation == Location.WhereAmI.notSet)
        {
            this.transform.SetParent(currentSlot.transform);
            this.transform.position = currentSlot.transform.position;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.transform.root.GetComponentInChildren<Canvas>().sortingOrder = 2;
        beingDragged = false;
        goingToLocation = Location.WhereAmI.notSet;
        gameMaster.GetComponent<InventoryManager>().inventoryPane.GetComponent<OverUI>().isOver = false;
        if (sceneName == "VillageScene")
        {
            villageSceneController = GameObject.FindGameObjectWithTag("VillageSceneManager");
        }
        if (villageSceneController != null)
        {
            villageSceneController.GetComponent<VillageInventoryManager>().addItemsToVillageInventory.GetComponent<OverUI>().isOver = false;
        }
        if (currentLocation == Location.WhereAmI.chest)
        {
            RemoveOneItemFromChest();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.transform.root.GetComponentInChildren<Canvas>().sortingOrder = 1;
        if (!beingDragged && (currentLocation == Location.WhereAmI.player || currentLocation == Location.WhereAmI.village))
        {
            gameMaster.GetComponentInChildren<Canvas>().sortingOrder = 3;
            gameMaster.GetComponent<ItemPopUp>().ShowItemPopUp(item, slotID, gameObject, currentLocation);
        }
        if (villageSceneController != null && beingDragged)
        {
            if (villageSceneController.GetComponent<VillageInventoryManager>().addItemsToVillageInventory.GetComponent<OverUI>().isOver)
            {
                goingToLocation = Location.WhereAmI.village;
                if (currentLocation == Location.WhereAmI.player)
                {
                    villageSceneController.GetComponent<VillageInventoryManager>().MoveItemsToVillageInventory(item, slotID, 1);
                    CheckCount();
                }
            }
        }
        if (gameMaster.GetComponent<InventoryManager>().inventoryPane.GetComponent<OverUI>().isOver)
        {
            goingToLocation = Location.WhereAmI.player;
            if (currentLocation == Location.WhereAmI.village)
            {
                gameMaster.GetComponent<InventoryManager>().MoveItemsToPlayerInventory(item, slotID, 1);
                CheckCount();
            }
        }
    }

    public void RemoveOneItemFromChest()
    {
        if (currentLocation == Location.WhereAmI.chest)
        {
            //TODO: See if player has space to receive item. If they do, delete this game object. If not, trigger a warning that there's not enough space.
            if (gameMaster.GetComponent<InventoryManager>().CanFitInInventory(item.Item.Size))
            {
                item.Count--;
                gameMaster.GetComponent<InventoryManager>().AddItemToInventory(item.Item);
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
        }
    }

    void CheckCount()
    {
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
}
