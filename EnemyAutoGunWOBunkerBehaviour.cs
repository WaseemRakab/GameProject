using System.Collections;
using UnityEngine;
using UnityEngine.UI;
/**
 * Enemy In-Game Behaviour
 */
public class EnemyAutoGunWOBunkerBehaviour : MonoBehaviour
{
    public ScriptableEnemyStats _EnemyStats;
    public int EnemyID;

    private Animator EnemyAnimator;

    private Rigidbody2D MyBody;
    private Vector2 zeroVec;

    public Player _Player;
    public GameObject BulletEnemyPrefab;

    private readonly float WalkSpeed = 2f;
    public Transform ViewRange;

    private float FlipEnemyMovement = 1f;
    private float PlayerDetected = 1f;

    public float EnemyHealth = 100f;

    public Image EnemyHealthUI;

    private EnemyBullet _EnemyBullet;
    public int MyDamage;

    private int EnemyCash;

    private SpawnManager _SpawnManagerObject;
    private UIManager _UIManager;
    private AudioManager _AudioManager;
    private GameManager _GameManager;

    private int EnemyAmmo = 10;
    private bool Reloading = false;

    private bool IsEnemyDead = false;
    private bool BeingHurt = false;

    private float canShoot = 0.0f;
    private readonly float EnemyFireRate = 0.2f;

    private void Awake()
    {
        zeroVec = Vector2.zero;
        MyBody = GetComponent<Rigidbody2D>();
        EnemyAnimator = GetComponent<Animator>();
        _EnemyBullet = BulletEnemyPrefab.GetComponent<EnemyBullet>();
        _SpawnManagerObject = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
        _UIManager = GameObject.FindGameObjectWithTag("UiSystem").GetComponent<UIManager>();
        GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);
        _Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        EnemyHealth = _EnemyStats._EnemyStats.EnemyAutoGunWOBunkerHealths[EnemyID];
        EnemyHealthUI.fillAmount = EnemyHealth / 100f;
        _EnemyBullet.BulletDamageFromThisEnemy[1] = MyDamage;
        EnemyCash = Random.Range(200, 300);

        if (!GetComponent<Renderer>().isVisible)
            enabled = false;
    }

    private void Update()
    {
        if (IsEnemyDead == false)
        {
            if (Reloading == false)
            {
                if (IsPlayerDetected())
                {
                    EnemyReacting();
                    StartShooting();
                }
                else//Player Not Found, Or Dead
                {
                    EnemyAnimator.SetBool("ShootStanding", false);
                    PlayerDetected = 1f;
                }
            }
            else
            {
                PlayerDetected = 0f;
            }
        }
    }
    private void FixedUpdate()
    {
        if (!IsEnemyDead)
            MyBody.velocity = Vector2.SmoothDamp(MyBody.velocity, new Vector2(WalkSpeed * FlipEnemyMovement * PlayerDetected, MyBody.velocity.y), ref zeroVec, Time.fixedDeltaTime);
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
                    _EnemyBullet.WhichEnemy = 1;
                    float Direction = Mathf.Sign(transform.localScale.x);
                    GameObject MyBullet = Instantiate(BulletEnemyPrefab, transform.position + new Vector3(2.57f * Direction, 3.8f, 0f), Direction == 1f ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f));
                    MyBullet.GetComponent<EnemyBullet>().ShootingDirection = Direction;
                    EnemyAmmo--;
                    if (EnemyAmmo <= 0)
                    {
                        EnemyAnimator.SetBool("Reloading", true);
                        Reloading = true;
                        if (!IsEnemyDead)
                        {
                            StartCoroutine(ReloadingRoutine());
                        }
                    }
                }
            }
        }
    }
    private IEnumerator ReloadingRoutine()
    {
        _AudioManager?.PlayEnemyWithAutoGunReloadSound(GetComponent<AudioSource>());
        yield return new WaitForSeconds(3f);
        EnemyAmmo = 10;
        EnemyAnimator.SetBool("Reloading", false);
        Reloading = false;
    }
    private void EnemyReacting()
    {
        EnemyAnimator.SetBool("ShootStanding", true);
        if (transform.position.x > _Player.transform.position.x)//PlayerDetectedOnLeftSide
        {
            FlipEnemyMovement = -1;
            FlipPlayerLeft();
        }
        else if (transform.position.x < _Player.transform.position.x)//PlayerDetectedOnRightSide
        {
            FlipEnemyMovement = 1;
            FlipPlayerRight();
        }
        PlayerDetected = 0f;//To Prevent Enemy From Moving
    }
    private bool IsPlayerDetected()
    {
        Vector2 sizeOfMyBody = GetComponent<CapsuleCollider2D>().size;
        Collider2D PlayerSaw = Physics2D.OverlapBox(ViewRange.position, sizeOfMyBody + new Vector2(45f, 3f), 0, LayerMask.GetMask("Player"));
        Collider2D[] WallsSaw = Physics2D.OverlapBoxAll(ViewRange.position, sizeOfMyBody + new Vector2(45f, 3f), 0, LayerMask.GetMask("Ground"));
        if (PlayerSaw != null)
        {
            bool IsPlayerDead = PlayerSaw.GetComponent<Player>().PlayerDead;
            if (WallsSaw.Length == 0)
                return IsPlayerDead == false;
            if (WallsSaw.Length == 1)
            {
                return OnVisionBlocked(transform.position.x, PlayerSaw.transform.position.x, WallsSaw[0].transform.position.x)
                    && IsPlayerDead == false;
            }
            else//WallsSaw.Length>1
            {
                Collider2D MinWallDistance = GetClosestWallToMe(WallsSaw);
                if (MinWallDistance != null)
                    return OnVisionBlocked(transform.position.x, PlayerSaw.transform.position.x, MinWallDistance.transform.position.x)
                        && IsPlayerDead == false;
                return false;
            }
        }
        return false;
    }
    private Collider2D GetClosestWallToMe(Collider2D[] Walls)
    {
        Collider2D ClosestWall = null;
        float MinPositionX = float.MaxValue;
        for (int i = 0; i < Walls.Length; ++i)
        {
            float Distance = Mathf.Abs(transform.position.x - Walls[i].transform.position.x);
            if (Distance <= MinPositionX)
            {
                MinPositionX = Distance;
                ClosestWall = Walls[i];
            }
        }
        return ClosestWall;
    }
    private bool OnVisionBlocked(float PistolPositionX, float PlayerPositionX, float WallPositionX)
    {
        //Wall on Right Side of the Enemy
        if (PistolPositionX < WallPositionX)
        {
            if (PlayerPositionX < WallPositionX)
                return true;
        }
        //Wall on Left Side of the Enemy
        else
        {
            if (PlayerPositionX > WallPositionX)
                return true;
        }
        return false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawCube(ViewRange.position, GetComponent<CapsuleCollider2D>().size + new Vector2(45f, 3f));
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
            _EnemyStats._EnemyStats.EnemyAutoGunWOBunkerHealths[EnemyID] = EnemyHealth;
            if (EnemyHealth <= 0)//When Enemy Dies
            {
                _SpawnManagerObject._ScriptableSpawnObjects._SpawnObjects.EnemiesWithAutoGunWithoutBunkerAlive[EnemyID] = false;
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
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("PushableBox") || other.gameObject.name == "Wall")
        {
            FlipEnemyMovement *= -1;
            FlipPlayerScale();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall") || other.gameObject.CompareTag("PushableBox") || other.name == "Wall")
        {
            FlipEnemyMovement *= -1;
            FlipPlayerScale();
        }
    }
    private void FlipPlayerScale()
    {
        Vector3 enemyScale = transform.localScale;
        enemyScale.x = FlipEnemyMovement * 3;
        transform.localScale = enemyScale;
    }

    private void FlipPlayerLeft()
    {
        Vector3 enemyScale = transform.localScale;
        enemyScale.x = -3;
        transform.localScale = enemyScale;
    }

    private void FlipPlayerRight()
    {
        Vector3 enemyScale = transform.localScale;
        enemyScale.x = 3;
        transform.localScale = enemyScale;
    }

    private void OnBecameVisible()
    {
        enabled = true;
    }
}