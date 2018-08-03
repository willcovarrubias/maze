using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FightSceneController : MonoBehaviour
{
    List<Character> listOfEnemies;
    Character activeCharacter;
    private bool onOffense, isFighting, moveSlider, waitingForAttack;

    int timeForNext = 1;
    float currentTime;

    public GameObject meter, slider, fightButton, star;
    float initialSliderHeight;
    float heightOfMeter;

    int enemyIndex;
    int attackNum;

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
    }

    void Update()
    {
        if (isFighting)
        {
            if (onOffense)
            {
                Offense();
            }
            else
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
            slider.transform.localPosition = new Vector3(
                slider.transform.localPosition.x,
                initialSliderHeight + Mathf.PingPong(Time.time * activeCharacter.speed / listOfEnemies[0].speed * 100, heightOfMeter),
                slider.transform.localPosition.z);
        }
        else
        {
            currentTime += Time.deltaTime;
            if (currentTime > timeForNext)
            {
                onOffense = false;
                currentTime = 0;
                fightButton.GetComponent<Button>().image.color = new Color(0, 0, 0, 0.5f);
            }
        }
    }

    void Defense()
    {
        if (waitingForAttack)
        {
            currentTime += Time.deltaTime;
            if (currentTime > 3) // use enemy speed
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
            if (currentTime > ((float)activeCharacter.speed / listOfEnemies[enemyIndex].speed) + 0.25f) // use stat
            {
                EnemyAttack(false);
            }
        }
    }

    public void ScreenPressed()
    {
        if (isFighting)
        {
            if (onOffense && moveSlider)
            {
                PlayerAttack();
            }
            else if (!onOffense)
            {
                if (waitingForAttack)
                {
                    //take full damage?
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
        float percentage = 1 - Mathf.Abs(pos / (heightOfMeter / 2));
        float playerAttack = ((float)activeCharacter.attack / listOfEnemies[0].defense) * percentage * 10;
        GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Delt " + Mathf.RoundToInt(playerAttack) + " damage to " + listOfEnemies[0].name);
        listOfEnemies[0].hp -= Mathf.RoundToInt(playerAttack);
        Debug.Log("Enemy HP: " + listOfEnemies[0].hp);
        if (listOfEnemies[0].hp <= 0)
        {
            listOfEnemies.RemoveAt(0);
            Debug.Log("Enemy Died");
            if (listOfEnemies.Count == 0)
            {
                GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("ALL ENEMIES DEAD");
                isFighting = false;
            }
        }
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
                onOffense = true;
                moveSlider = true;
                fightButton.GetComponent<Button>().image.color = new Color(0, 0, 0, 0);
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

    void LoadEnemies()
    {
        //Come up with algorithm here to determine amount of enemies and which enemies.
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
