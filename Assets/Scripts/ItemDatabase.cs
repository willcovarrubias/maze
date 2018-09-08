using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class ItemDatabase : MonoBehaviour
{
    Dictionary<int, Items> database = new Dictionary<int, Items>();

    public Items FetchItemByID(int id)
    {
        return database[id];
    }

    public void AddToDatabase(Items item)
    {
        database.Add(item.ID, item);
    }

    public Dictionary<int, Items> GetDatabase()
    {
        return database;
    }

    public void DisplayAllItems()
    {
        foreach (KeyValuePair<int, Items> keyValue in database)
        {
            Debug.Log(database[keyValue.Key].Title);
        }
    }

    public List<Inventory> GetRandomItemsForChest(int mazeRoomNumber)
    {
        List<Inventory> chestItems = new List<Inventory>();
        int numberOfItems = Random.Range(1, 6);
        for (int i = 0; i < numberOfItems; i++)
        {
            float randomValue = Random.value;
            int chestItemID;
            int amount;
            if (randomValue >= 0.7f)
            {
                KeyValuePair<int, int> amountAndID = GetComponent<ConsumableDatabase>().GetRandomConsumableID(mazeRoomNumber);
                chestItemID = amountAndID.Key;
                amount = amountAndID.Value;
                AddToList(chestItems, chestItemID, amount);
            }
            else if (randomValue >= 0.1f && randomValue < 0.7f)
            {
                KeyValuePair<int, int> amountAndID = GetComponent<MaterialDatabase>().GetRandomMaterialID(mazeRoomNumber);
                chestItemID = amountAndID.Key;
                amount = amountAndID.Value;
                AddToList(chestItems, chestItemID, amount);
            }
            else if (randomValue >= 0.06f && randomValue < 0.1f)
            {
                Items weapon = GetComponent<WeaponDatabase>().CreateWeapon(mazeRoomNumber);
                chestItems.Add(new Inventory(weapon, 1, chestItems.Count));
            }
            else if (randomValue >= 0.02f && randomValue < 0.06f)
            {
                chestItemID = GetComponent<ArmorDatabase>().GetRandomArmorID(mazeRoomNumber);
                AddToList(chestItems, chestItemID, 1);
            }
            else
            {
                KeyValuePair<int, int> amountAndID = GetComponent<GemsDatabase>().GetRandomGemID(mazeRoomNumber);
                chestItemID = amountAndID.Key;
                amount = amountAndID.Value;
                AddToList(chestItems, chestItemID, amount);
            }
        }
        return chestItems;
    }

    public List<Inventory> GetRandomItemsForEnemy(int mazeRoomNumber, int enemyID)
    {
        List<Inventory> enemyItems = new List<Inventory>();
        int numberOfItems = Random.Range(1, 4);
        for (int i = 0; i < numberOfItems; i++)
        {
            float randomValue = Random.value;
            int chestItemID;
            int amount;
            if (randomValue >= 0.75f)
            {
                KeyValuePair<int, int> amountAndID = GetComponent<ConsumableDatabase>().GetRandomConsumableID(mazeRoomNumber);
                chestItemID = amountAndID.Key;
                amount = amountAndID.Value;
                AddToList(enemyItems, chestItemID, amount);
            }
            else if (randomValue >= 0.5f && randomValue < 0.75f)
            {
                KeyValuePair<int, int> amountAndID = GetComponent<MaterialDatabase>().GetRandomMaterialID(mazeRoomNumber);
                chestItemID = amountAndID.Key;
                amount = amountAndID.Value;
                AddToList(enemyItems, chestItemID, amount);
            }
            else if (randomValue >= 0.05f && randomValue < 0.5f)
            {
                int itemID = GetComponent<CharacterDatabase>().FetchEnemyByID(enemyID).EnemyData.itemsHolding[Random.Range(0, GetComponent<CharacterDatabase>().FetchEnemyByID(enemyID).EnemyData.itemsHolding.Count)];
                amount = Random.Range(1, 3);
                AddToList(enemyItems, itemID, amount);
            }
            else
            {
                KeyValuePair<int, int> amountAndID = GetComponent<GemsDatabase>().GetRandomGemID(mazeRoomNumber);
                chestItemID = amountAndID.Key;
                amount = amountAndID.Value;
                AddToList(enemyItems, chestItemID, amount);
            }
        }
        return enemyItems;
    }


    void AddToList(List<Inventory> items, int id, int count)
    {
        if (count > 0)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Item.ID == id)
                {
                    items[i].Count += count;
                    return;
                }
            }
            Items chestItem = FetchItemByID(id);
            items.Add(new Inventory(chestItem, count, items.Count));
        }
    }
}

public class Items
{
    public int ID { get; set; }
    public string Title { get; set; }
    public List<int> Rarity { get; set; }
    public int Size { get; set; }
    public string Slug { get; set; }
    public Sprite Sprite { get; set; }

    public Items(int id, string title, int size, string slug)
    {
        this.ID = id;
        this.Title = title;
        this.Slug = slug;
        this.Size = size;
        this.Sprite = Resources.Load<Sprite>("Items/" + slug);
    }

    public Items()
    {
        this.ID = -1;
    }
}
