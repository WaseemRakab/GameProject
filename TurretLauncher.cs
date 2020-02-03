using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
/**
 * Turret Launcher Behaviour (Enemy) Towards Player
 */
public class TurretLauncher : MonoBehaviour
{
    public ScriptableEnemyStats _EnemyStats;

    public int TurretID;

    private int TurretCash;

    public string TurretType;

    public GameObject BulletLauncherPrefab;

    public int MyDamage;

    public Player _CurrentPlayer;
    public float TurrentHealth = 100f;
    private Image _HealthBar;
    public GameObject CrumblePrefab;

    private SpawnManager _SpawnManager;
    private AudioManager _AudioManager;
    private GameManager _GameManager;
    private UIManager _UiManager;

    private float currentTurretFireRate;
    private const float TurretFireRate = 5.0f;

    private const float MinWaitTime = 1.0f;
    private const float MaxWaitTime = 5.0f;
    private float WaitTime;

    public bool IsDestroyed = false;
    private bool TurrentActivated = false;

    private void Awake()
    {
        _GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);
        _SpawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
        _UiManager = GameObject.FindGameObjectWithTag("UiSystem").GetComponent<UIManager>();
        _CurrentPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if (TurretType == "Turret")
            TurrentHealth = _EnemyStats._EnemyStats.TurretHealths[TurretID];
        else
            TurrentHealth = _EnemyStats._EnemyStats.TurretOnWallHealths[TurretID];
        _HealthBar = transform.GetChild(0).transform.GetChild(1).GetComponent<Image>();
        _HealthBar.fillAmount = TurrentHealth / 100f;

        BulletLauncherPrefab.transform.localScale = transform.localScale;

        TurretCash = UnityEngine.Random.Range(100, 200);
        WaitTime = UnityEngine.Random.Range(MinWaitTime, MaxWaitTime);
    }
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(WaitTime);
        TurrentActivated = true;
    }
    public void ReduceHealth(float Amount)
    {
        if (Amount <= TurrentHealth)
        {
            TurrentHealth -= Amount;
            _HealthBar.fillAmount = TurrentHealth / 100f;
        }
        else
            TurrentHealth = 0;
        if (TurretType == "Turret")
            _EnemyStats._EnemyStats.TurretHealths[TurretID] = TurrentHealth;
        else
            _EnemyStats._EnemyStats.TurretOnWallHealths[TurretID] = TurrentHealth;
        if (TurrentHealth < 1)
        {
            _CurrentPlayer.AddCash(TurretCash);
            _UiManager.SetCash(_CurrentPlayer._Stats._PlayerStats.MyCash);
            _AudioManager?.PlayGotCashSound();
            DieOnAttack();
        }
    }
    private void DieOnAttack()
    {
        IsDestroyed = true;
        GameObject TurrentDestoryEffect = Instantiate(CrumblePrefab, transform.position, Quaternion.identity);
        TurrentDestoryEffect.GetComponent<ExplosionEffect>().WhichSoundToPlay.AddListener(() => _AudioManager.PlayTurrentDestroyed(TurrentDestoryEffect.GetComponent<AudioSource>()));
        if (TurretType == "Turret")
            _SpawnManager._ScriptableSpawnObjects._SpawnObjects.TurrentLaunchersAlive[TurretID] = false;
        else if (TurretType == "TurretOnWall")
            _SpawnManager._ScriptableSpawnObjects._SpawnObjects.TurrentLaunchersWallAlive[TurretID] = false;
        _GameManager.SetTotalKills(this);
        _GameManager.EnemyKilled();
        Destroy(gameObject);
    }

    private void Update()
    {
        if (_GameManager.GameOver == false && IsDestroyed == false && TurrentActivated)
        {
            if (Time.time > currentTurretFireRate)
            {
                currentTurretFireRate = Time.time + TurretFireRate;
                if (TurretType == "Turret")
                {
                    _AudioManager?.PlayTurretLaunchSound(GetComponent<AudioSource>());
                    GameObject CannonLauncher = Instantiate(BulletLauncherPrefab, transform.position + new Vector3(1f, 2f, 0f), BulletLauncherPrefab.transform.rotation);
                    CannonLauncher.GetComponent<CannonLauncher>().BulletDamageFromThisEnemy[0] = MyDamage;
                    CannonLauncher.GetComponent<CannonLauncher>().WhichTurret = 0;
                    int TurretDir = Convert.ToInt32(Mathf.Sign(transform.localScale.x));
                    CannonLauncher.GetComponent<CannonLauncher>().FlipDirection(TurretDir);
                    BulletLauncherPrefab.transform.position = transform.position;
                }
                else //TurretWall
                {
                    _AudioManager?.PlayTurretLaunchSound(GetComponent<AudioSource>());
                    GameObject CannonLauncher = Instantiate(BulletLauncherPrefab, transform.position + new Vector3(2f, 0.7f, 0f), BulletLauncherPrefab.transform.rotation);
                    CannonLauncher.GetComponent<CannonLauncher>().BulletDamageFromThisEnemy[1] = MyDamage;
                    CannonLauncher.GetComponent<CannonLauncher>().WhichTurret = 1;
                    int TurretDir = Convert.ToInt32(Mathf.Sign(transform.localScale.x));
                    CannonLauncher.GetComponent<CannonLauncher>().FlipDirection(TurretDir);
                    BulletLauncherPrefab.transform.position = transform.position;
                }
            }
        }
    }
}