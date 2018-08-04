using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FightSceneController : MonoBehaviour
{
    List<Character> listOfEnemies;
    Character activeCharacter;
    Character activeEnemy;
    GameObject activeEnemySprite;
    private bool onOffense, isFighting, moveSlider, waitingForAttack, selectedEnemy;

    int timeForNext = 1;
    float currentTime;

    public GameObject meter, slider, fightButton, star, enemySprite, enemyHighlightSprite;
    float initialSliderHeight;
    float heightOfMeter;

    int playerAttackNum, enemyIndex, attackNum;

    void Start()
    {
        isFighting = true;
        onOffense = true;
        moveSlider = true;
        waitingForAttack = true;
        listOfEnemies = GameMaster.gameMaster.GetComponent<CharacterDatabase>().GetListofEnemies();
        heightOfMeter = meter.GetComponent<RectTransform>().rect.height;
        initialSliderHeight = slider.transform.localPosition.y;
        activeCharacter = GameMaster.gameMaster.GetComponent<ActiveCharacterController>().GetActiveCharacter();
        for (int i = 0; i < listOfEnemies.Count; i++)
        {
            GameObject enemy = Instantiate(enemySprite, enemySprite.transform.parent, true);
            if (i % 2 == 0)
            {
                enemy.transform.localPosition = new Vector3(enemy.transform.localPosition.x - i * 155, enemy.transform.localPosition.y, enemy.transform.localPosition.z);
            }
            else
            {
                enemy.transform.localPosition = new Vector3(enemy.transform.localPosition.x + i * 155, enemy.transform.localPosition.y, enemy.transform.localPosition.z);
            }
            enemy.AddComponent<EnemyHolder>();
            enemy.GetComponent<EnemyHolder>().SetEnemyData(listOfEnemies[i]);
            enemy.GetComponentInChildren<Text>().text = listOfEnemies[i].name;
            enemy.SetActive(true);
        }
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
            float speed = 1000 - (activeCharacter.speed * 10) + (activeEnemy.speed * 10);
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
            if (currentTime > 3)
            {
                float sizeOfStar = ((float)activeCharacter.defense / listOfEnemies[enemyIndex].attack) * 200;
                star.SetActive(true);
                star.transform.localPosition = new Vector3(
                    Random.Range(-Screen.width / 2, Screen.width / 2),
                    Random.Range(-Screen.height / 2, Screen.height / 2),
                    0);
                star.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeOfStar, sizeOfStar);
                currentTime = 0;
                waitingForAttack = false;
            }
        }
        else
        {
            currentTime += Time.deltaTime;
            if (currentTime > ((float)activeCharacter.speed / listOfEnemies[enemyIndex].speed) + 0.25f)
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
        float pos = (slider.transform.localPosition.y - initialSliderHeight) - heightOfMeter / 2;
        float percentToMiddle = 1 - Mathf.Abs(pos / (heightOfMeter / 2));
        float playerAttack = ((float)activeCharacter.attack / activeEnemy.defense) * percentToMiddle * 10;
        activeEnemy.hp -= Mathf.RoundToInt(playerAttack);
        Debug.Log("Enemy HP: " + activeEnemy.hp);
        if (activeEnemy.hp <= 0)
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox(activeEnemy.name + " has fainted. Delt " + Mathf.RoundToInt(playerAttack) + " damage");
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
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Delt " + Mathf.RoundToInt(playerAttack) + " damage to " + activeEnemy.name);
        }
        enemyHighlightSprite.SetActive(false);
    }

    void EnemyAttack(bool pressed)
    {
        float initialAttack = (float)listOfEnemies[enemyIndex].attack / activeCharacter.defense;
        float enemyAttack = initialAttack;
        star.SetActive(false);
        waitingForAttack = true;
        if (pressed)
        {
            float distance = Vector3.Distance(Input.mousePosition, star.transform.position);
            distance /= star.GetComponent<RectTransform>().sizeDelta.x / 10;
            enemyAttack *= (currentTime / (((float)activeCharacter.speed / listOfEnemies[enemyIndex].speed) + 0.25f)) * distance;
            if (enemyAttack > initialAttack * 5)
            {
                enemyAttack = initialAttack * 5;
            }
        }
        else
        {
            initialAttack *= 5;
        }
        currentTime = 0;
        GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Recieved " + Mathf.RoundToInt(enemyAttack) + " damage from " + listOfEnemies[enemyIndex].name);
        activeCharacter.hp -= Mathf.RoundToInt(enemyAttack);
        Debug.Log("Your HP " + activeCharacter.hp);
        if (activeCharacter.hp <= 0)
        {
            isFighting = false;
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("YOU ARE DEAD");
        }
        if (listOfEnemies[enemyIndex].numberOfAttacks > attackNum + 1)
        {
            attackNum++;
        }
        else
        {
            if (enemyIndex >= listOfEnemies.Count - 1)
            {
                if (listOfEnemies.Count == 1)
                {
                    selectedEnemy = true;
                    activeEnemySprite = GameObject.Find("Enemy(Clone)");
                    activeEnemy = listOfEnemies[0];
                    enemyHighlightSprite.transform.position = activeEnemySprite.transform.position;
                    enemyHighlightSprite.SetActive(true);
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

    public void SelectEnemy(Character character, GameObject enemySprite)
    {
        selectedEnemy = true;
        activeEnemySprite = enemySprite;
        activeEnemy = character;
        enemyHighlightSprite.transform.position = activeEnemySprite.transform.position;
        enemyHighlightSprite.SetActive(true);
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
