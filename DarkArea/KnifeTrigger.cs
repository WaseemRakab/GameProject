using UnityEngine;
/**
 * Controling when to Activate Knife Drop Behaviour
 */
public class KnifeTrigger : MonoBehaviour
{
    public KnifeDrop _KnifeDrop;
    private bool KnifeTriggered = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (KnifeTriggered == false)
            {
                StartCoroutine(_KnifeDrop.ActivatingKnifeDropRoutine());
                KnifeTriggered = true;
            }
        }
    }
}