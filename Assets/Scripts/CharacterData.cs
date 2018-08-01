using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterData : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    public bool characterIsAlreadyRecruited;
    public int thisCharactersID;
    public Character character;
    GameObject villageManager;


    void Start()
    {
        villageManager = GameObject.FindGameObjectWithTag("VillageSceneManager");
    }


     
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!characterIsAlreadyRecruited)
        {
            //TODO: Add some checks here to make sure there is room on the roster.
            GameMaster.gameMaster.GetComponent<CharacterDatabase>().RecruitHero(character);
            //villageManager.GetComponent<RecruitmentManager>().RemoveWanderer();
            Destroy(transform.parent.gameObject);
        }
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       
    }    
    
}
