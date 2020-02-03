using UnityEngine;
/**
 * Enemy's Bullet behaviour Towards Player, other Interactable Objects Etc..
 */
public class EnemyBullet : MonoBehaviour
{
    private readonly float BulletSpeed = 20.0f;

    [Header("FinalBossDamage=[3]")]
    [Header("EnemyAutoGunWithBunkerDamage=[2]")]
    [Header("EnemyAutoGunWithoutBunkerDamage=[1]")]
    [Header("EnemyWithPistolBulletDamage=[0]")]

    public GameObject[] BulletHitEffectsPrefab;

    public int[] BulletDamageFromThisEnemy;
    public int WhichEnemy;

    private Rigidbody2D MyBody;
    private Vector2 zeroVec;

    public float ShootingDirection;

    private bool HitPlayer = false;

    private void Awake()
    {
        MyBody = GetComponent<Rigidbody2D>();
        zeroVec = Vector2.zero;
    }
    private void Update()
    {
        if (WentOutOfMap())
            Destroy(gameObject);
    }
    private void FixedUpdate()
    {
        MyBody.velocity = Vector2.SmoothDamp(MyBody.velocity, new Vector2(1f * BulletSpeed * ShootingDirection, MyBody.velocity.y), ref zeroVec, Time.fixedDeltaTime);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Ground") || other.collider.CompareTag("Wall")
            || other.collider.CompareTag("EventGround") || other.collider.CompareTag("PushableBox"))
        {
            Instantiate(BulletHitEffectsPrefab[Random.Range(0, BulletHitEffectsPrefab.Length)], transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else
        {
            if (!HitPlayer && other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<Player>().DecreaseShield(BulletDamageFromThisEnemy[WhichEnemy]);
                HitPlayer = true;
                Destroy(gameObject);
            }
        }
    }

    private bool WentOutOfMap()
    {
        return transform.position.x > 350f || transform.position.x < -200f;
    }
}