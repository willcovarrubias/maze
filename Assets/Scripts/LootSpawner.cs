using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootSpawner : MonoBehaviour {
    //Mostly a test script that calls the WeaponManager to spawn weapons as loot in a chest.
    //TODO: Check if this is even still necessary. Likely can just delete it.

    GameMaster gameMaster;
    WeaponManager wm;
    WeaponDatabase weaponDB;

    int slotAmount;

    public GameObject slotPanel;
    public GameObject slotToAddWeapon;
    public GameObject weaponObjectPrefab;

    public List<Weapons> weapons = new List<Weapons>();
    public List<GameObject> slots = new List<GameObject>();

    void Start()
    {
        //Test stuff.
        gameMaster = GameMaster.gameMaster;
        wm = GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<WeaponManager>();
        weaponDB = gameMaster.GetComponent<WeaponDatabase>(); //this might not work

        slotAmount = 4;
        for (int i = 0; i < slotAmount; i++)
        {
            weapons.Add(new Weapons());

            slots.Add(Instantiate(slotToAddWeapon));

            //Adds an ID to each slot when it generates the slots. Used for drag/drop.
            slots[i].GetComponent<ItemSlot>().id = i;

            slots[i].transform.SetParent(slotPanel.transform);

        }

  


    }

    /*
    public void AddWeaponToChestPool(int id)
    {
        Weapons weaponToAdd = weaponDB.FetchWeaponByID(id);
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].ID == -1)
            {
                weapons[i] = weaponToAdd;
                GameObject weaponObj = Instantiate(weaponObjectPrefab);

                //Added this for testing.
                weaponObj.GetComponent<ItemData>().weapons = weaponToAdd;
                weaponObj.GetComponent<ItemData>().slotID = i;

                weaponObj.transform.SetParent(slots[i].transform);
                weaponObj.transform.position = Vector2.zero;
                //weaponObj.GetComponent<Image>().sprite = weaponToAdd.Sprite;
                weaponObj.name = weaponToAdd.Title;
                weaponObj.GetComponent<Text>().text = weaponToAdd.Title;
                Debug.Log("Title: " + weaponToAdd.Title);

                break;
            }
        }
    }*/
}
