using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTargets : MonoBehaviour
{

    public GameObject TargetPrefab;
    
    private bool cooldownActive;
    public float cooldownInterval = 1.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        cooldownActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!cooldownActive)
        {
            SpawnNewTarget();
            cooldownActive = true;
            StartCoroutine(coolDownStart());
        }
    }



    public void SpawnNewTarget()
    {
        Debug.Log("Spawning Target!");
        Vector3 spawnPos = new Vector3(Random.Range(gameObject.transform.position.x - 7.5f, gameObject.transform.position.x + 7.5f), 
                                       gameObject.transform.position.y, 
                                       Random.Range(gameObject.transform.position.z - 7.5f, gameObject.transform.position.z + 7.5f));
        Instantiate(TargetPrefab, spawnPos, Quaternion.identity);
    }

    IEnumerator coolDownStart()
    {
        yield return new WaitForSeconds(cooldownInterval);
        cooldownActive = false;
    }
}
