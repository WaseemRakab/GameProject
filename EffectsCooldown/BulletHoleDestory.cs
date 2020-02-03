using UnityEngine;
/**
 * Effects Cooldown Behaviour
 */
public class BulletHoleDestory : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 10f);
    }
}