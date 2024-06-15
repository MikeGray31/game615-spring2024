using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript instance;

    public GameObject gameOverUI;
    public GameObject winGameUI;
    public GameObject pauseGameUI;
    public GameObject ControlsExplanation;

    public static bool gamePaused;

    private void Awake()
    {
        instance = this;
        //DontDestroyOnLoad(this.gameObject);
        gameOverUI.SetActive(false);
        winGameUI.SetActive(false);
        pauseGameUI.SetActive(false);
        gamePaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gamePaused) Pause();
            else Resume();
        }
    }


    public void GameOver()
    {
        //Debug.Log("Game Over!");
        gameOverUI.SetActive(true);
    }

    public void WinGame()
    {
        //Debug.Log("Player Wins!");
        winGameUI.SetActive(true);
        Time.timeScale = 0f;
    }


    public void Resume()
    {
        //Debug.Log("Should be Resumed!");
        pauseGameUI.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
    }

    public void Pause()
    {
        //Debug.Log("Should be paused!");
        pauseGameUI.SetActive(true);
        Time.timeScale = 0f;
        gamePaused = true;
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

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Resume();
    }

    public void QuitGame()
    {
        //Debug.Log("Quitting game...");
        Application.Quit();
    }
}
