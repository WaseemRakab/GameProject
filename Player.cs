using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/**
 * The Main Player Behaviour InGame
 * Controling Everything that the player can do In-Game
 */
public class Player : MonoBehaviour
{
    private SmoothCamera2D CurrentCamera;

    /*Scriptable Objects*/
    public Controls _Controls;
    public ScriptableWeapon _MyWeaponData;
    public ScriptablePlayerStats _Stats;

    public AudioManager _AudioManager;

    public CharacterController2D controller;

    private EventTriggers _EventTriggers;
    private DarkAreaTrigger _DarkAreaTrigger;

    private GameManager _GameManager;
    public UIManager _UiManager;

    public Animator MyAnimator;
    private Collider2D[] _PlayerCollider;

    public GameObject Bulletprefab;

    //melee attack vars
    public Transform attackpos;
    public float attackRange;
    public LayerMask WhatisEnemies;

    public RuntimeAnimatorController[] Controllers;

    public ParticleSystem MyShieldEffect;
    public GameObject JumpEffectPrefab;
    public GameObject BulletEffect;

    private float _Speed = 35.0f;
    private float canShoot = 0.0f;

    //For Sprint Speed
    private readonly float MultiplierSpeed = 60.0f / 35.0f;
    public float HorizontalInput { get; private set; }

    private int ReservedHealthAmount;

    //States For Animations
    public bool isJumping = false;
    public bool isCrouching = false;
    public bool isRunning = false;
    public bool isAttacking = false;
    //States for Push
    public bool CanGoRight = true;
    public bool CanGoLeft = true;
    public bool CanJump = true;
    public bool CanSprint = true;

    private bool HitDeadZone;
    private bool HitSpikes;

    public float timetoland = 0.8f;
    public Transform EventCheckPosition;

    //On Event Triggers
    public bool HasBeenInSecretChest;
    public bool[] HasBeenInCoinChests;
    public bool HasBeenInDarkAreaTrigger;

    public bool PlayerDead;

    private readonly float MinimumFallDistance = -25.0f;

    private float footStepsWaitingPeriod = 0.3f;
    private float currentFootStepsPeriod = 0.0f;

    private readonly float attackWaitingPeriod = 0.4f;
    private float currentAttackPeriod = 0.0f;

    private bool EventListenerDone = false;

    private void Awake()
    {
        CurrentCamera = Camera.main.GetComponent<SmoothCamera2D>();
        _PlayerCollider = GetComponents<Collider2D>();
        _UiManager = GameObject.FindGameObjectWithTag("UiSystem").GetComponent<UIManager>();
        _GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        MyAnimator = GetComponent<Animator>();

        GameObject.FindGameObjectWithTag("SecretChest")?.TryGetComponent(out _EventTriggers);
        GameObject.FindGameObjectWithTag("DarkAreaTrigger")?.TryGetComponent(out _DarkAreaTrigger);
    }
    private void Start()
    {
        SetPlayerPosition();
        if (_Stats._PlayerStats.HasWeapon == false)
            MyAnimator.runtimeAnimatorController = Controllers[0];//WithOutGun Controller
        else
        {
            MyAnimator.runtimeAnimatorController = Controllers[1];//WithGun Controller
            _UiManager.ShowAmmoContentInUI();
        }
        _UiManager.RefreshPlayerStatsInUI();
    }
    private void SetPlayerPosition()
    {
        Vector PlayerPos = _Stats._PlayerStats.PlayerPosition;
        transform.position = new Vector3(PlayerPos.X, PlayerPos.Y, PlayerPos.Z);
    }
    private void Update()
    {
        if (!_GameManager.GameOver) // if game is not over
        {
            if (!_GameManager.GamePaused && !_GameManager.InCutscene)
            {
                MovementInput();
                MyAnimator.SetFloat("Speed", Mathf.Abs(HorizontalInput));
                if (_Stats._PlayerStats.HasWeapon)
                {
                    Shoot();
                }
                Attack();
                Jump();
                Crouch();
                Sprint();
                Borders();
                DieOnOutOfBorders();
                EventActionListener();
            }
        }
    }

    private void LateUpdate()
    {
        Move();
        HorizontalInput = 0f;
    }
    private void GameOver()
    {
        if (_GameManager._FinishLevel != null)
        {
            if (_GameManager._FinishLevel.LevelFinished == false)
            {
                _UiManager.SetGameOverPortrait();
                IsDead();
                StartCoroutine(_GameManager.ShowAnimation());
            }
        }
        else
        {
            _UiManager.SetGameOverPortrait();
            IsDead();
            StartCoroutine(_GameManager.ShowAnimation());
        }
    }
    private void MovementInput()
    {
        if (isRunning)
            footStepsWaitingPeriod = 0.24f;
        else
            footStepsWaitingPeriod = 0.3f;
        if (Input.GetKey(_Controls.Forward) && CanGoRight == true)
        {
            if (!MyAnimator.GetBool("Jumping") && Time.time > currentFootStepsPeriod)
            {
                currentFootStepsPeriod = Time.time + footStepsWaitingPeriod;
                _AudioManager.PlayMovingSound();
            }
            HorizontalInput = 1f * _Speed;
        }
        if (Input.GetKey(_Controls.Backward) && CanGoLeft == true)
        {
            if (!MyAnimator.GetBool("Jumping") && Time.time > currentFootStepsPeriod)
            {
                currentFootStepsPeriod = Time.time + footStepsWaitingPeriod;
                _AudioManager.PlayMovingSound();
            }
            HorizontalInput = -1f * _Speed;
        }
    }
    private void DieOnOutOfBorders()
    {
        if (transform.position.y < -200f)
        {
            DecreaseLive(true);
        }
    }
    private void Attack()
    {
        if (Input.GetKeyDown(_Controls.Attack) && !MyAnimator.GetBool("Jumping"))
        {
            if (Time.time > currentAttackPeriod)
            {
                currentAttackPeriod = Time.time + attackWaitingPeriod;
                _AudioManager.PlayAttackSound();
                Collider2D EnemiesToDamage = Physics2D.OverlapCircle(attackpos.position, attackRange, WhatisEnemies);
                if (EnemiesToDamage != null)
                {
                    if (!EnemiesToDamage.CompareTag("EnemyAutoGunWithBunker"))
                    {
                        UnityAction<GameObject> WhichOneAttack = Delegate.CreateDelegate(typeof(UnityAction<GameObject>), this, EnemiesToDamage.tag + "Attacked") as UnityAction<GameObject>;
                        WhichOneAttack.Invoke(EnemiesToDamage.gameObject);
                    }
                }
                MyAnimator.SetBool("Attack", true);
                isAttacking = true;
                StartCoroutine(AttackDelay());
            }
        }
    }
    public void TurretLauncherAttacked(GameObject TurretLauncher)
    {
        TurretLauncher.GetComponent<TurretLauncher>().ReduceHealth(25f);
    }
    public void TurretWallAttacked(GameObject TurrentWall)
    {
        TurrentWall.GetComponent<TurretLauncher>().ReduceHealth(25f);
    }
    public void EnemyWithPistolAttacked(GameObject EnemyWithPistol)
    {
        EnemyWithPistol.GetComponent<EnemyPistolBehaviour>().ReduceHealth(25f);
    }
    public void EnemyAutoGunWithoutBunkerAttacked(GameObject EnemyAutoGunWithoutBunker)
    {
        EnemyAutoGunWithoutBunker.GetComponent<EnemyAutoGunWOBunkerBehaviour>().ReduceHealth(25f);
    }
    public void HelicopterAttacked(GameObject Helicopter)
    {
        Helicopter.GetComponent<HelicopterBehaviour>().ReduceHealth(25f);
    }
    public void FinalBossAttacked(GameObject FinalBoss)
    {
        FinalBoss.GetComponent<BossBehaviour>().ReduceHealth(25f);
    }
    private IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(0.25f);
        MyAnimator.SetBool("Attack", false);
        isAttacking = false;
    }
    private void Move()
    {
        if (isRunning)
            controller.Move(HorizontalInput * Time.fixedDeltaTime * MultiplierSpeed, isCrouching && !isAttacking, isJumping);
        else
            controller.Move(HorizontalInput * Time.fixedDeltaTime, isCrouching && !isAttacking, isJumping);
    }
    private void OnTriggerEnter2D(Collider2D Other)
    {
        if (Other.gameObject.CompareTag("DeadZone"))
        {
            if (HitDeadZone == false)
            {
                HitDeadZone = true;
                DecreaseLive(true);
                GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
                GetComponent<SpriteRenderer>().sortingOrder = -1;
            }
        }
        if (Other.gameObject.CompareTag("Spikes"))
        {
            if (HitSpikes == false)
            {
                HitSpikes = true;
                DecreaseLive(true);
            }
        }
    }
    private void DecreaseHealth(int Amount)
    {
        if (_Stats._PlayerStats.Lives > 0)
        {
            _AudioManager.PlayHurtSound();
            MyAnimator.SetBool("Hurt", true);
            StartCoroutine(HurtRoutine());
            if (Amount <= _Stats._PlayerStats.Health)
            {
                _Stats._PlayerStats.Health -= Amount;
                _UiManager.DecreaseHealth(_Stats._PlayerStats.Health);
            }
            else
            {
                ReservedHealthAmount = Amount - _Stats._PlayerStats.Health;
                _Stats._PlayerStats.Health = 0;
                _UiManager.DecreaseHealth(_Stats._PlayerStats.Health);
            }
            if (_Stats._PlayerStats.Health < 1)
            {
                DecreaseLive();
                DecreaseHealth(ReservedHealthAmount);
            }
        }
    }
    public void DecreaseShield(int Amount)
    {
        if (_Stats._PlayerStats.Shield > 0)
        {
            if (Amount <= _Stats._PlayerStats.Shield)
            {
                _Stats._PlayerStats.Shield -= Amount - Mathf.RoundToInt(Amount * 0.3f);
                _UiManager.DecreaseShield(_Stats._PlayerStats.Shield);
            }
            else
            {
                int ToReduceHealthAmount = Amount - _Stats._PlayerStats.Shield;
                _Stats._PlayerStats.Shield = 0;
                _UiManager.DecreaseShield(_Stats._PlayerStats.Shield);
                StopMyShieldEffect();
                DecreaseHealth(ToReduceHealthAmount);
            }
        }
        else
        {
            StopMyShieldEffect();
            DecreaseHealth(Amount);
        }
    }
    private IEnumerator HurtRoutine()
    {
        yield return new WaitForSeconds(0.25f);
        MyAnimator.SetBool("Hurt", false);
    }
    private void OnCollisionEnter2D(Collision2D Other)
    {
        if (Other.gameObject.CompareTag("Ground") || (Other.gameObject.CompareTag("Wall") && !isJumping))
        {
            MyAnimator.SetBool("Jumping", false);
        }
    }
    public void DecreaseLive(bool IsInstantKill = false)
    {
        if (IsInstantKill == true)
        {
            _AudioManager.PlayDeathSound();
            _UiManager.LoseAllLives();
            _Stats._PlayerStats.Lives = 0;
            _UiManager.DecreaseHealth(0f);
            _UiManager.DecreaseShield(0f);
            _GameManager.GameOver = true;
            GameOver();
        }
        else
        {
            if (_Stats._PlayerStats.Lives > 0)
            {
                _Stats._PlayerStats.Lives--;
                _UiManager.LoseLive(_Stats._PlayerStats.Lives);
                if (_Stats._PlayerStats.Lives == 0)
                {
                    _AudioManager.PlayDeathSound();
                    _UiManager.DecreaseHealth(0f);
                    _GameManager.GameOver = true;
                    GameOver();
                    return;
                }
                else
                {
                    _UiManager.ResetHealth();
                    _Stats._PlayerStats.Health = 100;
                }
            }
        }
    }
    private void Borders()
    {
        if (transform.position.x < -100f)
        {
            transform.position = new Vector3(-100f, transform.position.y, 0f);
        }
        if (transform.position.x > 350f)
        {
            transform.position = new Vector3(350f, transform.position.y, 0f);
        }
    }
    private void Sprint()
    {
        if (Input.GetKey(_Controls.Sprint) && CanSprint == true && HorizontalInput != 0)
        {
            isRunning = true;
            MyAnimator.SetBool("Running", isRunning);
        }
        if (Input.GetKeyUp(_Controls.Sprint))
        {
            isRunning = false;
            MyAnimator.SetBool("Running", isRunning);
        }
    }
    private void Crouch()
    {
        if (Input.GetKeyDown(_Controls.Crouch))
        {
            controller.m_Rigidbody2D.gravityScale = 12.0f;
            isCrouching = true;
            _Speed = 0f;
            MyAnimator.SetBool("Crouching", isCrouching);
        }

        else if (Input.GetKeyUp(_Controls.Crouch))
        {
            controller.m_Rigidbody2D.gravityScale = 8.0f;
            isCrouching = false;
            _Speed = 35.0f;
            MyAnimator.SetBool("Crouching", isCrouching);
        }
    }
    private void Jump()
    {
        if (Input.GetKeyDown(_Controls.Jump) && !isCrouching)
        {
            if (!MyAnimator.GetBool("Jumping") && !isJumping && CanJump == true)
            {
                _AudioManager.PlayJumpSound();
                MyAnimator.SetBool("Jumping", true);
                isJumping = true;
                StartCoroutine(JumpToLand());
            }
        }
    }
    private void Shoot()
    {
        //shoot button was inputed
        if (Input.GetKey(_Controls.Shoot) && Time.time > canShoot)
        {
            if (_MyWeaponData._Weapon.Ammo > 0)
            {
                _AudioManager.PlayShootingSound();
                canShoot = Time.time + _MyWeaponData._Weapon.FireRate;
                float Direction = Mathf.Sign(transform.localScale.x);
                if (!isCrouching)
                {
                    Instantiate(BulletEffect, transform.position + new Vector3(2.7f * Direction, 3.05f, 0f), Direction == 1f ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f));
                    GameObject bullet = Instantiate(Bulletprefab, transform.position + new Vector3(2.2f * Direction, 3.05f, 0f), Direction == 1f ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f));
                    bullet.GetComponent<Shooting>().ShootingDirection = Direction;
                }
                else
                {
                    Instantiate(BulletEffect, transform.position + new Vector3(2.7f * Direction, 1.6f, 0f), Direction == 1f ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f));
                    GameObject bullet = Instantiate(Bulletprefab, transform.position + new Vector3(2.2f * Direction, 1.5f, 0f), Direction == 1f ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f));
                    bullet.GetComponent<Shooting>().ShootingDirection = Direction;
                }
                _MyWeaponData._Weapon.Ammo--;
                _UiManager.SetAmmoBulletsInUI(_MyWeaponData._Weapon.Ammo);
                _MyWeaponData._Weapon.TotalWeaponFired++;
            }
        }
    }
    public void OnLanding(float FallingPosition)
    {
        isJumping = false;
        _AudioManager.PlayOnLandingSound();
        Instantiate(JumpEffectPrefab, transform.position - new Vector3(0f, 1f, 0f), Quaternion.identity);
        if (float.IsNaN(FallingPosition) == false) //Player Fall On Ground
        {
            FallDamage(FallingPosition);
        }
    }
    private void FallDamage(float StartYDistance)
    {
        float FallDistance = transform.position.y - StartYDistance;
        if (FallDistance <= MinimumFallDistance)
        {
            DecreaseShield(-Mathf.RoundToInt(FallDistance + 10f));
        }
    }
    private void IsDead()
    {
        PlayerDead = true;
        MyAnimator.SetBool("Dead", true);
        for (int i = 0; i < _PlayerCollider.Length; ++i)
        {
            _PlayerCollider[i].sharedMaterial = null;
        }
    }
    private IEnumerator JumpToLand()
    {
        yield return new WaitForSeconds(timetoland);
        MyAnimator.SetBool("Jumping", false);
    }
    public void LoseCash(int Amount)
    {
        if (_Stats._PlayerStats.MyCash - Amount >= 0)
        {
            _Stats._PlayerStats.MyCash -= Amount;
        }
    }
    public void AddCash(int Amount)
    {
        _Stats._PlayerStats.MyCash += Amount;
    }
    public void AttachWeapon()
    {
        _Stats._PlayerStats.HasWeapon = true;
        MyAnimator.runtimeAnimatorController = Controllers[1];
    }
    public void DetachWeapon()
    {
        _Stats._PlayerStats.HasWeapon = false;
        MyAnimator.runtimeAnimatorController = Controllers[0];
    }
    public void AddMyShieldEffect()
    {
        MyShieldEffect.Play();
    }
    private void StopMyShieldEffect()
    {
        MyShieldEffect.Stop();
    }
    public void GetInteractingMessage(GameObject WhichMessage, bool IsPushMessage = false)
    {
        GameObject Interacting = WhichMessage;
        Vector3 tempScale = Interacting.transform.GetChild(0).transform.localScale;
        tempScale.x = Mathf.Sign(transform.localScale.x) * 0.1f;
        Interacting.transform.GetChild(0).transform.localScale = tempScale;
        WhichMessage.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Text>().text = (IsPushMessage ? "Hold " : "Press ") + _Controls.Interact.ToString();
    }
    private void EventActionListener()
    {
        Collider2D collider = Physics2D.OverlapCircle(EventCheckPosition.position, 0.8f, LayerMask.GetMask("Event", "EventOnTrigger"));
        if (collider != null)
        {
            GetInteractingMessage(transform.GetChild(3).gameObject);
            UnityAction<GameObject> WhichEvent = Delegate.CreateDelegate(typeof(UnityAction<GameObject>), this, collider.tag + "Entry") as UnityAction<GameObject>;
            WhichEvent.Invoke(collider.gameObject);
            EventListenerDone = false;
        }
        else
        {
            if (EventListenerDone == false)
            {
                GetInteractingMessage(transform.GetChild(3).gameObject);
                _UiManager.HideInteractingEventMessage();
                _EventTriggers?.HideLockedDoorMessage();
                _DarkAreaTrigger?.HideLockedTriggerMessage();
                EventListenerDone = true;
            }
        }
    }
    public void SecretChestEntry(GameObject secretChest)
    {
        if (HasBeenInSecretChest == false)
        {
            _UiManager.ShowInteractingEventMessage();
            if (Input.GetKeyDown(_Controls.Interact))
            {
                EventTriggers Trigger = secretChest.GetComponent<EventTriggers>();
                _UiManager.HideInteractingEventMessage();
                Trigger.ShowKeyUI();
                _AudioManager.PlayGotKeySound();
                _GameManager.InCutscene = true;
                OnSkippingCutscenes.WhichCutsceneCanBeSkipped = new UnityAction(() => Trigger.SkipSecretChestRoutineCutscene());
                Trigger.OnSecretChestCutscene();
                _UiManager.ShowSkipCutscene();
                HasBeenInSecretChest = true;
                OnCutsceneTurnOffAnimation();
            }
        }
    }
    public void ItemShopEntry(GameObject itemShop)
    {
        if (itemShop.activeSelf)
        {
            _UiManager.ShowInteractingEventMessage();
            if (Input.GetKeyDown(_Controls.Interact))
            {
                _UiManager.ShowWeaponStore();
                _GameManager.PauseGame();
            }
        }
    }
    public void FinishDoorEntry(GameObject finishDoor)
    {
        FinishLevel finishLevel = finishDoor.GetComponent<FinishLevel>();
        if (finishLevel.HasEventTriggers)
        {
            if (_EventTriggers.IsDoorUnlocked == false)
                _EventTriggers.ShowLockedDoorMessage();
            else
            {
                if (!finishLevel.LevelFinished)
                {
                    _UiManager.ShowInteractingEventMessage();
                    if (Input.GetKeyDown(_Controls.Interact))
                    {
                        finishLevel.FinishingLevel(0.5f);
                        finishLevel.LevelFinished = true;
                        _UiManager.HideInteractingEventMessage();
                    }
                }
            }
        }
    }
    public void CoinChestEntry(GameObject coinChest)
    {
        CoinChest chest = coinChest.GetComponent<CoinChest>();
        if (HasBeenInCoinChests[chest.CoinChestID] == false)
        {
            _UiManager.ShowInteractingEventMessage();
            if (Input.GetKeyDown(_Controls.Interact))
            {
                chest.OpenChest();
                _UiManager.HideInteractingEventMessage();
                HasBeenInCoinChests[chest.CoinChestID] = true;
            }
        }
    }
    public void AmmoKitEntry(GameObject ammoCrate)
    {
        _UiManager.ShowInteractingEventMessage();
        if (Input.GetKeyDown(_Controls.Interact))
        {
            ammoCrate.GetComponent<AmmoCrate>().GetAmmoKit();
            _UiManager.HideInteractingEventMessage();
        }
    }
    public void DarkAreaTriggerEntry(GameObject darkAreaTrigger)
    {
        if (HasBeenInDarkAreaTrigger == false)
        {
            DarkAreaTrigger Trigger = darkAreaTrigger.GetComponent<DarkAreaTrigger>();
            if (HasBeenInSecretChest == false)
                Trigger.ShowLockedTriggerMessage();
            else // Player have a key
            {
                _UiManager.ShowInteractingEventMessage();
                if (Input.GetKeyDown(_Controls.Interact))//Cutscene
                {
                    _UiManager.HideInteractingEventMessage();
                    HasBeenInDarkAreaTrigger = true;
                    _GameManager.InCutscene = true;
                    _UiManager.ShowSkipCutscene();
                    OnCutsceneTurnOffAnimation();
                    OnSkippingCutscenes.WhichCutsceneCanBeSkipped = new UnityAction(() => Trigger.SkipTriggerCutsceneRoutine());
                    CurrentCamera.OnChangingCutsceneView(() => CurrentCamera.DarkAreaTriggerCutscene());
                    StartCoroutine(_EventTriggers.AfterCutscene(() => Trigger.OpenDarkArea()));
                }
            }
        }
    }
    public void TurnOffBodyRotationOnBoulderHitMe()
    {
        GetComponent<Rigidbody2D>().freezeRotation = false;
    }
    public void OnCutsceneTurnOffAnimation()
    {
        MyAnimator.SetFloat("Speed", 0.0f);
        MyAnimator.SetBool("Running", false);
    }
}