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
        int randomAmount = Random.Range(1, 5);
        chests = new List<GameObject>();
        int offset = 0;
        if (randomAmount > 1)
        {
            offset = randomAmount * 50;
        }
        for (int i = 0; i < randomAmount; i++)
        {
            GameObject newChest = Instantiate(chest, canvas.transform, true);
            newChest.transform.SetAsFirstSibling();
            newChest.AddComponent<Loot>();
            newChest.GetComponent<Loot>().RunRandom(slot, itemPrefab);
            GameObject chestPanel = newChest.transform.Find("ChestPanel").gameObject;
            newChest.transform.localPosition = new Vector3(
                chest.transform.localPosition.x + (i * 100) - offset,
                chest.transform.localPosition.y,
                chest.transform.localPosition.z
            );
            chestPanel.transform.localPosition = new Vector3(
                chestPanel.transform.localPosition.x - (i * 100) + offset,
                chestPanel.transform.localPosition.y,
                chestPanel.transform.localPosition.z
            );
            chestPanel.transform.SetParent(chestPanel.transform.parent.parent);
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
