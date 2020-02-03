using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelRestartedOrNextLevel", menuName = "ScriptableObjects/LevelRestartedOrNextLevel", order = 6)]
[Serializable]
/**
 * OnLevelRestartedOrNextLevel Storing Booleans, For SaveGame,LoadGame Etc..
 */
public class LevelRestartedOrNextLevel : ScriptableObject
{
    public bool OnLevelRestarted = false;
    public bool OnNextLevel = false;

    public bool OnLevelFinished = false;
    public bool OnStageFinished = false;
}