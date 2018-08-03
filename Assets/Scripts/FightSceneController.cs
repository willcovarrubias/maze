using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FightSceneController : MonoBehaviour
{
    List<Character> listOfEnemies;
    Character activeCharacter;
    private bool onOffense, isFighting, moveSlider;

    int timeForNext = 1;
    float currentTime;

    //Offence UI
    public GameObject meter, slider, fightButton;
    float initialSliderHeight;
    float heightOfMeter;

    void Start()
    {
        isFighting = true;
        onOffense = true;
        moveSlider = true;
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
                        fightButton.GetComponent<Button>().image.color = new Color(0, 0, 0, 0.5f);
                    }
                }
                //Player's turn to attack with meter.
                //Meter UI pops up. It moves up and down.
                //Player taps to stop the meter. His damage is calculated.
                //Enemy's vitals are updated.
                //Message is displayed for how much damage was dealt.
                //Check if 1 enemy is still alive, if so set onOffense to false. If not, isFighting is false;
                //onOffense is set to false.
            }
            else
            {
                //Player's turn to defense (with stars).
                //Calculate number of attacks.
                //Screen dims.
                //Begin timer.
                //Star UI pops up depending on timer (randomization?)
                //Player taps star(s) and their damage taken is calculated.
                //Character's vitals are  updated.
                //Results are displayed to the player.
                //onOffense is set to true.
            }
        }
        //Determine rewards, calculate EXP, etc.
        //End fight.?
    }

    public void ScreenPressed()
    {
        if (isFighting)
        {
            if (onOffense && moveSlider)
            {
                moveSlider = false;
                currentTime = 0;
                float pos = (slider.transform.localPosition.y - initialSliderHeight) - heightOfMeter / 2;
                float percentage = 1 - Mathf.Abs(pos / (heightOfMeter / 2));
                float playerAttack = ((float)activeCharacter.attack / listOfEnemies[0].defense) * percentage * 10;
                GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("Delt " + Mathf.RoundToInt(playerAttack) + " damage");
                listOfEnemies[0].hp -= Mathf.RoundToInt(playerAttack);
                Debug.Log("Enemy HP: " + listOfEnemies[0].hp);
                if (listOfEnemies[0].hp <= 0)
                {
                    listOfEnemies.RemoveAt(0);
                    Debug.Log("Enemy Died");
                    if (listOfEnemies.Count == 0)
                    {
                        GameMaster.gameMaster.GetComponent<InventoryManager>().ChangeDialogBox("FIGHTING OVER");
                        isFighting = false;
                    }
                }
            }
            else if (!onOffense)
            {
                onOffense = true;
                moveSlider = true;
                fightButton.GetComponent<Button>().image.color = new Color(0, 0, 0, 0);
            }
        }
    }

    void EnemyAttacks()
    {

    }

    void LoadEnemies()
    {
        //Come up with algorithm here to determine amount of enemies and which enemies.
    }

    void CalculateEnemyAttackRounds()
    {

    }
    public void Attack()
    {
    }

    public void Guard()
    {
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
