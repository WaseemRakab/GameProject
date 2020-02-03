using UnityEngine;


/**
 * Obstacle Behaviour Towards Player
 */
public class Obstacle : MonoBehaviour
{
    private Player CurrentPlayer;
    private AudioManager _AudioManager;

    public GameObject Crumble;

    private SpawnStone _SpawnStoneObject;

    //private Vector3 StartPosition;
    private bool Falling = false;
    private bool Hit = false;
    public bool ComingFromleft = false;

    private bool WhichDirToSee = false;// False From Left , True From Right

    public int ObstacleID;

    private void Awake()
    {
        CurrentPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);
        _SpawnStoneObject = GameObject.FindGameObjectWithTag("SpawnStone").GetComponent<SpawnStone>();

        if (!GetComponent<Renderer>().isVisible)
            enabled = false;
    }
    public void ChangeMyViewDirection(int whichDir)
    {
        if (whichDir == 1)
            WhichDirToSee = false;
        else
            WhichDirToSee = true;
    }
    void Update()
    {
        if (CurrentPlayer != null && !Falling)
        {
            if (WhichDirToSee == false)
            {
                if (transform.position.x - CurrentPlayer.transform.position.x <= 5f
                    && CurrentPlayer.transform.position.y < transform.position.y)
                {
                    Falling = true;
                }
            }
            else
            {
                if (transform.position.x - CurrentPlayer.transform.position.x >= -5f
                    && CurrentPlayer.transform.position.y < transform.position.y)
                {
                    Falling = true;
                }
            }
        }

        if (Falling == true)
        {
            Fall();
        }

        if (transform.position.y < -110f) // if the stone is out of the map (Falling Stone)
        {
            Falling = false;
            _SpawnStoneObject._SpawnObjects._SpawnObjects.ObstaclesAlive[ObstacleID] = false;
            Destroy(gameObject);
        }
    }
    private void Fall()
    {
        transform.Translate(Vector3.right * Time.deltaTime * 20f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !Hit)
        {
            Hit = true;
            CurrentPlayer.DecreaseShield(100);
        }
        else if (other.CompareTag("Ground") || other.CompareTag("EventGround"))
        {
            GameObject Crumble = Instantiate(this.Crumble, transform.position, Quaternion.identity);
            Crumble.GetComponent<ExplosionEffect>().WhichSoundToPlay.AddListener(() => _AudioManager.PlayCrumbleSound(Crumble.GetComponent<AudioSource>()));
        }
        if (!other.CompareTag("PlayerBullet"))
        {
            _SpawnStoneObject._SpawnObjects._SpawnObjects.ObstaclesAlive[ObstacleID] = false;
            Destroy(gameObject);
        }
    }
    private void OnBecameInvisible()
    {
        enabled = false;
    }
    private void OnBecameVisible()
    {
        enabled = true;
    }
}