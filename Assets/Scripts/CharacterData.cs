using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterData : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    public bool characterIsAlreadyRecruited;
    public int thisObjectsID;
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
            villageManager.GetComponent<RecruitmentManager>().characterObject.Remove(this.gameObject);
            villageManager.GetComponent<RecruitmentManager>().characterSlots.Remove(transform.parent.gameObject);
            Destroy(transform.parent.gameObject);
            Destroy(gameObject);
        }
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       
    }    
    
}
