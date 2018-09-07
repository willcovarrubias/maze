using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingPopUp : MonoBehaviour
{
    CraftableItem craftableItem;
    public GameObject popUp, imageOfItem, nameOfItem, statsOfItem, materials, gemsList, gemImage, statsAdditive;
    public GameObject craftButton, exitButton, gemButton;
    Items craftedItem;
    string tempItemName;
    int tempAttack, tempSpecial, tempSpeed, tempDuribility;
    Gem gem;

    void Start()
    {
        Button exit = exitButton.GetComponent<Button>();
        exit.onClick.AddListener(CloseUI);
        Button craft = craftButton.GetComponent<Button>();
        craft.onClick.AddListener(Craft);
        Button gems = gemButton.GetComponent<Button>();
        gems.onClick.AddListener(OpenGemList);
        popUp.transform.SetParent(GameMaster.gameMaster.transform.Find("Canvas").transform, true);
        popUp.transform.SetSiblingIndex(2);
    }

    public void ShowItemPopUp(CraftableItem item)
    {
        craftableItem = item;
        string statsText = "";
        string materialsText = "";
        if (VillageSceneController.villageScene.currentMenu == Location.VillageMenu.weapons)
        {
            gemButton.SetActive(true);
        }
        else
        {
            gemButton.SetActive(false);
        }
        if (item.CraftedItemID != 0)
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeSlotColor(imageOfItem.transform.parent.gameObject, item.CraftedItemID);
            craftedItem = GameMaster.gameMaster.GetComponent<ItemDatabase>().FetchItemByID(item.CraftedItemID);
        }
        else
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeSlotColor(imageOfItem.transform.parent.gameObject, 10000);
            Weapons weapon = CreateWeaponFromItem(item.Weapon);
            weapon.ID = GameMaster.gameMaster.GetComponent<WeaponDatabase>().GetNewWeaponsCount();
            craftedItem = weapon;
            tempItemName = craftedItem.Title;
            tempAttack = weapon.Attack;
            tempSpeed = weapon.Speed;
            tempSpecial = weapon.Special;
            tempDuribility = weapon.Durability;
        }
        nameOfItem.GetComponent<Text>().text = craftedItem.Title;
        imageOfItem.GetComponent<Image>().sprite = craftedItem.Sprite;
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
                materialsText += "<i> (Village has 0)</i>";
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
                if (gem != null)
                {
                    VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().RemoveItemsFromVillageInventory(
                        VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().villageItems[gem.ID],
                        1,
                        VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().villageItems[gem.ID].Count);
                }
                if (craftedItem.ID >= 10000)
                {
                    Weapons item = CreateWeaponFromItem(craftedItem);
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

    public void ChangeGem(Gem newGem)
    {
        gem = newGem;
        if (gem != null)
        {
            statsAdditive.GetComponent<Text>().text = "\n" + (gem.Attack > 0 ? "+" + gem.Attack : "") +
                "\n" + (gem.Special > 0 ? "+" + gem.Special : "") + "\n" + (gem.Speed > 0 ? "+" + gem.Speed : "") +
                "\n" + (gem.Durability > 0 ? "+" + gem.Durability : "") + "\n";
            nameOfItem.GetComponent<Text>().text = tempItemName + " " + gem.AddedTitle;
            Weapons weapon = (Weapons)craftedItem;
            weapon.Title = tempItemName + " " + gem.AddedTitle;
            weapon.Attack = tempAttack + gem.Attack;
            weapon.Speed = tempSpeed + gem.Speed;
            weapon.Durability = tempDuribility + gem.Durability;
            weapon.Special = tempSpecial + gem.Special;
            craftedItem = weapon;
            gemImage.SetActive(true);
        }
        else
        {
            statsAdditive.GetComponent<Text>().text = "";
            nameOfItem.GetComponent<Text>().text = tempItemName;
            Weapons weapon = (Weapons)craftedItem;
            weapon.Title = tempItemName;
            weapon.Attack = tempAttack;
            weapon.Speed = tempSpeed;
            weapon.Durability = tempDuribility;
            weapon.Special = tempSpecial;
            craftedItem = weapon;
            gemImage.SetActive(false);
        }
    }

    Weapons CreateWeaponFromItem(Items item)
    {
        Weapons tempItem = (Weapons)item;
        Weapons weapon = new Weapons();
        weapon.Attack = tempItem.Attack;
        weapon.Durability = tempItem.Durability;
        weapon.Size = tempItem.Size;
        weapon.Special = tempItem.Special;
        weapon.Speed = tempItem.Speed;
        weapon.Rarity = tempItem.Rarity;
        weapon.Slug = tempItem.Slug;
        weapon.Sprite = tempItem.Sprite;
        weapon.Title = tempItem.Title;
        weapon.ID = item.ID;
        return weapon;
    }

    public void OpenGemList()
    {
        gemsList.gameObject.SetActive(true);
    }

    public void CloseGemsList()
    {
        gemsList.gameObject.SetActive(false);
    }

    public void CloseUI()
    {
        if (VillageSceneController.villageScene.currentMenu == Location.VillageMenu.weapons)
        {
            ChangeGem(null);
        }
        popUp.SetActive(false);
    }

    public void DestroyUI()
    {
        Destroy(popUp);
    }
}
