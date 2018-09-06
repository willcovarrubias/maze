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
    int playerAtkStat, playerDefStat, playerSpecStat, playerSpdStat;

    void Start()
    {
        timeForNextEnemyAttack = Random.Range(1.0f, 4.0f);
        heightOfMeter = meter.GetComponent<RectTransform>().rect.height;
        initialSliderHeight = slider.transform.localPosition.y;
        activeCharacter = GameMaster.gameMaster.GetComponent<ActiveCharacterController>().GetActiveCharacter();
        UpdatePlayerStats();
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
            float speed = 1000 - (playerSpdStat * 10) + (activeEnemy.EnemyData.speed * 10);
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
                float sizeOfStar = ((float)(playerSpdStat + playerSpdStat + 1) /
                                    (playerSpdStat + listOfEnemies[enemyIndex].EnemyData.speed)) * 400;
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
            if (currentTime > (((float)(playerSpdStat + playerSpdStat) / (playerSpdStat + listOfEnemies[enemyIndex].EnemyData.speed)) + 0.25f))
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
        string dialog = "";
        int playerAttack = CalculatePlayerAttack();
        string weaponDestroyedString = DeductDuribiltyOfWeapon();
        activeEnemy.EnemyHP -= playerAttack;
        moveSlider = false;
        currentTime = 0;
        Debug.Log("Enemy HP: " + activeEnemy.EnemyHP);
        dialog += "Delt " + Mathf.RoundToInt(playerAttack) + " damage to " + activeEnemy.EnemyData.name + ". ";
        if (activeEnemy.EnemyHP <= 0)
        {
            dialog += activeEnemy.EnemyData.name + " has fainted. ";
            listOfEnemies.Remove(activeEnemy);
            activeEnemySprite.GetComponent<EnemyHolder>().Dead();
            activeEnemySprite.GetComponentInChildren<Text>().text = "Dead";
            if (listOfEnemies.Count == 0)
            {
                dialog += "All enemies have fainted. ";
                isFighting = false;
                inventoryButton.SetActive(true);
            }
        }
        if (weaponDestroyedString != "")
        {
            dialog += weaponDestroyedString + " has been destroyed.";
        }
        GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox(dialog);
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
                "Recieved " + enemyAttack + " damage from " + listOfEnemies[enemyIndex].EnemyData.name + ". " + activeCharacter.name + " has fainted.");
            isFighting = false;
            StartCoroutine(Fainted());
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
        float playerAttack = ((float)(playerAtkStat * playerAtkStat) /
                              (playerAtkStat + activeEnemy.EnemyData.defense)) * percentToMiddle;
        return Mathf.RoundToInt(playerAttack);
    }

    int CalculateEnemyAttack(bool pressed)
    {
        float initialAttack = (float)(listOfEnemies[enemyIndex].EnemyData.attack * listOfEnemies[enemyIndex].EnemyData.attack) /
            (listOfEnemies[enemyIndex].EnemyData.attack + playerDefStat);
        float enemyAttack = initialAttack;
        star.SetActive(false);
        waitingForAttack = true;
        if (pressed)
        {
            float distance = Vector3.Distance(Input.mousePosition, star.transform.position);
            float timePercentage = currentTime / (((float)playerSpdStat / listOfEnemies[enemyIndex].EnemyData.speed) + 0.25f);
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

    public void UpdatePlayerStats()
    {
        playerAtkStat = activeCharacter.attack;
        playerSpecStat = activeCharacter.special;
        playerDefStat = activeCharacter.defense;
        playerSpdStat = activeCharacter.speed;
        if (GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedWeaponID() > 0)
        {
            playerAtkStat += GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedWeapon().Attack;
            playerSpecStat += GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedWeapon().Special;
            playerSpdStat += GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedWeapon().Speed;
        }
        if (GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedHatID() > 0)
        {
            playerDefStat += GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedHat().Defense;
            playerSpdStat += GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedHat().Speed;
        }
        if (GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedBodyID() > 0)
        {
            playerDefStat += GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedBody().Defense;
            playerSpdStat += GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedBody().Speed;
        }
    }

    string DeductDuribiltyOfWeapon()
    {
        if (GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedWeaponID() > 0)
        {
            int id = GameMaster.gameMaster.GetComponent<InventoryManager>().GetEquippedWeaponID();
            Inventory equippedWeaponInv = GameMaster.gameMaster.GetComponent<InventoryManager>().playerItems[id];
            Weapons equippedWeapon = (Weapons)equippedWeaponInv.Item;
            int slotNum = equippedWeaponInv.SlotNum;
            string weaponName = equippedWeapon.Title;
            equippedWeapon.Durability--;
            if (equippedWeapon.Durability <= 0)
            {
                GameMaster.gameMaster.GetComponent<InventoryManager>().RemoveItemsFromInventory(equippedWeaponInv, 1, slotNum);
                UpdatePlayerStats();
                return weaponName;
            }
            GameMaster.gameMaster.GetComponent<InventoryManager>().SaveInventory();
        }
        return "";
    }

    IEnumerator Fainted()
    {
        GameMaster.gameMaster.GetComponent<ActiveCharacterController>().DecreaseLives();
        if (GameMaster.gameMaster.GetComponent<ActiveCharacterController>().GetActiveCharacter().lives > 0)
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().LoseRandomAmountOfItems();
        }
        else
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().LoseAllItems();
        }
        yield return new WaitForSeconds(3);
        if (GameMaster.gameMaster.GetComponent<ActiveCharacterController>().GetActiveCharacter().lives > 0)
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("You somehow appear back in the village with some items gone! " + activeCharacter.name + " loses a life.");
        }
        else
        {
            GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox(activeCharacter.name + " is dead. Lost all items.");
            GameMaster.gameMaster.GetComponent<CharacterDatabase>().DeleteHero(GameMaster.gameMaster.GetComponent<CharacterDatabase>().activeCharacter);
        }
        GoToVillage();
    }

    void LoadEnemies()
    {
        listOfEnemies = GameMaster.gameMaster.GetComponent<CharacterDatabase>().GetEnemiesForFightScene(GameMaster.gameMaster.roomCount);
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
        GameMaster.gameMaster.GetComponent<ActiveCharacterController>().GiveExpToActiveCharacter(50);
        SceneManager.LoadScene("PathScene");
    }
}
