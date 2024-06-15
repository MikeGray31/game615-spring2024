using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{


    public static LevelLoader instance;
    public string currentExploreLevel;
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        currentExploreLevel = SceneManager.GetActiveScene().name;
        if(SceneManager.GetActiveScene().name == "StartMenuScene")
        {
            Debug.Log("Starting game");
            StartCoroutine(StartGameExplorationLevel());
        }
    }



    public void LoadNamedLevel(string levelName, bool battling)
    {
        
        if (battling)
        {
            SceneManager.LoadScene(levelName);
            StartCoroutine(LoadBattleLevelCoroutine(levelName));
        }
        else
        {
            StartCoroutine(LoadExplorationLevelCoroutine(levelName));
        }
    }

    IEnumerator StartGameExplorationLevel()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("ExploreScene1");
        currentExploreLevel = "ExploreScene1";
        yield return new WaitForSeconds(0.05f);
        BattleManagerScript.instance.StartGame();
    }
    
    IEnumerator LoadBattleLevelCoroutine(string levelName)
    {
        yield return new WaitForSeconds(0.1f);
        //BattleManagerScript.instance.SetUpBattle();
        yield return new WaitForSeconds(0.1f);
        BattleManagerScript.instance.StartBattle();
    }

    IEnumerator LoadExplorationLevelCoroutine(string levelName)
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(levelName);
    }
}
