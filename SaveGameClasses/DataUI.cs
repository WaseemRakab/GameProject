using System;
/**
 * In-Game DataUI Storing
 */
[Serializable]
public class DataUI
{
    public string objectName = "DataUI";
    public bool[] CoinChestsOpened = new bool[] { false };
    public bool SecretChestOpened = false;
    public bool OpenedDarkArea = false;
    public DataUI() { }
}