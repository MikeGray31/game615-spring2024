using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponHitScript : MonoBehaviour
{
    public EnemyAttackScript attackScript;

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("EnemyAttackScript hit something!");
        if (attackScript != null) attackScript.CheckHit(collision);
    }
}
