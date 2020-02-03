using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/**
 * Controling Game on When Finish The Level
 */
public class FinishLevel : MonoBehaviour
{
    public bool LevelFinished = false;

    public LevelRestartedOrNextLevel _OnFinishingLevel;

    public bool HasEventTriggers;

    public UIManager _UiManager;
    public GameManager _GameManager;

    public UnityEvent OnFinishingLevelEvent;

    private void Awake()
    {
        if (_UiManager == null)
            GameObject.FindGameObjectWithTag("UiSystem")?.TryGetComponent(out _UiManager);
    }

    /*finish level*/
    public void FinishingLevel(float TimeToWait, bool BossLevel = false)
    {
        OnFinishingLevelEvent?.Invoke();
        StartCoroutine(FinishingLevelRoutine(TimeToWait, BossLevel));
    }
    private IEnumerator FinishingLevelRoutine(float TimeToWait, bool BossLevel = false)
    {
        yield return new WaitForSeconds(TimeToWait);
        _GameManager.GameOver = true;
        if (BossLevel == true)
        {
            _OnFinishingLevel.OnStageFinished = true;
        }
        else
            _OnFinishingLevel.OnLevelFinished = true;
        _UiManager.LevelFinished();
        _UiManager.GameOver();
    }
}