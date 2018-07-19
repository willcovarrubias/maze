using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class WeaponManager : MonoBehaviour
{

    GameObject slotPanel;
    WeaponDatabase database;
    public GameObject slotToAddWeapon;
    public GameObject weaponToDisplay;

    int slotAmount;
   
    public List<Weapons> weapon = new List<Weapons>();
    public List<GameObject> slots = new List<GameObject>();

    void Start()
    {
        database = GetComponent<WeaponDatabase>();

        slotAmount = 10;
        for (int i = 0; i < slotAmount; i++)
        {
            weapon.Add(new Weapons());
     
            slots.Add(Instantiate(slotToAddWeapon));
        }
    }

    public void AddWeaponToCharacter(int id)
    {
        Weapons weaponToSet = database.FetchWeaponByID(id);
        for (int i = 0; i < weapon.Count; i++)
        {
            if (weapon[i].ID == -1)
            {
                weapon[i] = weaponToSet;
                GameObject armorObj = Instantiate(weaponToDisplay);
                armorObj.transform.SetParent(slots[i].transform);

                armorObj.name = weaponToSet.Title;
                armorObj.GetComponent<SpriteRenderer>().sprite = weaponToSet.Sprite;

                break;
            }
        }
    }

   

    public Weapons SetActiveWeapon(int id)
    {
        Weapons numberToSendOff = database.FetchWeaponByID(id);

        return numberToSendOff;
    }
    

}
