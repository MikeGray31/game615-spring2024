using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public LayerMask layer;

    public GameObject enemyPivot;
    public Transform firepoint;
    public GameObject bulletPrefab;

    public GameObject player;

    [SerializeField]
    private float hitPoints;
    public float maxHitPoints = 10;

    public float bulletForce = 10f;
    public float bulletCooldown = 0.25f;
    private bool cooldownActive = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player/Visuals");
        hitPoints = maxHitPoints;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (CanSeePlayer())
            {
                Aim();
                if (!cooldownActive) Shoot();
            }
        }
        if (hitPoints <= 0) Destroy(gameObject);
    }

    public bool CanSeePlayer()
    {
        Vector3 direction = player.transform.position - enemyPivot.transform.position;
        //Debug.DrawRay(enemyPivot.transform.position + direction.normalized * 1f, direction, Color.green);
        Ray ray = new Ray(enemyPivot.transform.position, direction);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, layer)) {
            //Debug.DrawRay(enemyPivot.transform.position + direction.normalized * 1f, direction, Color.green);
            //Debug.Log("tag = " + hit.transform.tag);
            if (hit.transform.tag == "Player")
            {
                //Debug.Log("Direction.magnitude = " + direction.magnitude);
                return true;
            }
        }
        return false;
    }

    public void Aim()
    {
        Vector3 direction = player.transform.position - enemyPivot.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        enemyPivot.transform.rotation = Quaternion.Lerp(enemyPivot.transform.rotation, rotation, Time.deltaTime * 6f);
    }

    public void Shoot()
    {
        if (!cooldownActive)
        {
            cooldownActive = true;
            StartCoroutine(coolDownStart());
            GameObject bullet = Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);
            Destroy(bullet, 8.0f);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(firepoint.forward * bulletForce, ForceMode.Impulse);
        }
    }

    IEnumerator coolDownStart()
    {
        yield return new WaitForSeconds(bulletCooldown);
        cooldownActive = false;
    }


    public void takeDamage(float damage)
    {
        hitPoints -= damage;
    }
}
