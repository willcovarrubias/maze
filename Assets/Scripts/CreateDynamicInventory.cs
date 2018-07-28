using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateDynamicInventory : MonoBehaviour 
{
    public GameObject openButton;
    public GameObject slotPrefab;
    public GameObject itemPrefab;
    public GameObject invPanel;
    public List<GameObject> panelList = new List<GameObject>();

	void Start () 
    {
        //example
        for (int i = 0; i < 5; i++)
        {
            GameObject newButton = Instantiate(openButton, openButton.transform.parent, true);
            GameObject newPanel = Instantiate(invPanel, invPanel.transform.parent, true);
            newButton.transform.SetAsFirstSibling();
            panelList.Add(newPanel);
            newButton.SetActive(true);
            newButton.transform.position = new Vector3(openButton.transform.position.x, openButton.transform.position.y + i * 25, openButton.transform.position.z);
            List<Inventory> inventory = GameMaster.gameMaster.GetComponent<ItemDatabase>().GetRandomItemsForChest();
            newPanel.AddComponent<DynamicInventory>().Initialize(Location.WhereAmI.chest, inventory, slotPrefab, itemPrefab, newButton);
        }
        Destroy(invPanel);
        Destroy(openButton);
	}

    public void CloseUi()
    {
        for (int i = 0; i < panelList.Count; i++)
        {
            panelList[i].GetComponent<DynamicInventory>().CloseUI();
        }
    }
}
