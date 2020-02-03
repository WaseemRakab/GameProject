using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/**
 * Controling Events in-Game
 * 
 */
public class EventTriggers : MonoBehaviour
{
    private SmoothCamera2D CurrentCamera;

    private GameObject _FinishDoor;
    private GameObject _LockedDoorCanvasMessage;
    public GameObject _KeyUI;

    private UIManager _UIManager;
    private Player LaraPlayer;

    public ScriptableDataUI _DataUI;
    public LevelRestartedOrNextLevel _OnLevelRestarted;

    private DarkAreaTrigger _DarkAreaTrigger;

    private UnityEvent OnCutsceneFinishedEvent;

    public GameQuests _GameQuestsManager;
    private UnityEvent OnSecretChestOpenedEvent;

    private void Awake()
    {
        CurrentCamera = Camera.main.GetComponent<SmoothCamera2D>();

        if (_UIManager == null)
        {
            GameObject.FindGameObjectWithTag("UiSystem").TryGetComponent(out _UIManager);
            LaraPlayer = _UIManager.LaraPlayer;
        }
        _FinishDoor = GameObject.FindGameObjectWithTag("FinishDoor");
        GameObject.FindGameObjectWithTag("DarkAreaTrigger")?.TryGetComponent(out _DarkAreaTrigger);

        if (_GameQuestsManager == null)
            GameObject.FindGameObjectWithTag("GameQuest").TryGetComponent(out _GameQuestsManager);

        OnCutsceneFinishedEvent = new UnityEvent();
        OnCutsceneFinishedEvent.AddListener(new UnityAction(() => CurrentCamera.AllCutscenesFinishedOrSkipped()));
        OnCutsceneFinishedEvent.AddListener(new UnityAction(() => _UIManager.HideSkipCutscene()));

        OnSecretChestOpenedEvent = new UnityEvent();
        OnSecretChestOpenedEvent.AddListener(() => _GameQuestsManager.OnSecretChestOpened());

        _LockedDoorCanvasMessage = _FinishDoor.transform.GetChild(0).gameObject;
        if (_KeyUI == null)
            _KeyUI = _UIManager.transform.GetChild(2).gameObject;
        RefreshDataUI(); // Events Refresh on SaveGame
    }

    private void RefreshDataUI()
    {
        if (_DataUI._DataUI.SecretChestOpened == true)
        {
            OpenFinishDoor();
            HideLockedDoorMessage();
            ShowKeyUI();
            LaraPlayer.HasBeenInSecretChest = true;

            _DarkAreaTrigger?.ShowDarkAreaTrigger();
            //For Next level,Restart
            _DataUI._DataUI.SecretChestOpened = false;
        }
        if (_DataUI._DataUI.OpenedDarkArea == true)
        {
            _DarkAreaTrigger?.OpenDarkArea(true);
            LaraPlayer.HasBeenInDarkAreaTrigger = true;
            _DataUI._DataUI.OpenedDarkArea = false;
        }
    }
    public bool IsDoorUnlocked { get; private set; } = false;

    /*the key was taken open the finish level door*/
    public void OpenFinishDoor()
    {
        IsDoorUnlocked = true;
        _UIManager.UnlockFinishDoor(_FinishDoor);
    }

    /*the player doesnt have the key show message*/
    public void ShowLockedDoorMessage()
    {
        _UIManager.ShowLockedMessage(_LockedDoorCanvasMessage);
    }

    /*hide message*/
    public void HideLockedDoorMessage()
    {
        _UIManager.HideLockedMessage(_LockedDoorCanvasMessage);
    }

    /*shows the player in UI that he has a key*/
    public void ShowKeyUI()
    {
        _KeyUI.GetComponent<Image>().material = null;
        transform.GetChild(0).gameObject.SetActive(true);//KeyOn
        OnSecretChestOpenedEvent.Invoke();
    }

    public void OnSecretChestCutscene()
    {
        StartCoroutine(SecretChestRoutineCutscene());
    }
    private IEnumerator SecretChestRoutineCutscene()
    {
        if (_DarkAreaTrigger != null)
        {
            //When the Camera reached the Destination cutscene
            yield return new WaitForSeconds(CurrentCamera.CutsceneDampTime);
            CurrentCamera.OnChangingCutsceneView(() => CurrentCamera.SecretChestCutscene());
            yield return new WaitForSeconds(2.0f);
            yield return new WaitUntil(() => CurrentCamera.ReachedCutsceneDestination());
            OnAreaTriggerCutsceneFinished();
        }
        yield return null;
        yield return new WaitForSeconds(CurrentCamera.CutsceneDampTime);
        CurrentCamera.OnChangingCutsceneView(() => CurrentCamera.FinishDoorCutscene());
        yield return new WaitForSeconds(2.0f);
        yield return new WaitUntil(() => CurrentCamera.ReachedCutsceneDestination());
        OnFinishDoorCutsceneFinished();
        yield return new WaitForSeconds(2.0f);
        OnCutsceneFinishedEvent.Invoke();
    }
    private void OnFinishDoorCutsceneFinished()
    {
        OpenFinishDoor();
        HideLockedDoorMessage();
    }
    private void OnAreaTriggerCutsceneFinished()
    {
        GoingToDarkAreaTriggerCutscene();
    }
    private void GoingToDarkAreaTriggerCutscene()
    {
        _DarkAreaTrigger.ShowDarkAreaTrigger();
    }
    public void SkipSecretChestRoutineCutscene()
    {
        StopAllCoroutines();
        OnCutsceneFinishedEvent.Invoke();
        OnFinishDoorCutsceneFinished();
        if (_DarkAreaTrigger != null)
            OnAreaTriggerCutsceneFinished();
    }
    public IEnumerator AfterCutscene(UnityAction whichMethod)
    {
        yield return new WaitForSeconds(CurrentCamera.CutsceneDampTime);
        whichMethod.Invoke();
    }
}