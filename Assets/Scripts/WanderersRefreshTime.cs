using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WanderersRefreshTime : MonoBehaviour
{
    static int updateInterval = 60;
    GameObject refreshText;
    int refreshTime;
    int previousTime;
    //int tempPreviousTime;
    int currentSeconds;

    string url = "time.nist.gov";

    void Start()
    {
        refreshText = VillageSceneController.villageScene.GetComponent<RecruitmentManager>().refreshTimeText;
        refreshTime = VillageSceneController.villageScene.GetComponent<RecruitmentManager>().refreshTime;
        previousTime = VillageSceneController.villageScene.GetComponent<RecruitmentManager>().previousTime;
        //GetTime();
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
                refreshText.GetComponent<Text>().text = "Time until refresh: " + hours + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2");
            }
        }
    }

    int GetTimeInSeconds()
    {
        //tempPreviousTime = currentSeconds;
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        currentSeconds = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
        return currentSeconds;
    }

    void GetTime()
    {
        var client = new TcpClient(url, 13);
        if (client.Connected)
        {
            using (var streamReader = new StreamReader(client.GetStream()))
            {
                var response = streamReader.ReadToEnd();
                var utcDateTimeString = response.Substring(7, 17);
                DateTime localDateTime = DateTime.ParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                TimeSpan span = localDateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
                Debug.Log(GetTimeInSeconds());
                Debug.Log("Online Time: " + span.TotalSeconds);
            }
        }
        else
        {
            
        }
    }
}
