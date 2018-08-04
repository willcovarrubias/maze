using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FightSceneController : MonoBehaviour
{
    List<Enemy> listOfEnemies;
    Character activeCharacter;
    Enemy activeEnemy;
    GameObject activeEnemySprite;
    private bool onOffense, isFighting, moveSlider, waitingForAttack, selectedEnemy;

    int timeForNext = 1;
    float currentTime, timeForNextEnemyAttack;

    public GameObject meter, slider, fightButton, star, enemySprite, enemyHighlightSprite;
    float initialSliderHeight, heightOfMeter;

    int playerAttackNum, enemyIndex, attackNum;

    void Start()
    {
        isFighting = true;
        onOffense = true;
        moveSlider = true;
        waitingForAttack = true;
        timeForNextEnemyAttack = Random.Range(2.0f, 5.0f);
        heightOfMeter = meter.GetComponent<RectTransform>().rect.height;
        initialSliderHeight = slider.transform.localPosition.y;
        activeCharacter = GameMaster.gameMaster.GetComponent<ActiveCharacterController>().GetActiveCharacter();
        LoadEnemies();
        star.transform.SetAsLastSibling();
    }

    void Update()
    {
        if (isFighting)
        {
            if (onOffense && selectedEnemy)
            {
                Offense();
            }
            else if (!onOffense)
            {
                Defense();
            }
        }
        //Determine rewards, calculate EXP, etc.
        //End fight.?
    }

    void Offense()
    {
        if (moveSlider)
        {
            float speed = 1000 - (activeCharacter.speed * 10) + (activeEnemy.EnemyData.speed * 10);
            slider.transform.localPosition = new Vector3(
                slider.transform.localPosition.x,
                initialSliderHeight + Mathf.PingPong(Time.time * speed, heightOfMeter),
                slider.transform.localPosition.z);
        }
        else
        {
            currentTime += Time.deltaTime;
            if (currentTime > timeForNext)
            {
                selectedEnemy = false;
                playerAttackNum++;
                currentTime = 0;
                if (playerAttackNum >= activeCharacter.numberOfAttacks)
                {
                    onOffense = false;
                    fightButton.GetComponent<Button>().image.color = new Color(0, 0, 0, 0.5f);
                }
                else
                {
                    if (listOfEnemies.Count == 1)
                    {
                        AutoHighlightEnemy();
                    }
                    moveSlider = true;
                }
            }
        }
    }

    void Defense()
    {
        if (waitingForAttack)
        {
            currentTime += Time.deltaTime;
            if (currentTime > timeForNextEnemyAttack)
            {
                float sizeOfStar = ((float)activeCharacter.defense / listOfEnemies[enemyIndex].EnemyData.attack) * 200;
                star.SetActive(true);
                star.transform.localPosition = new Vector3(
                    Random.Range(-Screen.width / 2, Screen.width / 2),
                    Random.Range(-Screen.height / 2, Screen.height / 2),
                    0);
                star.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeOfStar, sizeOfStar);
                currentTime = 0;
                waitingForAttack = false;
                timeForNextEnemyAttack = Random.Range(2.0f, 5.0f);
            }
        }
        else
        {
            currentTime += Time.deltaTime;
            if (currentTime > ((float)activeCharacter.speed / listOfEnemies[enemyIndex].EnemyData.speed) + 0.25f)
            {
                EnemyAttack(false);
            }
        }
    }

    public void ScreenPressed()
    {
        if (isFighting)
        {
            if (onOffense && moveSlider && selectedEnemy)
            {
                PlayerAttack();
            }
            else if (!onOffense)
            {
                if (waitingForAttack)
                {
                    //when player presses screen before star appears, take full damage?
                }
                else
                {
                    EnemyAttack(true);
                }
            }
        }
    }

    void PlayerAttack()
    {
        moveSlider = false;
        currentTime = 0;
        float posOfSlider = (slider.transform.localPosition.y - initialSliderHeight) - heightOfMeter / 2;
        float percentToMiddle = 1 - Mathf.Abs(posOfSlider / (heightOfMeter / 2));
        float playerAttack = ((float)(activeCharacter.attack * activeCharacter.attack) / (activeCharacter.attack + activeEnemy.EnemyData.defense)) * percentToMiddle;
        activeEnemy.EnemyHP -= Mathf.RoundToInt(playerAttack);
        Debug.Log("Enemy HP: " + activeEnemy.EnemyHP);
        if (activeEnemy.EnemyHP <= 0)
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Delt " + Mathf.RoundToInt(playerAttack) + " damage. " + activeEnemy.EnemyData.name + " has fainted.");
            listOfEnemies.Remove(activeEnemy);
            Destroy(activeEnemySprite);
            if (listOfEnemies.Count == 0)
            {
                GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("ALL ENEMIES DEAD");
                isFighting = false;
            }
        }
        else
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Delt " + Mathf.RoundToInt(playerAttack) + " damage to " + activeEnemy.EnemyData.name);
        }
        enemyHighlightSprite.SetActive(false);
    }

    void EnemyAttack(bool pressed)
    {
        float initialAttack = (float)(listOfEnemies[enemyIndex].EnemyData.attack * listOfEnemies[enemyIndex].EnemyData.attack) / (listOfEnemies[enemyIndex].EnemyData.attack + activeCharacter.defense);
        float enemyAttack = initialAttack;
        star.SetActive(false);
        waitingForAttack = true;
        if (pressed)
        {
            float distance = Vector3.Distance(Input.mousePosition, star.transform.position);
            float timePercentage = currentTime / (((float)activeCharacter.speed / listOfEnemies[enemyIndex].EnemyData.speed) + 0.25f);
            distance /= star.GetComponent<RectTransform>().sizeDelta.x / 10;
            if (distance > 5)
            {
                distance = 5;
            }
            enemyAttack *= timePercentage * distance;
            if (enemyAttack > initialAttack * 3)
            {
                enemyAttack = initialAttack * 3;
            }
        }
        else
        {
            initialAttack *= 3;
        }
        currentTime = 0;
        GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Recieved " + Mathf.RoundToInt(enemyAttack) + " damage from " + listOfEnemies[enemyIndex].EnemyData.name);
        activeCharacter.hp -= Mathf.RoundToInt(enemyAttack);
        Debug.Log("Your HP " + activeCharacter.hp);
        if (activeCharacter.hp <= 0)
        {
            isFighting = false;
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("YOU ARE DEAD");
        }
        if (listOfEnemies[enemyIndex].EnemyData.numberOfAttacks > attackNum + 1)
        {
            attackNum++;
        }
        else
        {
            if (enemyIndex >= listOfEnemies.Count - 1)
            {
                if (listOfEnemies.Count == 1)
                {
                    AutoHighlightEnemy();
                }
                onOffense = true;
                moveSlider = true;
                fightButton.GetComponent<Button>().image.color = new Color(0, 0, 0, 0);
                playerAttackNum = 0;
                attackNum = 0;
                enemyIndex = 0;
            }
            else
            {
                attackNum = 0;
                enemyIndex++;
            }
        }
    }

    public void SelectEnemy(Enemy character, GameObject enemySprite)
    {
        if (onOffense)
        {
            selectedEnemy = true;
            activeEnemySprite = enemySprite;
            activeEnemy = character;
            enemyHighlightSprite.transform.position = activeEnemySprite.transform.position;
            enemyHighlightSprite.SetActive(true);
        }
    }

    void AutoHighlightEnemy()
    {
        selectedEnemy = true;
        activeEnemySprite = GameObject.Find("Enemy(Clone)");
        activeEnemy = listOfEnemies[0];
        enemyHighlightSprite.transform.position = activeEnemySprite.transform.position;
        enemyHighlightSprite.SetActive(true);
    }

    void LoadEnemies()
    {
        listOfEnemies = GameMaster.gameMaster.GetComponent<CharacterDatabase>().GetEnemiesForFightScene();
        int offSet = (listOfEnemies.Count - 1) * (155 / 2);
        for (int i = 0; i < listOfEnemies.Count; i++)
        {
            GameObject enemy = Instantiate(enemySprite, enemySprite.transform.parent, true);
            enemy.transform.localPosition = new Vector3((enemy.transform.localPosition.x + i * 155) - offSet, enemy.transform.localPosition.y, enemy.transform.localPosition.z);
            enemy.AddComponent<EnemyHolder>();
            enemy.GetComponent<EnemyHolder>().SetEnemyData(listOfEnemies[i]);
            enemy.GetComponentInChildren<Text>().text = listOfEnemies[i].EnemyData.name + "\nAtt:" + listOfEnemies[i].EnemyData.attack + "\nDef:" + listOfEnemies[i].EnemyData.defense + "\nSpd:" + listOfEnemies[i].EnemyData.speed;
            enemy.SetActive(true);
        }
    }

    public void Inventory()
    {

    }

    public void Special()
    {

    }

    public void GoToVillage()
    {
        SceneManager.LoadScene("VillageScene");
    }

    public void GoToPathRoom()
    {
        GameMaster.gameMaster.GetComponent<ActiveCharacterController>().GiveExpForBattleToActiveCharacter();
        SceneManager.LoadScene("PathScene");
    }
}
