using UnityEngine;
using static Communication.Proto.MessageToClient.Types;  //later find that this can simplify the code
using System.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using Communication.Proto;

public class PlayerManager : MonoBehaviour
{

    public GameObject team1Player = null;
    public GameObject team2Player = null;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public string Jkk()
    {
        return "hhhhh";
    }

    public GameObject PlayerMap(GameObjMessage playerMsg)
    {
        //Debug.Log("3");
        GameObject oneteamPlayer;

        oneteamPlayer = team1Player;
        //Debug.Log("TeamID");
        //Debug.Log(playerMsg.MessageOfCharacter.TeamID);
        //Debug.Log("There");
        //Debug.Log(playerMsg.MessageOfCharacter.Guid);


        if (playerMsg.MessageOfCharacter.TeamID == 1)
        {
            oneteamPlayer = team2Player;
        }        
        return oneteamPlayer;
    }
}
