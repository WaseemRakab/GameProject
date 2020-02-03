using UnityEngine;
using UnityEngine.Events;

/**
 * Main Camera In-Game Controller , Following Player
 * , Cutscenes Etc..
 */
public class SmoothCamera2D : MonoBehaviour
{
    public float PlayerDampTime { get; private set; } = 0.15f;
    public float CutsceneDampTime = 3f;

    private Vector3 velocity = Vector3.zero;

    public Transform currentTarget;

    private Transform playerTarget;

    public GameManager _GameManager;


    private readonly float ShowDownTime = 1.0f;
    private float CanShowDown = 1.0f;

    public GameObject[] CutscenesTargets;

    public Vector3 Delta;


    private void Awake()
    {
        playerTarget = currentTarget;
        if (_GameManager == null)
            GameObject.FindGameObjectWithTag("GameManager")?.TryGetComponent(out _GameManager);
    }

    private void LateUpdate()
    {
        if (currentTarget)
        {
            Vector3 point = GetComponent<Camera>().WorldToViewportPoint(currentTarget.position);
            Delta = currentTarget.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.25f, point.z));
            Vector3 destination = transform.position + Delta;
            if (TargetIsPlayer())
            {
                TrackingPlayer(destination);
            }
            else //Cutscene
            {
                ViewCutscene(destination);
            }
        }
    }
    public void AllCutscenesFinishedOrSkipped()
    {
        currentTarget = playerTarget;
        _GameManager.InCutscene = false;
    }

    private bool TargetIsPlayer()
    {
        return currentTarget.CompareTag("Player");
    }
    private void TrackingPlayer(Vector3 destination)
    {
        if (!currentTarget.gameObject.GetComponent<Player>().isCrouching)
        {
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, PlayerDampTime);
            CanShowDown = Time.time + ShowDownTime;
        }
        else
        {
            if (Time.time > CanShowDown)
                transform.position = Vector3.SmoothDamp(transform.position, destination - new Vector3(0f, 10f, 0f), ref velocity, PlayerDampTime);
            else
                transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, PlayerDampTime);
        }
    }
    public void OnChangingCutsceneView(UnityAction whichCutscene)
    {
        whichCutscene.Invoke();
    }
    public void SecretChestCutscene()
    {
        if (CutscenesTargets[0] != null)
            currentTarget = CutscenesTargets[0].transform;
    }
    public void FinishDoorCutscene()
    {
        currentTarget = CutscenesTargets[1].transform;
    }
    public void DarkAreaTriggerCutscene()
    {
        currentTarget = CutscenesTargets[2].transform;
    }
    public void FinalBossKilledCutscene()
    {
        currentTarget = CutscenesTargets[3].transform;
    }
    private void ViewCutscene(Vector3 destination)
    {
        transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, CutsceneDampTime);
    }
    public bool ReachedCutsceneDestination()
    {
        return ((Delta.x >= 0f && Delta.x <= 1f) || (Delta.x <= 0f && Delta.x >= -1f))
                                                 &&
               ((Delta.y >= 0f && Delta.y <= 1f) || (Delta.y <= 0f && Delta.y >= -1f));
    }
}