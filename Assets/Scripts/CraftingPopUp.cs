using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingPopUp : MonoBehaviour
{
    CraftableItem craftableItem;
    public GameObject popUp, imageOfItem, nameOfItem, statsOfItem, materials;
    public GameObject craftButton, exitButton;
    Items craftedItem;

    void Start()
    {
        Button exit = exitButton.GetComponent<Button>();
        exit.onClick.AddListener(CloseUI);
        Button craft = craftButton.GetComponent<Button>();
        craft.onClick.AddListener(Craft);
        popUp.transform.SetParent(GameMaster.gameMaster.transform.Find("Canvas").transform, true);
        popUp.transform.SetSiblingIndex(2);
    }

    public void ShowItemPopUp(CraftableItem item)
    {
        craftableItem = item;
        string statsText = "";
        string materialsText = "";
        if (item.CraftedItemID != 0)
        {
            craftedItem = GameMaster.gameMaster.GetComponent<ItemDatabase>().FetchItemByID(item.CraftedItemID);
        }
        else
        {
            Weapons weapon = new Weapons();
            weapon.Attack = item.Weapon.Attack;
            weapon.Durability = item.Weapon.Durability;
            weapon.Size = item.Weapon.Size;
            weapon.Slug = item.Weapon.Slug;
            weapon.Special = item.Weapon.Special;
            weapon.Speed = item.Weapon.Speed;
            weapon.Sprite = item.Weapon.Sprite;
            weapon.Title = item.Weapon.Title;
            weapon.ID = GameMaster.gameMaster.GetComponent<WeaponDatabase>().GetNewWeaponsCount();
            craftedItem = weapon;
        }
        nameOfItem.GetComponent<Text>().text = craftedItem.Title;
        if (craftedItem.ID >= 4000 && craftedItem.ID < 5000)
        {
            Armor craftedArmor = (Armor)craftedItem;
            statsText += "Armor\nDef " + craftedArmor.Defense + "\nspd " + craftedArmor.Speed +
                         "\nApp " + craftedArmor.Appendage + "\nWgt " + craftedArmor.Size;
        }
        else if (craftedItem.ID >= 1000 && craftedItem.ID < 2000)
        {
            Consumable consumable = (Consumable)craftedItem;
            statsText += "Consumable\nHP " + (consumable.Healing > 0 ? "+" : "") + consumable.Healing + "\nWgt " + consumable.Size;
        }
        else
        {
            statsText += "Weapon\nAtk " + item.Weapon.Attack + "\nSpec " + item.Weapon.Special + "\nSpd " + item.Weapon.Speed +
                         "\nDu " + item.Weapon.Durability + "\nWgt " + item.Weapon.Size;
        }
        statsOfItem.GetComponent<Text>().text = statsText;
        materialsText += "<b>Materials Needed:</b>";
        foreach (KeyValuePair<int, int> keyValue in craftableItem.Materials)
        {
            materialsText += "\n";
            materialsText += GameMaster.gameMaster.GetComponent<ItemDatabase>().FetchItemByID(keyValue.Key).Title;
            materialsText += " x" + keyValue.Value;
            if (VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().villageItems.ContainsKey(keyValue.Key))
            {
                materialsText += "<i> (Village has " + VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().villageItems[keyValue.Key].Count + ")</i>";
            }
            else
            {
                materialsText += "<i> (Village has none)</i>";
            }
        }
        materials.GetComponent<Text>().text = materialsText;
        popUp.SetActive(true);
    }

    public void Craft()
    {
        if (CheckIfHaveMaterials())
        {
            if (GameMaster.gameMaster.GetComponent<InventoryManager>().GetFreeSpaceCount() >= craftedItem.Size)
            {
                foreach (KeyValuePair<int, int> keyValue in craftableItem.Materials)
                {
                    VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().RemoveItemsFromVillageInventory(
                        VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().villageItems[keyValue.Key],
                        keyValue.Value,
                        VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().villageItems[keyValue.Key].SlotNum);
                }
                if (craftedItem.ID >= 10000)
                {
                    Weapons item = new Weapons();
                    item = (Weapons)craftedItem;
                    item.ID = GameMaster.gameMaster.GetComponent<WeaponDatabase>().GetNewWeaponsCount();
                    Inventory newItem = new Inventory(item, 1, 0);
                    GameMaster.gameMaster.GetComponent<InventoryManager>().AddBoughtItem(newItem);
                }
                else
                {
                    Inventory newItem = new Inventory(craftedItem, 1, 0);
                    GameMaster.gameMaster.GetComponent<InventoryManager>().AddBoughtItem(newItem);
                }
                GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Crafted " + craftedItem.Title);
                CloseUI();
            }
            else
            {
                GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Not enought space");
            }
        }
    }

    bool CheckIfHaveMaterials()
    {
        foreach (KeyValuePair<int, int> keyValue in craftableItem.Materials)
        {
            if (VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().villageItems.ContainsKey(keyValue.Key))
            {
                if (!(VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().villageItems[keyValue.Key].Count >= keyValue.Value))
                {
                    GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Not enough materials");
                    return false;
                }
            }
            else
            {
                GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Not enough materials");
                return false;
            }
        }
        return true;
    }

    public void CloseUI()
    {
        popUp.SetActive(false);
    }

    public void DestroyUI()
    {
        Destroy(popUp);
    }
}
