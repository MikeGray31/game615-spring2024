using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : Weapon
{
    
    public float maxDistanceFromCenter = 1f;
    
    //public BlockDirection blockdirection;

    public float minAttackCharge = 1f;
    public float maxAttackCharge = 5f;
    public float attackCharge { get; private set; }

    public Animator swordAnimator;

    public CurrentAttackStatus currentAttackStatus;

    // Start is called before the first frame update
    void Start()
    {
        currentAttackStatus = CurrentAttackStatus.NOTATTACKING;
        attackGameActive = false;
        attacking = false;
        attackCharge = minAttackCharge;
    }


    public override void AttackGameRun()
    {
        //Debug.Log("Sword should be allowed to move up and down!");
        float speed = 8f;
        float vAxis = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)) vAxis = 0;
        else if (Input.GetKey(KeyCode.W)) vAxis = 1;
        else if (Input.GetKey(KeyCode.S)) vAxis = -1;
        
        Vector3 targetPosition = transform.position;

        //targetPosition.y = attackStartPosition.y + (vAxis * maxDistanceFromCenter);

        if (Input.GetKey(KeyCode.Mouse0))
        {
            attacking = true;
            //Debug.Log("Mouse down => " + (Input.GetKeyDown(KeyCode.Mouse0)) + " / currentAttackStatus = " + currentAttackStatus);
            if (Input.GetKeyDown(KeyCode.Mouse0) && currentAttackStatus == CurrentAttackStatus.NOTATTACKING)
            {
                attackCharge = minAttackCharge;
                swordAnimator.Play("SwordCharge");
            }
            else
            {
                attackCharge = Mathf.Clamp(attackCharge + 2f * Time.deltaTime, 1f, maxAttackCharge);
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0)  && currentAttackStatus == CurrentAttackStatus.NOTATTACKING && attacking)
        {
            //Debug.Log("Sword released! Attack power = " + attackCharge);
            swordAnimator.SetTrigger("SwordReleased");
            currentAttackStatus = CurrentAttackStatus.STARTED;
            StartCoroutine(EnsureAttackFinishes());
        }
        else if(!attacking)
        {
            targetPosition = attackStartPosition;
            targetPosition.y = attackStartPosition.y + (vAxis * maxDistanceFromCenter);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    IEnumerator EnsureAttackFinishes()
    {
        yield return new WaitForSeconds(1.5f);
        swordAnimator.ResetTrigger("SwordReleased");
        attacking = false;
        currentAttackStatus = CurrentAttackStatus.NOTATTACKING;
    }

    public void FinishSwordPoke()
    {
        //Debug.Log("Attack animation ended!");
        swordAnimator.ResetTrigger("SwordReleased");
        attacking = false;
        currentAttackStatus = CurrentAttackStatus.NOTATTACKING;
    }

    public override void PlayAttackGame(PlayerController player, EnemyController target)
    {
        //Debug.Log("This is the sword attack game playing!");
        //Debug.Log("Active? " + gameObject.activeInHierarchy);
        if (player != null  && target != null) this.AttackCoroutine = StartCoroutine(AttackGameCoroutine(player, target));
    }

    public override IEnumerator AttackGameCoroutine(PlayerController player, EnemyController target)
    {
        player.targetposition = target.transform.position;
        
        player.movingToPosition = true;
        yield return new WaitUntil(() => Vector3.Distance(player.transform.position, target.transform.position) <= 4f);
        player.movingToPosition = false;
        
        CalculateAttackingAndIdlePositions(player.transform, player.weaponHand);
        
        player.weaponHand.transform.position = attackStartPosition;
        gameObject.transform.Rotate(new Vector3(90,0,0), Space.Self);
        
        SetAttackGameActiveState(true);
        yield return new WaitForSeconds(weaponGameTimeLimit);
        SetAttackGameActiveState(false);
        currentAttackStatus = CurrentAttackStatus.NOTATTACKING;
        attacking = false;

        //Debug.Log("Sword Attack finished!  Moving back to position!");
        swordAnimator.Play("SwordIdle");
        transform.position = player.weaponHand.transform.position;
        player.weaponHand.transform.position = idlePosition;
        gameObject.transform.Rotate(new Vector3(-90,0,0), Space.Self);
        player.targetposition = player.idlePosition;
        
        player.movingToPosition = true;
        yield return new WaitUntil(() => Vector3.Distance(player.transform.position, player.idlePosition) == 0f);
        player.movingToPosition = false;

        //Debug.Log("Back in position.  Ending sword attack coroutine");
        this.attackGameFinished = true;
    }

    public override void CalculateAttackingAndIdlePositions(Transform player, Transform weaponHandTransform)
    {
        idlePosition = weaponHandTransform.position;

        attackStartPosition = player.transform.position;
        attackStartPosition += transform.forward + transform.up;
        
    }              

    public void SetAttackGameActiveState(bool active)
    {
        attackGameActive = active;
    }

    public override void AttackHit(Collision collision)
    {
        if (attacking)
        {
            //Debug.Log("hit something!");
            if (collision.gameObject.name == "Shield" && currentAttackStatus != CurrentAttackStatus.HIT)
            {
                //Debug.Log("Hit a shield!");
                currentAttackStatus = CurrentAttackStatus.BLOCKED;
                swordAnimator.Play("SwordPokeRecoil");

            }
            if (collision.gameObject.GetComponent<UnitScript>() && currentAttackStatus != CurrentAttackStatus.BLOCKED)
            {
                currentAttackStatus = CurrentAttackStatus.HIT;
                UnitScript hitUnit = collision.gameObject.GetComponent<UnitScript>();
                hitUnit.TakeDamage(attackCharge);
            }
        }
    }

    public override void DefendGameRun()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && !blocking)
        {
            //Debug.Log("Block button hit!");
            blocking = true;
            if (Input.GetKey(KeyCode.D))
            {
                //Debug.Log("Block forward!");
                swordAnimator.Play("SwordBlockForward");
            }
            else if (Input.GetKey(KeyCode.W))
            {
                //Debug.Log("Block High!");
                swordAnimator.Play("SwordBlockUp");
            }
            else if (Input.GetKey(KeyCode.S))
            {
                //Debug.Log("Block Low!");
                swordAnimator.Play("SwordBlockDown");
            }
            else
            {
                //Debug.Log("Block forward!");
                swordAnimator.Play("SwordBlockForward");
            }
        }
    }

    public void EndBlock()
    {
        blocking = false;
    }

    public override void PlayDefendGame(PlayerController player, EnemyController attacker)
    {
        //base.PlayDefendGame(player, attacker);
        if (player != null && attacker != null)
        {
            defendGameActive = true;
            //StartCoroutine(DefendGameCoroutine(player, attacker));
        }
    }

    IEnumerator DefendGameCoroutine(PlayerController player, EnemyController attacker)
    {
        yield return null;
    }

    public override void EndDefendGame()
    {
        base.EndDefendGame();
        swordAnimator.Play("SwordIdle");
    }
}
