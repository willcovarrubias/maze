using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DynamicInventory : MonoBehaviour 
{
    public List<Inventory> items = new List<Inventory>();

    //UI Stuff.
    public List<GameObject> slots = new List<GameObject>();
    public int slotAmount;
    public GameObject inventoryPanel;
    public GameObject slotPanel;
    public GameObject slot;
    public GameObject itemPrefab;
    public GameObject inventoryPane;
    public RectTransform slotPanelRectTransform;
    public ScrollRect scrollView;
}
