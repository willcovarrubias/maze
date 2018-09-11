using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesShifter : MonoBehaviour
{
    //change these depending on scene
    static float MinDistance = 0f;
    static float MaxDistance = 10f;
    public Color startingColor = new Color(1, 1, 1);
    public Color endingColor = new Color(0.45f, 0.5f, 0.9f);

    protected SpriteRenderer SpriteRenderer;

    protected void Start()
    {
        //change color based on z positon
        var distance = Mathf.Abs(transform.position.z);
        var ratio = Mathf.Clamp01((distance - MinDistance) / (MaxDistance - MinDistance));
        Color lerped = Color.Lerp(startingColor, endingColor, ratio);
        GetComponent<Renderer>().material.color = lerped * 1.05f;
        //create shadow
        GameObject shadow = Instantiate(gameObject, transform, true);
        for (int i = 0; i < shadow.transform.childCount; i++)
        {
            Destroy(shadow.transform.GetChild(i).gameObject);
        }
        Destroy(shadow.GetComponent<SpritesShifter>());
        shadow.name = "Shadow";
        shadow.transform.position = new Vector3(transform.position.x - 0.025f, transform.position.y + 0.025f, transform.position.z + 0.01f);
        shadow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.75f);
        Destroy(GetComponent<SpritesShifter>());
    }
}
