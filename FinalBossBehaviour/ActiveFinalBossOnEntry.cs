using UnityEngine;

/**
 * Controling when to Activate the final boss
 */
public class ActiveFinalBossOnEntry : MonoBehaviour
{
    public BossBehaviour _BossBehaviour;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _BossBehaviour.ActiveBossBehaviour();
            _BossBehaviour.GetComponent<OnBossLevel>().DisableSaveGameOnFinishingBossLevel();
            Destroy(gameObject);
        }
    }
}