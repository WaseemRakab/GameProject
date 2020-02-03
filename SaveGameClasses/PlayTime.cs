using System;
/**
 * In-Game PlayTime Storing
 */
[Serializable]
public class PlayTime
{
    public float[] LevelsPlayTime = new float[6];
    public float PreviousLevelPlayTime;
    public float TotalPlayTimeValue = 0f;
    public string TotalPlayTime;
    public PlayTime() { }
}