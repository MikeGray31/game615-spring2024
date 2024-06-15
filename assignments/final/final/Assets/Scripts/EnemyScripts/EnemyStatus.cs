using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthStatusData", menuName = "StatusObjects/Enemy", order = 1)]

public class EnemyStatus : ScriptableObject
{

    public string enemyName;
    public float currentHealth;
    public float maxHealth;
    public GameObject enemyGameObject;
    //public GameObject currentWeapon;
    public float[] position = new float[3];

    public EnemyStatus(EnemyController enemySource)
    {
        currentHealth = enemySource.unitScript.unitHealth;
        maxHealth = enemySource.unitScript.maxHealth;
        position[0] = enemySource.gameObject.transform.position.x;
        position[1] = enemySource.gameObject.transform.position.y;
        position[2] = enemySource.gameObject.transform.position.z;
        enemyGameObject = enemySource.enemyGameObject;
    }

}
