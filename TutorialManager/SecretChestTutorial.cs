using UnityEngine;

public class SecretChestTutorial : MonoBehaviour
{
    public bool ChestOpened = false;
    public Controls _Controls;
    public void OnChestOpened()
    {
        if (Input.GetKey(_Controls.Interact))
        {
            transform.GetChild(0).gameObject.SetActive(true);//Viewing the Key
            ChestOpened = true;
        }
    }
}