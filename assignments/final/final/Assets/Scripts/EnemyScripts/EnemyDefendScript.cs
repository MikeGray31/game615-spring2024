using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDefendScript : MonoBehaviour
{

    public Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Defend(PlayerController attacker)
    {
        animator.Play("EnemyShieldBlock");
        StartCoroutine(WaitForAttackToEnd(attacker));
    }

    IEnumerator WaitForAttackToEnd(PlayerController attacker)
    {
        yield return new WaitUntil(() => attacker.actionTaken == true);
        animator.SetTrigger("EndBlocking");
        yield return new WaitForSeconds(0.05f);
        animator.ResetTrigger("EndBlocking");
    }


}
