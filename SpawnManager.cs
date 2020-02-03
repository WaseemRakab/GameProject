using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/**
 * Managing Spawning Objects in-Game Like Enemies , Interactable Objects , Obstalce and Etc..
 * Which Position , Scales..
 */
public class SpawnManager : MonoBehaviour
{
    public GameObject EnemyWithPistol;
    public GameObject EnemyWithAutoGunWithoutBunker;
    public GameObject EnemyWithAutoGunWithBunker;
    public GameObject TurretLauncher;
    public GameObject TurrentLauncherWall;
    public GameObject RockMovingSurface;
    public GameObject SciFiMovingPlatform;
    public GameObject PushableBox;
    public GameObject CoinChestsPrefab;

    public SpawnObjects _SpawnObjects;

    public Vector PlayerPosition;

    public ScriptableEnemyStats _ScriptableEnemyStats;
    public ScriptableSpawnObjects _ScriptableSpawnObjects;
    public ScriptablePlayerStats _ScriptablePlayerStats;
    public LevelRestartedOrNextLevel _OnLevelRestartedOrNextLevel;

    private LevelData LevelSpawnData;
    private EnemyDamages _EnemyDamages;

    private int StageNumber;

    private delegate void SpawningMovingSurfaces(GameObject MovingSurface);
    private event SpawningMovingSurfaces OnSpawningMovingSurfaces;
    public Teleporter SourceTeleporter;

    public List<bool> HasSwitch;//Moving Surfaces

    private void Awake()
    {
        Scene scene = SceneManager.GetActiveScene();
        StageNumber = (scene.buildIndex - ((scene.buildIndex + 1) % 3 + 1)) / 3 + 1;
        if (_OnLevelRestartedOrNextLevel.OnLevelRestarted || _OnLevelRestartedOrNextLevel.OnNextLevel)
        {
            OnRestartOrNextLevelResetSpawnPositions();
        }
        if (SourceTeleporter != null)
        {
            OnSpawningMovingSurfaces += SourceTeleporter.AddMovingSurfaces;
        }
        //LevelDataManager.SaveLevelData(scene.name);
        if (LevelDataManager.LoadLevelData(scene.name, out LevelSpawnData)
                                                        &&
            LevelDataManager.LoadEnemiesDamage("EnemiesDamage", out _EnemyDamages))//Succeded
        {
            if (_OnLevelRestartedOrNextLevel.OnLevelRestarted || _OnLevelRestartedOrNextLevel.OnNextLevel)
            {
                ResetEnemyStats();
            }
            SpawnCoinChests();
            SpawnRockMovingSurfaces();
            SpawnSciFiMovingPlatforms();
            SpawnPushableBoxes();
            SpawnEnemiesWithPistol();
            SpawniEnemiesWithAutoGunWithoutBunker();
            SpawnEnemiesWithAutoGunWithBunker();
            SpawnTurrentLaunchers();
            SpawnTurrentLaunchersWithWall();
        }
    }
    private void ResetEnemyStats()
    {
        ResetEnemyWithPistolStats();
        ResetEnemyAutoGunWithoutBunkerStats();
        ResetEnemyAutoGunWithBunkerStats();
        ResetTurretStats();
        ResetTurretOnWallStats();
    }
    private void ResetEnemyWithPistolStats()
    {
        if (LevelSpawnData.LevelPositions.ContainsKey("EnemyPistol"))
        {
            _ScriptableEnemyStats._EnemyStats.EnemyPistolHealths = new List<float>(LevelSpawnData.LevelPositions["EnemyPistol"].Count);
            ResetEnemyHealth(_ScriptableEnemyStats._EnemyStats.EnemyPistolHealths);
        }
    }
    private void ResetEnemyAutoGunWithoutBunkerStats()
    {
        if (LevelSpawnData.LevelPositions.ContainsKey("EnemyAutoGunWOBunker"))
        {
            _ScriptableEnemyStats._EnemyStats.EnemyAutoGunWOBunkerHealths = new List<float>(LevelSpawnData.LevelPositions["EnemyAutoGunWOBunker"].Count);
            ResetEnemyHealth(_ScriptableEnemyStats._EnemyStats.EnemyAutoGunWOBunkerHealths);
        }
    }
    private void ResetEnemyAutoGunWithBunkerStats()
    {
        if (LevelSpawnData.LevelPositions.ContainsKey("EnemyAutoGunWBunker"))
        {
            _ScriptableEnemyStats._EnemyStats.EnemyAutoGunWBunkerHealths = new List<float>(LevelSpawnData.LevelPositions["EnemyAutoGunWBunker"].Count);
            ResetEnemyHealth(_ScriptableEnemyStats._EnemyStats.EnemyAutoGunWBunkerHealths);
        }
    }
    private void ResetTurretStats()
    {
        if (LevelSpawnData.LevelPositions.ContainsKey("Turret"))
        {
            _ScriptableEnemyStats._EnemyStats.TurretHealths = new List<float>(LevelSpawnData.LevelPositions["Turret"].Count);
            ResetEnemyHealth(_ScriptableEnemyStats._EnemyStats.TurretHealths);
        }
    }
    private void ResetTurretOnWallStats()
    {
        if (LevelSpawnData.LevelPositions.ContainsKey("TurretOnWall"))
        {
            _ScriptableEnemyStats._EnemyStats.TurretOnWallHealths = new List<float>(LevelSpawnData.LevelPositions["TurretOnWall"].Count);
            ResetEnemyHealth(_ScriptableEnemyStats._EnemyStats.TurretOnWallHealths);
        }
    }
    private void ResetEnemyHealth(List<float> Healths)
    {
        for (int i = 0; i < Healths.Capacity; ++i)
        {
            Healths.Add(100f);
        }
    }
    private void SpawnCoinChests()
    {
        if (LevelSpawnData.LevelPositions.ContainsKey("CoinChest"))
        {
            for (int i = 0; i < LevelSpawnData.LevelPositions["CoinChest"].Count; ++i)
            {
                Vector CoinChestPos = LevelSpawnData.LevelPositions["CoinChest"][i];
                if (LevelSpawnData.LevelScales.ContainsKey("CoinChest"))
                {
                    Vector CoinChestScale = LevelSpawnData.LevelScales["CoinChest"][i];
                    CoinChestsPrefab.transform.localScale = new Vector3(CoinChestScale.X, CoinChestScale.Y, CoinChestScale.Z);
                }
                else
                    CoinChestsPrefab.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                Instantiate(CoinChestsPrefab, new Vector3(CoinChestPos.X, CoinChestPos.Y, CoinChestPos.Z), Quaternion.identity);
                CoinChestsPrefab.GetComponent<CoinChest>().CoinChestID++;
            }
            CoinChestsPrefab.GetComponent<CoinChest>().CoinChestID = 0;
        }
    }
    private void SpawnRockMovingSurfaces()
    {
        if (LevelSpawnData.LevelPositions.ContainsKey("RockMovingSurface"))
        {
            for (int i = 0; i < LevelSpawnData.LevelPositions["RockMovingSurface"].Count; ++i)
            {
                Vector RockMovingPos = LevelSpawnData.LevelPositions["RockMovingSurface"][i];
                Vector RockMovingScale = LevelSpawnData.LevelScales["RockMovingSurface"][i];
                RockMovingSurface.transform.localScale = new Vector3(RockMovingScale.X, RockMovingScale.Y, RockMovingScale.Z);
                GameObject RockMoving = Instantiate(RockMovingSurface, new Vector3(RockMovingPos.X, RockMovingPos.Y, RockMovingPos.Z), Quaternion.identity);
                if (HasSwitch[i])
                    RockMoving.GetComponent<MovingSurface>().HasSwitch = true;
                OnSpawningMovingSurfaces?.Invoke(RockMoving);
                RockMovingSurface.GetComponent<MovingSurface>().SurfaceID++;
            }
            RockMovingSurface.GetComponent<MovingSurface>().SurfaceID = 0;
        }
    }
    private void SpawnSciFiMovingPlatforms()
    {
        if (LevelSpawnData.LevelPositions.ContainsKey("SciFiMovingPlatform"))
        {
            for (int i = 0; i < LevelSpawnData.LevelPositions["SciFiMovingPlatform"].Count; ++i)
            {
                Vector SciFiMovingPos = LevelSpawnData.LevelPositions["SciFiMovingPlatform"][i];
                Vector SciFiMovingScale = LevelSpawnData.LevelScales["SciFiMovingPlatform"][i];
                SciFiMovingPlatform.transform.localScale = new Vector3(SciFiMovingScale.X, SciFiMovingScale.Y, SciFiMovingScale.Z);
                GameObject SciMoving = Instantiate(SciFiMovingPlatform, new Vector3(SciFiMovingPos.X, SciFiMovingPos.Y, SciFiMovingPos.Z), Quaternion.identity);
                if (HasSwitch[i])
                    SciMoving.GetComponent<MovingSurface>().HasSwitch = true;
                SciFiMovingPlatform.GetComponent<MovingSurface>().SurfaceID++;
            }
            SciFiMovingPlatform.GetComponent<MovingSurface>().SurfaceID = 0;
        }
    }
    private void SpawnPushableBoxes()
    {
        if (LevelSpawnData.LevelPositions.ContainsKey("PushableBox"))
        {
            for (int i = 0; i < LevelSpawnData.LevelPositions["PushableBox"].Count; ++i)
            {
                Vector BoxPos = LevelSpawnData.LevelPositions["PushableBox"][i];
                Instantiate(PushableBox, new Vector3(BoxPos.X, BoxPos.Y, BoxPos.Z), Quaternion.identity);
                PushableBox.GetComponent<PushableBox>().PushableBoxID++;
            }
            PushableBox.GetComponent<PushableBox>().PushableBoxID = 0;
        }
    }
    private void SpawnEnemiesWithPistol()
    {
        if (LevelSpawnData.LevelPositions.ContainsKey("EnemyPistol"))
        {
            for (int i = 0; i < LevelSpawnData.LevelPositions["EnemyPistol"].Count; ++i)
            {
                if (_ScriptableSpawnObjects._SpawnObjects.EnemiesWithPistolAlive[i] == true)
                {
                    Vector PistolVec = LevelSpawnData.LevelPositions["EnemyPistol"][i];
                    GameObject EnemyWithPistol = Instantiate(this.EnemyWithPistol, new Vector3(PistolVec.X, PistolVec.Y, PistolVec.Z), Quaternion.identity);
                    EnemyWithPistol.GetComponent<EnemyPistolBehaviour>().MyDamage = _EnemyDamages.AllEnemiesStats[StageNumber + ""]["EnemyPistol"];
                }
                EnemyWithPistol.GetComponent<EnemyPistolBehaviour>().EnemyID++;
            }
        }
        EnemyWithPistol.GetComponent<EnemyPistolBehaviour>().EnemyID = 0;
    }
    private void SpawniEnemiesWithAutoGunWithoutBunker()
    {
        if (LevelSpawnData.LevelPositions.ContainsKey("EnemyAutoGunWOBunker"))
        {
            for (int i = 0; i < LevelSpawnData.LevelPositions["EnemyAutoGunWOBunker"].Count; ++i)
            {
                if (_ScriptableSpawnObjects._SpawnObjects.EnemiesWithAutoGunWithoutBunkerAlive[i] == true)
                {
                    Vector WoBunkerPos = LevelSpawnData.LevelPositions["EnemyAutoGunWOBunker"][i];
                    GameObject EnemyWithAutoGunWithoutBunker = Instantiate(this.EnemyWithAutoGunWithoutBunker, new Vector3(WoBunkerPos.X, WoBunkerPos.Y, WoBunkerPos.Z), Quaternion.identity);
                    EnemyWithAutoGunWithoutBunker.GetComponent<EnemyAutoGunWOBunkerBehaviour>().MyDamage = _EnemyDamages.AllEnemiesStats[StageNumber + ""]["EnemyAutoGunWOBunker"];
                }
                EnemyWithAutoGunWithoutBunker.GetComponent<EnemyAutoGunWOBunkerBehaviour>().EnemyID++;
            }
        }
        EnemyWithAutoGunWithoutBunker.GetComponent<EnemyAutoGunWOBunkerBehaviour>().EnemyID = 0;
    }
    private void SpawnEnemiesWithAutoGunWithBunker()
    {
        if (LevelSpawnData.LevelPositions.ContainsKey("EnemyAutoGunWBunker"))
        {
            for (int i = 0; i < LevelSpawnData.LevelPositions["EnemyAutoGunWBunker"].Count; ++i)
            {
                if (_ScriptableSpawnObjects._SpawnObjects.EnemiesWithAutoGunWithBunkerAlive[i] == true)
                {
                    if (LevelSpawnData.LevelScales != null &&
                        LevelSpawnData.LevelScales.ContainsKey("EnemyAutoGunWBunker"))
                    {
                        Vector myScale = LevelSpawnData.LevelScales["EnemyAutoGunWBunker"][i];
                        this.EnemyWithAutoGunWithBunker.transform.localScale = new Vector3(myScale.X, myScale.Y, myScale.Z);
                    }
                    else
                        this.EnemyWithAutoGunWithBunker.transform.localScale = new Vector3(1f, 1f, 1f);
                    Vector WithBunkerPos = LevelSpawnData.LevelPositions["EnemyAutoGunWBunker"][i];
                    GameObject EnemyWithAutoGunWithBunker = Instantiate(this.EnemyWithAutoGunWithBunker, new Vector3(WithBunkerPos.X, WithBunkerPos.Y, WithBunkerPos.Z), Quaternion.identity);
                    EnemyWithAutoGunWithBunker.transform.GetChild(0).GetComponent<EnemyAutoGunWBunkerBehaviour>().MyDamage = _EnemyDamages.AllEnemiesStats[StageNumber + ""]["EnemyAutoGunWBunker"];
                }
                EnemyWithAutoGunWithBunker.transform.GetChild(0).GetComponent<EnemyAutoGunWBunkerBehaviour>().EnemyID++;
            }
        }
        EnemyWithAutoGunWithBunker.transform.GetChild(0).GetComponent<EnemyAutoGunWBunkerBehaviour>().EnemyID = 0;
    }
    private void SpawnTurrentLaunchers()
    {
        if (LevelSpawnData.LevelPositions.ContainsKey("Turret"))
        {
            for (int i = 0; i < LevelSpawnData.LevelPositions["Turret"].Count; ++i)
            {
                if (_ScriptableSpawnObjects._SpawnObjects.TurrentLaunchersAlive[i] == true)
                {
                    if (LevelSpawnData.LevelScales != null &&
                        LevelSpawnData.LevelScales.ContainsKey("Turret"))
                    {
                        Vector myScale = LevelSpawnData.LevelScales["Turret"][i];
                        this.TurretLauncher.transform.localScale = new Vector3(myScale.X, myScale.Y, myScale.Z);
                    }
                    else
                        this.TurretLauncher.transform.localScale = new Vector3(3f, 4f, 1f);//Default
                    Vector TurretPos = LevelSpawnData.LevelPositions["Turret"][i];
                    GameObject TurretLauncher = Instantiate(this.TurretLauncher, new Vector3(TurretPos.X, TurretPos.Y, TurretPos.Z), Quaternion.identity);
                    TurretLauncher.GetComponent<TurretLauncher>().MyDamage = _EnemyDamages.AllEnemiesStats[StageNumber + ""]["Turret"];
                }
                TurretLauncher.GetComponent<TurretLauncher>().TurretID++;
            }
        }
        TurretLauncher.GetComponent<TurretLauncher>().TurretID = 0;
    }
    private void SpawnTurrentLaunchersWithWall()
    {
        if (LevelSpawnData.LevelPositions.ContainsKey("TurretOnWall"))
        {
            for (int i = 0; i < LevelSpawnData.LevelPositions["TurretOnWall"].Count; ++i)
            {
                if (_ScriptableSpawnObjects._SpawnObjects.TurrentLaunchersWallAlive[i] == true)
                {
                    if (LevelSpawnData.LevelScales != null &&
                        LevelSpawnData.LevelScales.ContainsKey("TurretOnWall"))
                    {
                        Vector myScale = LevelSpawnData.LevelScales["TurretOnWall"][i];
                        this.TurrentLauncherWall.transform.localScale = new Vector3(myScale.X, myScale.Y, myScale.Z);
                    }
                    else
                        this.TurrentLauncherWall.transform.localScale = new Vector3(3.893728f, 4.412f, 0f);//Default
                    Vector TurretWallPos = LevelSpawnData.LevelPositions["TurretOnWall"][i];
                    GameObject TurrentLauncherWall = Instantiate(this.TurrentLauncherWall, new Vector3(TurretWallPos.X, TurretWallPos.Y, TurretWallPos.Z), Quaternion.identity);
                    TurrentLauncherWall.GetComponent<TurretLauncher>().MyDamage = _EnemyDamages.AllEnemiesStats[StageNumber + ""]["TurretOnWall"];
                }
                TurrentLauncherWall.GetComponent<TurretLauncher>().TurretID++;
            }
        }
        TurrentLauncherWall.GetComponent<TurretLauncher>().TurretID = 0;
    }
    private void OnRestartOrNextLevelResetSpawnPositions()
    {
        _ScriptableSpawnObjects._SpawnObjects.EnemiesWithAutoGunWithBunkerAlive = _SpawnObjects.EnemiesWithAutoGunWithBunkerAlive;
        _ScriptableSpawnObjects._SpawnObjects.EnemiesWithAutoGunWithoutBunkerAlive = _SpawnObjects.EnemiesWithAutoGunWithoutBunkerAlive;
        _ScriptableSpawnObjects._SpawnObjects.EnemiesWithPistolAlive = _SpawnObjects.EnemiesWithPistolAlive;
        _ScriptableSpawnObjects._SpawnObjects.TurrentLaunchersAlive = _SpawnObjects.TurrentLaunchersAlive;
        _ScriptableSpawnObjects._SpawnObjects.TurrentLaunchersWallAlive = _SpawnObjects.TurrentLaunchersWallAlive;

        _ScriptablePlayerStats._PlayerStats.PlayerPosition = PlayerPosition;
    }
}