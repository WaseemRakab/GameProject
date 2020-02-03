using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "EnemyStats", menuName = "ScriptableObjects/EnemyStats", order = 10)]
/**
 * Storing EnemyStats in AllScenes
 */
public class ScriptableEnemyStats : ScriptableObject
{
    public string objectName = "EnemyStats";

    public EnemyStats _EnemyStats;
}