using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum BattleState {NOBATTLE, START, PLAYERTURN, PLAYERMOVE, ENEMYTURN, ENEMYMOVE, WON, LOST, BATTLEEND }

public class BattleManagerScript : MonoBehaviour
{
    public static BattleManagerScript instance;

    //public LevelEnemiesScript LevelEnemies;
    public static BattleState currentState { get; private set; } = BattleState.NOBATTLE;

    public static List<EnemyController> enemiesInLevel;

    public static event UnityAction<BattleState> changeBattleState = delegate { };

    public static List<PlayerController> playerList;
    public static List<EnemyController> enemyList;

    //public EnemyStatus enemyStatus;

    private Coroutine playerTurnCoroutine;
    private Coroutine enemyTurnCoroutine;

    public Transform PlayerBattleSpot;
    public Transform EnemyBattleSpot;
    public Transform CameraBattleSpot;

    public Vector3 PlayerPreBattleTransformPosition;
    public Quaternion PlayerPreBattleTransformRotation;

    public Vector3 EnemyPreBattleTransformPosition;
    public Quaternion EnemyPreBattleTransformRotation;

    public Vector3 CameraPreBattlePosition;
    public Quaternion CameraPreBattleRotation;

    void Awake()
    {
        instance = this;
        //DontDestroyOnLoad(this.gameObject);
        enemiesInLevel = new List<EnemyController>();
        
        enemyList = new List<EnemyController>();
        playerList = new List<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Battle should be starting!");
        SetBattleState(BattleState.NOBATTLE);
        
        //StartCoroutine(StartBattleByKeyDownCoroutine());
    }

    IEnumerator StartBattleByKeyDownCoroutine()
    {
        //yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Q));
        yield return new WaitForSeconds(0.2f);
        //Debug.Log("This should only play once!");
        StartBattle();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Current state is: " + currentState);
        }
    }

    private void OnEnable()
    {
        changeBattleState += ChangeBattleManagerFromState;

        /*foreach (EnemyController ec in LevelEnemies.enemiesInLevel)
        {
            ec.EnemyDied += DoNotSpawnEnemy;
        }*/
    }

    private void OnDisable()
    {
        changeBattleState -= ChangeBattleManagerFromState;

        /*foreach (EnemyController ec in LevelEnemies.enemiesInLevel)
        {
            ec.EnemyDied -= DoNotSpawnEnemy;
        }*/
    }

    //Use only this function when you need to change the Battle State!
    private void SetBattleState(BattleState state)
    {
        currentState = state;
        changeBattleState?.Invoke(state);
    }

    public void ChangeBattleManagerFromState(BattleState state)
    {
        switch (state)
        {
            case BattleState.NOBATTLE:
                //Debug.Log("no battle!");
                break;
            case BattleState.START:
                playerList = GetPlayerList();
                //enemyList = GetEnemyList();
                StartCoroutine(DetermineStartingUnit());
                break;
            case BattleState.PLAYERTURN:
                playerTurnCoroutine = StartCoroutine(PlayerTurn());
                break;
            case BattleState.ENEMYTURN:
                enemyTurnCoroutine = StartCoroutine(EnemyTurn());
                break;
            case BattleState.LOST:
                EndBattle(state);
                break;
            case BattleState.WON:
                EndBattle(state);
                break;
            case BattleState.BATTLEEND:
                BattleEnd();
                break;
            default:
                break;
        }

    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().name == LevelLoader.instance.currentExploreLevel)
        {
            /*LevelEnemies.enemiesInLevel.Clear();
            LevelEnemies.enemiesInLevelStatuses.Clear();

            LevelEnemies.enemiesInLevel.AddRange(FindObjectsByType<EnemyController>(FindObjectsSortMode.None));
            LevelEnemies.levelName = LevelLoader.instance.currentExploreLevel;

            foreach (EnemyController ec in LevelEnemies.enemiesInLevel)
            {
                ec.EnemyDied += DoNotSpawnEnemy;
                LevelEnemies.enemiesInLevelStatuses.Add(new EnemyStatus(ec));
            }*/
        }
    }

    public void SetUpBattle(PlayerController player, EnemyController enemy)
    {
        //Instantiate(enemyStatus.enemyGameObject, new Vector3(-10, 0, 0), Quaternion.Euler(0,-90,0));
        foreach (EnemyController ec in enemiesInLevel)
        {
            if (ec != null && ec != enemy) 
            {
                ec.gameObject.SetActive(false);
            }
            else if(ec != null && ec == enemy)
            {
                ec.gameObject.SetActive(true);
            }
        }

        //moving the enemy to position
        EnemyPreBattleTransformPosition = enemy.transform.position;
        //EnemyPreBattleTransformRotation = enemy.transform.rotation;
        enemy.transform.position = EnemyBattleSpot.position;
        enemy.transform.rotation = EnemyBattleSpot.rotation;
        enemyList = GetSingleEnemyList(enemy);

        //moving player to position
        PlayerPreBattleTransformPosition = player.transform.position;
        //PlayerPreBattleTransformRotation = player.transform.rotation;
        player.cc.enabled = false;
        player.transform.position = PlayerBattleSpot.position;
        player.transform.rotation = PlayerBattleSpot.rotation;
        player.cc.enabled = true;
        //Debug.Log("Player position = " + player.transform.position);

        SetBattleState(BattleState.START);

    }

    public void SetEnemyBattleData(PlayerController player, EnemyController enemy)
    {
        /*enemyStatus.currentHealth = enemy.unitScript.unitHealth;
        enemyStatus.maxHealth = enemy.unitScript.maxHealth;
        enemyStatus.position[0] = enemy.gameObject.transform.position.x;
        enemyStatus.position[1] = enemy.gameObject.transform.position.y;
        enemyStatus.position[2] = enemy.gameObject.transform.position.z;
        enemyStatus.enemyGameObject = enemy.enemyGameObject;
        DontDestroyOnLoad(enemyStatus.enemyGameObject);*/
        
    }



    public void StartBattle()
    {
        SetBattleState(BattleState.START);
    }

    public List<PlayerController> GetPlayerList()
    {
        List<PlayerController> players = new List<PlayerController>();
        players.AddRange(FindObjectsByType<PlayerController>(FindObjectsSortMode.None));
        
        foreach(PlayerController player in players)
        {
            player.PlayerDied += APlayerIsDead;
        }

        return players;
    }
    
    public List<EnemyController> GetEnemyList()
    {
        List<EnemyController> enemies = new List<EnemyController>();
        //enemies.AddRange(FindObjectsByType<EnemyController>(FindObjectsSortMode.None));
        
        foreach (EnemyController enemy in enemies)
        {
            enemy.EnemyDied += AnEnemyIsDead;
        }
        return enemies;
    }

    public List<EnemyController> GetSingleEnemyList(EnemyController enemy)
    {
        List<EnemyController> enemies = new List<EnemyController>();
        enemies.Add(enemy);
        
        foreach (EnemyController e in enemies)
        {
            e.EnemyDied += AnEnemyIsDead;
        }
        return enemies;
    }

    public IEnumerator DetermineStartingUnit()
    {
        yield return new WaitForEndOfFrame();
        if (true)
        {
            SetBattleState(BattleState.PLAYERTURN);
        }
    }

    IEnumerator PlayerTurn()
    {
        PlayerController currentPlayer = SetupPlayerTurn();
        yield return new WaitUntil(() => currentPlayer.actionSelected == true);
        //Debug.Log("Player action selected!");
        //something goes here!
        yield return new WaitUntil(() => currentPlayer.actionTaken == true);
        EndPlayerTurn();
    }

    public PlayerController SetupPlayerTurn()
    {
        //Debug.Log("Setting up Player Turn!");
        return playerList[0];
    }

    public void EndPlayerTurn()
    {
        //Debug.Log("Ending Player Turn!");
        SetBattleState(BattleState.ENEMYTURN);
    }

    public IEnumerator EnemyTurn()
    {
        if (SetupEnemyTurn() != null)
        {
           
            EnemyController enemy = SetupEnemyTurn();
            //Debug.Log("enemy = " + enemy);
            yield return new WaitUntil(() => enemy.actionTaken == true);
            //Debug.Log("Enemy has taken its action!");
            yield return new WaitForSeconds(2f);
            EndEnemyTurn();
        }
        else Debug.Log("No enemy left for enemy turn");

    }

    public EnemyController SetupEnemyTurn()
    {
        //Debug.Log("Setting up Enemy Turn!");
        if (enemyList.Count != 0) 
        {
            return enemyList[0];
        }
        return null;
    }

    public void EndEnemyTurn()
    {
        //Debug.Log("Ending Enemy Turn!");
        //turn off player battle menu
        SetBattleState(BattleState.PLAYERTURN);
    }

    public void APlayerIsDead(PlayerController player)
    {
        if (playerList.Contains(player))
        {
            playerList.Remove(player);
            
        }
        if (playerList.Count == 0)
        {
           Debug.Log("All players are dead");
           SetBattleState(BattleState.LOST);
        }
    }

    public void AnEnemyIsDead(EnemyController enemy)
    {
        if (enemyList.Contains(enemy))
        {   
            enemyList.Remove(enemy);
        }
        if (enemyList.Count == 0 && playerList.Count != 0)
        {
            Debug.Log("All enemies defeated!");
            SetBattleState(BattleState.WON);
        }
    }


    public void EndBattle(BattleState winOrLose)
    {
        if (playerTurnCoroutine != null) StopCoroutine(playerTurnCoroutine);
        if (enemyTurnCoroutine  != null) StopCoroutine(enemyTurnCoroutine);

        if (winOrLose == BattleState.WON) PlayerWon();
        else if(winOrLose == BattleState.LOST) PlayerLost();


    }

    public void PlayerWon()
    {
        Debug.Log("Player won!");
        //LevelLoader.instance.LoadNamedLevel(LevelLoader.instance.currentExploreLevel, false);
        ReturnToExploration();
        SetBattleState(BattleState.BATTLEEND);
    }

    public void PlayerLost()
    {
        Debug.Log("Enemy won!");
        GameManagerScript.instance.GameOver();
        //SetBattleState(BattleState.BATTLEEND);
    }


    public void BattleEnd()
    {
        //Debug.Log("BattleEnd() called!");
        SetBattleState(BattleState.NOBATTLE);
    }

    public void ReturnToExploration()
    {
        //Debug.Log("Returning player to exploration");
        PlayerController player = playerList[0];
        player.cc.enabled = false;
        player.transform.position = PlayerPreBattleTransformPosition;
        player.transform.rotation = PlayerPreBattleTransformRotation;
        player.cc.enabled = true;


    }
}



/*public void Update[WhateverEntity]FromBattleState(BattleState battleState)
{

    switch (battleState)
    {
        case BattleState.START:
            break;
        case BattleState.NOBATTLE:
            break;
        case BattleState.PLAYERTURN:
            break;
        case BattleState.PLAYERMOVE:
            break;
        case BattleState.ENEMYTURN:
            break;
        case BattleState.ENEMYMOVE:
            break;
        case BattleState.LOST:
            break;
        case BattleState.WON:
            break;
        default:
            break;

    }
}*/