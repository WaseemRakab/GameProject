using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * Controlling Spawning Obstacles InGame
 */
public class SpawnStone : MonoBehaviour
{
    public GameObject FallingStonePrefab;

    public ScriptableSpawnObjects _SpawnObjects;
    public LevelRestartedOrNextLevel _OnLevelRestartedOrNextLevel;


    public int[] MaxStones;

    public int[] WhichDir; // 1 = Player Comes from left Side, -1 = Player Comes From Right Side


    [Range(-40f, 200f)] public float[] MinPositionX;

    [Range(-40f, 200f)] public float[] MaxPositionX;

    public float[] positionY;

    public bool[] ObstaclesToSpawn;


    private void Awake()
    {
        if (_OnLevelRestartedOrNextLevel.OnLevelRestarted || _OnLevelRestartedOrNextLevel.OnNextLevel)
        {
            _SpawnObjects._SpawnObjects.ObstaclesAlive = ObstaclesToSpawn;
        }
        StartSpawning();
    }
    private void StartSpawning()
    {
        List<GameObject> AllObstacles = new List<GameObject>();
        int ObstacleID = 0;
        for (int i = 0; i < MaxStones.Length; ++i)
        {
            for (int j = 0; j < MaxStones[i]; ++j)
            {
                if (_SpawnObjects._SpawnObjects.ObstaclesAlive[ObstacleID++] == true)
                {
                    int MaxRandomize = 0;
                    float obsatclePosX = Random.Range(MinPositionX[i], MaxPositionX[i]);
                    while (MaxRandomize <= 4 && !AllObstacles.All((obs) => Mathf.Abs(obs.transform.position.x - obsatclePosX) >= 7.5f))
                    {
                        obsatclePosX = Random.Range(MinPositionX[i], MaxPositionX[i]);
                        MaxRandomize++;
                    }
                    GameObject obstalce = Instantiate(FallingStonePrefab, new Vector3(obsatclePosX, positionY[i], 0f), FallingStonePrefab.transform.rotation);
                    obstalce.GetComponent<Obstacle>().ChangeMyViewDirection(WhichDir[i]);
                    AllObstacles.Add(obstalce);
                }
                FallingStonePrefab.GetComponent<Obstacle>().ObstacleID++;
            }
        }
        FallingStonePrefab.GetComponent<Obstacle>().ObstacleID = 0;
    }
}