using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingPopUp : MonoBehaviour
{
    CraftableItem craftableItem;
    public GameObject popUp, imageOfItem, nameOfItem, statsOfItem, valueOfItem, materials, villageAmount, gemsList, gemImage;
    public GameObject craftButton, exitButton, gemButton;
    Items craftedItem;
    string tempItemName;
    int tempAttack, tempSpecial, tempSpeed, tempDuribility, tempMaterial;
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
        string valueText = "";
        string materialsText = "";
        string villageAmountText = "";
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
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeSlotColor(imageOfItem.transform.parent.transform.parent.gameObject, item.CraftedItemID);
            craftedItem = GameMaster.gameMaster.GetComponent<ItemDatabase>().FetchItemByID(item.CraftedItemID);
        }
        else
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeSlotColor(imageOfItem.transform.parent.transform.parent.gameObject, 10000);
            Weapons weapon = CreateWeaponFromItem(item.Weapon);
            weapon.ID = GameMaster.gameMaster.GetComponent<WeaponDatabase>().GetNewWeaponsCount();
            craftedItem = weapon;
            tempItemName = craftedItem.Title;
            tempAttack = weapon.Attack;
            tempSpeed = weapon.Speed;
            tempSpecial = weapon.Special;
            tempDuribility = weapon.Durability;
            tempMaterial = weapon.MaterialID;
        }
        nameOfItem.GetComponent<Text>().text = craftedItem.Title;
        imageOfItem.GetComponent<Image>().sprite = craftedItem.Sprite;
        if (craftedItem.ID >= 4000 && craftedItem.ID < 5000)
        {
            Armor craftedArmor = (Armor)craftedItem;
            statsText += "Type\nAppendage\nDefense\nSpeed\nWeight";
            valueText += "Armor\n" + craftedArmor.Appendage +
                "\n" + (craftedArmor.Defense > 0 ? "+" : "") + craftedArmor.Defense +
                "\n" + (craftedArmor.Speed > 0 ? "+" : "") + craftedArmor.Speed +
                "\n" + craftedArmor.Size;
        }
        else if (craftedItem.ID >= 1000 && craftedItem.ID < 2000)
        {
            Consumable consumable = (Consumable)craftedItem;
            statsText += "Type\nHP\nMP\nWeight";
            valueText += "Consumable\n" + (consumable.Healing > 0 ? "+" : "") + consumable.Healing +
                "\n" + (consumable.MP > 0 ? "+" : "") + consumable.MP +
                "\n" + consumable.Size;
        }
        else
        {
            statsText += "Type\nAttack\nSpecial\nSpeed\nDuribility\nWeight";
            valueText += "Weapon\n" + (item.Weapon.Attack > 0 ? "+" : "") + item.Weapon.Attack +
                "\n" + (item.Weapon.Special > 0 ? "+" : "") + item.Weapon.Special +
                "\n" + (item.Weapon.Speed > 0 ? "+" : "") + item.Weapon.Speed +
                "\n" + item.Weapon.Durability +
                "\n" + item.Weapon.Size;
        }
        statsOfItem.GetComponent<Text>().text = statsText;
        valueOfItem.GetComponent<Text>().text = valueText;
        materialsText += "<b>Materials Needed:</b>";
        foreach (KeyValuePair<int, int> keyValue in craftableItem.Materials)
        {
            materialsText += "\n" + GameMaster.gameMaster.GetComponent<ItemDatabase>().FetchItemByID(keyValue.Key).Title;
            materialsText += " x" + keyValue.Value;
            if (VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().villageItems.ContainsKey(keyValue.Key))
            {
                villageAmountText += "\nVillage has " + VillageSceneController.villageScene.GetComponent<VillageInventoryManager>().villageItems[keyValue.Key].Count;
            }
            else
            {
                villageAmountText += "\nVillage has 0";
            }
        }
        villageAmount.GetComponent<Text>().text = villageAmountText;
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
            Weapons weapon = (Weapons)craftedItem;
            valueOfItem.GetComponent<Text>().text = "Weapon\n" + (weapon.Attack > 0 ? "+" : "") + weapon.Attack + "<color=#99ff99ff>" + (gem.Attack > 0 ? " +" + gem.Attack : "") + "</color>" +
                "\n" + (weapon.Special > 0 ? "+" : "") + weapon.Special + "<color=#99ff99ff>" + (gem.Special > 0 ? " +" + gem.Special : "") + "</color>" +
                "\n" + (weapon.Speed > 0 ? "+" : "") + weapon.Speed + "<color=#99ff99ff>" + (gem.Speed > 0 ? " +" + gem.Speed : "") + "</color>" +
                "\n" + weapon.Durability + "<color=#99ff99ff>" + (gem.Durability > 0 ? " +" + gem.Durability : "") + "</color>" +
                "\n" + weapon.Size;
            nameOfItem.GetComponent<Text>().text = tempItemName + " " + gem.AddedTitle;
            weapon.Title = tempItemName + " " + gem.AddedTitle;
            weapon.Attack = tempAttack + gem.Attack;
            weapon.Speed = tempSpeed + gem.Speed;
            weapon.Durability = tempDuribility + gem.Durability;
            weapon.Special = tempSpecial + gem.Special;
            weapon.MaterialID = tempMaterial;
            craftedItem = weapon;
            gemImage.GetComponent<Image>().sprite = newGem.Sprite;
            gemImage.SetActive(true);
        }
        else
        {
            Weapons weapon = (Weapons)craftedItem;
            valueOfItem.GetComponent<Text>().text = "Weapon\n" + (weapon.Attack > 0 ? "+" : "") + weapon.Attack +
                "\n" + (weapon.Special > 0 ? "+" : "") + weapon.Special +
                "\n" + (weapon.Speed > 0 ? "+" : "") + weapon.Speed +
                "\n" + weapon.Durability +
                "\n" + weapon.Size;
            nameOfItem.GetComponent<Text>().text = tempItemName;
            weapon.Title = tempItemName;
            weapon.Attack = tempAttack;
            weapon.Speed = tempSpeed;
            weapon.Durability = tempDuribility;
            weapon.Special = tempSpecial;
            weapon.MaterialID = tempMaterial;
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
        weapon.MaterialID = tempItem.MaterialID;
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
