using UnityEngine;

public class SwitchToggle : MonoBehaviour
{
    public MovingSurface _MovingSurface;

    public Sprite[] SignsSprites;
    public Sprite[] SwitchSprites;
    [Header("Direction : Up=0, Idle=1, Down=2 ")]
    public int _CurrentDirection = 1;

    private Player CurrentPlayer;
    private GameObject PlayerInteractingMessage;
    private GameObject PlayerInteractingMessagePushMessage;
    private bool IsIncrementing = true;//False= Decrementing
    private SpriteRenderer MyRenderer;
    public SpriteRenderer SignRenderer;

    private bool IsInteractHidden;

    public AudioManager _AudioManager;

    public AudioSource MyAudioSource;
    private void Awake()
    {
        if (_MovingSurface == null)
            _MovingSurface.transform.parent.gameObject.GetComponent<MovingSurface>();
        MyRenderer = GetComponent<SpriteRenderer>();

        if (_AudioManager == null)
            GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);

        MyAudioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (_MovingSurface.PlayerOnSurface)
        {
            if (CurrentPlayer != null)
            {
                if (!PlayerInteractingMessagePushMessage.activeSelf)
                {
                    IsInteractHidden = false;
                    CurrentPlayer.GetInteractingMessage(PlayerInteractingMessage);
                    CurrentPlayer._UiManager.ShowInteractingEventMessage();
                    if (Input.GetKeyDown(CurrentPlayer._Controls.Interact))
                    {
                        if (IsIncrementing)
                            _CurrentDirection++;
                        else
                            _CurrentDirection--;
                        if (_CurrentDirection == 2)
                            IsIncrementing = false;
                        else if (_CurrentDirection == 0)
                            IsIncrementing = true;
                        PlaySwitchToggle();
                        ChangeSprites();
                    }
                }
                else
                {
                    if (!IsInteractHidden)
                    {
                        CurrentPlayer._UiManager.HideInteractingEventMessage();
                        IsInteractHidden = true;
                    }
                }
            }
        }
    }
    public void ChangeSprites()
    {
        MyRenderer.sprite = SwitchSprites[_CurrentDirection];
        SignRenderer.sprite = SignsSprites[_CurrentDirection];
    }

    public void PlayerOnSurface(Player _CurrentPlayer)
    {
        CurrentPlayer = _CurrentPlayer;
        PlayerInteractingMessage = CurrentPlayer.transform.GetChild(3).gameObject;
        PlayerInteractingMessagePushMessage = CurrentPlayer.transform.GetChild(4).gameObject;
    }
    public void IdleState()
    {
        _CurrentDirection = 1;//Idle
        ChangeSprites();
    }

    public void SwitchOffState()
    {
        if (CurrentPlayer != null)
            CurrentPlayer._UiManager.HideInteractingEventMessage();
    }
    public void PlaySwitchToggle()
    {
        _AudioManager.PlaySwitchToggle(MyAudioSource);
    }
}