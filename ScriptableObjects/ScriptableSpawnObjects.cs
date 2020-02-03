using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableSpawnObjects", menuName = "ScriptableObjects/ScriptableSpawnObjects", order = 5)]
[Serializable]
/**
 * Storing SpawnObject in AllScenes
 */
public class ScriptableSpawnObjects : ScriptableObject
{
    public SpawnObjects _SpawnObjects;
}