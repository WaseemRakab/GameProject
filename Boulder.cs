using UnityEngine;

/**
 * Obstacle In-Game Behaviour
 */
public class Boulder : MonoBehaviour
{
    private AudioManager _AudioManager;
    private Rigidbody2D MyBody;
    private void Awake()
    {
        GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);
        MyBody = GetComponent<Rigidbody2D>();
    }
    public void StartRolling()
    {
        _AudioManager?.PlayBoulderRollingSound(GetComponent<AudioSource>());
        MyBody.gravityScale = 2.18f;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player>().TurnOffBodyRotationOnBoulderHitMe();
            other.gameObject.GetComponent<Player>().DecreaseLive(true);
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            _AudioManager?.StopBoulderRollingSound(GetComponent<AudioSource>());
            Destroy(MyBody);
            Destroy(this);
        }
    }
}
