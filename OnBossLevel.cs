using UnityEngine;
using UnityEngine.UI;

/**
 * Controling what to do when
 * Being on Boss Area
 */
public class OnBossLevel : MonoBehaviour
{
    public GameObject SaveGamePanelUI;
    public void DisableSaveGameOnFinishingBossLevel()
    {
        if (SaveGamePanelUI.activeSelf)
        {
            SaveGamePanelUI.GetComponent<Image>().color = Color.gray;
            SaveGamePanelUI.GetComponent<Button>().interactable = false;
        }
    }
}