using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowToSprite : MonoBehaviour
{
    void Start()
    {
        GameObject shadow = Instantiate(gameObject, transform, true);
        for (int i = 0; i < shadow.transform.childCount; i++)
        {
            Destroy(shadow.transform.GetChild(i).gameObject);
        }
        Destroy(shadow.GetComponent<ShadowToSprite>());
        shadow.name = "Shadow";
        shadow.transform.position = new Vector3(transform.position.x - 0.025f, transform.position.y + 0.025f, transform.position.z + 0.01f);
        shadow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.75f);
        Destroy(GetComponent<ShadowToSprite>());
    }
}
