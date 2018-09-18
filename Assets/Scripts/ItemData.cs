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

    public int slotID;
    //int id;
    Inventory item;

    GameObject gameMaster, currentSlot, currentPanel;

    Scene currentScene;
    string sceneName;

    Vector2 offsetToReturnItem;
    bool beingDragged;

    Location.WhereAmI currentLocation, goingToLocation;

    GameObject temp;

    void Start()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameController");
        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
        currentPanel = transform.parent.parent.parent.parent.gameObject;
    }

    public void UnequipItem()
    {
        if (transform.Find("Equipped"))
        {
            Destroy(transform.Find("Equipped").gameObject);
        }
        GameMaster.gameMaster.GetComponent<ActiveCharacterController>().UpdateActiveCharacterVisuals();
    }

    public void EquipItem()
    {
        if (!transform.Find("Equipped"))
        {
            GameObject equippedSprite = Instantiate(GameMaster.gameMaster.GetComponent<InventoryManager>().equippedCheckMark, transform, false);
            equippedSprite.transform.localPosition = new Vector3(-237.5f, 0, 0);
            equippedSprite.name = "Equipped";
        }
        GameMaster.gameMaster.GetComponent<ActiveCharacterController>().UpdateActiveCharacterVisuals();
    }

    public void SetItem(Inventory itemToBeSet)
    {
        //id = itemToBeSet.Item.ID;
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
        if (gameMaster != null && currentLocation != Location.WhereAmI.gems)
        {
            currentSlot = transform.parent.gameObject;
            if (item.Count > 1)
            {
                temp = Instantiate(transform.gameObject, transform.parent, true);
                if (transform.Find("Equipped"))
                {
                    Destroy(transform.Find("Equipped").gameObject);
                }
                if (item.Count > 3)
                {
                    temp.GetComponent<Text>().text = item.Item.Title + " x" + (item.Count - 1);
                }
                else
                {
                    temp.GetComponent<Text>().text = item.Item.Title;
                }
            }
            GetComponent<Text>().text = item.Item.Title;
            offsetToReturnItem = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
            this.transform.SetParent(this.transform.parent.parent.parent.parent.parent);
            this.transform.position = eventData.position - offsetToReturnItem;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentLocation != Location.WhereAmI.gems)
        {
            this.transform.position = eventData.position - offsetToReturnItem;
            beingDragged = true;
        }
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
                if (transform.Find("Equipped"))
                {
                    Destroy(transform.Find("Equipped").gameObject);
                }
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
            this.transform.SetParent(VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().slots[slotID].transform);
            this.transform.position = VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().slots[slotID].transform.position;
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
            this.transform.SetParent(currentPanel.GetComponent<DynamicInventory>().slots[slotID].transform);
            this.transform.position = currentPanel.GetComponent<DynamicInventory>().slots[slotID].transform.position;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else if (goingToLocation == Location.WhereAmI.notSet && currentLocation != Location.WhereAmI.gems)
        {
            this.transform.SetParent(currentSlot.transform);
            this.transform.position = currentSlot.transform.position;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        CheckCount();
        Destroy(temp);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        sceneName = SceneManager.GetActiveScene().name;
        beingDragged = false;
        goingToLocation = Location.WhereAmI.notSet;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!beingDragged && (currentLocation == Location.WhereAmI.player || currentLocation == Location.WhereAmI.village || currentLocation == Location.WhereAmI.temp))
        {
            gameMaster.GetComponent<ItemPopUp>().ShowItemPopUp(item, slotID, gameObject, currentLocation);
        }
        if (!beingDragged && currentLocation == Location.WhereAmI.gems)
        {
            VillageSceneController.villageScene.GetComponent<GemsInventory>().SelectGem(slotID, item.Item);
        }
        if (sceneName == "VillageScene")
        {
            OnPointerUpVillage(eventData);
        }
        if (sceneName == "BrandonTest" || sceneName == "LootScene" || sceneName == "FightScene")
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
        if (VillageSceneController.villageScene.currentMenu == Location.VillageMenu.inventory)
        {
            if (eventData.position.x > Screen.width / 2 && beingDragged)
            {
                goingToLocation = Location.WhereAmI.village;
                if (currentLocation == Location.WhereAmI.player)
                {
                    VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().MoveItemsToVillageInventory(item, slotID, 1);
                    VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().SaveVillageInventory();
                    CheckCount();
                }
            }
            if (eventData.position.x < Screen.width / 2 && beingDragged)
            {
                goingToLocation = Location.WhereAmI.player;
                if (currentLocation == Location.WhereAmI.village)
                {
                    gameMaster.GetComponent<InventoryManager>().MoveItemsToPlayerInventory(item, slotID, 1, true, null);
                    CheckCount();
                }
            }
        }
        if (VillageSceneController.villageScene.currentMenu == Location.VillageMenu.armor ||
            VillageSceneController.villageScene.currentMenu == Location.VillageMenu.pub ||
            VillageSceneController.villageScene.currentMenu == Location.VillageMenu.weapons)
        {
            if (eventData.position.x < Screen.width / 2 && beingDragged)
            {
                goingToLocation = Location.WhereAmI.player;
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
                GameObject panel = GetCurrentDynamicPanel();
                panel.GetComponent<DynamicInventory>().MoveItemsToHere(item, slotID, 1);
                CheckCount();
            }
        }
        else if (eventData.position.x < Screen.width / 2)
        {
            goingToLocation = Location.WhereAmI.player;
            if (currentLocation == Location.WhereAmI.temp)
            {
                GameObject panel = currentPanel;
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

    GameObject GetCurrentDynamicPanel()
    {
        GameObject panel = gameMaster.transform.GetChild(0).gameObject;
        for (int i = 0; i < panel.transform.childCount; i++)
        {
            if (panel.transform.GetChild(i).gameObject.activeInHierarchy == true && panel.transform.GetChild(i).gameObject.name == "InventoryPanel(Clone)")
            {
                return panel.transform.GetChild(i).gameObject;
            }
        }
        return null;
    }
}
