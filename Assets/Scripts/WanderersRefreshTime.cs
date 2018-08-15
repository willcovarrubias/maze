using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WanderersRefreshTime : MonoBehaviour
{
    static int updateInterval = 60;
    GameObject refreshText;
    int refreshTime;
    int previousTime;
    void Start()
    {
        //refreshText = GameObject.Find("");
        refreshTime = VillageSceneController.villageScene.GetComponent<RecruitmentManager>().refreshTime;
        previousTime = VillageSceneController.villageScene.GetComponent<RecruitmentManager>().previousTime;
    }

    void Update()
    {
        if (Time.frameCount % updateInterval == 0)
        {
            if (GetTimeInSeconds() - previousTime > refreshTime)
            {
                GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("New Wanderers have appeared!");
                VillageSceneController.villageScene.GetComponent<RecruitmentManager>().RefreshIfEnoughTimeHasPassed();
                previousTime = VillageSceneController.villageScene.GetComponent<RecruitmentManager>().previousTime;
            }
            else
            {
                int timeLeft = (refreshTime - (GetTimeInSeconds() - previousTime));
                int hours = timeLeft / 3600;
                int minutes = ((timeLeft - (hours * 3600)) / 60);
                int seconds = timeLeft % 60;
                //Debug.Log(hours + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2"));
                //refreshText.GetComponent<Text>().text = hours + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2");
            }
        }
    }

    int GetTimeInSeconds()
    {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        return (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
    }
}
