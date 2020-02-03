using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/**
 * GameManager , Handling TimeInGame , OnGamePaused,InA Cutscene , And More..
 */
public class GameManager : MonoBehaviour
{
    public FinishLevel _FinishLevel;
    public GameMenuScript _GameMenu;
    public UIManager _UiManager;
    public GameQuests _GameQuestsManager;

    public LevelRestartedOrNextLevel _OnLevelRestartedOrNextLevel;
    public ScriptablePlayerStats _PlayerStats;
    public ScriptableWeapon _WeaponData;
    public ScriptableDataUI _DataUI;
    public ScriptablePlayTime _PlayTime;

    public bool GameOver = false;
    public bool GamePaused = false;
    public bool InCutscene = false;

    private float StartTime;
    private float PlayTimerDuration;

    private int CurrentLevelIndex;

    private UnityEvent OnEnemyKilledEvent;

    private bool CursorHidden = false;

    private void Awake()
    {
        if (_UiManager == null)
            GameObject.FindGameObjectWithTag("UiSystem").TryGetComponent(out _UiManager);
        if (_GameMenu == null)
            GameObject.FindGameObjectWithTag("GameMenu").TryGetComponent(out _GameMenu);
        if (_GameQuestsManager == null)
            GameObject.FindGameObjectWithTag("GameQuest").TryGetComponent(out _GameQuestsManager);

        OnLevelRestartedOrNextLevel();

        OnEnemyKilledEvent = new UnityEvent();
        OnEnemyKilledEvent.AddListener(() => _GameQuestsManager.OnEnemyKilled());
    }

    /*get the current level we are in
     save the time in starttime*/
    private void Start()
    {
        CurrentLevelIndex = SceneManager.GetActiveScene().buildIndex - 2;
        StartTime = Time.time;

        OnGameStarted();
    }

    private void OnGameStarted()
    {
        _OnLevelRestartedOrNextLevel.OnLevelRestarted = false;
        _OnLevelRestartedOrNextLevel.OnNextLevel = false;
        _OnLevelRestartedOrNextLevel.OnLevelFinished = false;
        _OnLevelRestartedOrNextLevel.OnStageFinished = false;
    }

    private void Update()
    {
        if (!GameOver)
        {
            if (!InCutscene)
            {
                if (!GamePaused)
                {
                    PlayTimerDuration = Time.time - StartTime;
                    _PlayTime._PlayTime.LevelsPlayTime[CurrentLevelIndex] = PlayTimerDuration;
                }
                if (Input.GetKeyDown(KeyCode.Escape)) //Game Menu
                {
                    _GameMenu.ShowGameMenu();
                    PauseGame();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    OnSkippingCutscenes.WhichCutsceneCanBeSkipped();
                    InCutscene = false;
                }
            }
        }
        else
        {
            if (_FinishLevel != null)
            {
                if (_FinishLevel.LevelFinished == true)
                {
                    _PlayTime._PlayTime.LevelsPlayTime[CurrentLevelIndex] = 0f;
                }
            }
        }
        if (MouseMoved())
        {
            StopAllCoroutines();
            Cursor.visible = true;
            CursorHidden = false;
        }
        else
        {
            if (!CursorHidden)
            {
                if (Time.timeScale == 0f)
                    Cursor.visible = true;
                else
                    StartCoroutine(HideMouseCursorRoutine());
            }
        }
    }

    private bool MouseMoved()
    {
        return (Input.GetAxis("Mouse X") != 0f) || (Input.GetAxis("Mouse Y") != 0f);
    }

    private IEnumerator HideMouseCursorRoutine()
    {
        CursorHidden = true;
        yield return new WaitForSeconds(5.0f);
        Cursor.visible = false;
    }
    public IEnumerator ShowAnimation()
    {
        yield return new WaitForSeconds(4.0f);
        _UiManager.GameOver();
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
        GamePaused = true;
    }
    public void ResumeGame()
    {
        if (GameOver == false)
        {
            Time.timeScale = 1;
            GamePaused = false;
        }
    }
    /*count enemy and turret kills seperate*/
    public void SetTotalKills(object WhichGameObject)
    {
        if (WhichGameObject is EnemyAutoGunWOBunkerBehaviour
            || WhichGameObject is EnemyAutoGunWBunkerBehaviour
            || WhichGameObject is EnemyPistolBehaviour)
        {
            _PlayerStats._PlayerStats.TotalEnemyKills++;
        }
        else if (WhichGameObject is TurretLauncher)
        {
            _PlayerStats._PlayerStats.TotalTurretKills++;
        }
        _PlayerStats._PlayerStats.TotalKills = _PlayerStats._PlayerStats.TotalTurretKills
            + _PlayerStats._PlayerStats.TotalEnemyKills;
    }
    private void OnLevelRestartedOrNextLevel()
    {
        if (_OnLevelRestartedOrNextLevel.OnLevelRestarted == true)
        {
            ResetPreviousLevelPlayTime();
            ResetDataUI();
            RestorePreviousPlayerStats();
            RestorePreviousWeaponStats();
        }
        else if (_OnLevelRestartedOrNextLevel.OnNextLevel == true)
        {
            ResetPreviousLevelPlayTime();
            ResetDataUI();
            BackUpPlayerStats();
            BackUpWeaponStats();
        }
    }
    private void ResetDataUI()
    {
        _DataUI._DataUI.SecretChestOpened = false;
        _DataUI._DataUI.OpenedDarkArea = false;
    }
    private void ResetPreviousLevelPlayTime()
    {
        _PlayTime._PlayTime.PreviousLevelPlayTime = 0f;
    }
    private void BackUpPlayerStats()//Next Level
    {
        _PlayerStats._PlayerStats.PreviousStats = new PlayerStats.PreviousPlayerStats(_PlayerStats._PlayerStats);
    }
    private void RestorePreviousPlayerStats()//Restart Level
    {
        _PlayerStats._PlayerStats = new PlayerStats(_PlayerStats._PlayerStats.PreviousStats);
    }
    private void BackUpWeaponStats()
    {
        _WeaponData._Weapon.PreviousStats = new Weapon.PreviousWeaponStats(_WeaponData._Weapon);
    }
    private void RestorePreviousWeaponStats()
    {
        _WeaponData._Weapon = new Weapon(_WeaponData._Weapon.PreviousStats);
    }
    public void EnemyKilled()
    {
        OnEnemyKilledEvent.Invoke();
    }
}