using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * In-Game Menu handling Events , Button Presses ,UI managing , Etc..
 */
public class GameMenuScript : MonoBehaviour
{
    public GameOver _GameOver;
    public GameManager _GameManager;
    public UIManager _UiManager;
    public SpawnManager _SpawnManager;
    public AudioManager _AudioManager;

    public ScriptablePlayerStats _PlayerStats;
    public ScriptableWeapon _WeaponStats;
    public ScriptableSpawnObjects _SpawnObjects;
    public LevelRestartedOrNextLevel _OnLevelRestartedOrNextLevel;
    public ScriptableEnemyStats _EnemyStats;
    public ScriptablePlayTime _PlayTime;

    public GameObject[] SaveGamesData;
    public GameObject[] EmptySlotObjects;

    public TMP_Text[] SaveGamesFileNamesData;
    public TMP_Text[] SaveGamesDateData;
    public TMP_Text[] SaveGamesLevelData;
    public TMP_Text[] SaveGamesStageData;
    public TMP_Text[] SaveGamesCashData;

    public GameObject SaveGameInputPanel;

    public TMP_InputField SaveGameNameInputedField;

    private string SaveGameFileName;

    private int CurrentSceneIndex;
    private int WhichSaveGame;
    private bool IsOverWritten = false;

    public GameObject DuplicateSaveFileMessagePanel;

    private Button.ButtonClickedEvent onClick;

    private void OnEnable()
    {
        CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        _AudioManager.PauseAudioOnPauseGame();
    }
    private void OnDisable()
    {
        _AudioManager.UnPauseAudioOnResumeGame();
    }
    private void Awake()
    {
        if (_UiManager == null)
            _UiManager = GameObject.FindGameObjectWithTag("UiSystem").GetComponent<UIManager>();
        onClick = new Button.ButtonClickedEvent();
        onClick.AddListener(new UnityEngine.Events.UnityAction(() => _AudioManager.ClickSound()));
    }
    public void Resume()//Button Click
    {
        onClick.Invoke();
        ResumeGame();
    }
    private void ResumeGame()
    {
        _UiManager.ResumeGame();
        _GameManager.GamePaused = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_UiManager.SaveGamePanel.activeSelf)
                _UiManager.HideSaveGamePanel();
            else if (_UiManager.Confirmation.activeSelf)
                _UiManager.HideConfirmation();
            else if (_UiManager.InfoMenu.activeSelf)
                _UiManager.HideInfoMenu(false);
            else ResumeGame();
        }
    }
    public void RestartLevel()
    {
        onClick.Invoke();
        Time.timeScale = 1f;
        _OnLevelRestartedOrNextLevel.OnLevelRestarted = true;
        _AudioManager.UnPauseAudioOnResumeGame();
        PlayerPrefs.SetInt("Scene", CurrentSceneIndex);
        SceneManager.LoadSceneAsync("LoadingScreen");
    }
    public void ShowGameMenu()
    {
        _UiManager.ShowPanel();
    }

    public void SaveGameButton()
    {
        onClick.Invoke();
        SetSaveGamesInUI();
        _UiManager.ShowSaveGamePanel();
    }

    private void SetSaveGamesInUI()
    {
        List<SaveGame> SaveGamesToUI = SaveGameManager.RetrieveSaveGames();
        SaveGameManager.SetSaveGamesToUI(
             SaveGamesToUI, SaveGamesFileNamesData,
             SaveGamesDateData, SaveGamesLevelData,
             SaveGamesStageData, SaveGamesCashData,
             EmptySlotObjects, SaveGamesData);
    }
    public void CloseSaveGame()
    {
        onClick.Invoke();
        _UiManager.HideSaveGamePanel();
    }
    public void ReturnToMenu()
    {
        onClick.Invoke();
        _UiManager.ShowConfirmation();
    }
    public void SaveSlotSelected(Button SaveGamePress)
    {
        onClick.Invoke();
        WhichSaveGame = SaveGamePress.transform.GetSiblingIndex();
        if (EmptySlotObjects[WhichSaveGame].activeSelf)
        {
            ShowSaveGameInputPanel();
        }
        else
        {
            SetFileNameOnInputField();
            IsOverWritten = true;
            ShowSaveGameInputPanel();
        }
    }

    private void SetFileNameOnInputField()
    {
        SaveGameFileName = SaveGamesFileNamesData[WhichSaveGame].text;
        SaveGameNameInputedField.text = SaveGameFileName;
    }
    private void GetSaveGameInputedField()
    {
        SaveGameFileName = SaveGameNameInputedField.text;
    }
    public void GetSelectedSaveGameOptionOnInputField(Button PressedOption)
    {
        onClick.Invoke();
        if (PressedOption.name == "OK")
        {
            GetSaveGameInputedField();
            if (!string.IsNullOrWhiteSpace(SaveGameFileName))
            {
                if (DenyDuplicatesFileNames() == true)
                {
                    if (IsOverWritten == false)
                    {
                        GetLarasPosition();
                        SaveTheGameWithoutOverWrite();
                    }
                    else
                    {
                        string PreviousSaveFileName = SaveGamesFileNamesData[WhichSaveGame].text;
                        GetLarasPosition();
                        OverWriteSaveGame(PreviousSaveFileName);
                    }
                    HideSaveGameInputPanel();
                }
                else
                {
                    ShowDuplicateSaveFileMessage();
                }
            }
        }
        else//Pressed Cancel
        {
            HideSaveGameInputPanel();
        }
        IsOverWritten = false;
        SaveGameNameInputedField.text = string.Empty;
    }
    private void GetLarasPosition()
    {
        Vector3 LaraPos = _UiManager.LaraPlayer.transform.position;
        _PlayerStats._PlayerStats.PlayerPosition = new Vector(LaraPos.x, LaraPos.y, LaraPos.z);
    }
    public void HideDuplicateSaveFileMessage()
    {
        DuplicateSaveFileMessagePanel.SetActive(false);
    }
    private void ShowDuplicateSaveFileMessage()
    {
        DuplicateSaveFileMessagePanel.SetActive(true);
    }
    private bool DenyDuplicatesFileNames()
    {
        for (int i = 0; i < SaveGamesFileNamesData.Length; ++i)
        {
            if (i != WhichSaveGame)
            {
                if (SaveGamesFileNamesData[i].text == SaveGameFileName)
                    return false;
            }
        }
        return true;
    }
    private void OverWriteSaveGame(string PreviousSaveFileName)
    {
        SaveGame saveGame = GetSaveGame();
        SetNewSaveGameOnUI(saveGame);
        SaveGameManager.OverWriteSaveGame(SaveGameFileName, PreviousSaveFileName, saveGame);
        SaveGameManager.SaveRecentGame(saveGame);
    }
    private void SaveTheGameWithoutOverWrite()
    {
        SaveGame saveGame = GetSaveGame();
        SetNewSaveGameOnUI(saveGame);
        SaveGameManager.SaveGameData(SaveGameFileName, saveGame);
        SaveGameManager.SaveRecentGame(saveGame);
    }
    private SaveGame GetSaveGame()
    {
        _PlayTime._PlayTime.PreviousLevelPlayTime = _PlayTime._PlayTime.LevelsPlayTime[CurrentSceneIndex - 2];
        _GameOver.SetTotalPlayTime();
        SetMovingSurfacesOrPlatformsPositions();
        SetPushableBoxesPositions();
        bool[] CoinChestsOpened = CheckIfPlayerOpenedCoinChests();
        bool SecretChestOpened = CheckIfPlayerOpenedSecretChest();
        bool OpenedDarkArea = CheckIfPlayerOpenedDarkArea();
        SaveGame saveGame = new SaveGame()
        {
            _PlayTime = _PlayTime._PlayTime,
            _DataUI = new DataUI() { CoinChestsOpened = CoinChestsOpened, SecretChestOpened = SecretChestOpened, OpenedDarkArea = OpenedDarkArea },
            _DateTime = DateTime.Now.ToString("dddd MMMM dd yyyy HH:mm", new System.Globalization.CultureInfo("en-US", true)),
            _PlayerStats = _PlayerStats._PlayerStats,
            _WeaponStats = _WeaponStats._Weapon,
            _EnemyStats = _EnemyStats._EnemyStats,
            _PlayerLevel = new Level()
            {
                LevelNumber = (CurrentSceneIndex + 1) % 3 + 1,
                SceneIndex = CurrentSceneIndex,
                StageNumber = (CurrentSceneIndex - ((CurrentSceneIndex + 1) % 3 + 1)) / 3 + 1
            },
            _toSpawn = _SpawnObjects._SpawnObjects,
            _SaveFileName = SaveGameFileName,
            WhichSlot = WhichSaveGame
        };
        return saveGame;
    }
    private void SetMovingSurfacesOrPlatformsPositions()
    {
        MovingSurface[] MovingSurfaces = FindObjectsOfType<MovingSurface>();
        if (MovingSurfaces != null)
        {
            /*surface.WhichTypeOfSurface == 1 = Sci-Fi Platforms*/
            List<MovingSurface> SciFiPlatforms = MovingSurfaces.Where((surface) => surface.WhichTypeOfSurface == 1).ToList();
            if (SciFiPlatforms.Count > 0)
                _SpawnObjects._SpawnObjects.SciFiMovingSurfacesPositions = new List<Vector>(SciFiPlatforms.Count);
            /*surface.WhichTypeOfSurface == 0 = RockSurfaces*/
            List<MovingSurface> RockSurfaces = MovingSurfaces.Where((surface) => surface.WhichTypeOfSurface == 0).ToList();
            if (RockSurfaces.Count > 0)
                _SpawnObjects._SpawnObjects.RockMovingSurfacesPositions = new List<Vector>(RockSurfaces.Count);
            for (int i = SciFiPlatforms.Count - 1; i >= 0; --i)
            {
                Vector3 SciFiPos = SciFiPlatforms[i].gameObject.transform.position;
                _SpawnObjects._SpawnObjects.SciFiMovingSurfacesPositions.Add(new Vector(SciFiPos.x, SciFiPos.y, SciFiPos.z));
            }
            for (int i = RockSurfaces.Count - 1; i >= 0; --i)
            {
                Vector3 RockPos = RockSurfaces[i].gameObject.transform.position;
                _SpawnObjects._SpawnObjects.RockMovingSurfacesPositions.Add(new Vector(RockPos.x, RockPos.y, RockPos.z));
            }
        }
    }
    private void SetPushableBoxesPositions()
    {
        PushableBox[] PushableBox = FindObjectsOfType<PushableBox>();
        if (PushableBox != null)
        {
            _SpawnObjects._SpawnObjects.PushableBoxPositions = new List<Vector>(PushableBox.Length);
            for (int i = PushableBox.Length - 1; i >= 0; --i)
            {
                Vector3 BoxCurrentPos = PushableBox[i].gameObject.transform.position;
                _SpawnObjects._SpawnObjects.PushableBoxPositions.Add(new Vector(BoxCurrentPos.x, BoxCurrentPos.y, BoxCurrentPos.z));
            }
        }
    }
    private bool CheckIfPlayerOpenedDarkArea()
    {
        DarkAreaTrigger DarkTrigger = null;
        GameObject.FindGameObjectWithTag("DarkAreaTrigger")?.TryGetComponent(out DarkTrigger);
        return DarkTrigger != null && DarkTrigger.DarkAreaOpened;
    }
    private bool[] CheckIfPlayerOpenedCoinChests()//For Coin Chest Open Check
    {
        return _UiManager.LaraPlayer.HasBeenInCoinChests;
    }
    private bool CheckIfPlayerOpenedSecretChest()//For KeyUI Check (If Player Got Key in Secret Chest)
    {
        return _UiManager.LaraPlayer.HasBeenInSecretChest == true;
    }
    private void SetNewSaveGameOnUI(SaveGame saveGame)
    {
        SaveGamesFileNamesData[WhichSaveGame].text = saveGame._SaveFileName;
        SaveGamesDateData[WhichSaveGame].text = saveGame._DateTime;
        SaveGamesLevelData[WhichSaveGame].text = saveGame._PlayerLevel.LevelNumber.ToString();
        SaveGamesStageData[WhichSaveGame].text = saveGame._PlayerLevel.StageNumber.ToString();
        SaveGamesCashData[WhichSaveGame].text = saveGame._PlayerStats.MyCash + " $";
        EmptySlotObjects[WhichSaveGame].SetActive(false);
        SaveGamesData[WhichSaveGame].SetActive(true);
    }
    private void ShowSaveGameInputPanel()
    {
        SaveGameInputPanel.SetActive(true);
    }
    private void HideSaveGameInputPanel()
    {
        SaveGameInputPanel.SetActive(false);
    }
}