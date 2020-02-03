using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * Enemy In-Game Behaviour
 */
public class EnemyAutoGunWBunkerBehaviour : MonoBehaviour
{

    public ScriptableEnemyStats _EnemyStats;

    public int EnemyID;
    private Animator EnemyAnimator;

    public Collider2D CrouchDisableCollider;
    public GameObject BulletEnemyPrefab;
    public Transform ViewRange;

    public float EnemyHealth = 100f;

    public Image EnemyHealthUI;

    private EnemyBullet _EnemyBullet;
    public int MyDamage;

    private int EnemyCash;

    private SpawnManager _SpawnManagerObject;
    private UIManager _UIManager;
    private AudioManager _AudioManager;
    private GameManager _GameManager;
    public Player _Player;

    private int EnemyAmmo = 12;
    private bool CrouchReloading = false;

    private bool IsEnemyDead = false;
    private bool BeingHurt = false;

    private float canShoot = 0.0f;
    private readonly float EnemyFireRate = 0.2f;

    private float BunkerDirection;

    private void Awake()
    {
        int CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int StageNumber = (CurrentSceneIndex - ((CurrentSceneIndex + 1) % 3 + 1)) / 3 + 1;
        EnemyAnimator = GetComponent<Animator>();
        _EnemyBullet = BulletEnemyPrefab.GetComponent<EnemyBullet>();

        _SpawnManagerObject = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
        _UIManager = GameObject.FindGameObjectWithTag("UiSystem").GetComponent<UIManager>();
        GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);
        _GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (_Player == null)
            _Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        SetEnemyBunkerLook(StageNumber);
    }

    private void Start()
    {
        EnemyHealth = _EnemyStats._EnemyStats.EnemyAutoGunWBunkerHealths[EnemyID];
        EnemyHealthUI.fillAmount = EnemyHealth / 100f;
        BunkerDirection = transform.parent.gameObject.transform.localScale.x;
        _EnemyBullet.BulletDamageFromThisEnemy[2] = MyDamage;
        EnemyCash = Random.Range(300, 400);

        if (!GetComponent<Renderer>().isVisible)
            enabled = false;
    }
    private void SetEnemyBunkerLook(int StageNum)
    {
        for (int i = 0; i < transform.parent.childCount - 1; ++i)
        {
            if (StageNum != (i + 1))
            {
                transform.parent.GetChild(i + 1).gameObject.SetActive(false);
            }
        }
    }
    private void Update()
    {
        if (IsEnemyDead == false)
        {
            if (IsPlayerDetected())
            {
                if (CrouchReloading == false)
                {
                    StartShooting();
                }
            }
            else
            {
                EnemyAnimator.SetBool("ShootStanding", false);
            }
        }
    }
    private void StartShooting()
    {
        if (EnemyAmmo > 0)
        {
            if (BeingHurt == false)
            {
                if (Time.time > canShoot)
                {
                    canShoot = Time.time + EnemyFireRate;
                    _AudioManager?.PlayEnemyWithAutoGunBulletSound(GetComponent<AudioSource>());
                    _EnemyBullet.WhichEnemy = 2;
                    EnemyAnimator.SetBool("ShootStanding", true);
                    float Direction = Mathf.Sign(BunkerDirection);
                    GameObject MyBullet = Instantiate(BulletEnemyPrefab, transform.position + new Vector3(2.57f * Direction, 3.8f, 0f), Direction == 1f ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f));
                    MyBullet.GetComponent<EnemyBullet>().ShootingDirection = Direction;
                    EnemyAmmo--;
                }
            }
        }
        if (EnemyAmmo <= 0)
        {
            CrouchReloading = true;
            EnemyAnimator.SetBool("ShootStanding", false);
            StartCoroutine(ReloadingRoutine());
        }
    }
    private IEnumerator ReloadingRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        if (!IsEnemyDead)
        {
            _AudioManager?.PlayEnemyWithAutoGunReloadSound(GetComponent<AudioSource>());
            EnemyAnimator.SetBool("CrouchReloading", true);
            CrouchDisableCollider.enabled = false;
            yield return StartCoroutine(OffReloading());
        }
    }

    private IEnumerator OffReloading()
    {
        yield return new WaitForSeconds(3.0f);
        EnemyAmmo = 12;
        EnemyAnimator.SetBool("CrouchReloading", false);
        CrouchReloading = false;
        CrouchDisableCollider.enabled = true;
    }

    private bool IsPlayerDetected()
    {
        Collider2D PlayerSaw = Physics2D.OverlapBox(ViewRange.position, GetComponent<CapsuleCollider2D>().size + new Vector2(25f, 3f), 0, LayerMask.GetMask("Player"));
        if (PlayerSaw != null)
        {
            bool isPlayerDead = PlayerSaw.GetComponent<Player>().PlayerDead;
            return isPlayerDead == false;
        }
        return false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(ViewRange.position, GetComponent<CapsuleCollider2D>().size + new Vector2(25f, 3f));
    }
    public void ReduceHealth(float Takedamage)
    {
        if (IsEnemyDead == false)
        {
            BeingHurt = true;
            _AudioManager?.PlayEnemyHurtSound(GetComponent<AudioSource>());
            EnemyHealth -= Takedamage;
            EnemyHealthUI.fillAmount = EnemyHealth / 100f;
            EnemyAnimator.SetBool("HurtStanding", true);
            StartCoroutine(HurtStandingRoutine());
            _EnemyStats._EnemyStats.EnemyAutoGunWBunkerHealths[EnemyID] = EnemyHealth;
            if (EnemyHealth <= 0)//When Enemy Dies
            {
                _SpawnManagerObject._ScriptableSpawnObjects._SpawnObjects.EnemiesWithAutoGunWithBunkerAlive[EnemyID] = false;
                _Player.AddCash(EnemyCash);
                _UIManager.SetCash(_Player._Stats._PlayerStats.MyCash);
                _AudioManager?.PlayGotCashSound();
                DieOnAttack();
            }
        }
    }
    private void DieOnAttack()
    {
        _AudioManager?.PlayEnemyDeathSound(GetComponent<AudioSource>());
        IsEnemyDead = true;
        _GameManager.SetTotalKills(this);
        _GameManager.EnemyKilled();
        EnemyAnimator.SetBool("Dead", true);
        Destroy(gameObject, 5f);
    }
    private IEnumerator HurtStandingRoutine()
    {
        yield return new WaitForSeconds(0.25f);
        EnemyAnimator.SetBool("HurtStanding", false);
        BeingHurt = false;
    }
    private void OnBecameVisible()
    {
        enabled = true;
    }
}