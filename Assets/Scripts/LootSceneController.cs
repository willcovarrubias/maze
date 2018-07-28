using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LootSceneController : MonoBehaviour
{

    public GameObject lootRoom1ChestLayout, lootRoom2ChestLayout;
    //public GameObject chest;

    GameObject gameMaster;
    List<Inventory> chestItems = new List<Inventory>();
    List<Inventory> chestItemsAdChest = new List<Inventory>();

    public GameObject chestPanelFree;
    public GameObject chestPanelAd;
    //public GameObject inventoryPanel;

    public GameObject slotPanel;
    public GameObject slotPanelForAdChest;
    public GameObject slot;
    public GameObject itemPrefab;

    int slotAmount;

    //public List<Weapons> weapons = new List<Weapons>();
    public List<GameObject> slots = new List<GameObject>();
    public List<GameObject> adSlots = new List<GameObject>();


    //Testing out inventory with WeaponManager. TODO: Replace this with real inventory manager eventually.
    private WeaponManager weaponManagerScript;


    private void Start()
    {
        //Inventory testing. TODO: Replace this with real GameMaster call.
        //weaponManagerScript = GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<WeaponManager>();
        gameMaster = GameObject.FindGameObjectWithTag("GameController");


        Debug.Log("You're in the loot scene!!!");
        RandomizeLootRoomLayout();
        GetRandomizedListOfLoot();


        slotAmount = 10;
        for (int i = 0; i < slotAmount; i++)
        {
            //weapons.Add(new Weapons());

            slots.Add(Instantiate(slot));
            adSlots.Add(Instantiate(slot));

            //Adds an ID to each slot when it generates the slots. Used for drag/drop.
            //slots[i].GetComponent<ItemSlot>().id = i;

            slots[i].transform.SetParent(slotPanel.transform);
            adSlots[i].transform.SetParent(slotPanelForAdChest.transform);

        }

        AddListOfItemsToFreeChest();
        AddListOfItemsToAdChest();

    }

    private void RandomizeLootRoomLayout()
    {
        int lootRoomType = Random.Range(0, 3);

        if (lootRoomType == 2)
        {
            lootRoom1ChestLayout.SetActive(true);
        }
        else
        {
            lootRoom2ChestLayout.SetActive(true);
        }
    }

    private void GetRandomizedListOfLoot()
    {
        chestItems = gameMaster.GetComponent<ItemDatabase>().GetRandomItemsForChest();
        chestItemsAdChest = gameMaster.GetComponent<ItemDatabase>().GetRandomItemsForChest();
    }

    public void GoToPathScene()
    {
        SceneManager.LoadScene("PathScene");
    }

    public void OpenFreeChestUI()
    {
        chestPanelFree.SetActive(true);
        gameMaster.GetComponent<InventoryManager>().OpenInventoryPanelUI();
        //chest.SetActive(true);
    }
    public void OpenAdChestUI()
    {
        chestPanelAd.SetActive(true);
        gameMaster.GetComponent<InventoryManager>().OpenInventoryPanelUI();
        //chest.SetActive(true);
    }

    public void CloseChestUI()
    {
        //chestPanelFree.SetActive(false);
        //chestPanelAd.SetActive(false);
        //chest.SetActive(false);
    }

    public void AddListOfItemsToFreeChest()
    {

        if (chestItems.Count > 0)
        {
            Debug.Log("There were more than 0 items, chest count: " + chestItems.Count);
        }
        //Items itemsToAdd = weaponDB.FetchWeaponByID(id);
        for (int i = 0; i < chestItems.Count; i++)
        {

            Debug.Log("This item is: " + chestItems[i].Item.Title);
            //chestItems[i] = itemsToAdd;
            GameObject itemObject = Instantiate(itemPrefab);

            //Added this for testing.
            //itemObject.GetComponent<ItemData>().weapons = itemsToAdd;
            //itemObject.GetComponent<ItemData>().slotID = i;

            itemObject.transform.SetParent(slots[i].transform);
            itemObject.transform.localPosition = Vector2.zero;
            //weaponObj.GetComponent<Image>().sprite = weaponToAdd.Sprite;
            itemObject.name = chestItems[i].Item.Title;
            itemObject.GetComponent<ItemData>().SetItem(chestItems[i]);
            itemObject.GetComponentInChildren<ItemData>().SetLocation(Location.WhereAmI.temp);
            if (chestItems[i].Count > 1)
            {
                itemObject.GetComponent<Text>().text = chestItems[i].Item.Title + " x" + chestItems[i].Count;
            }
            else
            {
                itemObject.GetComponent<Text>().text = chestItems[i].Item.Title;
            }
            //Debug.Log("Title: " + itemsToAdd.Title);

            //break;

        }


    }

    public void AddListOfItemsToAdChest()
    {
        for (int i = 0; i < chestItemsAdChest.Count; i++)
        {

            Debug.Log("This item is: " + chestItemsAdChest[i].Item.Title);
            GameObject itemObject = Instantiate(itemPrefab);

            itemObject.transform.SetParent(adSlots[i].transform);
            itemObject.transform.localPosition = Vector2.zero;
            //weaponObj.GetComponent<Image>().sprite = weaponToAdd.Sprite;
            itemObject.name = chestItemsAdChest[i].Item.Title;
            itemObject.GetComponent<ItemData>().SetItem(chestItemsAdChest[i]);
            if (chestItemsAdChest[i].Count > 1)
            {
                itemObject.GetComponent<Text>().text = chestItemsAdChest[i].Item.Title + " x" + chestItemsAdChest[i].Count;
            }
            else
            {
                itemObject.GetComponent<Text>().text = chestItemsAdChest[i].Item.Title;
            }
        }
    }

    public void CollectAllFreeLoot()
    {
        for (int i = 0; i < chestItems.Count; i++)
        {
            int count = chestItems[i].Count;
            for (int j = 0; j < count; j++)
            {
                //slots[i].GetComponentInChildren<ItemData>().RemoveOneItemFromChest();
            }
        }
    }

    public void CollectAllAdLoot()
    {
        for (int i = 0; i < chestItemsAdChest.Count; i++)
        {
            int count = chestItemsAdChest[i].Count;
            for (int j = 0; j < count; j++)
            {
                //adSlots[i].GetComponentInChildren<ItemData>().RemoveOneItemFromChest();
            }
        }
    }
}
