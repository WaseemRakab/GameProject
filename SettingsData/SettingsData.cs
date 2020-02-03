using System.IO;
using UnityEngine;

/**
 * Getting Game Options Settings From Files Data in Game
 */
public class SettingsData : MonoBehaviour
{
    public AudioData _Audio;
    public Controls _Controls;
    public Graphics _Graphics;
    //Scriptable Objects, This Instance in All Scenes Being Overwritted in LoadOptions

    private readonly string ControlsOptionsFileName = "Controls.json";
    private readonly string AudioOptionsFileName = "Audio.json";
    private readonly string GraphicsOptionsFileName = "Graphics.json";

    private void Awake()
    {
        LoadSettings();
    }
    private void LoadSettings()
    {
        string ControlsPath = Path.Combine(Application.streamingAssetsPath, ControlsOptionsFileName);
        string AudioPath = Path.Combine(Application.streamingAssetsPath, AudioOptionsFileName);
        string GraphicsPath = Path.Combine(Application.streamingAssetsPath, GraphicsOptionsFileName);
        if (!File.Exists(ControlsPath))
            File.WriteAllText(ControlsPath, JsonUtility.ToJson(ScriptableObject.CreateInstance<Controls>()));
        if (!File.Exists(AudioPath))
            File.WriteAllText(AudioPath, JsonUtility.ToJson(ScriptableObject.CreateInstance<AudioData>()));
        if (!File.Exists(GraphicsPath))
            File.WriteAllText(GraphicsPath, JsonUtility.ToJson(ScriptableObject.CreateInstance<Graphics>()));
        JsonUtility.FromJsonOverwrite(File.ReadAllText(ControlsPath), _Controls);
        JsonUtility.FromJsonOverwrite(File.ReadAllText(AudioPath), _Audio);
        JsonUtility.FromJsonOverwrite(File.ReadAllText(GraphicsPath), _Graphics);
    }
    public void SaveControls()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, ControlsOptionsFileName);
        string dataAsJson = JsonUtility.ToJson(_Controls, true);
        File.WriteAllText(filePath, dataAsJson);
    }
    public void SaveAudioVolume()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, AudioOptionsFileName);
        string dataAsJson = JsonUtility.ToJson(_Audio, true);
        File.WriteAllText(filePath, dataAsJson);
    }
    public void SaveGraphics()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, GraphicsOptionsFileName);
        string dataAsJson = JsonUtility.ToJson(_Graphics, true);
        File.WriteAllText(filePath, dataAsJson);
    }
}