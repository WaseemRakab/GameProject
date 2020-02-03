using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio", menuName = "ScriptableObjects/Audio", order = 2)]
[Serializable]
/**
 * Storing AudioData , Volumes..
 */
public class AudioData : ScriptableObject
{
    public string objectName = "Audio";
    public float MasterAudioValue = 100f;
    public float MusicAudioValue = 70f;
    public float SFXAudioValue = 100f;
    public AudioSpeakerMode AudioSpeakerMode = AudioSpeakerMode.Stereo;
}