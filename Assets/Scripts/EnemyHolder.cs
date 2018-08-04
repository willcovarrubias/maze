using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHolder : MonoBehaviour
{
    Character enemyData;
    GameObject fightSceneController;

    void Start()
    {
        fightSceneController = GameObject.Find("FightController");
    }

    public void SetEnemyData(Character enemy)
    {
        enemyData = enemy;
    }

    public Character GetEnemyData()
    {
        return enemyData;
    }

    public void SetSelectedEnemy()
    {
        fightSceneController.GetComponent<FightSceneController>().SelectEnemy(enemyData, gameObject);
    }
}
