using UnityEngine;
using UnityEngine.Events;
/**
* Effects Cooldown Behaviour
*/
public class ExplosionEffect : MonoBehaviour
{
    public UnityEvent WhichSoundToPlay;
    private void Awake()
    {
        WhichSoundToPlay = new UnityEvent();
    }
    private void Start()
    {
        WhichSoundToPlay.Invoke();
        Destroy(gameObject, 4f);
    }
}