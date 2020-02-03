using UnityEngine;

/**
 * Controling Player Being Close to Boss Area Behaviour
 */
public class BossSign : MonoBehaviour
{
    private AudioManager _AudioManager;

    public bool SignTrigger;

    private void Awake()
    {
        GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);
    }

    private void Start()
    {
        SignTrigger = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var IsPlayer = other.gameObject.TryGetComponent<Player>(out var player);
            if (!IsPlayer)
            {
                _AudioManager?.PlayStoneDoorOpen(GetComponent<AudioSource>());
                SignTrigger = true;
            }
            else // PlayerTutorial
            {
                if (player._Stats._PlayerStats.HasWeapon)
                {
                    _AudioManager?.PlayStoneDoorOpen(GetComponent<AudioSource>());
                    SignTrigger = true;
                }
                else
                {
                    transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _AudioManager?.PlayStoneDoorClosing(GetComponent<AudioSource>());
            SignTrigger = false;
            if (transform.childCount > 0 && transform.GetChild(0).gameObject.activeSelf)
                transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}