using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    public string LoadThisFromStart;
    public GameObject ControlsExplanation;


    public void StartTheGame()
    {
        SceneManager.LoadScene(LoadThisFromStart);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ExplainControls()
    {
        ControlsExplanation.SetActive(true);
        StartCoroutine(WaitForExplainControlsToDeactivate());
    }

    IEnumerator WaitForExplainControlsToDeactivate()
    {
        yield return new WaitUntil(() => Input.anyKeyDown);
        ControlsExplanation.SetActive(false);
    }
}
