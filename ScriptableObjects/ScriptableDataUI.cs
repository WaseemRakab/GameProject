using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DataUI", menuName = "ScriptableObjects/DataUI", order = 7)]
[Serializable]
/**
 * Storing DataUI in AllScenes
 */
public class ScriptableDataUI : ScriptableObject
{
    public DataUI _DataUI;
}