using UnityEngine;
using UnityEngine.Events;
/**
 * Helicopter Missiles Behaviour Towards Player
 */
public class HelicopterMissileBehaviour : MonoBehaviour
{
    private float MissileSpeed = 1.2f;

    public LayerMask _WhatisMyEnemy;
    public AudioManager _AudioManager;

    public GameObject HelicopterMissileHitEffectPrefab;
    public HelicopterBehaviour HeliBehaviour;

    public bool HitPlayer = false;

    private UnityEvent OnMeDestroyed;
    private UnityEvent OnPlayerDeadByMissile;
    private void Awake()
    {
        HeliBehaviour = GameObject.FindGameObjectWithTag("Helicopter").GetComponent<HelicopterBehaviour>();
        OnMeDestroyed = new UnityEvent();
        OnMeDestroyed.AddListener(() => HeliBehaviour.OnMissileBeingDestroyed(gameObject));
        OnPlayerDeadByMissile = new UnityEvent();
        OnPlayerDeadByMissile.AddListener(() => HeliBehaviour.StopKillingOnPlayerDead());

        if (_AudioManager == null)
            GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);
    }
    private void OnDestroy()
    {
        OnMeDestroyed.Invoke();
    }
    private void Update()
    {
        transform.Translate(Vector3.down * MissileSpeed * Time.deltaTime);
        if (transform.position.y < -40f)
            Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + new Vector3(0f, 5f, 0f), 8f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.Equals(HeliBehaviour.HelicopterColliders[0]) && !other.Equals(HeliBehaviour.HelicopterColliders[1]))
        {
            if (other.CompareTag("Ground") || other.CompareTag("EventGround"))
            {
                GameObject Missile = Instantiate(HelicopterMissileHitEffectPrefab, transform.position + new Vector3(0f, 3f, 0f), Quaternion.identity);
                Missile.GetComponent<ExplosionEffect>().WhichSoundToPlay.AddListener(() => _AudioManager.PlayHelicopterMissleDestoryedSound(Missile.GetComponent<AudioSource>()));
                AreaDamageEnemy(transform.position, 8f, 150f);
                Destroy(gameObject);
            }
        }
    }

    private void AreaDamageEnemy(Vector3 missileLocation, float radius, float maxDamage)
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(missileLocation + new Vector3(0f, 5f, 0f), radius, _WhatisMyEnemy);
        if (playerCollider != null)//Player Detected
        {
            float Distance = (missileLocation - playerCollider.transform.position).magnitude;
            float DamageEffect = 1 - (Distance / radius);
            playerCollider.GetComponent<Player>().DecreaseShield(Mathf.FloorToInt(maxDamage * DamageEffect));
            HitPlayer = true;
            if (playerCollider.GetComponent<Player>().PlayerDead)
                OnPlayerDeadByMissile.Invoke();
        }
    }
}