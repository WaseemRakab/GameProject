using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * On Game Ended Menu - Handling UI, Button Presses Game Stats, Etc..
 */
public class GameOver : MonoBehaviour
{
    private int CurrentLevelIndex;

    public UIManager _UIManager;
    public AudioManager _AudioManager;

    public LevelRestartedOrNextLevel _OnLevelRestartedOrNextLevel;

    public ScriptablePlayerStats _PlayerStats;
    public ScriptableWeapon _WeaponData;
    public ScriptablePlayTime _PlayTime;
    public ScriptableDataUI _DataUI;
    public ScriptableEnemyStats _EnemyStats;
    public ScriptableSpawnObjects _SpawnObjects;

    private SaveGame _LastSavedGame;
    public GameObject RestartFromLastSaveGame;

    public TMP_Text TotalPlayTimeValue;
    public TMP_Text LevelPlayTimeValue;
    public TMP_Text TotalKills;
    public TMP_Text TotalEnemyKills;
    public TMP_Text TotalTurretKills;
    public TMP_Text TotalWeaponsFired;
    public TMP_Text PlayerCash;

    public GameObject[] GameOverUiTitles;//GameOver=[0](First Active), Level Finished=[1],Stage Complete=[2]
    public GameObject[] NextLevelStageButtons;//NextLevel=[0](First Active), NextStage=[1]

    private Button.ButtonClickedEvent onClick;
    private void Awake()
    {
        if (_UIManager == null)
            _UIManager = GameObject.FindGameObjectWithTag("UiSystem").GetComponent<UIManager>();
        if (_AudioManager == null)
            GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);
        onClick = new Button.ButtonClickedEvent();
        onClick.AddListener(new UnityAction(() => _AudioManager.ClickSound()));
    }
    private void OnEnable()
    {
        _AudioManager?.PauseAudioOnPauseGame();
        CheckLastUsedSaveGame();
        CurrentLevelIndex = SceneManager.GetActiveScene().buildIndex - 2; //First Level Starts At Index 2
        OnFinishingLevelOrStage();
        SetTotalPlayTime();
        SetPlayTimeInGameOverUI();
        SetPlayerStatsInGameOverUI();
        TotalWeaponsFired.text = _WeaponData._Weapon.TotalWeaponFired.ToString();
    }

    private void OnDisable()
    {
        _AudioManager?.UnPauseAudioOnResumeGame();
    }

    private void OnFinishingLevelOrStage()
    {
        if (_OnLevelRestartedOrNextLevel.OnStageFinished)
        {
            NextLevelStageButtons[0].SetActive(false);
            NextLevelStageButtons[1].SetActive(true);
            GameOverUiTitles[0].SetActive(false);
            GameOverUiTitles[2].SetActive(true);
        }
        else if (_OnLevelRestartedOrNextLevel.OnLevelFinished)
        {
            GameOverUiTitles[0].SetActive(false);
            GameOverUiTitles[1].SetActive(true);
            _AudioManager?.PlayFinishedLevelSound();
        }
        else
        {
            _AudioManager?.PlayGameOverSoundOnDying();
        }
    }
    private void CheckLastUsedSaveGame()
    {
        _LastSavedGame = SaveGameManager.LoadRecentGame();
        if (_LastSavedGame != null)
        {
            RestartFromLastSaveGame.GetComponent<Button>().interactable = true;
            RestartFromLastSaveGame.GetComponent<Image>().color = Color.white;
        }
    }

    private string ParseTimeToString(float Duration)
    {
        int FlooredDuration = Mathf.FloorToInt(Duration);
        int Hours = FlooredDuration / 3600;
        int Minutes = FlooredDuration / 60 % 60;
        int Seconds = FlooredDuration % 60;
        return string.Format("{0:00}:{1:00}:{2:00}", Hours, Minutes, Seconds);
    }
    private void SetPlayerStatsInGameOverUI()
    {
        PlayerCash.text = _PlayerStats._PlayerStats.MyCash + " $";
        TotalKills.text = _PlayerStats._PlayerStats.TotalKills.ToString();
        TotalEnemyKills.text = _PlayerStats._PlayerStats.TotalEnemyKills.ToString();
        TotalTurretKills.text = _PlayerStats._PlayerStats.TotalTurretKills.ToString();
    }

    public void SetTotalPlayTime()
    {
        _PlayTime._PlayTime.TotalPlayTimeValue += _PlayTime._PlayTime.LevelsPlayTime.Sum();
        _PlayTime._PlayTime.TotalPlayTime = ParseTimeToString(_PlayTime._PlayTime.TotalPlayTimeValue);
    }

    private void SetPlayTimeInGameOverUI()
    {
        TotalPlayTimeValue.text = _PlayTime._PlayTime.TotalPlayTime;
        _PlayTime._PlayTime.LevelsPlayTime[CurrentLevelIndex] += _PlayTime._PlayTime.PreviousLevelPlayTime;
        LevelPlayTimeValue.text = ParseTimeToString(_PlayTime._PlayTime.LevelsPlayTime[CurrentLevelIndex]);
    }

    public void RestartLevel()
    {
        onClick.Invoke();
        _OnLevelRestartedOrNextLevel.OnLevelRestarted = true;
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("Scene", SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadSceneAsync("LoadingScreen");
    }

    public void RestartFromLatestSaveGame()
    {
        onClick.Invoke();
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("Scene", _LastSavedGame._PlayerLevel.SceneIndex);
        _PlayTime._PlayTime = _LastSavedGame._PlayTime;
        _DataUI._DataUI = _LastSavedGame._DataUI;
        _WeaponData._Weapon = _LastSavedGame._WeaponStats;
        _PlayerStats._PlayerStats = _LastSavedGame._PlayerStats;
        _SpawnObjects._SpawnObjects = _LastSavedGame._toSpawn;
        _EnemyStats._EnemyStats = _LastSavedGame._EnemyStats;
        SceneManager.LoadSceneAsync("LoadingScreen");
    }

    public void NextLevel()
    {
        onClick.Invoke();
        _OnLevelRestartedOrNextLevel.OnNextLevel = true;
        PlayerPrefs.SetInt("Scene", SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadSceneAsync("LoadingScreen");
    }

    public void ReturnToMainMenu()
    {
        onClick.Invoke();
        _UIManager.ShowConfirmation();
    }
}