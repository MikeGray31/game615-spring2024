using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(UnitScript))]
public class EnemyController : MonoBehaviour
{
    public GameObject enemyGameObject;
    public GameObject enemyStartBattleBoxObject;

    public UnitScript unitScript;
    public event UnityAction<EnemyController> EnemyDied = delegate { };

    public EnemyAttackScript attackScript;
    public EnemyDefendScript defendScript;
    //public Weapon currentWeapon;

    public bool inBattle;

    public bool actionTaken { get; private set; }

    public bool attackFinished;

    public Vector3 idlePosition;
    public Vector3 targetposition;
    public bool movingToPosition;
    public float battleMoveSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        unitScript = GetComponent<UnitScript>();
        idlePosition = transform.position;
        attackScript = GetComponent<EnemyAttackScript>();
        defendScript = GetComponent<EnemyDefendScript>();
        //DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        BattleManagerScript.changeBattleState += UpdateEnemyFromBattleState;
        unitScript.unitDied += EnemyIsDead;
    }

    private void OnDisable()
    {
        BattleManagerScript.changeBattleState -= UpdateEnemyFromBattleState;
        unitScript.unitDied -= EnemyIsDead;
    }

    // Update is called once per frame
    void Update()
    {
        if (inBattle) BattleUpdate();
    }

    public void BattleUpdate()
    {
        if (movingToPosition)
        {
            //Debug.Log("Moving to position: " + targetposition);
            transform.position = Vector3.MoveTowards(transform.position, targetposition, battleMoveSpeed * Time.deltaTime);
        }
    }

    public void UpdateEnemyFromBattleState(BattleState battleState)
    {
        switch (battleState)
        {
            case BattleState.START:
                if (!IsEnemyPartOfBattle()) break;
                inBattle = true;
                actionTaken = false;
                idlePosition = transform.position;
                break;
            case BattleState.NOBATTLE:
                break;
            case BattleState.PLAYERTURN:
                break;
            case BattleState.PLAYERMOVE:
                break;
            case BattleState.ENEMYTURN:
                if (!IsEnemyPartOfBattle()) break;
                actionTaken = false;
                //Debug.Log("It's the enemy's turn!");
                PlayerController target = GetTarget();
                if (target != null) StartEnemyAttack(target);
                else
                {
                    Debug.Log("No player detected to attack!");
                    ActionTaken();
                }
                break;
            case BattleState.ENEMYMOVE:
                break;
            default:
                actionTaken = false;
                break;

        }
    }

    public bool IsEnemyPartOfBattle()
    {
        return BattleManagerScript.enemyList[0] == this;
    }

    public PlayerController GetTarget()
    {
        if (BattleManagerScript.playerList.Count > 0) return BattleManagerScript.playerList[0];
        else return null;
    }

    public void StartEnemyAttack(PlayerController target)
    {
        if (BattleManagerScript.currentState == BattleState.ENEMYTURN)
        {
            ResetAttackStatus();
            target.StartDefend(this);
            StartCoroutine(EnemyAttack(target));
        }
    }

    IEnumerator EnemyAttack(PlayerController target)
    {
        //Debug.Log("attacking " + target.unitScript.name + "!");
        MoveToAttack(target);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, targetposition) == 0);
        movingToPosition = false;
        //if (currentWeapon != null) Debug.Log("Enemy attacks with weapon!");
        attackScript.AttackPattern(target);
        yield return new WaitUntil(() => attackScript.attackFinished == true);
        MoveBackToSide();
        yield return new WaitUntil(() => Vector3.Distance(transform.position, targetposition) == 0);
        movingToPosition = false;
        
        attackFinished = true;
        //Debug.Log("attackFinished = " + attackFinished);

        ActionTaken();
    }


    public void StartEnemyDefend(PlayerController attacker)
    {
        if(defendScript!= null) defendScript.Defend(attacker);
    }

    public void MoveToAttack(PlayerController target)
    {
        float distanceFromTarget = 4f;
        movingToPosition = true;
        if (attackScript.attackRange == EnemyAttackRange.MELEE)
        {
            Vector3 moveAlongThisVector = transform.position - target.transform.position;
            moveAlongThisVector = (moveAlongThisVector.normalized * distanceFromTarget);

            targetposition = target.transform.position + moveAlongThisVector;
            //Debug.Log("Moving to position: " + targetposition);
        }
    }

    public void MoveBackToSide()
    {
        movingToPosition = true;
        targetposition = idlePosition;
    }

    public void ResetAttackStatus()
    {
        attackFinished = false;
    }

    public void ActionTaken()
    {
        actionTaken = true;
    }

    public void EnemyIsDead()
    {
        inBattle = false;
        enemyStartBattleBoxObject.SetActive(false);
        EnemyDied?.Invoke(this);
        Destroy(gameObject);
    }


    public void EnemyUnarmedAttack()
    {

    }
}
