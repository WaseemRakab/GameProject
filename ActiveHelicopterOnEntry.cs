using UnityEngine;
/**
 * Controling when to Activate HelicopterBoss
 */
public class ActiveHelicopterOnEntry : MonoBehaviour
{
    private AudioManager _AudioManager;
    public HelicopterBehaviour _HeliBehave;
    private void Awake()
    {
        GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);
    }
    private void Start()
    {
        if (!GetComponent<Renderer>().isVisible)
            enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _HeliBehave.ActiveHelicopterBehaviour();
            _HeliBehave.GetComponent<OnBossLevel>().DisableSaveGameOnFinishingBossLevel();
            _AudioManager?.PlayHelicopterFlyingSound();
            Invoke("ExitBossGate", 7.0f);
        }
    }

    private void Update()
    {
        if (_HeliBehave.ActiveHelicopter == true)
        {
            enabled = false;
            gameObject.SetActive(false);
        }
    }
    internal void ExitBossGate()
    {
        transform.parent.gameObject.GetComponent<BossGate>().enabled = false;
    }
    private void OnBecameVisible()
    {
        enabled = true;
    }
}