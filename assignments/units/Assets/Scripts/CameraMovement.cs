using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float MoveSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CameraSlide();
    }

    public void CameraSlide()
    {
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        Vector3 MoveInDirection = new Vector3(0, 0, 0);

        MoveInDirection.x = hAxis * MoveSpeed * Time.deltaTime;
        MoveInDirection.z = vAxis * MoveSpeed * Time.deltaTime;

        transform.position += MoveInDirection;
    }
}
