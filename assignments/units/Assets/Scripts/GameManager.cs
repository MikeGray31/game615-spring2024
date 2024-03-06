using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Camera cam;

    //public UnitScript selectedUnit;
    public GameObject selectedEntity;

    public GameObject selectedEntityPanel;

    public GameObject PlayerUnitInfo;
    public TMP_Text unitNameText;
    public TMP_Text unitGoldText;

    public GameObject HomeBaseInfo;
    public TMP_Text HomeBaseName;
    public TMP_Text HomeBaseGold;

    public GameObject MineInfo;
    public TMP_Text MineName;
    public TMP_Text MineResources;

    public GameObject EnemyInfo;
    public TMP_Text enemyName;
    public TMP_Text enemyHealth;

    public Slider entityHealth;
    public TMP_Text healthText;

    public UnitScript[] allPlayerUnits;
    private bool timeToGetUnits;

    // Start is called before the first frame update
    void Start()
    {
        timeToGetUnits = true;
        SetAllPlayerUnits();
    }

    // Update is called once per frame
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        //selectedEntity = null;
                    }
                }

                else if (Input.GetMouseButtonDown(1))
                {
                    if (selectedEntity != null && selectedEntity.GetComponent<UnitScript>()) {
                        UnitScript selectedUnit = selectedEntity.GetComponent<UnitScript>();
                        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                        {
                            //Debug.Log("Setting Ground Destination!");
                            if (selectedUnit != null) selectedUnit.GoToPoint(hit.point);
                        }
                        if (hit.collider.gameObject.GetComponent<GoldMine>())
                        {
                            //Debug.Log("component 'GoldMine' = " + hit.collider.gameObject.GetComponents<GoldMine>());
                            if (selectedUnit != null) selectedUnit.StartMining(hit.collider.gameObject.GetComponent<GoldMine>());
                        }
                        if (hit.collider.gameObject.GetComponent<HomeBase>())
                        {
                            //Debug.Log("Selected home base!");
                            if (selectedUnit != null) selectedUnit.WalkToBase(hit.collider.gameObject.GetComponent<HomeBase>());
                        }
                        if (hit.collider.gameObject.GetComponent<EnemyScript>())
                        {
                            //Debug.Log("Attacking Enemy!");
                            if (selectedUnit != null) selectedUnit.MoveToAttackEnemy(hit.collider.gameObject.GetComponent<EnemyScript>());
                        }
                    }
                }


            }
            else
            {
                if (selectedEntity.GetComponent<UnitScript>())
                {
                    UnitScript selectedUnit = selectedEntity.GetComponent<UnitScript>();
                    // If the raycast didn't collide with anything, deselect the unit.
                    if (selectedUnit != null)
                    {
                        selectedUnit.bodyRenderer.material.color = selectedUnit.unselectedColor;
                    }
                    selectedUnit = null;
                    // Turn off the UI.
                    selectedEntityPanel.SetActive(false);
                }
            }
        }
        
        
        if (selectedEntity != null)
        {
            if (selectedEntity.GetComponent<UnitScript>()) UpdatePlayerUnitDisplay(selectedEntity.GetComponent<UnitScript>());
            else if (selectedEntity.GetComponent<HomeBase>()) SelectBase(selectedEntity.GetComponent<HomeBase>());
            else if (selectedEntity.GetComponent<GoldMine>()) SelectMine(selectedEntity.GetComponent<GoldMine>());
            else if (selectedEntity.GetComponent<EnemyScript>()) UpdateEnemyDisplay(selectedEntity.GetComponent<EnemyScript>());
        }


        if (timeToGetUnits)
        {
            StartCoroutine(SetAllPlayerUnits()); ;
        }
    }

    public void SelectEntity(GameObject selectThis)
    {

        if(selectedEntity != null && selectedEntity.GetComponent<UnitScript>())
        {
            //Debug.Log("Is this even useful?");
            selectedEntity.GetComponent<UnitScript>().bodyRenderer.material.color = selectedEntity.GetComponent<UnitScript>().unselectedColor;
        }

        PlayerUnitInfo.SetActive(false);
        HomeBaseInfo.SetActive(false);
        MineInfo.SetActive(false);
        EnemyInfo.SetActive(false);
        //entityHealth.gameObject.SetActive(false);

        entityHealth.maxValue = 1;
        entityHealth.value = 0;
        healthText.text = " / ";

        if (selectThis.GetComponent<UnitScript>())
        {
            selectedEntity = selectThis;
            selectedEntityPanel.SetActive(true);
            PlayerUnitInfo.SetActive(true);
            entityHealth.gameObject.SetActive(true);
            SelectPlayerUnit(selectThis.GetComponent<UnitScript>());
        }
        else if (selectThis.GetComponent<GoldMine>())
        {
            selectedEntity = selectThis;
            selectedEntityPanel.SetActive(true);
            MineInfo.SetActive(true);
            SelectMine(selectThis.GetComponent<GoldMine>());
        }
        else if (selectThis.GetComponent<HomeBase>())
        {
            selectedEntity = selectThis;
            selectedEntityPanel.SetActive(true);
            HomeBaseInfo.SetActive(true);
            SelectBase(selectThis.GetComponent<HomeBase>());
        }
        else if (selectThis.GetComponent<EnemyScript>())
        {
            selectedEntity = selectThis;
            selectedEntityPanel.SetActive(true);
            EnemyInfo.SetActive(true);
            entityHealth.gameObject.SetActive(true);
            SelectEnemy(selectThis.GetComponent<EnemyScript>());
        }
    }

    public void SelectPlayerUnit(UnitScript unit)
    {
        unit.bodyRenderer.material.color = unit.selectedColor;
        unitNameText.text = "Unit Type: " + unit.characterName;
        unitGoldText.text = "Gold Carried: " + unit.currentGold + "/" + unit.maxGoldCapacity;

        entityHealth.maxValue = unit.maxHealth;
        entityHealth.value = unit.health;
        healthText.text = unit.health + " / " + unit.maxHealth;
    }

    public void UpdatePlayerUnitDisplay(UnitScript unit)
    {
        unitNameText.text = "Unit Type: " + unit.characterName;
        unitGoldText.text = "Gold Carried: " + unit.currentGold + "/" + unit.maxGoldCapacity;
        entityHealth.value = unit.health;
    }

    public void SelectMine(GoldMine mine)
    {
        MineName.text = "Mine Name: " + mine.mineName;
        MineResources.text = "Resources Left: " + mine.currentMinables;
    }

    public void SelectBase(HomeBase homeBase)
    {
        HomeBaseName.text = "Base Name: " + homeBase.baseName;
        HomeBaseGold.text = "Gold Stored: " + homeBase.storedGold;
    }

    public void SelectEnemy(EnemyScript enemy)
    {
        enemyName.text = "Enemy Name: " + enemy.enemyName;
        enemyHealth.text = "Health: " + enemy.health + " / " + enemy.maxHealth;
        entityHealth.value = enemy.health;
        entityHealth.maxValue = enemy.maxHealth;
        healthText.text = enemy.health + " / " + enemy.maxHealth;
    }
    public void UpdateEnemyDisplay(EnemyScript enemy)
    {
        enemyName.text = "Enemy Name: " + enemy.enemyName;
        enemyHealth.text = "Health: " + enemy.health + " / " + enemy.maxHealth;
        entityHealth.value = enemy.health;
    }

    public UnitScript[] getAllPlayerUnits()
    {
        return allPlayerUnits;
    }

    IEnumerator SetAllPlayerUnits()
    {
        allPlayerUnits = GameObject.FindObjectsByType<UnitScript>(0);
        timeToGetUnits = false;
        yield return new WaitForSeconds(0.5f);
        //Debug.Log("This should be called every 0.5 seconds!");
        timeToGetUnits = true;
    }

    public void DeployFighterFromBase()
    {
        if (selectedEntity != null && selectedEntity.GetComponent<HomeBase>())
        {
            selectedEntity.GetComponent<HomeBase>().DeployFighter();   
        }
    }

    public void DeployMinerFromBase()
    {
        if (selectedEntity != null && selectedEntity.GetComponent<HomeBase>())
        {
            selectedEntity.GetComponent<HomeBase>().DeployMiner();
        }
    }
}

