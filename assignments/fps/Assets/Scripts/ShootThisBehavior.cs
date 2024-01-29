using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootThisBehavior : MonoBehaviour
{
    void Start()
    {
        Renderer rend = gameObject.GetComponent<Renderer>();
        rend.material.color = new Color(Random.value, Random.value, Random.value);
    }
}
