using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAttackRange {MELEE,RANGE}
public enum CurrentAttackStatus { NOTATTACKING, STARTED, BLOCKED, HIT}

public class EnemyAttackScript : MonoBehaviour
{
    public PlayerController currentTarget;
    public EnemyAttackRange attackRange;

    public Animator animator;

    public bool attackFinished;
    public CurrentAttackStatus currentAttackStatus;

    public float attackPower = 7f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        attackFinished = false;
        attackRange = EnemyAttackRange.MELEE;
        currentAttackStatus = CurrentAttackStatus.NOTATTACKING;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public virtual void AttackPattern(PlayerController target)
    {
        currentTarget = target;
        StartCoroutine(AttackPatternCoroutine(target));
    }

    public virtual IEnumerator AttackPatternCoroutine(PlayerController target)
    {
        //Debug.Log("AttackPatternCoroutine running!");
        animator.Play("Attack1");
        yield return new WaitUntil(() => animator.GetBool("animationFinished"));
        //yield return new WaitForSeconds(4f);
        //Debug.Log("AttackPatternCoroutine finished!");
        AttackFinished();
        yield return new WaitForSeconds(0.1f);
        ResetAttackFinished();
    }

    public void AttackAnimationFinished()
    {
        animator.SetTrigger("animationFinished");
        animator.ResetTrigger("BlockedTrigger");
        animator.ResetTrigger("ContinueAttack");
        currentAttackStatus = CurrentAttackStatus.NOTATTACKING;
        StartCoroutine(ResetAttackAnimationFinished());
    }

    IEnumerator ResetAttackAnimationFinished()
    {
        yield return new WaitForSeconds(0.05f);
        animator.ResetTrigger("animationFinished");
    }

    public void StartWeaponHitDetection()
    {
        currentAttackStatus = CurrentAttackStatus.STARTED;
    }


    public void CheckHit(Collision collision)
    {
        if (currentAttackStatus == CurrentAttackStatus.STARTED)
        {
            if (collision.gameObject.GetComponent<WeaponHitScript>() && currentAttackStatus != CurrentAttackStatus.HIT)
            {
                //Debug.Log("Attack was blocked!");
                currentAttackStatus = CurrentAttackStatus.BLOCKED;
                currentTarget.Blocked(attackPower);
                animator.SetTrigger("BlockedTrigger");
            }
            if (collision.gameObject.GetComponent<PlayerController>() && currentAttackStatus != CurrentAttackStatus.BLOCKED)
            {
                //Debug.Log("Player takes damage!");
                collision.gameObject.GetComponent<PlayerController>().unitScript.TakeDamage(attackPower);
                currentAttackStatus = CurrentAttackStatus.HIT;
                animator.SetTrigger("ContinueAttack");
            }
        }
    }

    public void AttackFinished()
    {
        attackFinished = true;
    }

    public void ResetAttackFinished()
    {
        attackFinished = false;
    }
}
