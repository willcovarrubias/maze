using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class MaterialDatabase : MonoBehaviour
{
    List<int> materialIDs = new List<int>();
    private JsonData itemsData;

    void Start()
    {
        itemsData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Materials.json"));
        AddToDatabase();
    }

    void AddToDatabase()
    {
        for (int i = 0; i < itemsData.Count; i++)
        {
            Material material = new Material((int)itemsData[i]["id"],
                itemsData[i]["title"].ToString(),
                (int)itemsData[i]["rarity"],
                (int)itemsData[i]["size"],
                itemsData[i]["slug"].ToString());
            GetComponent<ItemDatabase>().AddToDatabase(material);
            materialIDs.Add(material.ID);
        }
    }

    //TODO: use mazeRoomNumber and rarity in the future
    public int GetRandomMaterialID(/*int mazeRoomNumber*/)
    {
        return materialIDs[Random.Range(0, materialIDs.Count)];
    }
}

public class Material : Items
{
    public Material(int id, string title, int rarity, int size, string slug)
    {
        this.ID = id;
        this.Title = title;
        this.Rarity = rarity;
        this.Size = size;
        this.Slug = slug;
        this.Sprite = Resources.Load<Sprite>("Items/" + slug);
    }
}