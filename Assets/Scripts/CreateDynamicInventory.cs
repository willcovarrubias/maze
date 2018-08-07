using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateDynamicInventory : MonoBehaviour
{
    public GameObject openButton;
    public GameObject slotPrefab;
    public GameObject itemPrefab;
    public GameObject invPanel;
    public List<GameObject> panelList = new List<GameObject>();

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "LootScene")
        {
            CreateForLootScene();
        }
    }

    public void CloseUi()
    {
        for (int i = 0; i < panelList.Count; i++)
        {
            panelList[i].GetComponent<DynamicInventory>().CloseUI();
        }
    }

    void CreateForLootScene()
    {
        int amount = Random.Range(1, 6);
        int offSet = (amount - 1) * (155 / 2);
        for (int i = 0; i < amount; i++)
        {
            GameObject newButton = Instantiate(openButton, openButton.transform.parent, true);
            GameObject newPanel = Instantiate(invPanel, invPanel.transform.parent, true);
            newButton.transform.SetAsFirstSibling();
            panelList.Add(newPanel);
            newButton.SetActive(true);
            newButton.transform.localPosition = new Vector3((newButton.transform.localPosition.x + i * 155) - offSet, newButton.transform.localPosition.y, newButton.transform.localPosition.z);
            List<Inventory> inventory = GameMaster.gameMaster.GetComponent<ItemDatabase>().GetRandomItemsForChest();
            newPanel.AddComponent<DynamicInventory>().Initialize(Location.WhereAmI.temp, inventory, slotPrefab, itemPrefab, newButton);
        }
        Destroy(invPanel);
        Destroy(openButton);
    }

    public void CreateForFightScene(List<Enemy> enemyList)
    {
        int offSet = (enemyList.Count - 1) * (155 / 2);
        for (int i = 0; i < enemyList.Count; i++)
        {
            GameObject enemy = Instantiate(openButton, openButton.transform.parent, true);
            GameObject newPanel = Instantiate(invPanel, invPanel.transform.parent, true);
            panelList.Add(newPanel);
            enemy.transform.localPosition = new Vector3((enemy.transform.localPosition.x + i * 155) - offSet, enemy.transform.localPosition.y, enemy.transform.localPosition.z);
            enemy.GetComponent<EnemyHolder>().SetEnemyData(enemyList[i]);
            enemy.GetComponentInChildren<Text>().text = enemyList[i].EnemyData.name + "\nAtt:" + enemyList[i].EnemyData.attack + "\nDef:" + enemyList[i].EnemyData.defense + "\nSpd:" + enemyList[i].EnemyData.speed;
            enemy.SetActive(true);
            List<Inventory> inventory = GameMaster.gameMaster.GetComponent<ItemDatabase>().GetRandomItemsForChest();
            newPanel.AddComponent<DynamicInventory>().Initialize(Location.WhereAmI.temp, inventory, slotPrefab, itemPrefab, enemy);
        }
        Destroy(invPanel);
        Destroy(openButton);
    }

    public void DestroyDynamicPanels()
    {
        for (int i = 0; i < panelList.Count; i++)
        {
            Destroy(panelList[i].gameObject);
        }
    }
}
