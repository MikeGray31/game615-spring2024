using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public GameObject hitEffect;

    private void OnTriggerEnter(Collider other)
    {


        if (other.tag != "Player")
        {
            //Debug.Log("Bullet hit something!");
            if(other.tag == "ShootThis")
            {
                Debug.Log("Found Shootable Target!");
                Destroy(other.gameObject);
            }
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, 0.15f);
            Destroy(gameObject);
        }



    }
}

