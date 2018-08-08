using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftableItemData : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public int slotID;
    CraftableItem item;

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData)
    {
        VillageSceneController.villageScene.GetComponent<CraftingPopUp>().ShowItemPopUp(item);
    }

    public void SetItem(CraftableItem itemToBeSet)
    {
        item = itemToBeSet;
    }
}
