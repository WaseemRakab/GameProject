using System.Collections;
using UnityEngine;
using UnityEngine.UI;
/**
 * Enemy In-Game Behaviour
 */
public class EnemyPistolBehaviour : MonoBehaviour
{
    public int EnemyID;

    public ScriptableEnemyStats _EnemyStats;

    public Player _Player;
    public GameObject BulletEnemyPrefab;

    private readonly float WalkSpeed = 2f;

    private Rigidbody2D MyBody;
    private Vector2 zeroVec;
    private Animator EnemyAnimator;
    public Transform ViewRange;
    private float FlipEnemyMovement = 1f;
    private float PlayerDetected = 1f;

    public float EnemyHealth = 100f;
    private bool IsEnemyCrouching = false;

    public Image EnemyHealthUI;

    private bool IsEnemyDead = false;
    private bool BeingHurt = false;


    private float canShoot = 0.0f;
    private readonly float EnemyFireRate = 0.8f;

    private EnemyBullet _EnemyBullet;
    public int MyDamage;

    private int EnemyCash;

    private SpawnManager _SpawnManagerObject;
    private UIManager _UIManager;
    private AudioManager _AudioManager;
    private GameManager _GameManager;

    private void Awake()
    {
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
        EnemyHealth = _EnemyStats._EnemyStats.EnemyPistolHealths[EnemyID];
        EnemyHealthUI.fillAmount = EnemyHealth / 100f;
        _EnemyBullet.BulletDamageFromThisEnemy[0] = MyDamage;
        EnemyCash = Random.Range(200, 300);

        if (!GetComponent<Renderer>().isVisible)
            enabled = false;
    }
    private void Update()
    {
        if (IsEnemyDead == false)
        {
            if (IsPlayerDetected())
            {
                EnemyReacting();
                StartShooting();
            }
            else//Player Not Found, Or Dead
            {
                EnemyAnimator.SetBool("ShootStanding", false);
                EnemyAnimator.SetBool("ShootCrouching", false);
                PlayerDetected = 1f;
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
        if (BeingHurt == false)
        {
            if (Time.time > canShoot)
            {
                canShoot = Time.time + EnemyFireRate;
                _AudioManager?.PlayEnemyWithPistolBulletSound(GetComponent<AudioSource>());
                _EnemyBullet.WhichEnemy = 0;
                float Direction = Mathf.Sign(transform.localScale.x);//1f Positive,-1f Negative
                if (!IsEnemyCrouching)
                {
                    GameObject MyBullet = Instantiate(BulletEnemyPrefab, transform.position + new Vector3(2.57f * Direction, 4.05f, 0f), Direction == 1f ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f));
                    MyBullet.GetComponent<EnemyBullet>().ShootingDirection = Direction;
                }
                else
                {
                    GameObject MyBullet = Instantiate(BulletEnemyPrefab, transform.position + new Vector3(2.57f * Direction, 2.9f, 0f), Direction == 1f ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f));
                    MyBullet.GetComponent<EnemyBullet>().ShootingDirection = Direction;
                }
            }
        }
    }
    private void EnemyReacting()
    {
        if (_Player.isCrouching)
        {
            EnemyAnimator.SetBool("ShootCrouching", true);
            EnemyAnimator.SetBool("ShootStanding", false);
            IsEnemyCrouching = true;
        }
        else
        {
            EnemyAnimator.SetBool("ShootStanding", true);
            EnemyAnimator.SetBool("ShootCrouching", false);
            IsEnemyCrouching = false;
        }
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
            return false;
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
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(ViewRange.position, GetComponent<CapsuleCollider2D>().size + new Vector2(45f, 3f));
    }
    public void ReduceHealth(float Takedamage)
    {
        if (IsEnemyDead == false)
        {
            BeingHurt = true;
            _AudioManager?.PlayEnemyHurtSound(GetComponent<AudioSource>());
            EnemyHealth -= Takedamage;
            _EnemyStats._EnemyStats.EnemyPistolHealths[EnemyID] = EnemyHealth;
            EnemyHealthUI.fillAmount = EnemyHealth / 100f;
            if (!IsEnemyCrouching)
            {
                EnemyAnimator.SetBool("HurtStanding", true);
                StartCoroutine(HurtStandingRoutine());
            }
            else
            {
                EnemyAnimator.SetBool("HurtCrouching", true);
                StartCoroutine(HurtCrouchingRoutine());
            }
            StartCoroutine(ShootingRoutine());
            if (EnemyHealth <= 0)
            {
                _SpawnManagerObject._ScriptableSpawnObjects._SpawnObjects.EnemiesWithPistolAlive[EnemyID] = false;
                _Player.AddCash(EnemyCash);
                _UIManager.SetCash(_Player._Stats._PlayerStats.MyCash);
                _AudioManager?.PlayGotCashSound();
                DieOnAttack();
            }

        }
    }
    private IEnumerator ShootingRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        BeingHurt = false;
    }
    public void DieOnAttack()
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
    }
    private IEnumerator HurtCrouchingRoutine()
    {
        yield return new WaitForSeconds(0.25f);
        EnemyAnimator.SetBool("HurtCrouching", false);
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
        if (other.CompareTag("Wall") || other.CompareTag("PushableBox") || other.name == "Wall")
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