using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class CraftingDatabase : MonoBehaviour
{
    JsonData armoryCraftingData;
    List<CraftableItem> craftableItems = new List<CraftableItem>();

    void Start()
    {
        armoryCraftingData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ArmoryCrafting.json"));
        AddToArmoryDatabase();
        //Add to list
    }

    void AddToArmoryDatabase()
    {
        for (int i = 0; i < armoryCraftingData.Count; i++)
        {
            CraftableItem item = new CraftableItem();
            Dictionary<int, int> materials = new Dictionary<int, int>();
            item.SlotNum = i;
            item.CraftedItemID = (int)armoryCraftingData[i]["item"];
            item.Level = (int)armoryCraftingData[i]["level"];
            for (int j = 0; j < armoryCraftingData[i]["materials"].Count; j++)
            {
                materials.Add((int)armoryCraftingData[i]["materials"][j]["material"], (int)armoryCraftingData[i]["materials"][j]["amount"]);
            }
            item.Materials = materials;
            craftableItems.Add(item);
        }
    }
}

public class CraftableItem
{
    public int SlotNum { get; set; }
    public int CraftedItemID { get; set; }
    public int Level { get; set; }
    public Dictionary<int, int> Materials { get; set; }

    public CraftableItem(int slotNum, int craftedItemId, int level, Dictionary<int, int> materials)
    {
        SlotNum = slotNum;
        CraftedItemID = craftedItemId;
        Level = level;
        Materials = materials;
    }

    public CraftableItem() { }
}
