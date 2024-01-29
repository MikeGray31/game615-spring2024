using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{

    public Transform firepoint;
    public GameObject bulletPrefab;
    public float bulletForce = 30f;
    public float bulletCooldown = 0.5f;
    public bool cooldownActive = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        if (!cooldownActive)
        {
            cooldownActive = true;
            StartCoroutine(coolDownStart());
            GameObject bullet = Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);
            Destroy(bullet, 2.0f);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(firepoint.forward * bulletForce, ForceMode.Impulse);
        }
    }

    IEnumerator coolDownStart()
    {
        yield return new WaitForSeconds(bulletCooldown);
        cooldownActive = false;
    }

}
