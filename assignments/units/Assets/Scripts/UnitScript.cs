using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitScript : MonoBehaviour
{
    NavMeshAgent nma;
    GameManager gameManager;

    public string characterName;

    public Renderer bodyRenderer;

    public Color unselectedColor;
    public Color selectedColor;

    public float maxHealth;
    public float health;

    private bool attackReady;
    public float attackCooldown = 1.0f;
    public float attackPower = 1.0f;

    public float currentGold;
    public float maxGoldCapacity;

    public bool currentlyMining;
    public GoldMine currentMine;

    public float miningRate = 10f;
    public float miningCooldown = 1.0f;
    public bool miningActionAvailable;

    public bool goingToBase;
    public HomeBase currentHomeBase;

    public bool attackingEnemy;
    public EnemyScript currentEnemy;

    // Start is called before the first frame update
    void Start()
    {

        health = maxHealth;

        nma = gameObject.GetComponent<NavMeshAgent>();

        GameObject gmObj = GameObject.Find("GameManagerObject");
        gameManager = gmObj.GetComponent<GameManager>();

        unselectedColor = bodyRenderer.material.color;

        miningActionAvailable = true;

        attackReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentlyMining && currentMine != null)
        {
            //Debug.Log("Mine() called!");
            Mine();
        }
        else if (goingToBase && currentHomeBase != null)
        {
            //Debug.Log("StoreGoldInBase() called!");
            StoreGoldInBase();
        }
        else if (attackingEnemy && currentEnemy != null)
        {
            AttackEnemy();
        }

        if(health <= 0)
        {
            Destroy(gameObject);
            //Debug.Log("Player Unit Destroyed!");
        }
        
    }

    public void GoToPoint(Vector3 point)
    {
        goingToBase = false;
        currentlyMining = false;
        attackingEnemy = false;
        nma.SetDestination(point);
    }

    public void OnMouseDown()
    {
        gameManager.SelectEntity(gameObject);
    }

    public void beSelected()
    {
        bodyRenderer.material.color = selectedColor;
    }

    public void StartMining(GoldMine mine)
    {
        nma.SetDestination(mine.transform.position);
        currentlyMining = true;
        currentMine = mine;
        //Debug.Log("StartMining() called!");
    }

    public void Mine()
    {
        if(Vector3.Distance(transform.position, currentMine.transform.position) < 4)
        {
            nma.SetDestination(transform.position);
            if (miningActionAvailable && currentMine != null)
            {
                currentGold += currentMine.beMined(miningRate);
                if(currentGold > maxGoldCapacity)
                {
                    //Debug.Log("Returning some gold!");
                    currentMine.returnMined(currentGold - maxGoldCapacity);
                    currentGold = maxGoldCapacity;
                    currentlyMining = false;
                }
                miningActionAvailable = false;
                StartCoroutine(MiningActionCooldown());
            }
            
            
        }
    }

    IEnumerator MiningActionCooldown()
    {
        yield return new WaitForSeconds(miningCooldown);
        miningActionAvailable = true;
    }

    public void WalkToBase(HomeBase homeBase) 
    {
        nma.SetDestination(homeBase.transform.position);
        currentlyMining = false;
        goingToBase = true;
        currentHomeBase = homeBase;
    }

    public void StoreGoldInBase()
    {
        if (Vector3.Distance(transform.position, currentHomeBase.transform.position) < 6.5)
        {
            nma.SetDestination(transform.position);
            if (currentGold > 0)
            {
                currentHomeBase.StoreGold(currentGold);
                currentGold = 0;
            }
        }
    }

    public void takeDamage(float damage)
    {
        health = Mathf.Clamp(health - damage, 0, maxHealth);
    }

    public void MoveToAttackEnemy(EnemyScript targetEnemy)
    {
        nma.SetDestination(targetEnemy.transform.position);
        currentEnemy = targetEnemy;
        attackingEnemy = true;
    }

    public void AttackEnemy()
    {
        //Debug.Log("AttackEnemy() called");
        nma.SetDestination(currentEnemy.transform.position);
        //Debug.Log("Close enough? -> " + (Vector3.Distance(gameObject.transform.position, currentEnemy.transform.position) < 2f));
        if (Vector3.Distance(gameObject.transform.position, currentEnemy.transform.position) < 2f)
        {
            //Debug.Log("Close enough?");
            nma.SetDestination(gameObject.transform.position);

            if (currentEnemy != null && attackReady)
            {
                //Debug.Log("Enemy should be taking damage.  Unit Attack Power = " + attackPower);
                currentEnemy.takeDamage(attackPower);
                StartCoroutine(AttackCooldown());
            }
        }
    }

    IEnumerator AttackCooldown()
    {
        attackReady = false;
        yield return new WaitForSeconds(attackCooldown);
        attackReady = true;

    }
}
