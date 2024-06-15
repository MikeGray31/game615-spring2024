using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleDoorScript : MonoBehaviour
{

    public GameObject Door1Pivot;
    public GameObject Door2Pivot;

    public List<EnemyController> enemiesKilledToOpen;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (enemiesKilledToOpen.Count != 0) CloseDoors();
        else OpenDoors();
        

    }

    private void OnEnable()
    {
        foreach (EnemyController e in enemiesKilledToOpen)
        {
            e.EnemyDied += EnemyKilled;
        }
    }

    private void OnDisable()
    {
        foreach (EnemyController e in enemiesKilledToOpen)
        {
            e.EnemyDied -= EnemyKilled;
        }
    }

    public void OpenDoors()
    {
        animator.Play("DoubleDoorsOpening");
        //Debug.Log("Opening doors!");
        
    }
    public void CloseDoors()
    {
        animator.Play("DoubleDoorsClosing");
        //Debug.Log("Closing doors!");
    }

    public void EnemyKilled(EnemyController enemy)
    {
        if (enemiesKilledToOpen.Contains(enemy))
        {
            enemiesKilledToOpen.Remove(enemy);
        }
        if (enemiesKilledToOpen.Count == 0)
        {
            OpenDoors();
        }
    }

    public void StopPlayback()
    {
        //animator.StopPlayback();
    }
}
