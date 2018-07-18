using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    public static GameMaster gameMaster;

    public void Awake()
    {
        if (gameMaster == null)
        {
            DontDestroyOnLoad(gameObject);
            gameMaster = this;
        }
        else if (gameMaster != this)
        {
            Destroy(gameObject);
        }
    }
}
