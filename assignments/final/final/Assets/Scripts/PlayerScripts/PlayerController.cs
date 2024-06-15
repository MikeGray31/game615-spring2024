using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(UnitScript))]
public class PlayerController : MonoBehaviour
{
    public CharacterStatus playerStatus;

    public UnitScript unitScript;
    public event UnityAction<PlayerController> PlayerDied = delegate { };

    public event UnityAction<CameraTrigger> PlayerSwitchedRooms = delegate { };

    public Transform cam;

    public CharacterController cc;

    public bool inBattle;

    public float exploreMoveSpeed = 5f;

    public bool actionSelected { get; private set; }
    public bool actionTaken { get; private set; }

    public Weapon currentWeaponPrefab;
    public Transform weaponHand;
    public Weapon currentWeapon;
    public GameObject currentWeaponInstance;

    public Vector3 idlePosition;
    public Vector3 targetposition;
    public bool movingToPosition;
    public float battleMoveSpeed;

    public bool defending;
    public bool absorbingAttacks;

    public ParticleSystem absorbingParticles;
    
    public bool jumping;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpForce = 10f;
    public float gravity = -9.8f;
    private float yVel = 0;

    public bool controlsAllowed;
    // Start is called before the first frame update
    void Start()
    {
        
        cc = GetComponent<CharacterController>();

        cam = FindObjectOfType<Camera>().transform;

        unitScript = GetComponent<UnitScript>();
        weaponHand = transform.Find("WeaponHand");
        

        defending = false;
        jumping = false;
        
        absorbingAttacks = false;
        if (absorbingParticles != null) absorbingParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (currentWeapon == null)
        {
            Debug.Log("No weapon equipped!");
        }
    }
    
    private void OnEnable()
    {
        BattleManagerScript.changeBattleState += UpdatePlayerFromBattleState;
        unitScript.unitDied += PlayerIsDead;
    }

    private void OnDisable()
    {
        BattleManagerScript.changeBattleState -= UpdatePlayerFromBattleState;
        unitScript.unitDied -= PlayerIsDead;
    }

    private void Update()
    {
        if (inBattle) BattleUpdate();
        else ExploreUpdate();
    }

    public void ExploreUpdate()
    {
        //Debug.Log("inBattle = " + inBattle);
        Movement();
    }

    public void Movement()
    {
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        Vector3 amountToMove = new Vector3(0, 0, 0);
        
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        //projecting the vectors onto the xz plane
        camForward.y = 0;
        camRight.y = 0;

        //making vector magnitudes = 1
        camForward = camForward.normalized;
        camRight = camRight.normalized;

        //each vector now has forward or back direction and a magnitude from 0 to 1;
        Vector3 MoveForward = camForward * vAxis;
        Vector3 MoveRight = camRight * hAxis;

        Vector3 MoveInDirection = MoveForward + MoveRight;
        MoveInDirection = Mathf.Clamp(MoveInDirection.magnitude, 0, 1) * MoveInDirection.normalized;
        MoveInDirection.y = -1f;
        cc.Move(MoveInDirection * exploreMoveSpeed * Time.deltaTime);

        if (hAxis != 0 & vAxis != 0)
        {
            Vector3 rotateTowards = MoveInDirection;
            rotateTowards.y = 0;
            transform.rotation = Quaternion.LookRotation(rotateTowards);
        }
    }

    public void BattleUpdate()
    {
        if (movingToPosition)
        {
            //Debug.Log("Player moving towards: " + targetposition);
            transform.position = Vector3.MoveTowards(transform.position, targetposition, battleMoveSpeed * Time.deltaTime);
        }

        if (defending) DefendingUpdate();
    }


    public void DefendingUpdate()
    {
        DefenseJump();
    }

    public void DefenseJump()
    {
        //Debug.Log("Jump should be allowed right now!");
        Vector3 jumpVector = new(0, 0, 0);
        if (cc.isGrounded)
        {
            if (Input.GetKeyDown(jumpKey))
            {
                yVel = jumpForce;
            }
            else
            {
                yVel = -2f;
            }
        }
        else
        {
            yVel += gravity * Time.deltaTime;
        }
        cc.Move(new Vector3(0, yVel * Time.deltaTime, 0));
    }

    public void UpdatePlayerFromBattleState(BattleState battleState)
    {
        switch (battleState)
        {
            case BattleState.START:
                PlayerStartBattle();
                break;
            case BattleState.NOBATTLE:
                break;
            case BattleState.PLAYERTURN:
                ResetTurnStatus();
                break;
            case BattleState.ENEMYTURN:
                break;
            case BattleState.BATTLEEND:
                PlayerEndBattle();
                break;
            case BattleState.WON:
                break;
            default:
                break;
        }
    }

    public void PlayerStartBattle()
    {
        //Debug.Log("PlayerStartBattleCalled!");
        unitScript.unitHealth = playerStatus.currentHealth;
        unitScript.maxHealth = playerStatus.maxHealth;
        EquipWeapon(currentWeapon.GetComponent<Weapon>());

        idlePosition = transform.position;
        inBattle = true;
    }


    public void EquipWeapon(Weapon equipThis)
    {
        Weapon equippedWeapon = Instantiate(equipThis.gameObject, weaponHand).GetComponent<Weapon>();
        currentWeaponInstance = equippedWeapon.gameObject;
        //Debug.Log("Equipping " + equippedWeapon.gameObject.name);
        if(currentWeapon != null)
        {
            //Destroy(currentWeapon);
            currentWeapon = null;
        }
        
        currentWeapon = equippedWeapon;
        currentWeapon.gameObject.SetActive(true);

    }

    public void UnequipWeapon()
    {
        Destroy(currentWeaponInstance);
        currentWeapon = currentWeaponPrefab;
        //currentWeapon.gameObject.SetActive(false);
    }


    public void PlayerEndBattle()
    {
        playerStatus.currentHealth = unitScript.unitHealth;
        playerStatus.maxHealth = unitScript.maxHealth;
        playerStatus.currentWeapon = currentWeapon.gameObject;
        inBattle = false;

        if(currentWeapon.AttackCoroutine != null)
        {
            StopCoroutine(currentWeapon.AttackCoroutine);
        }
        //Debug.Log("Setting weapon to idle!");
        //currentWeapon.SetWeaponToIdle(this.weaponHand.transform);
        UnequipWeapon();
    }

    public void ResetTurnStatus() 
    {
        actionSelected = false;
        actionTaken = false;
    }

    public void StartAttack(EnemyController target)
    {
        if (BattleManagerScript.currentState == BattleState.PLAYERTURN && !actionSelected)
        { 
            //Debug.Log("Starting Player Attack!");
            ActionSelected();
            StartCoroutine(PlayerAttack(target));
        }
    }

    IEnumerator PlayerAttack(EnemyController target)
    {
        currentWeapon.PlayAttackGame(this, target);
        target.StartEnemyDefend(this);
        yield return new WaitUntil(() => currentWeapon.attackGameFinished == true);
        currentWeapon.ResetWeaponGame();
        ActionTaken();
    }

    public void AbsorbAttacks()
    {
        if (BattleManagerScript.currentState == BattleState.PLAYERTURN && !actionSelected)
        {
            absorbingAttacks = true;
            ActionSelected();
            StartCoroutine(PlayAbsorbAnimation());
        }
    }

    IEnumerator PlayAbsorbAnimation()
    {
        absorbingParticles.Play();
        yield return new WaitForSeconds(absorbingParticles.main.duration);
        absorbingParticles.Stop();
        Debug.Log("Absorbing ready!");
        yield return new WaitForSeconds(0.3f);
        ActionTaken();
    }

    public void PlayerWait()
    {
        ActionSelected();
        ActionTaken();
    }

    public void StartDefend(EnemyController attacker)
    {
        defending = true;
        StartCoroutine(PlayerDefend(attacker));
    }

    IEnumerator PlayerDefend(EnemyController attacker)
    {
        currentWeapon.PlayDefendGame(this, attacker);
        //Debug.Log("Is this being reached?");
        yield return new WaitUntil(() => attacker.attackFinished == true);
        //Debug.Log("How about this?");
        currentWeapon.EndDefendGame();
        defending = false;
        movingToPosition = true;
        targetposition = idlePosition; 
        yield return new WaitUntil(() => Vector3.Distance(transform.position, targetposition) == 0f);
        movingToPosition = false;

        //Debug.Log("finished defending!");
    }

    public void Blocked(float damage)
    {
        if (absorbingAttacks)
        {
            Debug.Log("Player should heal right now!");
            this.unitScript.healUnit(damage);
        }
    }

    public void ActionSelected() 
    {
        actionSelected = true;
    }

    public void ActionTaken()
    {
        actionTaken = true;
    }

    public void PlayerIsDead()
    {
        //Debug.Log("PlayerIsDead called!");
        PlayerDied?.Invoke(this);
        Destroy(gameObject, 0.1f);
    }


    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger tag = " + other.tag);
        if (other.gameObject.GetComponent<EnemyStartBattleBoxScript>() && !inBattle)
        {
            BattleManagerScript.instance.SetUpBattle(this, other.gameObject.GetComponent<EnemyStartBattleBoxScript>().enemyController);
            //BattleManagerScript.instance.SetEnemyBattleData(this, other.gameObject.GetComponent<EnemyStartBattleBoxScript>().enemyController);
            //Debug.Log("Collision with " + other.gameObject.name + "!");
            //SetPlayerBattleData();
            //LevelLoader.instance.LoadNamedLevel("BattleArena1", true);
            //BattleManagerScript.instance.StartBattle();
        }

        if (other.gameObject.GetComponent<CameraTrigger>())
        {
            PlayerSwitchedRooms?.Invoke(other.gameObject.GetComponent<CameraTrigger>());
        }

        if (other.gameObject.GetComponent<HealthPickup>())
        {
            unitScript.healUnit(other.gameObject.GetComponent<HealthPickup>().healingPoints);
            Destroy(other.gameObject);
        }

        if(other.gameObject.name == "EndGameRing")
        {
            Debug.Log("EndGameRing found!");
            GameManagerScript.instance.WinGame();
        }
    }
}
