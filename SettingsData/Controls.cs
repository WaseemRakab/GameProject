using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Controls", menuName = "ScriptableObjects/Controls", order = 1)]
[Serializable]
/**
 * Storing Controls Settings , Keys..
 */
public class Controls : ScriptableObject
{
    public string objectName = "Controls";

    [SerializeField]
    private KeyCode[] keys =
    {
            KeyCode.RightArrow,
            KeyCode.LeftArrow,
            KeyCode.LeftShift,
            KeyCode.E,
            KeyCode.Z,
            KeyCode.X,
            KeyCode.DownArrow,
            KeyCode.Space
    };
    [SerializeField]
    private string[] keyNames =
    {
            "Forward",
            "Backward",
            "Sprint",
            "Interact",
            "Attack",
            "Shoot",
            "Crouch",
            "Jump",
    };
    public KeyCode Forward
    {
        get
        {
            return keys[0];
        }
    }
    public KeyCode Backward
    {
        get
        {
            return keys[1];
        }
    }
    public KeyCode Sprint
    {
        get
        {
            return keys[2];
        }
    }
    public KeyCode Interact
    {
        get
        {
            return keys[3];
        }
    }
    public KeyCode Attack
    {
        get
        {
            return keys[4];
        }
    }
    public KeyCode Shoot
    {
        get
        {
            return keys[5];
        }
    }
    public KeyCode Crouch
    {
        get
        {
            return keys[6];
        }
    }
    public KeyCode Jump
    {
        get
        {
            return keys[7];
        }
    }

    public void SetButtonsDisplays(ref Button[] btns)
    {
        for (int i = 0; i < btns.Length; ++i)
        {
            btns[i].transform.GetChild(0).GetComponent<TMP_Text>().text = keys[i].ToString();
        }
    }
    public bool IsValid(KeyCode KeyCheck)
    {
        for (int i = 0; i < keys.Length; ++i)
        {
            if (keys[i] == KeyCheck)
            {
                return false;
            }
        }
        return true;
    }
    public void SetNewKey(string KeyName, KeyCode Value)
    {
        int keyPlace = Array.IndexOf(keyNames, KeyName);
        keys[keyPlace] = Value;
    }
    public void RefreshButtonDisplay(ref Button btn)
    {
        btn.transform.GetChild(0).GetComponent<TMP_Text>().text = keys[Array.IndexOf(keyNames, btn.name)].ToString();
    }
}