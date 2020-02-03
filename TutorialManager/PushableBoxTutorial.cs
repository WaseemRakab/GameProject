using UnityEngine;

public class PushableBoxTutorial : MonoBehaviour
{
    private Rigidbody2D _MyBoxBody;
    public PlayerTutorial _Player;

    public AudioManager _AudioManager;

    public bool PressedInteractPush = false;

    private float PushDir;
    private readonly float PushForce = 5f;

    private void Awake()
    {
        _MyBoxBody = GetComponent<Rigidbody2D>();
        if (_AudioManager == null)
            GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);
        if (_Player == null)
            GameObject.FindGameObjectWithTag("Player")?.TryGetComponent(out _Player);
    }
    private void Start()
    {
        if (!GetComponent<Renderer>().isVisible)
            enabled = false;
    }
    private void FixedUpdate()
    {
        if (PressedInteractPush)
        {
            MoveBox();
        }
    }
    private void MoveBox()
    {
        Vector2 zeroVec = Vector2.zero;
        _MyBoxBody.velocity = Vector2.SmoothDamp(_MyBoxBody.velocity, new Vector2(PushDir * PushForce, _MyBoxBody.velocity.y), ref zeroVec, Time.fixedDeltaTime);
    }
    private void Update()
    {
        Collider2D Collider = Physics2D.OverlapBox(transform.position, GetComponent<CapsuleCollider2D>().size + new Vector2(3.9f, 0f), 0, LayerMask.GetMask("Player"));
        if (Collider != null)
        {
            if (CanPush(Collider))
            {
                if (Input.GetKey(_Player._Controls.Interact))
                {
                    _Player.CanJump = false;
                    _Player.CanSprint = false;
                    if (_Player.transform.localScale.x > 0f)
                    {
                        PushDir = 1f;
                    }
                    else
                    {
                        PushDir = -1f;
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
            }
        }
        else
        {
            _AudioManager?.StopPushableBoxSound(GetComponent<AudioSource>());
            PressedInteractPush = false;
            _Player.MyAnimator.SetBool("Push", false);
            EnablePlayerMovementsOnPushDisabled();
        }
    }

    private bool CanPush(Collider2D Collider)
    {
        return (Collider.transform.position.x < transform.position.x && Collider.transform.localScale.x > 0)
                                                                     ||
               (Collider.transform.position.x > transform.position.x && Collider.transform.localScale.x < 0);
    }
    private void EnablePlayerMovementsOnPushDisabled()
    {
        _Player.CanSprint = true;
        _Player.CanJump = true;
    }
    private void OnBecameVisible()
    {
        enabled = true;
    }
}