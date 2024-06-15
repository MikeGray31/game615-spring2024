using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public PlayerController player;
    public bool followPlayer;

    private void Awake()
    {
        followPlayer = true;
        player = FindObjectOfType<PlayerController>();
    }

    private void OnEnable()
    {
        BattleManagerScript.changeBattleState += ChangeCameraFromState;
        player.PlayerSwitchedRooms += ChangeRoomCamera;
    }

    private void OnDisable()
    {
        BattleManagerScript.changeBattleState -= ChangeCameraFromState;
    }

    private void Update()
    {
        if(player != null && followPlayer)
        {
            transform.LookAt(player.gameObject.transform);
        }
    }


    public void ChangeCameraFromState(BattleState state)
    {
        switch (state)
        {
            case BattleState.NOBATTLE:
                //Debug.Log("no battle!");
                break;
            case BattleState.START:
                //Debug.Log("Camera Start Battle!");
                CameraStartBattle();
                break;
            case BattleState.PLAYERTURN:
                break;
            case BattleState.ENEMYTURN:
                break;
            case BattleState.LOST:
                break;
            case BattleState.WON:
                break;
            case BattleState.BATTLEEND:
                //Debug.Log("Camera End Battle!");
                ReturnToExploration();
                break;
            default:
                break;
        }

    }

    private void CameraStartBattle()
    {
        followPlayer = false;
        BattleManagerScript.instance.CameraPreBattlePosition = transform.position;
        BattleManagerScript.instance.CameraPreBattleRotation = transform.rotation;
        transform.position = BattleManagerScript.instance.CameraBattleSpot.position;
        transform.rotation = BattleManagerScript.instance.CameraBattleSpot.rotation;
    }

    public void ReturnToExploration()
    {
        followPlayer = true;
        transform.position = BattleManagerScript.instance.CameraPreBattlePosition;
        transform.rotation = BattleManagerScript.instance.CameraPreBattleRotation;
    }


    public void ChangeRoomCamera(CameraTrigger ct) 
    {
        //Debug.Log("Camera should change position right now!");
        transform.position = ct.CameraPoint.position;
    }
}
