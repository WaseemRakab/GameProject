using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
/**
* 
* Graphics Settings Manager, Handling Resolution Changes , Screen Display Mode
*/
public class GraphicsManager : MonoBehaviour
{
    public Graphics _GraphicsData;
    public AudioManager _AudioManager;

    public TMP_Dropdown _DisplayMode;
    public TMP_Dropdown _Resolution;

    private Button.ButtonClickedEvent onClick;
    private void Awake()
    {
        if (_AudioManager == null)
            GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);
        onClick = new Button.ButtonClickedEvent();
        onClick.AddListener(new UnityAction(() => _AudioManager.ClickSound()));
    }
    private void OnEnable()
    {
        SetResolutionsBasedOnDevice();
        SetDisplayModesForDevice();
        SetGraphicsSettings();
    }
    private void SetDisplayModesForDevice()
    {
        _DisplayMode.options.Clear();
        List<FullScreenMode> Modes = Enum.GetValues(typeof(FullScreenMode)).Cast<FullScreenMode>().ToList();
        for (int i = 0; i < Modes.Count; ++i)
        {
            _DisplayMode.options.Add(new TMP_Dropdown.OptionData(Modes[i].ToString()));
        }
    }
    private void SetResolutionsBasedOnDevice()
    {
        _Resolution.options.Clear();
        Resolution[] resolutions = Screen.resolutions;
        for (int i = resolutions.Length - 1; i >= 0; --i)
        {
            _Resolution.options.Add(new TMP_Dropdown.OptionData(resolutions[i].ToString()));
        }
    }
    public void ChangeResolution()// on Resolution Click
    {
        onClick.Invoke();
        int[] resValues = GetWidthAndHeightAndAspectRatio(_Resolution.captionText.text);
        if (resValues != null)
        {
            Resolution _CurrentResolution = new Resolution
            {
                width = resValues[0],
                height = resValues[1],
                refreshRate = resValues[2]
            };
            if (_GraphicsData.Resolution != _CurrentResolution.ToString())
            {
                _GraphicsData.Resolution = _CurrentResolution.ToString();
                _GraphicsData.ResolutionValue = _Resolution.value;
                _GraphicsData.ResolutionHeight = _CurrentResolution.height;
                _GraphicsData.ResolutionWidth = _CurrentResolution.width;
                _GraphicsData.ResolutionRefreshRate = _CurrentResolution.refreshRate;
                Screen.SetResolution(_CurrentResolution.width, _CurrentResolution.height, _GraphicsData.FullScreenMode);
            }
        }
    }
    private int[] GetWidthAndHeightAndAspectRatio(string value)
    {
        char[] splitter = { ' ', 'x', ' ', ' ', '@', ' ', 'H', 'z' };
        string[] res = value.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
        if (int.TryParse(res[0], out int width) && int.TryParse(res[1], out int height) && int.TryParse(res[2], out int refreshRate))
            return new int[3] { width, height, refreshRate };
        return null;
    }
    public void DisplayModeChange()//Button Event
    {
        onClick.Invoke();
        string DisplayModeValue = _DisplayMode.captionText.text;
        if (Enum.TryParse(DisplayModeValue, out FullScreenMode DisplayMode))
        {
            if (_GraphicsData.FullScreenMode != DisplayMode)
            {
                Screen.fullScreenMode = DisplayMode;
                _GraphicsData.FullScreenMode = DisplayMode;
            }
        }
    }
    private void SetGraphicsSettings()
    {
        SetGraphics();
        SetGraphicsUI();
    }
    private void SetGraphics()
    {
        if (_GraphicsData.IsDefaultSettings)
        {
            Screen.SetResolution(_GraphicsData.ResolutionWidth, _GraphicsData.ResolutionHeight, _GraphicsData.FullScreenMode);
        }
        _GraphicsData.IsDefaultSettings = false;
    }
    private void SetGraphicsUI()
    {
        _DisplayMode.value = (int)_GraphicsData.FullScreenMode;
        _Resolution.value = _GraphicsData.ResolutionValue;
    }
}