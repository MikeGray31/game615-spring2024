using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    //Components + Object Variables
    private CharacterController cc;
    public Animator animator;
    public Transform cam;
    public GameObject reticle;

    public Weapon currentWeapon;

    public LayerMask layer;

    //Keyboard Control variables
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode shootKey = KeyCode.Mouse0;
    public KeyCode aimKey = KeyCode.Mouse1;

    //Move variables
    public float moveSpeed = 10f;
    public float sprintSpeed = 15f;
    private float currentSpeed;

    //gravity variables
    private float yVel = 0;
    float gravity = -9.8f;
    float gravityScale = 3f;
    private float currentGravity;

    //jump + long jump variables
    public float jumpForce = 10f;
    private bool stillJumping;
    
    private bool longJumpReady;
    public float longJumpForce = 30f;
    private float currentLongJumpForce;
    private Vector3 longJumpDirection;


    // hitpoint variables
    public float maxHitPoints;
    [SerializeField]
    private float hitPoints = 10;
    public PlayerHealthBar playerHealthBar;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        hitPoints = maxHitPoints;
        currentSpeed = moveSpeed;
        currentGravity = gravity;
        stillJumping = false;
        longJumpReady = true;
        currentLongJumpForce = 0f;
        Cursor.lockState = CursorLockMode.Locked;
        reticle.SetActive(false);

        playerHealthBar = FindObjectOfType<PlayerHealthBar>();
        playerHealthBar.slider.maxValue = maxHitPoints;
        playerHealthBar.slider.minValue = 0;
        playerHealthBar.slider.value = hitPoints;
    }

    // Update is called once per frame
    void Update()
    {
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        Vector3 amountToMove = new Vector3(0, 0, 0);

        Vector3 InputDirection = CalculateInputDirection(vAxis, hAxis);
        amountToMove = PlayerMove(vAxis, hAxis, amountToMove, InputDirection);
        PlayerRotate(vAxis, hAxis, amountToMove);
        amountToMove = CalculateYVel(amountToMove);
        amountToMove = PlayerLongJump(amountToMove, InputDirection);
        //amountToMove = PlayerJump(amountToMove);

        cc.Move(amountToMove);

        PlayerAim();
        PlayerShoot();

        if (hitPoints <= 0) Destroy(gameObject);
    }

    public Vector3 CalculateInputDirection(float v, float h)
    {
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        //projecting the vectors onto the xz plane
        camForward.y = 0;
        camRight.y = 0;

        //making vector magnitudes = 1
        camForward = camForward.normalized;
        camRight = camRight.normalized;

        //each vector now has forward or back direction and a magnitude from 0 to 1;
        Vector3 MoveForward = camForward * v;
        Vector3 MoveRight = camRight * h;
        
        Vector3 MoveInDirection = MoveForward + MoveRight;
        MoveInDirection = Mathf.Clamp(MoveInDirection.magnitude, 0, 1) * MoveInDirection.normalized;
        return MoveInDirection;
    }
    
    public Vector3 PlayerMove(float v, float h, Vector3 currentVector ,Vector3 MoveInDirection)
    {
        calculateSpeed();
        currentVector = currentVector + (MoveInDirection * currentSpeed * Time.deltaTime);
        return currentVector;
    }

    private void calculateSpeed()
    {
        if (Input.GetKey(sprintKey)) currentSpeed = sprintSpeed;
        else currentSpeed = moveSpeed;
    }

    public void PlayerRotate(float v, float h, Vector3 currentVector)
    {
        Vector3 rotateTowards;

        if (Input.GetKey(aimKey))
        {
            //Debug.Log("aiming!");
            Vector3 camForward = cam.forward;
            camForward.y = 0;
            rotateTowards = camForward;
            transform.rotation = Quaternion.LookRotation(rotateTowards);
        }
        else if(v != 0 || h != 0)
        {
            rotateTowards = currentVector;
            rotateTowards.y = 0;
            transform.rotation = Quaternion.LookRotation(rotateTowards);
        }
        //transform.rotation = Quaternion.LookRotation(rotateTowards);
        //return currentVector;
    }

    public Vector3 CalculateYVel(Vector3 currentVector)
    {
        if (cc.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                yVel = jumpForce;
                stillJumping = true;
            }
            else
            {
                yVel = -2f;  
            }
            longJumpReady = true;
        }
        else
        {
            CalculateCurrentGravity();
            //Debug.Log("Current Gravity = " + currentGravity);
            yVel += currentGravity * Time.deltaTime;
        }

        currentVector.y = currentVector.y + (yVel * Time.deltaTime);
        return currentVector;
    }

    private void CalculateCurrentGravity()
    {
        isStillJumping();
        if (stillJumping) currentGravity = gravity;
        else currentGravity = gravity * gravityScale;
    }

    private void isStillJumping()
    {
        if (Input.GetKey(jumpKey) && yVel >= -1 && stillJumping)
        {
            stillJumping = true;
        }
        else
        {
            stillJumping = false;
        }
    }

    private Vector3 PlayerLongJump(Vector3 currentVector, Vector3 MoveInDirection)
    {
        if (Input.GetKeyDown(KeyCode.Space) && !cc.isGrounded && longJumpReady)
        {
            //Debug.Log("Try Long Jump!");
            longJumpReady = false;
            currentLongJumpForce = longJumpForce;
            longJumpDirection = MoveInDirection;
        }

        if(currentLongJumpForce != 0)
        {
            if (currentLongJumpForce < 0) currentLongJumpForce = 0;
            else
            {
                currentLongJumpForce = currentLongJumpForce - (30f * Time.deltaTime);
            }
        }

        Vector3 LongJumpVector = longJumpDirection * currentLongJumpForce * Time.deltaTime;
        currentVector = currentVector + LongJumpVector;
        return currentVector;
    }

    private void PlayerShoot()
    {
        if (Input.GetKey(shootKey))
        {
            //Debug.Log("PlayerShoot() called!");
            currentWeapon.Shoot();
        }
    }

    private void PlayerAim()
    {
        if (Input.GetKey(aimKey))
        {
            //Debug.Log("Aiming!");
            reticle.SetActive(true);
            currentWeapon.SetToAimSpread();
            Ray ray = new Ray(cam.position, cam.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 200f, layer))
            {
                currentWeapon.transform.rotation = Quaternion.LookRotation(hit.point - currentWeapon.transform.position);
            }
            else
            {
                currentWeapon.transform.rotation = Quaternion.LookRotation((cam.position + (cam.forward * 200f)) - currentWeapon.transform.position);
            }
        }
        else
        {
            reticle.SetActive(false);
            //Debug.Log("Hip fire!");
            currentWeapon.transform.rotation = Quaternion.LookRotation(transform.forward);
            currentWeapon.SetToHipFireSpread();
        }
    }


    public void takeDamage(float damage)
    {
        hitPoints -= damage;
        playerHealthBar.SetHealth(hitPoints);
    }


    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger tag = " + other.tag);
        if (other.tag == "Bullet")
        {
            //Debug.Log("Player taking damage!");
            takeDamage(1);
            Destroy(other.gameObject);
        }
    }



    // -------- Old Methods, no longer in use ---------


    /*    IEnumerator LongJumpCoroutine()
        {
            currentLongJumpForce = currentLongJumpForce - 0.02f;
            if (currentLongJumpForce < 0)
            {
                Debug.Log("Setting currentLongJumpForce = 0");
                currentLongJumpForce = 0;
            }
            else
            {
                LongJumpCoroutine();

            }
            yield return null;
        }*/

    /*public Vector3 PlayerMoveOld(float v, Vector3 currentVector)
    {
        currentVector += v * transform.forward * moveSpeed * Time.deltaTime;
        return currentVector;
    }*/

    /*public void PlayerRotateOld(float h)
    {
        Vector3 amountToRotate = new Vector3(0, 0, 0);
        amountToRotate.y = h * rotateSpeed * Time.deltaTime;
        transform.Rotate(amountToRotate, Space.Self);
    }*/
}
