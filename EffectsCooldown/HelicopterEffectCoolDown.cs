using UnityEngine;

/**
 * Effects Cooldown Behaviour
 */
public class HelicopterEffectCoolDown : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 0.05f);
    }
}