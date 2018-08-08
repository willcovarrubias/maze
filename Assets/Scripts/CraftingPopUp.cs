using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingPopUp : MonoBehaviour
{
    CraftableItem craftableItem;
    public GameObject popUp, imageOfItem, nameOfItem, statsOfItem, materials;
    public GameObject craftButton, exitButton;
    Items craftedItem;
    int sizeOfAllMaterials;

    void Start()
    {
        Button exit = exitButton.GetComponent<Button>();
        exit.onClick.AddListener(CloseUI);
        Button craft = craftButton.GetComponent<Button>();
        craft.onClick.AddListener(Craft);
        popUp.transform.SetParent(GameMaster.gameMaster.transform.Find("Canvas").transform, true);
        popUp.transform.SetSiblingIndex(1);

    }

    public void ShowItemPopUp(CraftableItem item)
    {
        craftableItem = item;
        string statsText = "";
        string materialsText = "";
        craftedItem = GameMaster.gameMaster.GetComponent<ItemDatabase>().FetchItemByID(item.CraftedItemID);
        nameOfItem.GetComponent<Text>().text = craftedItem.Title;
        if (craftedItem.ID >= 4000 && craftedItem.ID < 5000)
        {
            Armor craftedArmor = (Armor)craftedItem;
            statsText += "Armor\nDef " + craftedArmor.Defense + "\nspd " + craftedArmor.Speed +
                         "\nApp " + craftedArmor.Appendage + "\nWgt " + craftedArmor.Size;
        }
        statsOfItem.GetComponent<Text>().text = statsText;
        materialsText += "<b>Materials Needed:</b>";
        foreach (KeyValuePair<int, int> keyValue in craftableItem.Materials)
        {
            materialsText += "\n";
            materialsText += GameMaster.gameMaster.GetComponent<ItemDatabase>().FetchItemByID(keyValue.Key).Title;
            materialsText += " x" + keyValue.Value;
            if (GameMaster.gameMaster.GetComponent<InventoryManager>().playerItems.ContainsKey(keyValue.Key))
            {
                materialsText += "<i> (Have x" + GameMaster.gameMaster.GetComponent<InventoryManager>().playerItems[keyValue.Key].Count + ")</i>";
            }
            else
            {
                materialsText += "<i> (Have 0)</i>";
            }
        }
        materials.GetComponent<Text>().text = materialsText;
        popUp.SetActive(true);
    }

    public void Craft()
    {
        if (CheckIfHaveMaterials())
        {
            if ((GameMaster.gameMaster.GetComponent<InventoryManager>().GetFreeSpaceCount() - sizeOfAllMaterials) >= craftedItem.Size)
            {
                foreach (KeyValuePair<int, int> keyValue in craftableItem.Materials)
                {
                    GameMaster.gameMaster.GetComponent<InventoryManager>().RemoveItemsFromInventory(
                        GameMaster.gameMaster.GetComponent<InventoryManager>().playerItems[keyValue.Key],
                        keyValue.Value,
                        GameMaster.gameMaster.GetComponent<InventoryManager>().playerItems[keyValue.Key].SlotNum);
                }
                GameMaster.gameMaster.GetComponent<InventoryManager>().AddBoughtItem(craftedItem, 1);
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
        sizeOfAllMaterials = 0;
        foreach (KeyValuePair<int, int> keyValue in craftableItem.Materials)
        {
            if (GameMaster.gameMaster.GetComponent<InventoryManager>().playerItems.ContainsKey(keyValue.Key))
            {
                if (!(GameMaster.gameMaster.GetComponent<InventoryManager>().playerItems[keyValue.Key].Count >= keyValue.Value))
                {
                    GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Not enough materials");
                    return false;
                }
                sizeOfAllMaterials += (GameMaster.gameMaster.GetComponent<InventoryManager>().playerItems[keyValue.Key].Item.Size * keyValue.Value);
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
