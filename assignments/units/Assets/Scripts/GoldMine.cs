using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldMine : MonoBehaviour
{

    GameManager gameManager;

    public string mineName = "Gold Mine";

    public float maxMinables = 1000f;
    public float currentMinables;

    public Renderer bodyRenderer;
    public Color HasGoldColor;
    public Color EmptyColor;
    

    void Start()
    {
        GameObject gmObj = GameObject.Find("GameManagerObject");
        gameManager = gmObj.GetComponent<GameManager>();

        currentMinables = maxMinables;
    }


    public float beMined(float miningRate)
    {

        float mined = 0;
        if (miningRate > currentMinables)
        {
            mined = currentMinables;
            currentMinables = 0;
            bodyRenderer.material.color = EmptyColor;

        }
        else
        {
            currentMinables -= miningRate;
            mined = miningRate;
        }

        return mined;
    }

    public void returnMined(float returnAmount)
    {
        currentMinables = Mathf.Clamp(currentMinables + returnAmount, 0, maxMinables);
    }

    public void OnMouseDown()
    {
        //Debug.Log("OnMouseDown called!");
        gameManager.SelectEntity(gameObject);
    }
}
