using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthStatusData", menuName = "StatusObjects/Health", order = 1)]
public class CharacterStatus : ScriptableObject
{

    public string PlayerName;
    public float currentHealth;
    public float maxHealth;
    public GameObject characterGameObject;
    public GameObject currentWeapon;
    public float[] position = new float[3];
    


    
}
