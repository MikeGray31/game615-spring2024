using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeBase : MonoBehaviour
{
    

    GameManager gameManager;

    public float storedGold;

    public string baseName = "Home Base";

    public Transform spawnPoint;

    public GameObject minerPrefab;
    public GameObject fighterPrefab;

    public float fighterCost = 100;
    public float minerCost = 100;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject gmObj = GameObject.Find("GameManagerObject");
        gameManager = gmObj.GetComponent<GameManager>();

        storedGold = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown()
    {
        //Debug.Log("OnMouseDown called!");
        gameManager.SelectEntity(gameObject);
    }


    public void StoreGold(float gold)
    {
        storedGold += gold;
    }

    public void DeployFighter()
    {
        if(storedGold >= fighterCost)
        {
            Debug.Log("Deploying Fighter!");
            Vector3 spawnHere = new Vector3(spawnPoint.position.x + Random.Range(-0.2f, 0.2f),
                                            spawnPoint.position.y + Random.Range(-0.2f, 0.2f),
                                            spawnPoint.position.z + Random.Range(-0.2f, 0.2f));
            Instantiate(fighterPrefab, spawnHere, Quaternion.identity);
            storedGold = Mathf.Clamp(storedGold - fighterCost, 0, Mathf.Infinity);
        }
    }

    public void DeployMiner()
    {
        if (storedGold >= minerCost)
        {
            Debug.Log("Deploying Miner!");
            Vector3 spawnHere = new Vector3(spawnPoint.position.x + Random.Range(-0.2f, 0.2f),
                                            spawnPoint.position.y + Random.Range(-0.2f, 0.2f),
                                            spawnPoint.position.z + Random.Range(-0.2f, 0.2f));
            Instantiate(minerPrefab, spawnHere, Quaternion.identity);
            storedGold = Mathf.Clamp(storedGold - minerCost, 0, Mathf.Infinity);
        }
    }
}
