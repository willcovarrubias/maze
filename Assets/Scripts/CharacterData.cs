using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterData : MonoBehaviour, IPointerDownHandler, IPointerUpHandler { 
 

    public int thisCharactersID;
    public Character character;


    void Start()
    {
        
    }


     
    public void OnPointerDown(PointerEventData eventData)
    {
        //TODO: Add some checks here to make sure there is room on the roster.
        GameMaster.gameMaster.GetComponent<CharacterDatabase>().RecruitHero(character);
        Destroy(transform.parent.gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       
    }    
    
}
