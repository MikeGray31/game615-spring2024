using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    public string LoadThisFromStart;


    public void StartTheGame()
    {
        SceneManager.LoadScene(LoadThisFromStart);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
