using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RosterManager : MonoBehaviour {

    private int maxRosterSize;

    //UI Stuff.
    public GameObject rosterPanel;
    public GameObject characterSlotPanel;
    public GameObject characterSlot;
    public GameObject characterObjectPrefab;
    private List<GameObject> characterObject = new List<GameObject>();
    public int characterSlotAmount;
    public List<GameObject> characterSlots = new List<GameObject>();

    // Use this for initialization
    void Start () {
        maxRosterSize = 10;

        AddHeroSlot();

    }

    // Update is called once per frame
    void Update () {
		
	}

    private void AddHeroSlot()
    {
        for (int i = 0; i < maxRosterSize; i++)
        {
            characterSlots.Add(Instantiate(characterSlot));
            //characterSlotAmount++;
            //Adds an ID to each slot when it generates the slots. Used for drag/drop.
            //characterSlots[characterSlotAmount - 1].GetComponent<ItemSlot>().id = characterSlotAmount - 1;
            //characterSlots[characterSlotAmount - 1].name = "Slot" + (characterSlotAmount - 1);
            characterSlots[i].transform.SetParent(characterSlotPanel.transform);



        }

    }
}
