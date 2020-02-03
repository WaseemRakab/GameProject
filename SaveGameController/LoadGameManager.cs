using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/*Controling LoadGames InMainMenu*/
public class LoadGameManager : MonoBehaviour
{
    public ScriptablePlayerStats _GamePlayerStats;
    public ScriptableWeapon _GameWeaponStats;
    public ScriptableSpawnObjects _GameSpawnObjects;
    public ScriptableDataUI _GameDataUI;
    public ScriptablePlayTime _GamePlayTime;
    public ScriptableEnemyStats _GameEnemyStats;

    public GameObject _MainMenu;
    public GameObject _EmptySlotMessagePanel;
    public GameObject _LoadGamePanel;

    public GameObject[] EmptySlotSaveGames;
    public GameObject[] LoadGamesData;
    public TMP_Text[] LoadGamesFileNameData;
    public TMP_Text[] LoadGamesDateData;
    public TMP_Text[] LoadGamesLevelData;
    public TMP_Text[] LoadGamesStageData;
    public TMP_Text[] LoadGamesCashData;

    private Button.ButtonClickedEvent onClick;

    public AudioManager _AudioManager;

    private int WhichLoadGame;

    private List<SaveGame> LoadGames;

    public GameObject DeleteHint;
    private int WhichLoadGameSlotToDelete = -1;
    public GameObject DeleteSaveGameSlotConfirmation;
    private bool DeleteHintToggle = false;
    private void Awake()
    {
        if (_AudioManager == null)
            GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);

        onClick = new Button.ButtonClickedEvent();//On AnyButton Clicked Event Sound
        onClick.AddListener(new UnityAction(() => _AudioManager.ClickSound()));
    }
    private void OnEnable()
    {
        SetLoadGamesInUI();
    }

    private void SetLoadGamesInUI()
    {
        LoadGames = SaveGameManager.RetrieveSaveGames();
        SaveGameManager.SetLoadGamesToUI(
             LoadGames, LoadGamesFileNameData,
             LoadGamesDateData, LoadGamesLevelData,
             LoadGamesStageData, LoadGamesCashData,
             EmptySlotSaveGames, LoadGamesData);
    }

    public void LoadSlotSelected(Button LoadGamePress)//Button Event
    {
        onClick.Invoke();
        WhichLoadGame = LoadGamePress.transform.GetSiblingIndex();
        if (!EmptySlotSaveGames[WhichLoadGame].activeSelf) //There Is A Load Game Saved
        {
            SaveGame LoadedGame = SaveGameManager.LoadSaveGame(LoadGamesFileNameData[WhichLoadGame].text);
            if (LoadedGame != null)//On Load Game Verified
            {
                LoadGame(LoadedGame);
            }
        }
        else
        {
            ShowEmptySlotMessage();
        }
    }

    private void LoadGame(SaveGame LoadGame)
    {
        _GamePlayTime._PlayTime = LoadGame._PlayTime;
        _GameDataUI._DataUI = LoadGame._DataUI;
        _GameWeaponStats._Weapon = LoadGame._WeaponStats;
        _GamePlayerStats._PlayerStats = LoadGame._PlayerStats;
        _GameSpawnObjects._SpawnObjects = LoadGame._toSpawn;
        _GameEnemyStats._EnemyStats = LoadGame._EnemyStats;
        PlayerPrefs.SetInt("Scene", LoadGame._PlayerLevel.SceneIndex);
        SceneManager.LoadSceneAsync("LoadingScreen");
    }

    public void CloseLoadGame()//Button Event
    {
        onClick.Invoke();
        HideLoadGamePanel();
        _MainMenu.SetActive(true);
    }

    private void HideLoadGamePanel()
    {
        _LoadGamePanel.SetActive(false);
    }
    private void ShowEmptySlotMessage()
    {
        _EmptySlotMessagePanel.SetActive(true);
    }
    private void HideEmptySlotMessage()
    {
        _EmptySlotMessagePanel.SetActive(false);
    }

    public void OnCloseEmptySlotMessage()//Button Event
    {
        onClick.Invoke();
        HideEmptySlotMessage();
    }

    public void ShowDeleteLoadGameConfirmation()//OnEvent
    {
        DeleteSaveGameSlotConfirmation.SetActive(true);
    }
    private void HideDeleteLoadGameConfirmation()
    {
        DeleteSaveGameSlotConfirmation.SetActive(false);
    }
    public void DeleteLoadGameSlot(int WhichSlot)//OnEvent
    {
        WhichLoadGameSlotToDelete = WhichSlot;
    }
    public void DeleteSaveGameConfirmation(Button Answer)
    {
        onClick.Invoke();
        if (Answer.name == "Yes")
        {
            if (WhichLoadGameSlotToDelete != -1)
            {
                SaveGame WhichLoadGame = LoadGames.FirstOrDefault((load) => load.WhichSlot == WhichLoadGameSlotToDelete);
                if (WhichLoadGame != null)
                {
                    RefreshSaveGameSlotsOnDeletion();
                    SaveGameManager.RemoveSaveGameSlot(WhichLoadGame._SaveFileName);
                    LoadGames.Remove(WhichLoadGame);
                    WhichLoadGameSlotToDelete = -1;
                    if (LoadGames.Count == 0)
                        SaveGameManager.DeleteRecentGame();
                }
            }
        }
        HideDeleteLoadGameConfirmation();
    }
    public void ShowDeleteHint()
    {
        DeleteHint.SetActive(!DeleteHintToggle);
        DeleteHintToggle = !DeleteHintToggle;
    }

    private void RefreshSaveGameSlotsOnDeletion()
    {
        LoadGamesData[WhichLoadGameSlotToDelete].SetActive(false);
        EmptySlotSaveGames[WhichLoadGameSlotToDelete].SetActive(true);
    }
}