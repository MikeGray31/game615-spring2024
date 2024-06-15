using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitScript : MonoBehaviour
{
    public Weapon weapon;

    private void OnCollisionEnter(Collision collision)
    { 
        if(weapon!= null) weapon.AttackHit(collision);
    }
}
