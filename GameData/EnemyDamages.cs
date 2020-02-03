using System;
using System.Collections.Generic;

/**
 * Enemy Damages Data in Each Stage, Stored InGameData
 */
[Serializable]
public class EnemyDamages
{
    public Dictionary<string, Dictionary<string, int>> AllEnemiesStats;
}