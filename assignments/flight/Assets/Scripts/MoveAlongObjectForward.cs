using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlongObjectForward : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Space offsetPositionSpace = Space.Self;

    public float speed = 6;

    // Update is called once per frame
    void LateUpdate()
    {
        if (target != null)
        {
            transform.Translate(target.forward * speed, offsetPositionSpace);
        }
    }
}
