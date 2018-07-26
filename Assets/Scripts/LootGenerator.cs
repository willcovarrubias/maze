using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootGenerator : MonoBehaviour
{
    public GameObject chest;
    public GameObject slot;
    public GameObject itemPrefab;
    public GameObject canvas;
    public List<GameObject> chests;

    void Start()
    {
        int randomAmount = Random.Range(1, 4);
        chests = new List<GameObject>();
        for (int i = 0; i < randomAmount; i++)
        {
            GameObject newChest = Instantiate(chest, canvas.transform, true);
            newChest.AddComponent<Loot>();
            newChest.GetComponent<Loot>().RunRandom(slot, itemPrefab);
            newChest.transform.position = new Vector3(
                chest.transform.position.x + (i * 100),
                chest.transform.position.y,
                chest.transform.position.z
            );
            newChest.transform.Find("ChestPanel").transform.position = new Vector3(
                newChest.transform.Find("ChestPanel").transform.position.x - (i * 100),
                newChest.transform.Find("ChestPanel").transform.position.y,
                newChest.transform.Find("ChestPanel").transform.position.z
            );
            chests.Add(newChest);
        }
    }

    public void CloseAllChestUi()
    {
        for (int i = 0; i < chests.Count; i++)
        {
            chests[i].GetComponent<Loot>().CloseChestUI();
        }
    }
}
