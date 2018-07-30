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
        if (gameMaster != null /*&& currentLocation != Location.WhereAmI.chest*/)
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
        this.transform.position = eventData.position - offsetToReturnItem;
        beingDragged = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentLocation == Location.WhereAmI.player && goingToLocation == Location.WhereAmI.player)
        {
            this.transform.SetParent(gameMaster.GetComponent<InventoryManager>().slots[slotID].transform);
            this.transform.position = gameMaster.GetComponent<InventoryManager>().slots[slotID].transform.position;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else if (currentLocation == Location.WhereAmI.player && goingToLocation == Location.WhereAmI.village)
        {
            if (item.Count > 0)
            {
                this.transform.SetParent(currentSlot.transform);
                this.transform.position = currentSlot.transform.position;
                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
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
        else if (currentLocation == Location.WhereAmI.temp && goingToLocation == Location.WhereAmI.player)
        {
            if (item.Count > 0)
            {
                this.transform.SetParent(currentSlot.transform);
                this.transform.position = currentSlot.transform.position;
                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }
        else if (currentLocation == Location.WhereAmI.player && goingToLocation == Location.WhereAmI.temp)
        {
            if (item.Count > 0)
            {
                this.transform.SetParent(currentSlot.transform);
                this.transform.position = currentSlot.transform.position;
                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }
        else if (currentLocation == Location.WhereAmI.temp && goingToLocation == Location.WhereAmI.temp)
        {
            GameObject panel;
            if (transform.parent.parent.parent.gameObject.name == "Manager")
            {
                panel = transform.parent.gameObject;
            }
            else
            {
                panel = transform.parent.parent.parent.parent.gameObject;
            }
            this.transform.SetParent(panel.GetComponent<DynamicInventory>().slots[slotID].transform);
            this.transform.position = panel.GetComponent<DynamicInventory>().slots[slotID].transform.position;
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
        sceneName = SceneManager.GetActiveScene().name;
        this.transform.root.GetComponentInChildren<Canvas>().sortingOrder = 2;
        beingDragged = false;
        goingToLocation = Location.WhereAmI.notSet;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.transform.root.GetComponentInChildren<Canvas>().sortingOrder = 1;
        if (!beingDragged && (currentLocation == Location.WhereAmI.player || currentLocation == Location.WhereAmI.village || currentLocation == Location.WhereAmI.temp))
        {
            gameMaster.GetComponentInChildren<Canvas>().sortingOrder = 3;
            gameMaster.GetComponent<ItemPopUp>().ShowItemPopUp(item, slotID, gameObject, currentLocation);
        }
        if (sceneName == "VillageScene")
        {
            OnPointerUpVillage(eventData);
        }
        if (sceneName == "BrandonTest" || sceneName == "LootScene")
        {
            OnPointerUpLootScene(eventData);
        }
        if (sceneName == "PathScene")
        {
            goingToLocation = Location.WhereAmI.player;
        }
    }

    void OnPointerUpVillage(PointerEventData eventData)
    {
        if (eventData.position.x > Screen.width / 2 && beingDragged)
        {
            goingToLocation = Location.WhereAmI.village;
            if (currentLocation == Location.WhereAmI.player)
            {
                villageSceneController = GameObject.FindGameObjectWithTag("VillageSceneManager");
                villageSceneController.GetComponent<VillageInventoryManager>().MoveItemsToVillageInventory(item, slotID, 1);
                CheckCount();
            }
        }
        if (eventData.position.x < Screen.width / 2)
        {
            goingToLocation = Location.WhereAmI.player;
            if (currentLocation == Location.WhereAmI.village)
            {
                gameMaster.GetComponent<InventoryManager>().MoveItemsToPlayerInventory(item, slotID, 1, true, null);
                CheckCount();
            }
        }
    }

    void OnPointerUpLootScene(PointerEventData eventData)
    {
        if (eventData.position.x > Screen.width / 2)
        {
            goingToLocation = Location.WhereAmI.temp;
            if (currentLocation == Location.WhereAmI.player)
            {
                GameObject panel = GameObject.Find("Manager");
                panel = panel.transform.GetChild(0).gameObject;
                for (int i = 0; i < panel.transform.childCount; i++)
                {
                    if (panel.transform.GetChild(i).gameObject.activeInHierarchy == true && panel.transform.GetChild(i).gameObject.name == "InventoryPanel(Clone)")
                    {
                        panel = panel.transform.GetChild(i).gameObject;
                        panel.GetComponent<DynamicInventory>().MoveItemsToHere(item, slotID, 1);
                        break;
                    }
                }
                CheckCount();
            }
        }
        else if (eventData.position.x < Screen.width / 2)
        {
            goingToLocation = Location.WhereAmI.player;
            if (currentLocation == Location.WhereAmI.temp)
            {
                GameObject panel = transform.parent.gameObject;
                gameMaster.GetComponent<InventoryManager>().MoveItemsToPlayerInventory(item, slotID, 1, false, panel);
                CheckCount();
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
