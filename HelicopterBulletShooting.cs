using UnityEngine;
using UnityEngine.Events;

/**
 * Helicopter Bullets Behaviour Towards Player
 */
public class HelicopterBulletShooting : MonoBehaviour
{
    private readonly float HeliBulletSpeed = 25f;

    public HelicopterBehaviour HeliBehaviour;
    public GameObject HelicopterBulletHitEffectPrefab;

    public bool HitPlayer = false;

    private UnityEvent OnMeDestroyed;
    private UnityEvent OnPlayerDeadByBullet;

    private void Awake()
    {
        HeliBehaviour = GameObject.FindGameObjectWithTag("Helicopter").GetComponent<HelicopterBehaviour>();
        OnMeDestroyed = new UnityEvent();
        OnMeDestroyed.AddListener(() => HeliBehaviour.OnBulletBeingDestroyed(gameObject));
        OnPlayerDeadByBullet = new UnityEvent();
        OnPlayerDeadByBullet.AddListener(() => HeliBehaviour.StopKillingOnPlayerDead());
    }

    private void OnDestroy()
    {
        OnMeDestroyed.Invoke();
    }

    void Update()
    {
        transform.Translate(Vector3.left * HeliBulletSpeed * Time.deltaTime);
        if (transform.position.x > 150f || transform.position.x < -50f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.Equals(HeliBehaviour.HelicopterColliders[0]) && !other.Equals(HeliBehaviour.HelicopterColliders[1]))
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Player>().DecreaseShield(20);
                HitPlayer = true;
                if (other.GetComponent<Player>().PlayerDead)
                    OnPlayerDeadByBullet.Invoke();
            }
            Instantiate(HelicopterBulletHitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject, 0.09f);
        }
    }
}