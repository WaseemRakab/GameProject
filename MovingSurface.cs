using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/**
 * Interactable Object Towards Player Behaviour
 */
public class MovingSurface : MonoBehaviour
{
    public ScriptableSpawnObjects _SpawnObjects;
    public LevelRestartedOrNextLevel _OnNextLevel;

    private AudioManager _AudioManager;

    public int SurfaceID;

    public bool PlayerOnSurface = false;

    public float SmoothTime;
    public float speed;

    private Vector3 StartPosition;

    private float Dir = -1f;

    [Header("0 = Rock Surface, 1 = Sci-fi Surface , -1 = For Obstacles")]
    public int WhichTypeOfSurface;

    private Rigidbody2D myBody;
    private Vector2 zeroVec = Vector2.zero;

    public bool HasSwitch = false;
    public SwitchToggle _SwitchToggle;
    public GameObject SwitchObject;

    private bool isAudioPlaying = false;//For SwitchEnabledSurface

    private AudioSource MovingSurfaceSource;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        StartPosition = transform.position;
        if (_OnNextLevel != null)
        {
            GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);
            MovingSurfaceSource = GetComponent<AudioSource>();
            //When the Game Loaded From Main Menu Only
            if (_OnNextLevel.OnNextLevel == false)
                ChangeMyPositionOnGameLoaded();
        }
    }
    private void Start()
    {
        if (HasSwitch)
            SwitchObject.SetActive(true);
        ResetSurfacesPositions();
    }
    private void ChangeMyPositionOnGameLoaded()
    {
        if (_SpawnObjects != null)
        {
            if (WhichTypeOfSurface == 0)//This is Rock Surface
            {
                if (_SpawnObjects._SpawnObjects.RockMovingSurfacesPositions.Count > 0)//The Game Loaded
                {
                    Vector LoadedPos = _SpawnObjects._SpawnObjects.RockMovingSurfacesPositions[SurfaceID];
                    transform.position = new Vector3(LoadedPos.X, LoadedPos.Y, LoadedPos.Z);
                }
            }
            else
            {
                if (_SpawnObjects._SpawnObjects.SciFiMovingSurfacesPositions.Count > 0)//The Game Loaded
                {
                    Vector LoadedPos = _SpawnObjects._SpawnObjects.SciFiMovingSurfacesPositions[SurfaceID];
                    transform.position = new Vector3(LoadedPos.X, LoadedPos.Y, LoadedPos.Z);
                }
            }
        }
    }
    private void ResetSurfacesPositions()//After Getting the position reset the Surfaces position on the scriptable for 
                                         //another save,session etc
    {
        if (_SpawnObjects != null)
        {
            _SpawnObjects._SpawnObjects.SciFiMovingSurfacesPositions.Clear();
            _SpawnObjects._SpawnObjects.RockMovingSurfacesPositions.Clear();
        }
    }
    private void FixedUpdate()
    {
        if (PlayerOnSurface)
        {
            if (!HasSwitch)
            {
                myBody.velocity = Vector2.SmoothDamp(myBody.velocity, new Vector2(myBody.velocity.x, Dir * speed), ref zeroVec, SmoothTime);
                if (Mathf.Round(transform.position.y) == Mathf.Round(StartPosition.y))
                {
                    Dir = -1f;
                }
            }
            else
            {
                UnityAction WhichWay = Delegate.CreateDelegate(typeof(UnityAction), this, "Go" + _SwitchToggle._CurrentDirection) as UnityAction;
                WhichWay.Invoke();
            }
        }
    }
    #region OnHaveSwitch
    private async void Go0()//Go Up
    {
        if (!isAudioPlaying)
        {
            _AudioManager.PlayMovingSurfaceSound(MovingSurfaceSource, WhichTypeOfSurface, 0.5f);
            isAudioPlaying = true;
            await Task.Delay(600);
        }
        myBody.constraints = ~RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        myBody.velocity = Vector2.SmoothDamp(myBody.velocity, new Vector2(myBody.velocity.x, 1f * speed), ref zeroVec, SmoothTime);
        if (Mathf.Round(transform.position.y) == Mathf.Round(StartPosition.y))
        {
            Go1();
            _SwitchToggle.PlaySwitchToggle();
        }
    }
    private void Go1()//Go Idle
    {
        if (isAudioPlaying)
        {
            _AudioManager.StopMovingSurfaceSound(MovingSurfaceSource);
            isAudioPlaying = false;
        }
        myBody.constraints = RigidbodyConstraints2D.FreezeAll;
        _SwitchToggle.IdleState();
    }
    private async void Go2()//Go Down
    {
        if (!isAudioPlaying)
        {
            _AudioManager.PlayMovingSurfaceSound(MovingSurfaceSource, WhichTypeOfSurface, 0.5f);
            isAudioPlaying = true;
            await Task.Delay(600);
        }
        myBody.constraints = ~RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        myBody.velocity = Vector2.SmoothDamp(myBody.velocity, new Vector2(myBody.velocity.x, -1f * speed), ref zeroVec, SmoothTime);
    }
    #endregion
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerOnSurface = true;
            if (!HasSwitch)//Auto
                if (_AudioManager != null)
                    _AudioManager.PlayMovingSurfaceSound(MovingSurfaceSource, WhichTypeOfSurface);
            myBody.constraints = ~RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
            if (HasSwitch)
                _SwitchToggle.PlayerOnSurface(other.gameObject.GetComponent<Player>());
        }
        else if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("PushableBox"))
        {
            if (WhichTypeOfSurface == -1)
            {
                myBody.gravityScale = 0.0f;
                myBody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
                PlayerOnSurface = false;
            }
            Dir = 1f;
            if (HasSwitch)
                Go1();//GoIdle
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (WhichTypeOfSurface != -1)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                PlayerOnSurface = false;
                if (!HasSwitch)
                {
                    _AudioManager.StopMovingSurfaceSound(GetComponent<AudioSource>());
                    myBody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
                }
                else
                {
                    _SwitchToggle.SwitchOffState();
                    if (_SwitchToggle._CurrentDirection != 1)
                    {
                        Go1();
                        _SwitchToggle.PlaySwitchToggle();
                    }
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (WhichTypeOfSurface != -1)
            if (other.gameObject.CompareTag("DeadZone") || other.gameObject.CompareTag("Spikes"))
            {
                ResetStone();
            }
    }
    private void ResetStone()
    {
        PlayerOnSurface = false;
        myBody.constraints = ~RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }
    public void ResetStoneState()
    {
        ResetStone();
        transform.position = StartPosition;
    }
}