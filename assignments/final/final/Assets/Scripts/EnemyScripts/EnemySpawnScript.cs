using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnScript : MonoBehaviour
{

    public GameObject EnemyPrefab;
    public bool defeated;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        defeated = false;
    }

    public void SpawnEnemy()
    {
        Instantiate(EnemyPrefab, this.transform);
    }

}
