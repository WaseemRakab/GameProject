using System;
using System.Collections.Generic;
/**
 * In-Game SpawnObjects Storing,To Detected which Enemy,Obstalce Gone,For SaveGame-LoadGame
 */
[Serializable]
public class SpawnObjects
{
    public string objectName = "SpawnObjects";
    public bool[] EnemiesWithPistolAlive = new bool[] { true };
    public bool[] EnemiesWithAutoGunWithoutBunkerAlive = new bool[0];
    public bool[] EnemiesWithAutoGunWithBunkerAlive = new bool[0];
    public bool[] TurrentLaunchersAlive = new bool[] { true };
    public bool[] TurrentLaunchersWallAlive = new bool[0];
    public bool[] ObstaclesAlive = new bool[] { true, true, true, true, true };

    public List<Vector> RockMovingSurfacesPositions = new List<Vector>(0);
    public List<Vector> SciFiMovingSurfacesPositions = new List<Vector>(0);

    public List<Vector> PushableBoxPositions = new List<Vector>(0);
    public SpawnObjects() { }
}