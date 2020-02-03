using UnityEngine;

/**
 * Effects Cooldown Behaviour
 */
public class PlayerBulletEffectOff : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 0.2f);
    }
}