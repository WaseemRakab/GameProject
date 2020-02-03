using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTutorial : MonoBehaviour
{
    public Controls _Controls;
    public TutorialUI _TutorialUI;
    public AudioManager _AudioManager;
    public Animator MyAnimator;
    public CharacterController2D controller;


    public bool isJumping = false;
    public bool isCrouching = false;
    public bool isRunning = false;

    public LayerMask WhatisEnemies;

    public GameObject JumpEffectPrefab;


    private readonly float attackWaitingPeriod = 0.4f;
    private float currentAttackPeriod = 0.0f;
    public bool isAttacking = false;
    public Transform attackpos;
    public float attackRange;

    private float footStepsWaitingPeriod = 0.3f;
    private float currentFootStepsPeriod = 0.0f;

    private float _Speed = 35.0f;
    private readonly float MultiplierSpeed = 60.0f / 35.0f;


    public float timetoland = 0.8f;

    private readonly float weaponFireRate = 1.0f;
    private float canShoot = 0.0f;

    public GameObject Bulletprefab;
    public GameObject BulletEffect;

    public bool MovedRight = false;
    public bool MovedLeft = false;

    public bool CanMoveRight = false;
    public bool CanMoveLeft = false;
    public bool CanJump = false;
    public bool CanSprint;
    public bool CanShoot = false;
    public bool CanAttack = false;
    public bool CanCrouch = false;

    public RuntimeAnimatorController SwitchToGunController;

    public bool ReachedSecretChest;
    public Transform EventCheckPosition;

    public float HorizontalInput { get; private set; } = 0.0f;

    private void Awake()
    {
        MyAnimator = GetComponent<Animator>();
    }
    private void Update()
    {
        MovementInput();
        MyAnimator.SetFloat("Speed", Mathf.Abs(HorizontalInput));
        if (CanShoot)
            Shoot();
        if (CanAttack)
            Attack();
        if (CanJump)
            Jump();
        if (CanCrouch)
            Crouch();
        if (CanSprint)
            Sprint();
        if (ReachedSecretChest)
            EventCheck();
    }

    private void EventCheck()
    {
        Collider2D eventCheck = Physics2D.OverlapCircle(EventCheckPosition.position, 0.8f, LayerMask.GetMask("Event"));
        if (eventCheck != null)
        {
            eventCheck.GetComponent<SecretChestTutorial>().OnChestOpened();
        }
    }

    private void LateUpdate()
    {
        Move();
        HorizontalInput = 0f;
    }
    private void Move()
    {
        if (isRunning)
            controller.Move(HorizontalInput * Time.fixedDeltaTime * MultiplierSpeed, isCrouching && !isAttacking, isJumping);
        else
            controller.Move(HorizontalInput * Time.fixedDeltaTime, isCrouching && !isAttacking, isJumping);
    }
    private void Sprint()
    {
        if (Input.GetKey(_Controls.Sprint) && HorizontalInput != 0)
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
    private void MovementInput()
    {
        if (isRunning)
            footStepsWaitingPeriod = 0.24f;
        else
            footStepsWaitingPeriod = 0.3f;
        if (CanMoveRight)
        {
            if (Input.GetKey(_Controls.Forward))
            {
                if (!MyAnimator.GetBool("Jumping") && Time.time > currentFootStepsPeriod)
                {
                    currentFootStepsPeriod = Time.time + footStepsWaitingPeriod;
                    _AudioManager.PlayMovingSound();
                }
                HorizontalInput = 1f * _Speed;
                MovedRight = true;
            }
        }
        if (CanMoveLeft)
        {
            if (Input.GetKey(_Controls.Backward))
            {
                if (!MyAnimator.GetBool("Jumping") && Time.time > currentFootStepsPeriod)
                {
                    currentFootStepsPeriod = Time.time + footStepsWaitingPeriod;
                    _AudioManager.PlayMovingSound();
                }
                HorizontalInput = -1f * _Speed;
                MovedLeft = true;
            }
        }
    }
    public void SwitchToGunMode()
    {
        MyAnimator.runtimeAnimatorController = SwitchToGunController;
    }
    public void OnLanding(float FallingPosition)
    {
        isJumping = false;
        _AudioManager.PlayOnLandingSound();
        Instantiate(JumpEffectPrefab, transform.position - new Vector3(0f, 1f, 0f), Quaternion.identity);
    }
    private void Attack()
    {
        if (Input.GetKeyDown(_Controls.Attack) && MyAnimator.GetBool("Jumping") == false)
        {
            if (Time.time > currentAttackPeriod)
            {
                currentAttackPeriod = Time.time + attackWaitingPeriod;
                _AudioManager.PlayAttackSound();
                Collider2D EnemiesToDamage = Physics2D.OverlapCircle(attackpos.position, attackRange, WhatisEnemies);
                if (EnemiesToDamage != null)
                {
                    UnityAction<GameObject> WhichOneAttack = Delegate.CreateDelegate(typeof(UnityAction<GameObject>), this, EnemiesToDamage.tag + "Attacked") as UnityAction<GameObject>;
                    WhichOneAttack.Invoke(EnemiesToDamage.gameObject);
                }
                MyAnimator.SetBool("Attack", true);
                isAttacking = true;
                StartCoroutine(AttackDelay());
            }
        }
    }
    private void EnemyTutorialAttacked(GameObject Enemy)
    {
        Enemy.GetComponent<EnemyDummyTutorial>().Attacked();
    }
    private IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(0.25f);
        MyAnimator.SetBool("Attack", false);
        isAttacking = false;
    }
    private void Shoot()
    {
        if (Input.GetKey(_Controls.Shoot) && Time.time > canShoot)
        {
            _AudioManager.PlayShootingSound();
            canShoot = Time.time + weaponFireRate;
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

        }
    }
    private void Jump()
    {
        if (Input.GetKeyDown(_Controls.Jump) && !isCrouching)
        {
            if (!MyAnimator.GetBool("Jumping") && !isJumping)
            {
                _AudioManager.PlayJumpSound();
                MyAnimator.SetBool("Jumping", true);
                isJumping = true;
                StartCoroutine(JumpToLand());
            }
        }
    }
    private IEnumerator JumpToLand()
    {
        yield return new WaitForSeconds(timetoland);
        MyAnimator.SetBool("Jumping", false);
    }
    private void Crouch()
    {
        if (Input.GetKeyDown(_Controls.Crouch) && !MyAnimator.GetBool("Jumping"))
        {
            isCrouching = true;
            _Speed = 0f;
            MyAnimator.SetBool("Crouching", isCrouching);
        }
        else if (Input.GetKeyUp(_Controls.Crouch))
        {
            isCrouching = false;
            _Speed = 35.0f;
            MyAnimator.SetBool("Crouching", isCrouching);
        }
    }
}