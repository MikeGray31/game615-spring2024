using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject model;
    public float damage;

    public bool attacking;
    public bool blocking;

    public Vector3 attackStartPosition;
    public Vector3 attackStartRotation;
    public Vector3 idlePosition;
    public Vector3 idleRotation;

    public Coroutine AttackCoroutine;

    public bool attackGameActive;
    public bool defendGameActive;

    public float weaponGameTimeLimit = 7f;

    //public WeaponHitScript hitScript;
    public bool attackGameFinished { get; protected set; }

    // Update is called once per frame
    public virtual void Update()
    {
        if (attackGameActive)
        {
            //Debug.Log("Default attackGameActive Update running!");
            AttackGameRun();
        }
        if (defendGameActive)
        {
            //Debug.Log("Defend game running!");
            DefendGameRun();
        }
    }

    public virtual void AttackGameRun()
    {
        Debug.Log("Default AttackGameRun running!");
    }

    public virtual void PlayAttackGame(PlayerController player, EnemyController target)
    {
        //Debug.Log("This is the default AttackGame method!");
        this.attackGameFinished = true;
    }

    public virtual IEnumerator AttackGameCoroutine(PlayerController player, EnemyController target)
    {
        yield return null;
    }

    public void ResetWeaponGame()
    {
        this.attackGameFinished = false;
    }

    public virtual void CalculateAttackingAndIdlePositions(Transform player, Transform weaponHandTransform)
    {
        idlePosition = weaponHandTransform.position;

        attackStartPosition = player.transform.position;
        attackStartPosition += transform.forward + transform.up;
    }

    public virtual void AttackHit(Collision collision)
    {
        if (collision.gameObject.name == "Shield")
        {
            Debug.Log("Hit a shield!");
        }
        if (collision.gameObject.GetComponent<UnitScript>())
        {
            UnitScript hitUnit = collision.gameObject.GetComponent<UnitScript>();
        }
    }


    public virtual void DefendGameRun()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("Block forward!");
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log("Block High!");
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("Block Low!");
            }
        }
    }

    public virtual void PlayDefendGame(PlayerController player, EnemyController attacker)
    {
        Debug.Log("This is the default DefendGame method!");
        defendGameActive = true;
    }

    public virtual void EndDefendGame()
    {
        defendGameActive = false;
    }

    public void SetWeaponToIdle(Transform idleTransform)
    {
        Debug.Log("Setting weapon to idle!");
        attacking = false;
        blocking = false;
        attackGameFinished = true;
        //transform.position = idleTransform.position;
        //transform.rotation = idleTransform.rotation;
    }
}
