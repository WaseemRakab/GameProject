using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/*Handling BossGate Cutscene*/
public class FinalBossGate : MonoBehaviour
{
    private SmoothCamera2D _CurrentCamera;
    private UnityEvent OnCutsceneFinishedEvent;

    public UIManager _UIManager;

    private void Awake()
    {
        _CurrentCamera = Camera.main.GetComponent<SmoothCamera2D>();
        if (_UIManager == null)
            GameObject.FindGameObjectWithTag("UiSystem")?.TryGetComponent(out _UIManager);

        OnCutsceneFinishedEvent = new UnityEvent();
        OnCutsceneFinishedEvent.AddListener(() => _CurrentCamera.AllCutscenesFinishedOrSkipped());
        OnCutsceneFinishedEvent.AddListener(() => _UIManager.HideSkipCutscene());
    }
    public void OnGateOpeningCutscene()
    {
        OnSkippingBossGateCutscene();
        StartCoroutine(AfterCutscene(() => StartCoroutine(OpeningGateScene())));
    }
    private void OnSkippingBossGateCutscene()
    {
        OnSkippingCutscenes.WhichCutsceneCanBeSkipped = new UnityAction(() => SkipBossGateCutscene());
        _UIManager.ShowSkipCutscene();
    }

    private IEnumerator AfterCutscene(UnityAction whichMethod)
    {
        yield return new WaitForSeconds(1.5f);
        _CurrentCamera.OnChangingCutsceneView(() => _CurrentCamera.FinalBossKilledCutscene());
        yield return new WaitForSeconds(_CurrentCamera.CutsceneDampTime);
        whichMethod.Invoke();
    }
    private IEnumerator OpeningGateScene()
    {
        yield return new WaitUntil(() => _CurrentCamera.ReachedCutsceneDestination());
        while (transform.localScale.x >= 2.0f)
        {
            transform.position -= new Vector3(0f, 0.05f, 0f);
            transform.localScale -= new Vector3(0.05f, 0f, 0f);
            yield return null;
        }
        OnCutsceneFinishedEvent.Invoke();
        Destroy(gameObject);
    }
    public void SkipBossGateCutscene()
    {
        StopAllCoroutines();
        OnCutsceneFinishedEvent.Invoke();
        Destroy(gameObject);
    }
}