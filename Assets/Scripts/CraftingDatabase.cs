using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class CraftingDatabase : MonoBehaviour
{
    JsonData armoryCraftingData, consumableCraftingData, weaponsCraftingData;
    List<CraftableItem> craftableArmor = new List<CraftableItem>();
    List<CraftableItem> craftableConsumables = new List<CraftableItem>();
    List<CraftableItem> craftableWeapons = new List<CraftableItem>();
    public GameObject armorMenu, consumablesMenu, weaponsMenu;
    int armoryLevel, consumablesLevel, weaponsLevel;

    void Start()
    {
        armoryLevel = GetComponent<BuildingsManager>().GetArmorSmithLevel();
        consumablesLevel = GetComponent<BuildingsManager>().GetItemShopLevel();
        weaponsLevel = GetComponent<BuildingsManager>().GetWeaponSmithLevel();
        armoryCraftingData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ArmoryCrafting.json"));
        consumableCraftingData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ConsumablesCrafting.json"));
        weaponsCraftingData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/WeaponsCrafting.json"));
        RefreshArmory();
        RefreshConsumables();
        RefreshWeapons();
    }

    public void UpdateArmory()
    {
        armoryLevel = GetComponent<BuildingsManager>().GetArmorSmithLevel();
        RefreshArmory();
    }

    public void UpdateConsumables()
    {
        consumablesLevel = GetComponent<BuildingsManager>().GetItemShopLevel();
        RefreshConsumables();
    }

    public void UpdateWeapons()
    {
        weaponsLevel = GetComponent<BuildingsManager>().GetWeaponSmithLevel();
        RefreshWeapons(); 
    }

    public void RefreshArmory()
    {
        armorMenu.GetComponent<CraftingMenu>().ClearSlots();
        craftableArmor.Clear();
        for (int i = 0; i < armoryCraftingData.Count; i++)
        {
            if (((int)armoryCraftingData[i]["level"]) <= armoryLevel)
            {
                CraftableItem item = new CraftableItem();
                Dictionary<int, int> materials = new Dictionary<int, int>();
                item.SlotNum = i;
                item.CraftedItemID = (int)armoryCraftingData[i]["item"];
                item.Level = (int)armoryCraftingData[i]["level"];
                for (int j = 0; j < armoryCraftingData[i]["materials"].Count; j++)
                {
                    materials.Add((int)armoryCraftingData[i]["materials"][j]["material"],
                                  (int)armoryCraftingData[i]["materials"][j]["amount"]);
                }
                item.Materials = materials;
                craftableArmor.Add(item);
                armorMenu.GetComponent<CraftingMenu>().CreateNewItem(item, false);
            }
            else
            {
                break;
            }
        }
    }

    void RefreshConsumables()
    {
        consumablesMenu.GetComponent<CraftingMenu>().ClearSlots();
        craftableConsumables.Clear();
        for (int i = 0; i < consumableCraftingData.Count; i++)
        {
            if ((int)consumableCraftingData[i]["level"] <= consumablesLevel)
            {
                CraftableItem item = new CraftableItem();
                Dictionary<int, int> materials = new Dictionary<int, int>();
                item.SlotNum = i;
                item.CraftedItemID = (int)consumableCraftingData[i]["item"];
                item.Level = (int)consumableCraftingData[i]["level"];
                for (int j = 0; j < consumableCraftingData[i]["materials"].Count; j++)
                {
                    materials.Add((int)consumableCraftingData[i]["materials"][j]["material"],
                                  (int)consumableCraftingData[i]["materials"][j]["amount"]);
                }
                item.Materials = materials;
                craftableConsumables.Add(item);
                consumablesMenu.GetComponent<CraftingMenu>().CreateNewItem(item, false);
            }
            else
            {
                return;
            }
        }
    }

    void RefreshWeapons()
    {
        weaponsMenu.GetComponent<CraftingMenu>().ClearSlots();
        craftableWeapons.Clear();
        for (int i = 0; i < weaponsCraftingData.Count; i++)
        {
            if ((int)weaponsCraftingData[i]["level"] <= weaponsLevel)
            {
                CraftableItem item = new CraftableItem();
                Dictionary<int, int> materials = new Dictionary<int, int>();
                item.SlotNum = i;
                item.CraftedItemID = 0;
                item.Weapon.Title = weaponsCraftingData[i]["title"].ToString();
                item.Weapon.Attack = (int)weaponsCraftingData[i]["attack"];
                item.Weapon.Durability = (int)weaponsCraftingData[i]["duribilty"];
                item.Weapon.Special = (int)weaponsCraftingData[i]["special"];
                item.Weapon.Speed = (int)weaponsCraftingData[i]["speed"];
                item.Weapon.Size = (int)weaponsCraftingData[i]["size"];
                item.Level = (int)weaponsCraftingData[i]["level"];
                for (int j = 0; j < weaponsCraftingData[i]["materials"].Count; j++)
                {
                    materials.Add((int)weaponsCraftingData[i]["materials"][j]["material"],
                                  (int)weaponsCraftingData[i]["materials"][j]["amount"]);
                }
                item.Materials = materials;
                craftableWeapons.Add(item);
                weaponsMenu.GetComponent<CraftingMenu>().CreateNewItem(item, true);
            }
            else
            {
                return;
            }
        }
    }
}

public class CraftableItem
{
    public int SlotNum { get; set; }
    public int CraftedItemID { get; set; }
    public int Level { get; set; }
    public Dictionary<int, int> Materials { get; set; }
    public Weapons Weapon { get; set; }

    public CraftableItem(int slotNum, int craftedItemId, int level, Dictionary<int, int> materials)
    {
        SlotNum = slotNum;
        CraftedItemID = craftedItemId;
        Level = level;
        Materials = materials;
    }

    public CraftableItem() { Weapon = new Weapons(); }
}
