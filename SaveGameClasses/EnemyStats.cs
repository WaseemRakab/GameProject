using System;
using System.Collections.Generic;
/**
 * In-Game EnemyStats Storing
 */
[Serializable]
public class EnemyStats
{
    public List<float> EnemyPistolHealths = new List<float>() { 100f };
    public List<float> EnemyAutoGunWOBunkerHealths = new List<float>(0);
    public List<float> EnemyAutoGunWBunkerHealths = new List<float>(0);
    public List<float> TurretHealths = new List<float>() { 100f };
    public List<float> TurretOnWallHealths = new List<float>(0);
}