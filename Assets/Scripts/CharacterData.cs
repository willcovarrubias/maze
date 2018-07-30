using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterData : MonoBehaviour, IPointerDownHandler, IPointerUpHandler { 
 

    public int amount;
    public Character character;


    void Start()
    {
        
    }


     
    public void OnPointerDown(PointerEventData eventData)
    {
        GameMaster.gameMaster.GetComponent<CharacterDatabase>().RecruitHero(character);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       
    }    
    
}
