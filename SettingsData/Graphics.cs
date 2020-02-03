using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Graphics", menuName = "ScriptableObjects/Graphics", order = 2)]
[Serializable]
/**
 *  Storing Graphics Settings , Resolution, ScreenMode..
 */
public class Graphics : ScriptableObject
{
    public string objectName = "Graphics";
    public bool IsDefaultSettings = true;
    public FullScreenMode FullScreenMode = FullScreenMode.ExclusiveFullScreen;
    public string Resolution = "1920 x 1080 @ 60Hz";
    public int ResolutionValue = 0;
    public int ResolutionHeight = 1080;
    public int ResolutionWidth = 1920;
    public int ResolutionRefreshRate = 60;
}