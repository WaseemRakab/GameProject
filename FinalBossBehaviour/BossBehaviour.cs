using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/**
 * The Main Behaviour Of The Final Boss in-Game
 */
public class BossBehaviour : MonoBehaviour
{
    public GameObject JumpEffectPrefab;

    public CharacterController2D controller;
    private Animator MyAnimator;
    private GameManager _GameManager;
    private Player _CurrentPlayer;

    public GameObject BulletPrefab;
    private EnemyBullet BossBullet;
    private float currentBulletFireRate = 0.0f;
    private readonly float BulletFireRate = 1.2f;

    private AudioManager _AudioManager;

    public Transform MyJumpView;

    public Transform MeleeAttackView;
    public Transform MeleeAttackDamage;

    public Transform LeftView;
    public Transform RightView;

    private float Direction = -1f;

    private bool isJumping = false;
    private bool Jumped = false;
    public bool isRunning = false;

    private const float FullHealthAmount = 800f;
    private float Health = 800f;
    private const int AttackDamage = 28;
    public Image LiveHealth;


    public float RunningSpeed = 0.0f;
    private float SpeedBoost = 0.0f;

    private bool IsBossDead = false;

    public bool BossActivated = false;

    private int Ammo = 5;

    private const float attackWaitingPeriod = 1f;
    private float currentAttackPeriod = 0.0f;

    private bool WasPlayerCrouching = false;

    private const int MyBulletDamage = 22;

    public UnityEvent OnFinalBossKilled;
    public UnityEvent OnSkippingCutscene;

    private bool BlockShootingWhenBeingFlipped = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController2D>();
        GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);
        MyAnimator = GetComponent<Animator>();
        _GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _CurrentPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        BossBullet = BulletPrefab.GetComponent<EnemyBullet>();
        BossBullet.BulletDamageFromThisEnemy[3] = MyBulletDamage;
    }

    private void Update()
    {
        if (BossActivated && !IsBossDead)
        {
            bool IsPlayerDead = false;
            if (!PlayerDetected(ref IsPlayerDead))
            {
                isRunning = true;//Searching for Player
                WasPlayerCrouching = false;
                if (IsPlayerDead)
                {
                    isRunning = false;
                    RunningSpeed = 0.0f;
                    BossActivated = false;
                }
            }
            else
            {
                float FlipDirection = CheckPlayerDirectionWhenDetected();
                if (FlipDirection != Direction)
                {
                    BlockShootingWhenBeingFlipped = true;
                    Direction = FlipDirection;
                    RunningSpeed = 0.1f;
                    StartCoroutine(FlipRoutine());
                }
                else
                {
                    if (!isJumping)//Found Player , Stops Running
                        RunningSpeed = 0.0f;
                }
                if (_CurrentPlayer.isCrouching)//Checking if Player is Crouching
                {
                    if (!WasPlayerCrouching)//if Crouched once
                        WasPlayerCrouching = true;
                }
                if (WasPlayerCrouching && !_CurrentPlayer.isCrouching)//Pressed Crouch Key Once(Not Hold)
                {
                    MeleeAttack();
                }
                else if (Ammo == 0 || _CurrentPlayer.isCrouching)//Ammo Empty or player is crouching (Holding Crouch)
                {
                    MeleeAttack();
                }
                else //Has Ammo And shooting Player
                {
                    isRunning = false;
                    if (!WasPlayerCrouching)
                        StartCoroutine(GroundedShooting());
                    WasPlayerCrouching = false;
                }
            }
            MyAnimator.SetBool("Running", isRunning);
            if (isRunning)
            {
                RunningSpeed = 45.0f + SpeedBoost;
            }
            JumpOnSawWall();
            BlockDetectingPlayerWhenJumping();

        }
        else
            MyAnimator.SetBool("Running", false);
    }

    private void LateUpdate()
    {
        if (BossActivated && !IsBossDead)
            controller.Move(Direction * RunningSpeed * Time.fixedDeltaTime, false, isJumping);
    }
    private IEnumerator GroundedShooting()
    {
        yield return new WaitForSeconds(0.05f);
        if (controller.m_Grounded)
            Shoot();
    }
    private IEnumerator FlipRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        RunningSpeed = 0.0f;
    }
    private void BlockDetectingPlayerWhenJumping()
    {
        if (!controller.m_Grounded)
        {
            LeftView.gameObject.SetActive(false);
            RightView.gameObject.SetActive(false);
        }
        else
        {
            if (isJumping)
                Jumped = true;
            LeftView.gameObject.SetActive(true);
            RightView.gameObject.SetActive(true);
        }
    }

    private void JumpOnSawWall()
    {
        Collider2D jumpColliders = Physics2D.OverlapCircle(MyJumpView.position, 0.6f);
        if (jumpColliders != null)
        {
            if (jumpColliders.CompareTag("Ground"))
            {
                if (!Jumped)
                {
                    isJumping = true;
                }
            }
            else if (jumpColliders.CompareTag("Wall"))
            {
                Direction *= -1;
            }
        }
        else
            Jumped = false;
    }

    private void Shoot()
    {
        if (Time.time > currentBulletFireRate)
        {
            if (!BlockShootingWhenBeingFlipped)
            {
                currentBulletFireRate = Time.time + BulletFireRate;
                BossBullet.WhichEnemy = 3;
                float Direction = Mathf.Sign(transform.localScale.x);
                GameObject MyBullet = Instantiate(BulletPrefab, transform.position + new Vector3(3f * Direction, 3.45f, 0f), Direction == 1f ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f));
                MyBullet.GetComponent<EnemyBullet>().ShootingDirection = Direction;
                Ammo--;
            }
            BlockShootingWhenBeingFlipped = false;
        }
    }

    private void MeleeAttack()
    {
        int PlayerMask = LayerMask.GetMask("Player");
        Collider2D InMeleeRange = Physics2D.OverlapBox(MeleeAttackView.position, new Vector2(5f, 1.5f), 0f, PlayerMask);
        if (InMeleeRange != null)
        {
            if (Time.time > currentAttackPeriod)
            {
                MyAnimator.SetBool("Attack", true);
                Collider2D InMeleeDamageRange = Physics2D.OverlapBox(MeleeAttackDamage.position, new Vector2(3.8f, 1.5f), 0f, PlayerMask);
                if (InMeleeDamageRange != null)
                {
                    currentAttackPeriod = Time.time + attackWaitingPeriod;
                    isRunning = false;
                    SpeedBoost = 0.0f;
                    InMeleeDamageRange.GetComponent<Player>().DecreaseShield(AttackDamage);
                }
                StartCoroutine(AttackDelay());
            }
        }
        else
        {
            isRunning = true;
            SpeedBoost = 20.0f;
        }
    }
    private IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(0.3f);
        MyAnimator.SetBool("Attack", false);
        if (Ammo == 0)
        {
            yield return new WaitForSeconds(1f);
            Ammo = 5;
        }
    }

    private bool PlayerDetected(ref bool IsPlayerDead)
    {
        if (LeftView.gameObject.activeSelf && RightView.gameObject.activeSelf)
        {
            int PlayerMask = LayerMask.GetMask("Player");
            Collider2D SawPlayer =
                Physics2D.OverlapBox(LeftView.position, new Vector2(40.5f, 6f), 0f, PlayerMask)
                                                        ??
                Physics2D.OverlapBox(RightView.position, new Vector2(22.5f, 1.5f), 0f, PlayerMask) ?? null;
            if (SawPlayer != null)
            {
                bool PlayerNotDead = !SawPlayer.GetComponent<Player>().PlayerDead;
                IsPlayerDead = !PlayerNotDead;
                return PlayerNotDead;
            }
            return false;
        }
        return false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(MeleeAttackView.position, new Vector3(5f, 1.5f, 0f));

        Gizmos.color = Color.red;
        Gizmos.DrawCube(MeleeAttackDamage.position, new Vector3(3.8f, 1.5f, 0f));

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(MyJumpView.position, 0.6f);

        Gizmos.color = Color.blue;
        Gizmos.DrawCube(LeftView.position, new Vector3(22.5f, 6f, 1f));

        Gizmos.color = Color.red;
        Gizmos.DrawCube(RightView.position, new Vector3(22.5f, 1.5f, 1f));
    }
    private float CheckPlayerDirectionWhenDetected()
    {
        if (LeftView.gameObject.activeSelf && RightView.gameObject.activeSelf)
        {
            float MyScaleSign = Mathf.Sign(transform.localScale.x);
            if (Physics2D.OverlapBox(LeftView.position, new Vector2(22.5f, 6f), 0f, LayerMask.GetMask("Player")))
            {
                return -1f * MyScaleSign;
            }
            else if (Physics2D.OverlapBox(RightView.position, new Vector2(22.5f, 1.5f), 0f, LayerMask.GetMask("Player")))
            {
                return -1f * -MyScaleSign;
            }
        }
        return Direction;
    }

    public void OnLanding()
    {
        isJumping = false;
        _AudioManager.PlayOnLandingSound();
        Instantiate(JumpEffectPrefab, transform.position - new Vector3(0f, 1f, 0f), Quaternion.identity);
    }

    public void ReduceHealth(float Amount)
    {
        if (IsBossDead == false)
        {
            MyAnimator.SetBool("Hurt", true);
            _AudioManager.PlayFinalBossHurtSound();
            Health -= Amount;
            LiveHealth.fillAmount = Health / FullHealthAmount;
            StartCoroutine(HurtRoutine());
            if (Health <= 0)
            {
                _AudioManager.PlayFinalBossDeathSound();
                DieOnAttack();
            }
        }
    }

    private void DieOnAttack()
    {
        IsBossDead = true;
        _GameManager.SetTotalKills(this);
        MyAnimator.SetBool("Dead", true);
        _GameManager.InCutscene = true;
        /**
        Methods Being Invoked (OnEvent)
        * GameQuests.OnFinalBossKilled()
        * FinalBossGate.OnGateOpeningCutscene()
        * AudioManager.OnFinalBossKilled()
        * Player.OnCutsceneTurnOffAnimation()
        */
        OnFinalBossKilled.Invoke();

        Destroy(gameObject, 10.0f);
    }

    public void ActiveBossBehaviour()
    {
        BossActivated = true;
    }
    private IEnumerator HurtRoutine()
    {
        yield return new WaitForSeconds(0.15f);
        MyAnimator.SetBool("Hurt", false);
    }
}