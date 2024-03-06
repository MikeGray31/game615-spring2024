using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{

    NavMeshAgent nma;
    GameManager gameManager;

    public string enemyName;

    public float maxHealth;
    public float health;

    private bool attackReady;
    public float attackCooldown = 1.0f;
    public float attackPower = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        nma = gameObject.GetComponent<NavMeshAgent>();
        
        GameObject gmObj = GameObject.Find("GameManagerObject");
        gameManager = gmObj.GetComponent<GameManager>();
        
        attackReady = true;
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        UnitScript nearestUnit = CheckForPlayerUnit();
        if(nearestUnit != null)
        {
            MoveToAttackUnit(nearestUnit);
        }

        if (health <= 0)
        {
            Destroy(gameObject);
            //Debug.Log("Enemy Unit Destroyed!");
        }
    }
    public void OnMouseDown()
    {
        gameManager.SelectEntity(gameObject);
    }

    public UnitScript CheckForPlayerUnit()
    {
        UnitScript[] units = gameManager.getAllPlayerUnits();
        UnitScript closestUnit = null;
        float distance = Mathf.Infinity;
        foreach (UnitScript us in units){
            if(us != null && Vector3.Distance(us.transform.position, gameObject.transform.position) < distance)
            {
                distance = Vector3.Distance(us.transform.position, gameObject.transform.position);
                closestUnit = us;
            }
        }

        if (distance < 15f) return closestUnit;
        else return null;
    }

    public void MoveToAttackUnit(UnitScript targetUnit)
    {
        nma.SetDestination(targetUnit.transform.position);
        if (Vector3.Distance (gameObject.transform.position, targetUnit.transform.position) < 2f) 
        {
            nma.SetDestination(gameObject.transform.position);
            if (attackReady)
            {
                StartCoroutine(AttackUnit(targetUnit));
            }
        }
    }

    IEnumerator AttackUnit(UnitScript targetUnit)
    {
        attackReady = false;
        targetUnit.takeDamage(attackPower);
        yield return new WaitForSeconds(attackCooldown);
        attackReady = true;

    }

    public void takeDamage(float damage)
    {
        health = Mathf.Clamp(health -  damage, 0, maxHealth);
    }
}
