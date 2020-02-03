using UnityEngine;

/**
 * Effects Cooldown Behaviour
 */
public class JumpEffectOff : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 0.4f);
    }
}
