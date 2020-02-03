using UnityEngine;
using UnityEngine.Events;

/**
 * Player Movement Controller , Physics Behaviour , Etc..
 */
public class CharacterController2D : MonoBehaviour
{
    public float m_JumpForce = 1000f;                          // Amount of force added when the player jumps.
    [Range(0, 1)] public float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)] public float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    public bool m_AirControl = true;                         // Whether or not a player can steer while jumping;
    public LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    public Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    public Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    public Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

    private Animator MyAnimator;
    public float k_GroundedRadius = 0.8f; // Radius of the overlap circle to determine if grounded
    public bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    public Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    private bool IsFalling = false;

    private float FallingYPosition = float.NaN;

    private CapsuleCollider2D BodyCollider;

    private Player _Player;
    private BossBehaviour Boss;


    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;

    private void Awake()
    {
        MyAnimator = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (TryGetComponent(out _Player))
        {
            OnLandEvent = new UnityEvent();
            OnLandEvent.AddListener(() => _Player.OnLanding(FallingYPosition));
        }
        else if (TryGetComponent(out Boss))
        {
            OnLandEvent.AddListener(() => Boss.OnLanding());
        }
        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();

        BodyCollider = GetComponent<CapsuleCollider2D>();
    }
    private void Update()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;
        MyAnimator.SetBool("Grounded", false);

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; ++i)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                MyAnimator.SetBool("Grounded", true);
                if (!wasGrounded)
                {
                    OnLandEvent.Invoke();
                    IsFalling = false;
                    FallingYPosition = float.NaN;
                    BodyCollider.size = new Vector2(BodyCollider.size.x, 1.75f);
                    BodyCollider.offset = new Vector2(BodyCollider.offset.x, 0.75f);
                }
            }
        }
        if (!IsFalling)
        {
            if (m_Grounded == false)//Falling
            {
                IsFalling = true;
                FallingYPosition = transform.position.y;
                BodyCollider.size = new Vector2(BodyCollider.size.x, 1f);
                BodyCollider.offset = new Vector2(BodyCollider.offset.x, 1f);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(m_GroundCheck.position, k_GroundedRadius);
    }

    public void Move(float move, bool crouch, bool jump)
    {
        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
            // If crouching
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier
                move *= m_CrouchSpeed;

                // Disable one of the colliders when crouching
                m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                m_CrouchDisableCollider.enabled = true;
                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        // If the player should jump...
        if (m_Grounded && jump)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }
    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}