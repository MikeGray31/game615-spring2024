using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject playerBattleUI;
    public GameObject playerTurnUIElements;
    public GameObject playerExploreUI;

    // Start is called before the first frame update
    void Start()
    {
        //DisplayBattleUI(false);
    }

    private void OnEnable()
    {
        //Debug.Log("UI Subscribing to BattleManager!");
        BattleManagerScript.changeBattleState += UpdateUIFromBattleState;
    }

    private void OnDisable()
    {
        BattleManagerScript.changeBattleState -= UpdateUIFromBattleState;
    }

    public void UpdateUIFromBattleState(BattleState battleState)
    {
        playerTurnUIElements.SetActive(false);

        switch(battleState)
        {
            case BattleState.START:
                //Debug.Log("DisplayBattleUI should happen right now!");
                DisplayBattleUI(true);
                DisplayPlayerExploreUI(false);
                break;
            case BattleState.NOBATTLE:
                //Debug.Log("UIManager reacting to BattleState.NOBATTLE!");
                DisplayBattleUI(false);
                DisplayPlayerExploreUI(true);
                break;
            case BattleState.PLAYERTURN:
                //Debug.Log("Displaying playerUI!");
                DisplayPlayerUI(true);
                break;
            case BattleState.PLAYERMOVE:
                break;
            case BattleState.ENEMYTURN:
                DisplayPlayerUI(false);
                break;
            case BattleState.ENEMYMOVE:
                break;
            case BattleState.LOST:
                DisplayPlayerUI(false);
                break;
            case BattleState.WON:
                DisplayPlayerUI(false);
                break;
            default:
                break;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayBattleUI(bool displayIt)
    {
        playerBattleUI.SetActive(displayIt);
    }

    public void DisplayPlayerUI(bool displayIt)
    {
        //Debug.Log("Displaying PlayerUI!");
        playerTurnUIElements.SetActive(displayIt);
    }

    public void DisplayPlayerExploreUI(bool displayIt)
    {
        playerExploreUI.SetActive(displayIt);
    }

    public PlayerController GetCurrentPlayer()
    {
        if (BattleManagerScript.playerList.Count > 0)
        {
            return BattleManagerScript.playerList[0];
        }

        else return null;
    }

    public EnemyController GetTargetEnemy()
    {
        if (BattleManagerScript.enemyList.Count > 0) return BattleManagerScript.enemyList[0];
        else return null;
    }

    public void PlayerAttackButtonHit()
    {
        //Debug.Log("Attack button hit!");
        PlayerController currentPlayer = GetCurrentPlayer();
        EnemyController target = GetTargetEnemy();
        if(currentPlayer != null && target != null) currentPlayer.StartAttack(target);
    }
    
    public void PlayerHealButtonHit()
    {
        //Debug.Log("Absorb button hit!");
        PlayerController currentPlayer = GetCurrentPlayer();
        if (currentPlayer != null) currentPlayer.AbsorbAttacks();
    }
    
    public void PlayerWaitButtonHit()
    {
        //Debug.Log("Wait button hit!");
        PlayerController currentPlayer = GetCurrentPlayer();
        if (currentPlayer != null) currentPlayer.PlayerWait();
    }


}