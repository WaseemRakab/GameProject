using System;
using System.Collections.Generic;
/**
 * Level Data in Each Level, Stored InGameData
 * Storing All Interactable Objects position , Scales Etc..
 */
[Serializable]
public class LevelData
{
    public Dictionary<string, List<Vector>> LevelScales;
    public Dictionary<string, List<Vector>> LevelPositions;
}