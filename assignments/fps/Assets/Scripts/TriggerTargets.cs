using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTargets : MonoBehaviour
{

    public GameObject lights;

    private void Start()
    {
        lights.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("Activating Lights!");
            lights.SetActive(true);
            Destroy(gameObject);
        }
    }
}
