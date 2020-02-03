using UnityEngine;

/**
 * Cannon Behaviour From TurretLauncher
 */
public class CannonLauncher : MonoBehaviour
{
    private readonly float launcherSpeed = 35.0f;

    private bool PlayerGotHit = false;

    public int LeftOrRight = 1;

    private AudioManager _AudioManager;

    public int WhichTurret;
    public int[] BulletDamageFromThisEnemy;
    private Rigidbody2D MyBody;

    private Vector2 zeroVec;

    private void Awake()
    {
        GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);
        MyBody = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        _AudioManager?.PlayFlareSound(GetComponent<AudioSource>());
    }
    private void Update()
    {
        DieOnOutOfBorders();
    }
    private void FixedUpdate()
    {
        MyBody.velocity = Vector2.SmoothDamp(MyBody.velocity, new Vector2(launcherSpeed * -LeftOrRight, MyBody.velocity.y), ref zeroVec, Time.fixedDeltaTime);
    }
    public void FlipDirection(int flipDirection)
    {
        LeftOrRight = flipDirection;
        Vector3 localScale = transform.localScale;
        localScale.y *= flipDirection;
        transform.localScale = localScale;
    }

    private void DieOnOutOfBorders()
    {
        if (transform.position.x < -150f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && PlayerGotHit == false)
        {
            other.gameObject.GetComponent<Player>().DecreaseShield(BulletDamageFromThisEnemy[WhichTurret]);
            PlayerGotHit = true;
            Destroy(gameObject);
        }
        else if
            (
            !other.gameObject.CompareTag("TurretLauncher")
            && !other.gameObject.CompareTag("TurretWall")
            && !other.gameObject.CompareTag("PlayerBullet")
            && PlayerGotHit == false
            )
            Destroy(gameObject);
    }
}