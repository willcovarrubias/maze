using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightSceneController : MonoBehaviour {

    Character enemy1, enemy2;
    List<Character> listOfEnemies;


     
    private bool onOffense;
    private bool isFighting;

    private void Start()
    {
        Debug.Log("You're in the Fight scene!");
        listOfEnemies = GameMaster.gameMaster.GetComponent<CharacterDatabase>().GetListofEnemies();

    }

    private void Update()
    {
        if (isFighting)
        {
            if (onOffense)
            {
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

    void EnemyAttacks()
    {
        
    }

    void LoadEnemies()
    {
        //Come up with algorithm here to determine amount of enemies and which enemies.
        enemy1 = listOfEnemies[0];
        enemy2 = listOfEnemies[1];
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
