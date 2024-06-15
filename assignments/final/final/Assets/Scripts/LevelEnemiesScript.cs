using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemiesInLevelData", menuName = "StatusObjects/LevelData", order = 1)]
public class LevelEnemiesScript : ScriptableObject
{

    public string levelName;
    public List<EnemyController> enemiesInLevel;
    public List<EnemyStatus> enemiesInLevelStatuses;

}
