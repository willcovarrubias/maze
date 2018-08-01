using UnityEngine;

public class DialogTimer : MonoBehaviour
{
    static int maxTime = 3;
    float currentTime;

    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > maxTime)
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("");
        }
    }

    public void ResetCurrentTime()
    {
        currentTime = 0;
    }
}
