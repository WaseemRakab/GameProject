using UnityEngine;
using UnityEngine.UI;

/**
 * Controls Settings Manager, Handling KeyPresses when changing Controls
 */
public class ControlsManager : MonoBehaviour
{
    public MainMenuScript _MainMenuScript;
    public AudioManager _AudioManager;

    public Button[] _ControlButtons;

    public Controls _Controls;

    private Button ButtonPressed;

    private Button.ButtonClickedEvent onClick;

    private void Awake()
    {
        onClick = new Button.ButtonClickedEvent();
        onClick.AddListener(new UnityEngine.Events.UnityAction(() => _AudioManager.ClickSound()));
    }
    private void OnEnable()
    {
        _Controls.SetButtonsDisplays(ref _ControlButtons);
    }
    public void GetPressedKey(Button PressedKey)//Button Event
    {
        onClick.Invoke();
        ButtonPressed = PressedKey;
        _MainMenuScript.ShowMessageForKeyInput();
        _MainMenuScript.WaittingForKey = true;
    }
    public void CheckForKey(KeyCode pressedKey)
    {
        if (pressedKey == KeyCode.Escape)
        {
            _MainMenuScript.HideMessageForKeyInput();
        }
        else if (_Controls.IsValid(pressedKey))
        {
            _Controls.SetNewKey(ButtonPressed.name, pressedKey);
            _Controls.RefreshButtonDisplay(ref ButtonPressed);
            _MainMenuScript.HideMessageForKeyInput();
        }
        else //Key Already in use
        {
            _MainMenuScript.ShowDuplicateMessage();
        }
        _MainMenuScript.WaittingForKey = false;
    }
}