using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/*Final Treasure In-Game Handling, Ending Game*/
public class Artifact : MonoBehaviour
{
    private readonly float RotateZ = 100.0f;

    public Transform ArtifactView;

    private int PlayerMask;

    private bool OpenedArtifact = false;
    private bool PlayerInArtifactView = false;

    private Player _Player;

    public UIManager _UIManager;

    public GameQuests _GameQuestsManager;
    private UnityEvent OnArtifactOpened;

    private void Awake()
    {
        PlayerMask = LayerMask.GetMask("Player");

        if (_UIManager == null)
            GameObject.FindGameObjectWithTag("UiSystem")?.TryGetComponent(out _UIManager);

        if (_GameQuestsManager == null)
            GameObject.FindGameObjectWithTag("GameQuest")?.TryGetComponent(out _GameQuestsManager);
        OnArtifactOpened = new UnityEvent();
        OnArtifactOpened.AddListener(() => _GameQuestsManager.OnArtifactOpened());
    }

    private void Start()
    {
        if (!GetComponent<Renderer>().isVisible)
            enabled = false;
    }
    private void Update()
    {
        if (!OpenedArtifact)
        {
            if (PlayerNearArtifact(out _Player))
            {
                PlayerInArtifactView = true;
                _Player.GetInteractingMessage(_Player.transform.GetChild(3).gameObject);
                _UIManager.ShowInteractingEventMessage();
                if (Input.GetKeyDown(_Player._Controls.Interact))
                {
                    StartCoroutine(OpeningArtifact());
                    OpenedArtifact = true;
                    _UIManager.HideInteractingEventMessage();
                    OnArtifactOpened.Invoke();
                }
            }
            else
            {
                if (PlayerInArtifactView)
                {
                    _UIManager.HideInteractingEventMessage();
                    PlayerInArtifactView = false;
                }
            }
        }
        if (OpenedArtifact)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, RotateZ * Time.time);
        }
    }
    private IEnumerator OpeningArtifact()
    {
        yield return new WaitForSeconds(10.0f);
        int CreditsSceneIndex = 9;
        yield return SceneManager.LoadSceneAsync(CreditsSceneIndex);
    }

    private bool PlayerNearArtifact(out Player _Player)
    {
        _Player = Physics2D.OverlapBox(ArtifactView.position, new Vector2(9f, 10f), 0f, PlayerMask)?.gameObject.GetComponent<Player>();
        return _Player != null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawCube(ArtifactView.position, new Vector3(9f, 10f, 0f));
    }
    private void OnBecameVisible()
    {
        enabled = true;
    }
}
