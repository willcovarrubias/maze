using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class WeaponManager : MonoBehaviour
{
    GameMaster gameMaster;
    //public GameObject treasureChestPanel;
    public GameObject slotPanel;
    WeaponDatabase weaponDB;
    public GameObject slotToAddWeapon;
    public GameObject weaponObjectPrefab;

    int slotAmount;
   
    public List<Weapons> weapons = new List<Weapons>();
    public List<GameObject> slots = new List<GameObject>();

    void Start()
    {
        gameMaster = GameMaster.gameMaster;
        weaponDB = gameMaster.GetComponent<WeaponDatabase>();

        slotAmount = 6;
        for (int i = 0; i < slotAmount; i++)
        {
            weapons.Add(new Weapons());
     
            slots.Add(Instantiate(slotToAddWeapon));

            //Adds an ID to each slot when it generates the slots. Used for drag/drop.
            slots[i].GetComponent<ItemSlot>().id = i;

            slots[i].transform.SetParent(slotPanel.transform);
            
        }

        AddWeapon(2000);
        AddWeapon(2001);
    
    }

    public void AddWeapon(int id)
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
    }



    public Weapons SetActiveWeapon(int id)
    {
        Weapons numberToSendOff = weaponDB.FetchWeaponByID(id);

        return numberToSendOff;
    }
    

}
