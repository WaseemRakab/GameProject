using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayTime", menuName = "ScriptableObjects/PlayTime", order = 8)]
[Serializable]
/**
 * Storing PlayTime in AllScenes
 */
public class ScriptablePlayTime : ScriptableObject
{
    public PlayTime _PlayTime;
}