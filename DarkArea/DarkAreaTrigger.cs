using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/**
 * Class For Controling the behaviour
 * of dark area place opening
 */
public class DarkAreaTrigger : MonoBehaviour
{
    public GameObject SurfaceRemove;

    public GameObject _KnifeObject;
    public bool DarkAreaOpened = false;

    private GameObject LockedTriggerMessage;

    private UnityEvent OnCutsceneFinishedEvent;
    private SmoothCamera2D CurrentCamera;
    public UIManager _UiManager;

    public GameQuests _GameQuestsManager;
    private UnityEvent OnDarkAreaTriggerOpenedEvent;

    private bool _StopCoroutine = false;


    private void Awake()
    {
        CurrentCamera = Camera.main.GetComponent<SmoothCamera2D>();
        LockedTriggerMessage = transform.GetChild(0).gameObject;

        if (_GameQuestsManager == null)
            GameObject.FindGameObjectWithTag("GameQuest").TryGetComponent(out _GameQuestsManager);
        if (_UiManager == null)
            GameObject.FindGameObjectWithTag("UiSystem").TryGetComponent(out _UiManager);

        OnCutsceneFinishedEvent = new UnityEvent();
        OnCutsceneFinishedEvent.AddListener(new UnityAction(() => CurrentCamera.AllCutscenesFinishedOrSkipped()));
        OnCutsceneFinishedEvent.AddListener(new UnityAction(() => _UiManager.HideSkipCutscene()));


        OnDarkAreaTriggerOpenedEvent = new UnityEvent();
        OnDarkAreaTriggerOpenedEvent.AddListener(() => _GameQuestsManager.OnDarkAreaOpened());
    }
    public void OpenDarkArea(bool FromLoadedGame = false)
    {
        HideDarkAreaTriggerAnim();
        if (SurfaceRemove.name == "SurfaceRemover")
            StartCoroutine(ChangingSurfaceOnCutscene(FromLoadedGame));
        else
            StartCoroutine(MovingGateCutscene(FromLoadedGame));
    }
    private IEnumerator ChangingSurfaceOnCutscene(bool FromLoadedGame)
    {
        if (!FromLoadedGame)
        {
            yield return new WaitUntil(() => CurrentCamera.ReachedCutsceneDestination());
            while (SurfaceRemove.transform.position.y > -36f)
            {
                SurfaceRemove.transform.Translate(new Vector3(0f, -0.1f, 0f));
                yield return new WaitForSeconds(0.02f);
                if (_StopCoroutine)
                    yield break;
            }
            while (SurfaceRemove.transform.position.x > 39.4f)
            {
                SurfaceRemove.transform.Translate(new Vector3(-0.1f, 0f, 0f));
                yield return new WaitForSeconds(0.02f);
                if (_StopCoroutine)
                    yield break;
            }
            OnCutsceneFinishedEvent.Invoke();
        }
        ChangingSurfaceFinished();
        TriggerCutsceneFinishedOrSkipped();
        yield return null;
    }
    private IEnumerator MovingGateCutscene(bool FromLoadedGame)
    {
        if (!FromLoadedGame)
        {
            yield return new WaitUntil(() => CurrentCamera.ReachedCutsceneDestination());
            while (SurfaceRemove.transform.localScale.x >= 20f)
            {
                SurfaceRemove.transform.position += new Vector3(0f, 0.05f, 0f);
                SurfaceRemove.transform.localScale -= new Vector3(0.05f, 0f, 0f);
                yield return null;
                if (_StopCoroutine)
                    yield break;
            }
            OnCutsceneFinishedEvent.Invoke();
        }
        else
        {
            AfterGateMoved();
        }
        TriggerCutsceneFinishedOrSkipped();
        yield return null;
    }

    private void AfterGateMoved()
    {
        SurfaceRemove.transform.localScale = new Vector3(20f, SurfaceRemove.transform.localScale.y, SurfaceRemove.transform.localScale.z);
        SurfaceRemove.transform.position = new Vector3(SurfaceRemove.transform.position.x, -29f, SurfaceRemove.transform.position.z);
    }
    private void ChangingSurfaceFinished()
    {
        _KnifeObject?.SetActive(true);
        Destroy(SurfaceRemove, 10f);
    }

    private void TriggerCutsceneFinishedOrSkipped()
    {
        DarkAreaOpened = true;
        OnDarkAreaTriggerOpenedEvent.Invoke();
    }

    public void SkipTriggerCutsceneRoutine()
    {
        StopAllCoroutines();
        _StopCoroutine = true;
        OnCutsceneFinishedEvent.Invoke();
        TriggerCutsceneFinishedOrSkipped();
        if (SurfaceRemove.name == "SurfaceRemover")
            ChangingSurfaceFinished();
        else
            AfterGateMoved();
    }
    public void ShowDarkAreaTrigger()
    {
        GetComponent<Animator>().enabled = true;
    }

    private void HideDarkAreaTriggerAnim()
    {
        GetComponent<Animator>().enabled = false;
    }

    public void ShowLockedTriggerMessage()
    {
        LockedTriggerMessage.SetActive(true);
    }

    public void HideLockedTriggerMessage()
    {
        LockedTriggerMessage.SetActive(false);
    }
}