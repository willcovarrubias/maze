using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterData : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler {

    public bool characterIsAlreadyRecruited;
    public int thisObjectsID;
    public Character character;
    GameObject villageManager;
    Character currentlyClickedCharacter;

    bool beingDragged = false;

    void Start()
    {
        villageManager = GameObject.FindGameObjectWithTag("VillageSceneManager");
    }


     
    public void OnPointerDown(PointerEventData eventData)
    {
        beingDragged = false;
        if (!characterIsAlreadyRecruited)
        {
            CaravanPopUpOpen();

            /*//TODO: Add some checks here to make sure there is room on the roster.
            GameMaster.gameMaster.GetComponent<CharacterDatabase>().RecruitHero(character);
            */
        }
        else
        {
        }
        
    }

    public void DestroyExWandererObjects()
    {
        villageManager.GetComponent<RecruitmentManager>().characterObject.Remove(this.gameObject);
        villageManager.GetComponent<RecruitmentManager>().characterSlots.Remove(transform.parent.gameObject);
        Destroy(transform.parent.gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!beingDragged && characterIsAlreadyRecruited)
        {
            BarracksPopUpOpen();

        }
    }

    void BarracksPopUpOpen()
    {
        villageManager.GetComponent<RosterManager>().RosterAdvancedUIOpen();
        villageManager.GetComponent<RosterManager>().PopulateBarracksPopUp(character);
        //currentlyClickedCharacter = character;
        villageManager.GetComponent<RosterManager>().SetCurrentlyClickedCharacter(character);
    }

    void CaravanPopUpOpen()
    {
        currentlyClickedCharacter = character;
        villageManager.GetComponent<RecruitmentManager>().CaravanAdvancedUIOpen();
        villageManager.GetComponent<RecruitmentManager>().PopulateCaravanPopUp(character);
        villageManager.GetComponent<RecruitmentManager>().SetCurrentlyClickedCharacter(character, this.gameObject);


    }



    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        beingDragged = true;
    }
}
