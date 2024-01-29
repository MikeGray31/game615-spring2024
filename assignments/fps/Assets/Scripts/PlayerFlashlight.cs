using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlashlight : MonoBehaviour
{

    public GameObject playerFlashlight;
    // Start is called before the first frame update
    void Start()
    {
        playerFlashlight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Switching flashlight!");
            switchFlashlight();
        }
    }


    private void switchFlashlight()
    {
        if (playerFlashlight.activeInHierarchy) playerFlashlight.SetActive(false);
        else playerFlashlight.SetActive(true);
    }
}
