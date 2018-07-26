using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollElement : MonoBehaviour, IDragHandler
{

    ScrollRect scrollRect;

    // Use this for initialization
    void Start()
    {
        scrollRect = GetComponentInParent<ScrollRect>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        scrollRect.verticalNormalizedPosition += eventData.delta.y / (((float)Screen.height/ 4f));
    }
}
