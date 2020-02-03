using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptablePlayerStats", menuName = "ScriptableObjects/ScriptablePlayerStats", order = 4)]
[Serializable]
/**
 * Storing PlayerStats in AllScenes
 */
public class ScriptablePlayerStats : ScriptableObject
{
    public PlayerStats _PlayerStats;
}