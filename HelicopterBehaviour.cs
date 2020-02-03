using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/**
 * Helicopter Boss Behaviour Towards Player, Other Interactble Objects Etc..
 */
public class HelicopterBehaviour : MonoBehaviour
{
    public Player _CurrentPlayer;
    private AudioManager _AudioManager;

    public BoxCollider2D[] HelicopterColliders; // 0 = Body,Rotors - 1=Back Body Rotors

    public Transform HelicopterBulletViewPosition;
    public float BulletRadius;
    public Transform HelicopterMissileViewPosition;
    public float MissileRadius;

    public GameObject HelicopterBulletPrefab;
    public GameObject HelicopterBulletEffectPrefab;

    private List<GameObject> _HeliBulletsShooting;
    private List<GameObject> _HeliMissilesLaunching;

    public GameObject HelicopterMissilePrefab;

    private const float MaxHeliHealth = 650f;
    private float HeliHealth = 650f;
    public Image HeliLiveHealthBar;

    private const float _HeliSpeed = 10.0f;


    public bool IsHelicopterDestroyed = false;
    public bool ActiveHelicopter = false;

    private readonly int StartHelicopterWayX = 110;
    private readonly int LastHelicopterWayX = 50;

    //On First Move
    private float FirstMaxForwardRotate = 12f;
    private float FirstMinBackwardRotate = 348f;

    //On Second Move
    private float SecondMaxForwardRotate = 7f;
    private float SecondMinBackwardRotate = 353f;

    private readonly int SecondMoveYPosition = 28;
    private readonly int FirstMoveYPosition = 21;

    private readonly int HelicopterIdleRotation = 0;


    private bool HalfWay = false;
    private bool IsGoingLeft = true;
    private bool IsUp = false;
    private bool SecondMove = false;
    private bool IsGoingDown = false;

    private bool HelicopterOnFire = false;

    private bool iHitPlayerByBullet = false;
    private bool iHitPlayerByMissile = false;

    private readonly float HeliBulletShootingFireRate = 0.2f;
    private float canShootBullet = 0.0f;

    private readonly float HeliMissileFireRate = 0.9f;
    private float canShootMissile = 0.0f;

    private bool IsMyEnemyDead = false;


    private Animator MyAnimator;
    private Rigidbody2D MyBody;

    private GameObject helicopterDestroyedPrefab;


    private float PreviousPlayerPosX;
    private float LastPlayerPosX;//To Track Player's Movement
    private void Awake()
    {
        _CurrentPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);
        MyAnimator = GetComponent<Animator>();
        MyBody = GetComponent<Rigidbody2D>();
        helicopterDestroyedPrefab = transform.GetChild(1).gameObject;
        _HeliBulletsShooting = new List<GameObject>();
        _HeliMissilesLaunching = new List<GameObject>();
    }
    private void Update()
    {
        if (ActiveHelicopter == true)//If Helicopter is Not Destroyed
        {
            if (IsGoingLeft == true && IsUp == false)//First Move Going Left
            {
                if (transform.position.x <= (StartHelicopterWayX + LastHelicopterWayX) / 2)
                {
                    HalfWay = true;
                    SteadyHelictoper(!IsGoingLeft);
                }
                else
                {
                    HalfWay = false;
                }
                if (IsMyEnemyDead == false)
                {
                    if (SearchPlayer(ref LastPlayerPosX, ref HelicopterBulletViewPosition, ref BulletRadius))//Player Detected on Left Side of me FirstWay
                    {
                        StartShooting();
                        AimAndKillForward();
                    }
                    else
                    {
                        iHitPlayerByBullet = false;
                        MoveForward(ref FirstMaxForwardRotate, FirstMoveYPosition, _HeliSpeed, false);
                    }
                }
                else
                    MoveForward(ref FirstMaxForwardRotate, FirstMoveYPosition, _HeliSpeed, false);
            }
            else if (IsUp == false)//First Moving Going Right
            {
                if (transform.position.x >= (StartHelicopterWayX + LastHelicopterWayX) / 2)
                {
                    HalfWay = true;
                    SteadyHelictoper(!IsGoingLeft);
                }
                else
                {
                    HalfWay = false;
                }
                if (IsMyEnemyDead == false)
                {
                    if (SearchPlayer(ref LastPlayerPosX, ref HelicopterBulletViewPosition, ref BulletRadius))//Player Detected on Right Side of me FirstWay
                    {
                        StartShooting();
                        AimAndKillBackward();
                    }
                    else
                    {
                        iHitPlayerByBullet = false;
                        MoveBackward(ref FirstMinBackwardRotate, FirstMoveYPosition, _HeliSpeed, false);
                    }
                }
                else
                    MoveBackward(ref FirstMinBackwardRotate, FirstMoveYPosition, _HeliSpeed, false);
                if (IsGoingLeft == true)//Finished First Move, Now Going Up
                {
                    IsUp = true;
                }
            }
            if (IsGoingLeft == true && IsUp == true && IsGoingDown == false)//Second Move,Going Left
            {
                if (SecondMove == false)
                    GoingUp();
                if (SecondMove == true)
                {
                    if (IsMyEnemyDead == false)
                    {
                        if (SearchPlayer(ref LastPlayerPosX, ref HelicopterMissileViewPosition, ref MissileRadius))//Player Detected on Left Side of me SecondWay
                        {
                            StartLaunchingMissiles();
                            AimAndLaunchForward();
                        }
                        else
                        {
                            iHitPlayerByMissile = false;
                            MoveForward(ref SecondMaxForwardRotate, SecondMoveYPosition, _HeliSpeed / 1.2f, true, 12f);
                        }
                    }
                    else
                        MoveForward(ref SecondMaxForwardRotate, SecondMoveYPosition, _HeliSpeed / 1.2f, true, 12f);
                }
            }
            else if (IsUp == true)//Second Move,Going Right
            {
                if (IsGoingDown == false)
                {
                    if (IsMyEnemyDead == false)
                    {
                        if (SearchPlayer(ref LastPlayerPosX, ref HelicopterMissileViewPosition, ref MissileRadius))//Player Detected on Right Side of me SecondWay
                        {
                            StartLaunchingMissiles();
                            AimAndLaunchBackward();
                        }
                        else
                        {
                            iHitPlayerByMissile = false;
                            MoveBackward(ref SecondMinBackwardRotate, SecondMoveYPosition, _HeliSpeed / 1.5f);
                        }
                    }
                    else
                        MoveBackward(ref SecondMinBackwardRotate, SecondMoveYPosition, _HeliSpeed / 1.5f);
                }
                if (IsGoingLeft == true) //Final Way Of Second Move, Now Going Down , Back to First Move
                {
                    IsGoingDown = true;
                    SteadyHelictoper(true);
                    GoingDown();
                }

            }
        }
        else if (HelicopterOnFire == true)
        {
            SlowDownHelicopterRotor();//On Helicopter Destroyed
        }
    }
    private void AimAndKillBackward()
    {
        if (iHitPlayerByBullet == false)
        {
            if (_HeliBulletsShooting.Any(bullet => bullet.GetComponent<HelicopterBulletShooting>().HitPlayer == true))
                iHitPlayerByBullet = true;
            else
                MoveBackward(ref FirstMinBackwardRotate, FirstMoveYPosition, _HeliSpeed / 2.0f, false);
        }
        if (Mathf.Round(PreviousPlayerPosX) != Mathf.Round(LastPlayerPosX))
        {
            iHitPlayerByBullet = false;
        }
        PreviousPlayerPosX = LastPlayerPosX;
    }
    private void AimAndKillForward()
    {
        if (iHitPlayerByBullet == false)
        {
            if (_HeliBulletsShooting.Any(bullet => bullet.GetComponent<HelicopterBulletShooting>().HitPlayer == true))
                iHitPlayerByBullet = true;
            else
                MoveForward(ref FirstMaxForwardRotate, FirstMoveYPosition, _HeliSpeed / 2.0f, false);
        }
        if (Mathf.Round(PreviousPlayerPosX) != Mathf.Round(LastPlayerPosX))
        {
            iHitPlayerByBullet = false;
        }
        PreviousPlayerPosX = LastPlayerPosX;
    }
    private void AimAndLaunchForward()
    {
        if (iHitPlayerByMissile == false)
        {
            if (_HeliMissilesLaunching.Any(missle => missle.GetComponent<HelicopterMissileBehaviour>().HitPlayer == true))
                iHitPlayerByMissile = true;
            else
                MoveForward(ref SecondMaxForwardRotate, SecondMoveYPosition, _HeliSpeed / (1.2f / 0.5f), true, 12f);
        }
        if (Mathf.Round(PreviousPlayerPosX) != Mathf.Round(LastPlayerPosX))
        {
            iHitPlayerByMissile = false;
        }
        PreviousPlayerPosX = LastPlayerPosX;
    }
    private void AimAndLaunchBackward()
    {
        if (iHitPlayerByMissile == false)
        {
            if (_HeliMissilesLaunching.Any(missle => missle.GetComponent<HelicopterMissileBehaviour>().HitPlayer == true))
                iHitPlayerByMissile = true;
            else
                MoveBackward(ref SecondMinBackwardRotate, SecondMoveYPosition, _HeliSpeed / (1.2f / 0.5f));
        }
        if (Mathf.Round(PreviousPlayerPosX) != Mathf.Round(LastPlayerPosX))
        {
            iHitPlayerByMissile = false;
        }
        PreviousPlayerPosX = LastPlayerPosX;
    }
    private void DestroyHelicopter()
    {
        MatchExplosionEffect();
        helicopterDestroyedPrefab.SetActive(true);
    }
    public void OnBulletBeingDestroyed(GameObject thisBullet)
    {
        _HeliBulletsShooting.Remove(thisBullet);
    }
    public void OnMissileBeingDestroyed(GameObject thisMissile)
    {
        _HeliMissilesLaunching.Remove(thisMissile);
    }
    private bool SearchPlayer(ref float LastPos, ref Transform WhichView, ref float WhichRadius)
    {
        Collider2D PlayerDetection = Physics2D.OverlapCircle(WhichView.position, WhichRadius, LayerMask.GetMask("Player"));
        if (PlayerDetection != null)
        {
            LastPos = PlayerDetection.gameObject.transform.position.x;
            return true;
        }
        return false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(HelicopterBulletViewPosition.position, BulletRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(HelicopterMissileViewPosition.position, MissileRadius);
    }
    private void MatchExplosionEffect()
    {
        for (int i = 0; i < helicopterDestroyedPrefab.transform.childCount; ++i)
        {
            Vector3 localScale = helicopterDestroyedPrefab.transform.GetChild(i).gameObject.transform.localScale;
            Vector3 updatedScale;
            if (IsGoingLeft == false)
            {
                updatedScale = new Vector3(localScale.x * -1, localScale.y, localScale.z);
                helicopterDestroyedPrefab.transform.GetChild(i).gameObject.transform.localScale = updatedScale;
            }
        }
    }
    private void StartShooting()
    {
        if (Time.time > canShootBullet)
        {
            canShootBullet = Time.time + HeliBulletShootingFireRate;
            _AudioManager?.PlayHelicopterShootingSound(GetComponent<AudioSource>());
            if (transform.localScale.x >= 0)
            {
                Instantiate(HelicopterBulletEffectPrefab, transform.position - new Vector3(1.6f, 2.75f, 0f), Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z - 45f));
                _HeliBulletsShooting.Add(Instantiate(HelicopterBulletPrefab, transform.position - new Vector3(2f, 2.9f, 0f), Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + 30f)));
            }
            else
            {
                Instantiate(HelicopterBulletEffectPrefab, transform.position - new Vector3(-1.6f, 2.75f, 0f), Quaternion.Euler(0f, 180f, -transform.rotation.eulerAngles.z - 45f));
                _HeliBulletsShooting.Add(Instantiate(HelicopterBulletPrefab, transform.position - new Vector3(-2f, 2.9f, 0f), Quaternion.Euler(0, 180f, -transform.rotation.eulerAngles.z + 30f)));
            }
        }
    }
    private void StartLaunchingMissiles()
    {
        if (Time.time > canShootMissile)
        {
            canShootMissile = Time.time + HeliMissileFireRate;
            _AudioManager?.PlayHelicopterMissleLaunchSound(GetComponent<AudioSource>());
            if (transform.localScale.x >= 0)
            {
                _HeliMissilesLaunching.Add(Instantiate(HelicopterMissilePrefab, transform.position - new Vector3(1.6f, 3.6f, 0f), Quaternion.identity));
            }
            else
            {
                _HeliMissilesLaunching.Add(Instantiate(HelicopterMissilePrefab, transform.position - new Vector3(-1.6f, 3.6f, 0f), Quaternion.Euler(0f, 180f, 0f)));
            }
        }
    }
    private void GoingUp()
    {
        if (Mathf.FloorToInt(transform.position.y) != SecondMoveYPosition)
        {
            transform.Translate(new Vector3(0f, 0.5f, 0f) * _HeliSpeed * Time.deltaTime);
        }
        else
        {
            SecondMove = true;
        }
    }
    private void GoingDown()
    {
        if (Mathf.FloorToInt(transform.position.y) != FirstMoveYPosition)
        {
            transform.Translate(new Vector3(0f, -0.5f, 0f) * _HeliSpeed * Time.deltaTime);
        }
        else //Finished Going Down(Aka Moving Back to First Move)
        {
            SecondMove = false;
            IsUp = false;
            IsGoingDown = false;
        }
    }
    private void SteadyHelictoper(bool WhichDirection)//True = Right, False = Left
    {
        if (Mathf.FloorToInt(transform.rotation.eulerAngles.z) != HelicopterIdleRotation)
        {
            if (WhichDirection == true)
                transform.Rotate(new Vector3(0, 0, 0.2f));
            else
                transform.Rotate(new Vector3(0, 0, -0.2f));
        }
    }
    private void MoveBackward(ref float MinBackwardRotate, int HelicopterMaxYPosition, float HelicopterSpeed, bool SecondMove = true)
    {
        if (Mathf.Round(transform.rotation.eulerAngles.z) != MinBackwardRotate)
        {
            if (SecondMove == false)
            {
                if (HalfWay == false)
                {
                    transform.Rotate(new Vector3(0, 0, -0.2f));
                }
            }
            else
            {
                transform.Rotate(new Vector3(0, 0, -0.2f));
            }
        }
        transform.position = new Vector3(transform.position.x, HelicopterMaxYPosition, transform.position.z);
        transform.Translate(new Vector3(1f, 0, 0f) * HelicopterSpeed * Time.deltaTime);
        if (Mathf.FloorToInt(transform.position.x) == StartHelicopterWayX)
        {
            transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            IsGoingLeft = true;
        }
    }
    private void MoveForward(ref float MaxForwardRotate, int HelicopterMaxYPosition, float HelicopterSpeed, bool SecondMove = true, float MoreWayPoint = 0.0f)
    {
        if (Mathf.Round(transform.rotation.eulerAngles.z) != MaxForwardRotate)
        {
            if (SecondMove == false)
            {
                if (HalfWay == false)
                {
                    transform.Rotate(new Vector3(0, 0, 0.2f));
                }
            }
            else
            {
                transform.Rotate(new Vector3(0, 0, 0.2f));
            }
        }
        transform.position = new Vector3(transform.position.x, HelicopterMaxYPosition, transform.position.z);
        transform.Translate(new Vector3(-1f, 0, 0f) * HelicopterSpeed * Time.deltaTime);
        if (Mathf.FloorToInt(transform.position.x) == LastHelicopterWayX - MoreWayPoint)
        {
            transform.localScale = new Vector3(-1.5f, 1.5f, 1f);
            IsGoingLeft = false;
        }
    }
    public void ReduceHealth(float DamageAmount)
    {
        if (IsHelicopterDestroyed == false)
        {
            HeliHealth -= DamageAmount;
            HeliLiveHealthBar.fillAmount = HeliHealth / MaxHeliHealth;
            if (HeliHealth <= 0) // Finished Stage 1 , Defeated Boss
            {
                MyBody.freezeRotation = false;
                DestroyHelicopter();
                if (!HelicopterOnFire)
                {
                    _AudioManager?.PlayHelicopterBeingDestoryedSound();
                    _AudioManager?.PlayHelicopterStageCompleteMusicSound();
                }
                MyBody.gravityScale = 0.5f;
                HelicopterOnFire = true;
                ActiveHelicopter = false;
            }
        }
    }
    private void SlowDownHelicopterRotor()
    {
        if (MyAnimator.speed >= 0.003f)
            MyAnimator.speed -= 0.003f;
    }
    private void DieOnAttack()
    {
        if (HelicopterOnFire == true)
        {
            IsHelicopterDestroyed = true;
            gameObject.layer = 8;//Setting Layer To Ground
            GetComponent<FinishLevel>().FinishingLevel(25f, true);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!IsHelicopterDestroyed)
            {
                if (other.otherCollider.Equals(HelicopterColliders[0]))//Player Hitted Body
                {
                    //Force Player To Go Left
                    if (transform.localScale.x >= 0)
                        PushPlayerWhenTouchingme(-1f);
                    else
                        PushPlayerWhenTouchingme(1f);
                    _CurrentPlayer.DecreaseShield(25);
                }
                else if (other.otherCollider.Equals(HelicopterColliders[1]))//Player Hitted Back
                {
                    //Force Player To Go Right
                    if (transform.localScale.x >= 0)
                        PushPlayerWhenTouchingme(1f);
                    else
                        PushPlayerWhenTouchingme(-1f);
                    _CurrentPlayer.DecreaseShield(25);
                }
                else if (other.otherCollider.Equals(HelicopterColliders[2]))
                {
                    other.gameObject.GetComponent<Player>().DecreaseLive(true);
                }
            }
        }
        else if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Wall"))//When helicopter already destroyed
            DieOnAttack();
    }
    private void PushPlayerWhenTouchingme(float whichDir)
    {
        Vector3 zeroVec = Vector3.zero;
        Vector2 Rig = _CurrentPlayer.GetComponent<Rigidbody2D>().velocity;
        _CurrentPlayer.GetComponent<Rigidbody2D>().velocity = Vector3.SmoothDamp(Rig, new Vector3(50f * whichDir, _CurrentPlayer.transform.position.y - 100f, 0f), ref zeroVec, 0.08f);

    }
    public void ActiveHelicopterBehaviour()
    {
        ActiveHelicopter = true;
    }
    public void StopKillingOnPlayerDead()//By Event Invoked (Bullet Behaviour,Missile Behaviour)
    {
        IsMyEnemyDead = true;
    }
}