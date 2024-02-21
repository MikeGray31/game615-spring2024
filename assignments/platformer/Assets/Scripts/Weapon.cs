using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    //public GameObject bulletPrefab;
    public Transform firepoint;
    public GameObject muzzleFlashPrefab;
    public GameObject bulletHitFlash;
    public float bulletForce = 30f;
    public float shootDelay = 0.1f;
    private bool cooldownActive = false;

    [SerializeField]
    private bool bulletSpread = true;
    //private Vector3 bulletSpreadVariance;
    private float currentBulletSpreadVariance = 0.1f;
    public float hipFireSpreadVariance = 0.1f;
    public float aimSpreadVariance = 0.015f;    

    [SerializeField]
    private TrailRenderer bulletTrail;

    private void Start()
    {
        cooldownActive = false;
        //bulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
        currentBulletSpreadVariance = hipFireSpreadVariance;
    }


    public void Shoot()
    {
        if (!cooldownActive)
        {
            cooldownActive = true;
            StartCoroutine(coolDownStart());

            Vector3 direction = GetDirection();

            if(Physics.Raycast(firepoint.position, direction, out RaycastHit hit))
            {
                if(hit.distance < 200f)
                {
                    TrailRenderer trail = Instantiate(bulletTrail, firepoint.position, Quaternion.identity);
                    StartCoroutine(SpawnTrail(trail, hit.point, hit.normal));
                    if (hit.collider.gameObject.GetComponent<EnemyController>())
                    {
                        //Debug.Log("Enemy Damaged!");
                        hit.collider.gameObject.GetComponent<EnemyController>().takeDamage(1f);
                    }
                    if(hit.collider.tag == "Bullet")
                    {
                        //Debug.Log("Bullet Destroyed!");
                        Destroy(hit.collider.gameObject);
                    }
                }
                else
                {
                    TrailRenderer trail = Instantiate(bulletTrail, firepoint.position, Quaternion.identity);
                    Vector3 aimPoint = firepoint.position + firepoint.forward * 200f;
                    //Vector3 hitNothingNormal = firepoint.forward;
                    StartCoroutine(SpawnTrail(trail, aimPoint, firepoint.forward));
                }
            }
            else
            {
                TrailRenderer trail = Instantiate(bulletTrail, firepoint.position, Quaternion.identity);
                Vector3 aimPoint = firepoint.position + firepoint.forward * 200f;
                //Vector3 hitNothingNormal = firepoint.forward;
                StartCoroutine(SpawnTrail(trail, aimPoint, firepoint.forward));
            }

            //Debug.Log("Making Muzzle flash!");
            GameObject effect = Instantiate(muzzleFlashPrefab, firepoint.position, Quaternion.identity);
            Destroy(effect, 0.10f);
            //Destroy(gameObject);
            /*GameObject bullet = Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);
            Destroy(bullet, 2.0f);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(firepoint.forward * bulletForce, ForceMode.Impulse);*/
        }
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = firepoint.forward;
        if (bulletSpread)
        {
            direction += new Vector3(
                                Random.Range(-currentBulletSpreadVariance, currentBulletSpreadVariance),
                                Random.Range(-currentBulletSpreadVariance, currentBulletSpreadVariance),
                                Random.Range(-currentBulletSpreadVariance, currentBulletSpreadVariance));
            direction.Normalize();
        }
        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint, Vector3 hitPointNormal)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while(time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hitPoint, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }

        trail.transform.position = hitPoint;
        GameObject bulletHitFlashObject = Instantiate(bulletHitFlash, hitPoint, Quaternion.LookRotation(hitPointNormal));
        Destroy(bulletHitFlashObject, 0.1f);
        Destroy(trail.gameObject, trail.time);
    }

    IEnumerator coolDownStart()
    {
        yield return new WaitForSeconds(shootDelay);
        cooldownActive = false;
    }

    public bool getCooldownActive()
    {
        return cooldownActive;
    }


    public void SetToHipFireSpread()
    {
        currentBulletSpreadVariance = hipFireSpreadVariance;
    }

    public void SetToAimSpread()
    {
        currentBulletSpreadVariance = aimSpreadVariance;
    }
}
