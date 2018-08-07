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
    public GameObject meter, slider, fightButton, star, enemySprite, enemyHighlightSprite, inventoryButton, selectOptionPopUp;
    bool onOffense, isFighting, moveSlider, waitingForAttack, selectedEnemy, pressedButton, pickingOption, pickedOption;
    float initialSliderHeight, heightOfMeter, currentTime, timeForNextEnemyAttack;
    int playerAttackNum, enemyIndex, attackNum;
    int timeForDefense = 1;

    void Start()
    {
        timeForNextEnemyAttack = Random.Range(1.0f, 4.0f);
        heightOfMeter = meter.GetComponent<RectTransform>().rect.height;
        initialSliderHeight = slider.transform.localPosition.y;
        activeCharacter = GameMaster.gameMaster.GetComponent<ActiveCharacterController>().GetActiveCharacter();
        LoadEnemies();
        star.transform.SetAsLastSibling();
        selectOptionPopUp.transform.SetAsLastSibling();
        pickingOption = true;
    }

    void Update()
    {
        if (pickingOption)
        {
            if (pickedOption)
            {
                currentTime += Time.deltaTime;
                if (currentTime > 1)
                {
                    currentTime = 0;
                    pickingOption = false;
                    isFighting = true;
                }
            }
        }
        if (isFighting && !pickingOption)
        {
            if (onOffense && selectedEnemy)
            {
                Offense();
            }
            else if (!onOffense)
            {
                Defense();
            }
            if (Input.GetMouseButtonUp(0))
            {
                pressedButton = false;
            }
        }
        if (!isFighting && !pickingOption)
        {
            //win or lose
            //if lose, deduct lives, lose inventory items, show button to go back to the village
            //if win, calculate exp, show rooms button
        }
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
            if (currentTime > timeForDefense)
            {
                currentTime = 0;
                selectedEnemy = false;
                playerAttackNum++;
                inventoryButton.SetActive(false);
                if (playerAttackNum >= activeCharacter.numberOfAttacks)
                {
                    onOffense = false;
                    fightButton.GetComponent<Button>().image.color = new Color(0, 0, 0, 0.5f);
                    GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Defend yourself!");
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
                float sizeOfStar = ((float)(activeCharacter.speed + activeCharacter.speed) /
                                    (activeCharacter.speed + listOfEnemies[enemyIndex].EnemyData.speed)) * 400;
                star.SetActive(true);
                star.transform.localPosition = new Vector3(
                    Random.Range(-Screen.width / 2, Screen.width / 2),
                    Random.Range(-Screen.height / 2, Screen.height / 2),
                    0);
                star.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeOfStar, sizeOfStar);
                currentTime = 0;
                waitingForAttack = false;
                timeForNextEnemyAttack = Random.Range(1.0f, 4.0f);
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
        if (isFighting && !pressedButton)
        {
            pressedButton = true;
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
        int playerAttack = CalculatePlayerAttack();
        activeEnemy.EnemyHP -= playerAttack;
        moveSlider = false;
        currentTime = 0;
        Debug.Log("Enemy HP: " + activeEnemy.EnemyHP);
        if (activeEnemy.EnemyHP <= 0)
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox(
                "Delt " + Mathf.RoundToInt(playerAttack) + " damage. " + activeEnemy.EnemyData.name + " has fainted.");
            listOfEnemies.Remove(activeEnemy);
            activeEnemySprite.GetComponent<EnemyHolder>().Dead();
            activeEnemySprite.GetComponentInChildren<Text>().text = "Dead";
            if (listOfEnemies.Count == 0)
            {
                GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("All enemies have fainted");
                isFighting = false;
                inventoryButton.SetActive(true);
            }
        }
        else
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox(
                "Delt " + Mathf.RoundToInt(playerAttack) + " damage to " + activeEnemy.EnemyData.name);
        }
        enemyHighlightSprite.SetActive(false);
    }

    void EnemyAttack(bool pressed)
    {
        int enemyAttack = CalculateEnemyAttack(pressed);
        currentTime = 0;
        GameMaster.gameMaster.GetComponent<ActiveCharacterController>().DecreaseHP(enemyAttack);
        if (activeCharacter.currentHP <= 0)
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox(
                "Recieved " + enemyAttack + " damage from " + listOfEnemies[enemyIndex].EnemyData.name + ". You have fainted.");
            isFighting = false;
        }
        else
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox(
                "Recieved " + enemyAttack + " damage from " + listOfEnemies[enemyIndex].EnemyData.name);
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
                inventoryButton.SetActive(true);
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

    int CalculatePlayerAttack()
    {
        float posOfSlider = (slider.transform.localPosition.y - initialSliderHeight) - heightOfMeter / 2;
        float percentToMiddle = 1 - Mathf.Abs(posOfSlider / (heightOfMeter / 2));
        float playerAttack = ((float)(activeCharacter.attack * activeCharacter.attack) /
                              (activeCharacter.attack + activeEnemy.EnemyData.defense)) * percentToMiddle;
        return Mathf.RoundToInt(playerAttack);
    }

    int CalculateEnemyAttack(bool pressed)
    {
        float initialAttack = (float)(listOfEnemies[enemyIndex].EnemyData.attack * listOfEnemies[enemyIndex].EnemyData.attack) /
            (listOfEnemies[enemyIndex].EnemyData.attack + activeCharacter.defense);
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
        return Mathf.RoundToInt(enemyAttack);
    }

    public void SelectEnemy(Enemy character, GameObject enemySprite)
    {
        if (onOffense && isFighting && enemySprite.GetComponent<EnemyHolder>().IsLiving())
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

    public void UseItemFromInventory()
    {
        if (isFighting)
        {
            moveSlider = false;
            currentTime = 0;
            enemyHighlightSprite.SetActive(false);
            onOffense = false;
            fightButton.GetComponent<Button>().image.color = new Color(0, 0, 0, 0.5f);
            inventoryButton.SetActive(false);
        }
    }

    public void SelectFight()
    {
        GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Let's fight!");
        pickedOption = true;
        selectOptionPopUp.SetActive(false);
        currentTime = 0;
        playerAttackNum = 0;
        onOffense = true;
        moveSlider = true;
        waitingForAttack = true;
        inventoryButton.SetActive(true);
        selectedEnemy = false;
        if (listOfEnemies.Count == 1)
        {
            AutoHighlightEnemy();
        }
    }

    public void SelectSneak()
    {
        int maxAmount = 50;
        for (int i = 0; i < listOfEnemies.Count; i++)
        {
            maxAmount += listOfEnemies[i].EnemyData.luck;
        }
        if (Random.Range(0, maxAmount) > activeCharacter.luck)
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Sneak successful!");
            GoToPathRoom();
        }
        else
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("You've been spotted!");
            pickedOption = true;
            selectOptionPopUp.SetActive(false);
            currentTime = 0;
            playerAttackNum = 0;
            onOffense = false;
            waitingForAttack = true;
            fightButton.GetComponent<Button>().image.color = new Color(0, 0, 0, 0.5f);
        }
    }

    void LoadEnemies()
    {
        listOfEnemies = GameMaster.gameMaster.GetComponent<CharacterDatabase>().GetEnemiesForFightScene();
        GameObject.Find("Manager").GetComponent<CreateDynamicInventory>().CreateForFightScene(listOfEnemies);
    }

    public bool IsFighting()
    {
        return isFighting;
    }

    public bool IsPickingOption()
    {
        return pickingOption;
    }

    public void OpenInventory()
    {
        GameMaster.gameMaster.GetComponent<InventoryManager>().OpenInventoryPanelUI();
    }

    public void GoToVillage()
    {
        GameObject.Find("Manager").GetComponent<CreateDynamicInventory>().DestroyDynamicPanels();
        SceneManager.LoadScene("VillageScene");
    }

    public void GoToPathRoom()
    {
        GameObject.Find("Manager").GetComponent<CreateDynamicInventory>().DestroyDynamicPanels();
        GameMaster.gameMaster.GetComponent<ActiveCharacterController>().GiveExpForBattleToActiveCharacter();
        SceneManager.LoadScene("PathScene");
    }
}
