using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlaneScript : MonoBehaviour
{

    public float normalSpeed = 0.05f;
    public float boostSpeed = 0.1f;
    //public float brakeSpeed = 0.01f;
    public float turnSpeedV = 10f;
    public float turnSpeedH = 10f;

    //public Transform cameraParent;
    public Transform playerModel;
    public MoveAlongObjectForward GamePlayPlane;

    int score = 0;

    public TMPro.TMP_Text speedText;
    public TMPro.TMP_Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        GamePlayPlane.speed = normalSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        RotationLook(h, v);
        HorizontalLean(playerModel, h, v, 80, 0.5f);
        StopFromGoingUnderground();
        SetForwardSpeed();
        DisplaySpeed();
    }

    void RotationLook(float h, float v)
    {
        Vector3 aimRot = GamePlayPlane.transform.localEulerAngles;
        aimRot.z = 0;
        aimRot.y += h * turnSpeedH * Time.deltaTime;
        aimRot.x += v * turnSpeedV * Time.deltaTime;
        aimRot.x = ClampAngle(aimRot.x, -60, 60);
        GamePlayPlane.transform.localRotation = Quaternion.Euler(aimRot.x, aimRot.y, aimRot.z);
        //GamePlayPlane.transform.DOLocalRotate(aimRot, 0.1f);
    }

    void HorizontalLean(Transform target, float axisH, float axisV, float leanLimit, float lerpTime)
    {
        Vector3 targetEulerAngels = target.localEulerAngles;
        target.localEulerAngles =
            new Vector3(targetEulerAngels.x,
                        targetEulerAngels.y,
                        Mathf.LerpAngle(targetEulerAngels.z, -axisH * leanLimit, Time.deltaTime / lerpTime));
        /*target.localEulerAngles =
            new Vector3(Mathf.LerpAngle(targetEulerAngels.x, axisV * leanLimit, Time.deltaTime / lerpTime),
                        targetEulerAngels.y,
                        Mathf.LerpAngle(targetEulerAngels.z, -axisH * leanLimit, Time.deltaTime / lerpTime));*/
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        float start = (min + max) * 0.5f - 180;
        float floor = Mathf.FloorToInt((angle - start) / 360) * 360;
        min += floor;
        max += floor;
        return Mathf.Clamp(angle, min, max);
    }

    public void StopFromGoingUnderground()
    {
        float terrainY = Terrain.activeTerrain.SampleHeight(transform.position);
        if (transform.position.y < terrainY + 0.5f)
        {
            GamePlayPlane.transform.position = new Vector3(transform.position.x, terrainY + 0.51f, transform.position.z);
        }
    }

    public void SetForwardSpeed()
    {
        if (GamePlayPlane.speed > 0.3f) GamePlayPlane.speed = 0.3f;
        if(GamePlayPlane.speed > normalSpeed)
        {
            GamePlayPlane.speed -= 0.0175f * Time.deltaTime;
            if (GamePlayPlane.speed < normalSpeed) GamePlayPlane.speed = normalSpeed;
        }
        turnSpeedH = 50f + ((GamePlayPlane.speed - 0.09f) * 100f);
        turnSpeedV = 50f + ((GamePlayPlane.speed - 0.09f) * 100f);
    }

    public void DisplaySpeed()
    {
        speedText.text = "Speed " + (GamePlayPlane.speed * 1000).ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ring"))
        {
            Destroy(other.gameObject);
            score++; // score = score + 1;
            scoreText.text = "Score: " + score.ToString();
            //Debug.Log("Ring gotten!");
            GamePlayPlane.speed += 0.05f;
        }
    }
}
