using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneScript : MonoBehaviour
{
    int score = 0;

    public float forwardSpeed = 5f;
    public float rotateSpeed = 50;

    public GameObject cameraObject;

    public GameObject projectilePrefab;

    //public TMP_Text scoreText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Get directional input (up, down, left, right)
        float hAxis = Input.GetAxis("Horizontal"); // -1 if left is pressed, 1 if right is pressed, 0 if neither
        float vAxis = Input.GetAxis("Vertical"); // -1 if down is pressed, 1 if up is pressed, 0 if neither

        // Create a vector3 with all rotation amounts based on input
        Vector3 amountToRotate = new Vector3(0, 0, 0);
        amountToRotate.x = vAxis * rotateSpeed * Time.deltaTime;
        amountToRotate.y = hAxis * rotateSpeed * Time.deltaTime;

        // Apply the rotation
        transform.Rotate(amountToRotate, Space.Self);
        
        // This line of code will modify how fast we are moving forward based on
        // how far up or down (transform.forward.y) the plane is looking. The
        // effect is that we speed up when looking down, and slow down when looking up.
        forwardSpeed = forwardSpeed - 15 * transform.forward.y * Time.deltaTime;

        // The above line is equivalent to the below if statements
        //if (forwardSpeed < 0)
        //{
        //    forwardSpeed = 0;
        //}
        //if (forwardSpeed > 10)
        //{
        //    forwardSpeed = 10;
        //}

        // Boost!
        if (Input.GetKey(KeyCode.B))
        {
            forwardSpeed = forwardSpeed + 50 * Time.deltaTime;
        }

        // Prevent forwardSpeed from getting greater than 10, or more importantly, less than 0.
        forwardSpeed = Mathf.Clamp(forwardSpeed, 3, 30);

        // Now that we have modified forwardSpeed, we can scale transform.forward by it, and
        // ad it to the position to make the plane move forward.
        Vector3 amountToMove = transform.forward * forwardSpeed * Time.deltaTime;
        transform.position = transform.position + amountToMove;

        // Keep us from going underground
        float terrainY = Terrain.activeTerrain.SampleHeight(transform.position);
        if (transform.position.y < terrainY)
        {
            transform.position = new Vector3(transform.position.x, terrainY, transform.position.z);
            forwardSpeed -= 100 * Time.deltaTime;
        }

        // Positioning the camera behind and above the plane
        Vector3 cameraPosition = transform.position - transform.forward * 10;
        cameraPosition.y = cameraPosition.y + 8;
        cameraObject.transform.position = cameraPosition;
        // Make the camera look at the plane
        cameraObject.transform.LookAt(transform);

        // Launch projectile
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject projObj = Instantiate(projectilePrefab, transform.position + transform.forward * 5, transform.rotation);
            Rigidbody projRB = projObj.GetComponent<Rigidbody>();
            projRB.AddForce(transform.forward * 1000);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ring"))
        {
            /*score++; // score = score + 1;
            scoreText.text = "Score: " + score.ToString();*/
            
        }
    }

}
