using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/**
 * Main Menu Controller For In-Game , Start of game, Etc..
 */
public class MainMenuScript : MonoBehaviour
{
    public GameObject _MainMenu;
    public GameObject _Options;
    public GameObject _AudioSettings;
    public GameObject _ControlSettings;
    public GameObject _GraphicsSettings;
    public GameObject _LoadGamePanel;
    public GameObject _TutorialGamePanel;

    public AudioData _AudioData;
    public AudioManager _AudioManager;

    public SettingsData _SettingData;

    public Slider _MasterVolumeSlider;
    public TMP_Text _MasterVolumeValue;
    public Slider _MusicAudioSlider;
    public TMP_Text _MusicAudioValue;
    public Slider _SFXAudioSlider;
    public TMP_Text _SFXAudioValue;
    public TMP_Dropdown SpeakerModeDropDown;


    public GameObject _WaitForInputPanel;
    public TMP_Text _MessageInputPanel;
    private string _OriginalMessage;
    private ControlsManager _ControlsManager;

    public ScriptablePlayerStats _GamePlayerStats;
    public ScriptableWeapon _GameWeaponStats;
    public ScriptableSpawnObjects _GameSpawnObjects;
    public ScriptableDataUI _GameDataUI;
    public ScriptablePlayTime _GamePlayTime;
    public ScriptableEnemyStats _GameEnemyStats;

    private Button.ButtonClickedEvent onClick;

    public bool WaittingForKey = false;
    private void Awake()
    {
        Time.timeScale = 1f;

        onClick = new Button.ButtonClickedEvent();//On AnyButton Clicked Event Sound
        onClick.AddListener(new UnityEngine.Events.UnityAction(() => _AudioManager.ClickSound()));

        _ControlsManager = _ControlSettings.GetComponent<ControlsManager>();

        AudioAndTextControl();
    }
    private void Start()
    {
        _OriginalMessage = _MessageInputPanel.text;
    }

    public void StartNewGame()//Button Event
    {
        onClick.Invoke();
        SaveGameManager.LoadNewGameData(_GamePlayerStats, _GameWeaponStats, _GameSpawnObjects, _GameDataUI, _GamePlayTime, _GameEnemyStats);
        ShowTutorialMessage();
    }
    public void LoadGameButton()//Button Event
    {
        onClick.Invoke();
        _MainMenu.SetActive(false);
        ShowLoadGamePanel();
    }

    private void ShowLoadGamePanel()
    {
        _LoadGamePanel.SetActive(true);
    }

    public void Exit()//Button Event
    {
        onClick.Invoke();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
    public void Options()//Button Event
    {
        onClick.Invoke();
        _MainMenu.SetActive(false);
        _Options.SetActive(true);
    }

    public void ReturnToMainMenu()//Button Event
    {
        onClick.Invoke();
        _MainMenu.SetActive(true);
        _Options.SetActive(false);
    }

    public void Audio()//Button Event
    {
        onClick.Invoke();
        _Options.SetActive(false);
        _AudioSettings.SetActive(true);
    }

    public void CloseAudio()//Button Event
    {
        onClick.Invoke();
        _SettingData.SaveAudioVolume();
        _AudioSettings.SetActive(false);
        _Options.SetActive(true);
    }

    public void Controls()//Button Event
    {
        onClick.Invoke();
        _Options.SetActive(false);
        _ControlSettings.SetActive(true);
    }

    public void Graphics()//Button Event
    {
        onClick.Invoke();
        _Options.SetActive(false);
        _GraphicsSettings.SetActive(true);
    }

    public void CloseGraphics()//Button Event
    {
        onClick.Invoke();
        _SettingData.SaveGraphics();
        _GraphicsSettings.SetActive(false);
        _Options.SetActive(true);
    }

    public void CloseControls()//Button Event
    {
        onClick.Invoke();
        _SettingData.SaveControls();
        _ControlSettings.SetActive(false);
        _Options.SetActive(true);
    }

    private void ShowTutorialMessage()
    {
        _TutorialGamePanel.SetActive(true);
    }

    private void HideTutorialMessage()
    {
        _TutorialGamePanel.SetActive(false);
    }

    public void TutorialMessageAnswer(Button AnswerBtn)
    {
        onClick.Invoke();
        if (AnswerBtn.name == "Yes")
        {
            PlayerPrefs.SetInt("Scene", 8);//Tutorial Scene Index Number
            SceneManager.LoadSceneAsync("LoadingScreen");
        }
        else if (AnswerBtn.name == "No")//Answered No
        {
            SceneManager.LoadSceneAsync(10);
        }
        HideTutorialMessage();
    }

    public void ShowMessageForKeyInput()
    {
        _WaitForInputPanel.SetActive(true);
    }

    private void Update()
    {
        if (WaittingForKey == true)
        {
            List<KeyCode> KeyCodeValues = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToList();
            for (int i = 0; i < KeyCodeValues.Count; ++i)
            {
                if (Input.GetKey(KeyCodeValues[i]))
                {
                    _ControlsManager.CheckForKey(KeyCodeValues[i]);
                    break;
                }
            }
        }
    }
    public void HideMessageForKeyInput()
    {
        _WaitForInputPanel.SetActive(false);
    }

    public void ShowDuplicateMessage()
    {
        _MessageInputPanel.text = "Key Is Already Used.";
        StartCoroutine(ReturnOriginalMessage());
    }

    private IEnumerator ReturnOriginalMessage()
    {
        yield return new WaitForSeconds(1.5f);
        HideMessageForKeyInput();
        _MessageInputPanel.text = _OriginalMessage;
    }

    public void OnMasterVolumeChanged()
    {
        _AudioData.MasterAudioValue = _MasterVolumeSlider.value;
        _MasterVolumeValue.text = _AudioData.MasterAudioValue.ToString() + "%";
        _AudioManager.ChangeMasterVolume();
    }

    public void OnMusicVolumeChanged() // OnEveryChangeOfMusicVolume, Call This Method
    {
        _AudioData.MusicAudioValue = _MusicAudioSlider.value;
        _MusicAudioValue.text = _AudioData.MusicAudioValue.ToString() + "%";
        _AudioManager.ChangeMusicVolume();
    }

    public void OnSFXVolumeChanged()
    {
        _AudioData.SFXAudioValue = _SFXAudioSlider.value;
        _SFXAudioValue.text = _AudioData.SFXAudioValue.ToString() + "%";
        _AudioManager.ChangeSFXVolume();
    }

    public void OnSpeakerModeChanged()
    {
        onClick.Invoke();
        string SpeakerModeLabel = SpeakerModeDropDown.captionText.text;
        if (Enum.TryParse(SpeakerModeLabel, out AudioSpeakerMode speakerMode))
        {
            if (_AudioData.AudioSpeakerMode != speakerMode)
            {
                _AudioData.AudioSpeakerMode = speakerMode;
                _AudioManager.OnSpeakerModeChanged(speakerMode);
            }
        }
    }

    private void AudioAndTextControl()//Retrieve Audio Volume From The ScriptableObject/Json Once.
    {
        _MasterVolumeSlider.value = _AudioData.MasterAudioValue;
        _MasterVolumeValue.text = _MasterVolumeSlider.value.ToString() + "%";
        _MusicAudioSlider.value = _AudioData.MusicAudioValue;
        _MusicAudioValue.text = _MusicAudioSlider.value.ToString() + "%";
        _SFXAudioSlider.value = _AudioData.SFXAudioValue;
        _SFXAudioValue.text = _SFXAudioSlider.value.ToString() + "%";
        SetSpeakerModeValuesOnDropDown();
    }

    private void SetSpeakerModeValuesOnDropDown()
    {
        SpeakerModeDropDown.options.Clear();
        List<AudioSpeakerMode> Values = Enum.GetValues(typeof(AudioSpeakerMode)).Cast<AudioSpeakerMode>().ToList();
        for (int i = 0; i < Values.Count; ++i)
        {
            SpeakerModeDropDown.options.Add(new TMP_Dropdown.OptionData(Values[i].ToString()));
        }
        SpeakerModeDropDown.value = (int)_AudioData.AudioSpeakerMode;
    }
}