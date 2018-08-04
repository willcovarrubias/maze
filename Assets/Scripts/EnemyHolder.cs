﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHolder : MonoBehaviour
{
    Enemy enemyData;
    GameObject fightSceneController;

    void Start()
    {
        fightSceneController = GameObject.Find("FightController");
    }

    public void SetEnemyData(Enemy enemy)
    {
        enemyData = enemy;
    }

    public Enemy GetEnemyData()
    {
        return enemyData;
    }

    public void SetSelectedEnemy()
    {
        fightSceneController.GetComponent<FightSceneController>().SelectEnemy(enemyData, gameObject);
    }
}
