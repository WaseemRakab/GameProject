using System;
using UnityEngine;
using UnityEngine.Events;

/**
 * Player's Bullet Behaviour Towards Enemies, Obstacles, Other Objects Etc
 */
public class Shooting : MonoBehaviour
{
    public ScriptableWeapon _WeaponData;
    public GameObject[] BulletHitEffectsPrefab;

    private readonly float Speed = 35.0f;

    private bool HitEnemy = false;

    public float ShootingDirection;

    private Rigidbody2D MyBody;

    private Vector2 zeroVec;

    private Transform PlayerX;
    private void Awake()
    {
        MyBody = GetComponent<Rigidbody2D>();
        zeroVec = Vector2.zero;
        PlayerX = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Update()
    {
        float PlayerXPos = PlayerX.position.x;
        if (WentOutOfCameraView(PlayerXPos))
            Destroy(gameObject);
    }
    private void FixedUpdate()
    {
        MyBody.velocity = Vector2.SmoothDamp(MyBody.velocity, new Vector2(1f * Speed * ShootingDirection, MyBody.velocity.y), ref zeroVec, Time.fixedDeltaTime);
        RaycastHit2D BulletHit = Physics2D.Raycast(transform.position, transform.right, Mathf.Infinity, LayerMask.GetMask("Enemies", "Turret"));
        if (HitEnemy == false && BulletHit.collider != null)
        {
            float distance = Mathf.Abs(BulletHit.point.x - transform.position.x);
            if (distance <= 1f)
            {
                UnityAction<GameObject> whichOneHit = Delegate.CreateDelegate(typeof(UnityAction<GameObject>), this, BulletHit.collider.tag + "Hit") as UnityAction<GameObject>;
                whichOneHit.Invoke(BulletHit.collider.gameObject);
                HitEnemy = true;
                Destroy(gameObject);
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Ground") || other.collider.CompareTag("Wall") || other.collider.CompareTag("EventGround"))
        {
            Instantiate(BulletHitEffectsPrefab[UnityEngine.Random.Range(0, BulletHitEffectsPrefab.Length)], transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    private bool WentOutOfCameraView(float PlayerXPos)
    {
        return transform.position.x > (PlayerXPos + 25.5f) || transform.position.x < (PlayerXPos - 25.5f);
    }
    private void TurretLauncherHit(GameObject Turret)
    {
        Turret.GetComponent<TurretLauncher>().ReduceHealth(_WeaponData._Weapon._Damage);
    }
    private void TurretWallHit(GameObject TurretWall)
    {
        TurretWall.GetComponent<TurretLauncher>().ReduceHealth(_WeaponData._Weapon._Damage);
    }
    private void EnemyWithPistolHit(GameObject EnemyWithPistol)
    {
        EnemyWithPistol.GetComponent<EnemyPistolBehaviour>().ReduceHealth(_WeaponData._Weapon._Damage);
    }
    private void EnemyAutoGunWithoutBunkerHit(GameObject EnemyAutoGunWithoutBunker)
    {
        EnemyAutoGunWithoutBunker.GetComponent<EnemyAutoGunWOBunkerBehaviour>().ReduceHealth(_WeaponData._Weapon._Damage);
    }
    private void EnemyAutoGunWithBunkerHit(GameObject EnemyAutoGunWithBunker)
    {
        EnemyAutoGunWithBunker.GetComponent<EnemyAutoGunWBunkerBehaviour>().ReduceHealth(_WeaponData._Weapon._Damage);
    }
    private void HelicopterHit(GameObject Helicopter)
    {
        Helicopter.GetComponent<HelicopterBehaviour>().ReduceHealth(_WeaponData._Weapon._Damage);
    }
    private void FinalBossHit(GameObject FinalBoss)
    {
        FinalBoss.GetComponent<BossBehaviour>().ReduceHealth(_WeaponData._Weapon._Damage);
    }
    private void EnemyTutorialHit(GameObject EnemyTutorialHit)
    {
        EnemyTutorialHit.GetComponent<EnemyDummyTutorial>().GotShot();
    }
}