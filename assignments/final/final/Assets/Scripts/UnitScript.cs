using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitScript : MonoBehaviour
{

    public string unitName;
    public float unitHealth;
    public float maxHealth;

    public HealthBar healthBar;

    public event UnityAction unitDied = delegate { };

    private void Start()
    {
        unitHealth = maxHealth;

    }

    private void OnEnable()
    {
        unitDied += unitDeath;
    }

    private void OnDisable()
    {
        unitDied -= unitDeath;
    }

    private void Update()
    {
      
    }
    public void TakeDamage(float damage)
    {
        unitHealth = Mathf.Clamp(unitHealth - damage, 0, maxHealth);
        //Debug.Log(unitName + " took " + damage + " damage! " + unitName + "'s health is " + unitHealth + "/" + maxHealth);
        UpdateHealthBar();
        if(unitHealth <= 0)
        {
            //Debug.Log(unitName + "'s health reached 0!");
            unitDied?.Invoke();
        }
    }

    public void healUnit(float healingPoints)
    {
        Debug.Log(unitName + " should heal for " + healingPoints + " points!");
        unitHealth = Mathf.Clamp(unitHealth + healingPoints, 0, maxHealth);
        UpdateHealthBar();
    }

    public void unitDeath()
    {
        //Debug.Log(unitName + " died!");
    }

    public void UpdateHealthBar()
    {
        if(healthBar != null)
        {
            healthBar.UpdateHealthBar(unitHealth, maxHealth);
        }
    }
}
