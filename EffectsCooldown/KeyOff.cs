using UnityEngine;

/**
 * Effects Cooldown Behaviour
 */
public class KeyOff : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 2f);
    }
}