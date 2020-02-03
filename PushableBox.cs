using UnityEngine;

/**
 * Interactable Object Towards Player Behaviour
 */
public class PushableBox : MonoBehaviour
{
    private Rigidbody2D _MyBoxBody;
    private Player _Player;
    private UIManager _UiManager;
    private AudioManager _AudioManager;
    public ScriptableSpawnObjects _SpawnObjects;
    public LevelRestartedOrNextLevel _OnNextLevel;

    public int PushableBoxID;

    private bool PressedInteractPush = false;

    private float PushDir;
    private readonly float PushForce = 5f;

    public CapsuleCollider2D Surface;
    private void Awake()
    {
        _Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _UiManager = GameObject.FindGameObjectWithTag("UiSystem").GetComponent<UIManager>();
        GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);
        _MyBoxBody = GetComponent<Rigidbody2D>();
        if (_OnNextLevel.OnNextLevel == false)
        {
            OnGameLoadedSetMyPosition();
        }
    }

    private void Start()
    {
        ResetPushableBoxPositions();

        if (!GetComponent<Renderer>().isVisible)
            enabled = false;
    }

    private void ResetPushableBoxPositions()
    {
        if (_SpawnObjects != null)
        {
            _SpawnObjects._SpawnObjects.PushableBoxPositions.Clear();
        }
    }

    private void OnGameLoadedSetMyPosition()
    {
        if (_SpawnObjects != null)
        {
            if (_SpawnObjects._SpawnObjects.PushableBoxPositions.Count > 0)
            {
                Vector MyPushPos = _SpawnObjects._SpawnObjects.PushableBoxPositions[PushableBoxID];
                transform.position = new Vector3(MyPushPos.X, MyPushPos.Y, MyPushPos.Z);
            }
        }
    }

    private void FixedUpdate()
    {
        if (PressedInteractPush)
        {
            MoveBox();
        }
    }

    private void Update()
    {
        Collider2D Collider = Physics2D.OverlapBox(transform.position, Surface.size + new Vector2(3.9f, 0f), 0, LayerMask.GetMask("Player"));
        if (Collider != null)
        {
            if (CanPush(Collider))
            {
                _Player.GetInteractingMessage(_Player.transform.GetChild(4).gameObject, true);
                _UiManager.ShowInteractingPushMessage();
                if (Input.GetKey(_Player._Controls.Interact))
                {
                    _UiManager.HideAllInteractingMessages();
                    _Player.CanJump = false;
                    _Player.CanSprint = false;
                    if (_Player.transform.localScale.x > 0f)
                    {
                        PushDir = 1f;
                        _Player.CanGoLeft = false;
                    }
                    else
                    {
                        PushDir = -1f;
                        _Player.CanGoRight = false;
                    }
                    if (PressedInteractPush == false)
                    {
                        _AudioManager?.PlayPushableBoxSound(GetComponent<AudioSource>());
                        PressedInteractPush = true;
                    }
                    _Player.MyAnimator.SetBool("Push", true);
                }
                else
                {
                    _AudioManager?.StopPushableBoxSound(GetComponent<AudioSource>());
                    PressedInteractPush = false;
                    _Player.MyAnimator.SetBool("Push", false);
                    EnablePlayerMovementsOnPushDisabled();
                }
            }
            else
            {
                _AudioManager?.StopPushableBoxSound(GetComponent<AudioSource>());
                _UiManager.HideInteractingPushMessage();
            }
        }
        else
        {
            _AudioManager?.StopPushableBoxSound(GetComponent<AudioSource>());
            PressedInteractPush = false;
            _Player.MyAnimator.SetBool("Push", false);
            _UiManager.HideInteractingPushMessage();
            EnablePlayerMovementsOnPushDisabled();
        }
    }
    private void MoveBox()
    {
        Vector2 zeroVec = Vector2.zero;
        _MyBoxBody.velocity = Vector2.SmoothDamp(_MyBoxBody.velocity, new Vector2(PushDir * PushForce, _MyBoxBody.velocity.y), ref zeroVec, Time.fixedDeltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, Surface.size + new Vector2(3.9f, 0f));
    }

    private void EnablePlayerMovementsOnPushDisabled()
    {
        _Player.CanGoRight = true;
        _Player.CanGoLeft = true;
        _Player.CanSprint = true;
        _Player.CanJump = true;
    }

    private bool CanPush(Collider2D Collider)
    {
        return (Collider.transform.position.x < transform.position.x && Collider.transform.localScale.x > 0)
                                                                     ||
               (Collider.transform.position.x > transform.position.x && Collider.transform.localScale.x < 0);
    }
    private void OnBecameVisible()
    {
        enabled = true;
    }
    private void OnBecameInvisible()
    {
        enabled = false;
    }
}