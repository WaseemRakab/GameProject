using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/**
 * User Interface Manager In-Game
 */
public class UIManager : MonoBehaviour
{
    public AudioManager _AudioManager;

    public ScriptablePlayerStats _Stats;

    public ScriptableWeapon _Weapon;

    public Image[] Lives;
    public Image noLive;
    public Image Live;

    public TMP_Text LiveDisplay;
    public Image LiveHealth;

    public Image LiveShield;
    public TMP_Text _CashInGame;
    public TMP_Text _CashInWeaponStore;

    public GameObject AmmoContentInUI;
    public TMP_Text LiveAmmoBullets;

    public GameObject _ItemStore;
    public GameObject _NotEnoughMoney;
    public GameObject _QuanityMessage;
    public GameObject _ItemStoreContents;
    public ScrollRect _ItemStoreContentsScrollView;

    public GameObject _WeaponContentInUI;

    public GameObject GameMenu;
    public GameObject Confirmation;
    public GameObject _GameOver;

    public Image _Portrait;
    public Sprite _GameOverPortrait;

    public GameObject _NextLevel;
    public GameObject ItemShopObject;

    public GameObject SellWeaponConfirmPanel;

    private GameManager _GameManager;

    public Player LaraPlayer;

    private GameObject _PlayerInteractingEventMessage;
    private GameObject _PlayerInteractingPushMessage;

    public GameObject SaveGamePanel;

    private bool GameQuestToggle;
    public GameObject GameQuestsPanel;
    public ScrollRect GameQuestsContentScrollView;
    public GameObject GameQuestsContent;

    public GameObject InfoMenu;

    public GameObject SkipCutscene;

    private Button.ButtonClickedEvent onClick;

    private void Awake()
    {
        _GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (LaraPlayer == null)
            GameObject.FindGameObjectWithTag("Player")?.TryGetComponent(out LaraPlayer);
        onClick = new Button.ButtonClickedEvent();
        onClick.AddListener(new UnityEngine.Events.UnityAction(() => _AudioManager.ClickSound()));

        _PlayerInteractingEventMessage = LaraPlayer.transform.GetChild(3).gameObject;
        _PlayerInteractingPushMessage = LaraPlayer.transform.GetChild(4).gameObject;
    }
    public void LoseLive(int Live)
    {
        LiveDisplay.text = Live.ToString();
        Lives[Live].sprite = noLive.sprite;
    }

    public void LoseAllLives()
    {
        for (int i = 0; i < Lives.Length; ++i)
        {
            Lives[i].sprite = noLive.sprite;
        }
        LiveDisplay.text = 0.ToString();
    }

    public void AddLive(int Live)
    {
        Lives[Live].sprite = this.Live.sprite;
    }

    public void ShowSellWeaponConfirmation()
    {
        SellWeaponConfirmPanel.SetActive(true);
    }

    public void HideSellWeaponConfirmation()
    {
        SellWeaponConfirmPanel.SetActive(false);
    }

    public void RefreshPlayerStatsInUI()
    {
        RefreshShield();
        RefreshHealth();
        RefreshLives();
        RefreshCash();
        RefreshAmmo();
    }

    private void RefreshAmmo()
    {
        LiveAmmoBullets.text = _Weapon._Weapon.Ammo.ToString();
    }

    public void RefreshCash()
    {
        _CashInGame.text = _Stats._PlayerStats.MyCash.ToString() + " " + "$";
    }

    public void RefreshLives()
    {
        LiveDisplay.text = _Stats._PlayerStats.Lives.ToString();
        for (int i = _Stats._PlayerStats.Lives - 1; i >= 0; --i)
        {
            Lives[i].sprite = Live.sprite;
        }
    }

    private void RefreshShield()
    {
        if (_Stats._PlayerStats.Shield > 0)
            LaraPlayer.AddMyShieldEffect();
        LiveShield.fillAmount = _Stats._PlayerStats.Shield / 100f;
    }

    public void RefreshHealth()
    {
        LiveHealth.fillAmount = _Stats._PlayerStats.Health / 100f;
    }

    public void DecreaseHealth(float Health)
    {
        LiveHealth.fillAmount = Health / 100f;
    }

    public void DecreaseShield(float Shield)
    {
        LiveShield.fillAmount = Shield / 100f;
    }

    public void FillShield(float Capacity)
    {
        LiveShield.fillAmount = Capacity / 100f;
    }

    public void ResetHealth()
    {
        _Stats._PlayerStats.Health = 100;
        LiveHealth.fillAmount = 1f;
    }

    public void ResetLives()
    {
        _Stats._PlayerStats.Lives = 3;
        RefreshLives();
    }

    public void ShowPanel()
    {
        GameMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        GameMenu.SetActive(false);
    }

    public void ShowConfirmation()
    {
        Confirmation.SetActive(true);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("Scene", 0);
        SceneManager.LoadSceneAsync("LoadingScreen");
    }

    public void HideConfirmation()
    {
        Confirmation.SetActive(false);
    }

    public void GameOver()
    {
        _GameOver.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ConfirmAnswer(Button Pressed)
    {
        onClick.Invoke();
        if (Pressed.name == "Yes")
        {
            GoToMainMenu();
        }
        else
        {
            HideConfirmation();
        }
    }
    public void LevelFinished()
    {
        _NextLevel.GetComponent<Image>().color = Color.white;
        _NextLevel.GetComponent<Button>().interactable = true;

        ItemShopObject.GetComponent<Button>().interactable = true;
        ItemShopObject.GetComponent<Image>().color = Color.white;
    }

    public void ShowInteractingPushMessage()
    {
        _PlayerInteractingPushMessage.SetActive(true);
    }

    public void HideInteractingPushMessage()
    {
        _PlayerInteractingPushMessage.SetActive(false);
    }

    public void HideAllInteractingMessages()
    {
        _PlayerInteractingPushMessage.SetActive(false);
        _PlayerInteractingEventMessage.SetActive(false);
    }

    public void ShowInteractingEventMessage()
    {
        _PlayerInteractingEventMessage.SetActive(true);
    }

    public void HideInteractingEventMessage()
    {
        _PlayerInteractingEventMessage.SetActive(false);
    }

    public void UnlockFinishDoor(GameObject lockedDoor)
    {
        lockedDoor.GetComponent<Animator>().enabled = true;
    }

    public void SetGameOverPortrait()
    {
        _Portrait.sprite = _GameOverPortrait;
    }

    public void ShowLockedMessage(GameObject lockedDoorCanvasMessage)
    {
        lockedDoorCanvasMessage.SetActive(true);
    }
    public void HideLockedMessage(GameObject lockedDoorCanvasMessage)
    {
        lockedDoorCanvasMessage.SetActive(false);
    }
    public void EnterItemShopFromGameMenu()
    {
        onClick.Invoke();
        ShowWeaponStore();
    }

    public void ShowWeaponStore()
    {
        _GameManager.PauseGame();
        _ItemStore.SetActive(true);
    }

    public void HideWeaponStore()
    {
        onClick.Invoke();
        _ItemStore.SetActive(false);
        _GameManager.ResumeGame();
    }

    public void SetCash(int Cash)
    {
        _CashInGame.text = Cash + " " + "$";
    }

    public void UpdateCashInItemShop()
    {
        _CashInGame.text = _Stats._PlayerStats.MyCash + " " + "$";
        _CashInWeaponStore.text = _CashInGame.text;
    }

    public int GetCashFromUI(string Cash)
    {
        char[] splitArgs = { ' ', '$' };
        string[] splitter = Cash.Split(splitArgs, StringSplitOptions.RemoveEmptyEntries);
        if (int.TryParse(splitter[0], out int Check))
        {
            return Check;
        }
        return -1;
    }

    public void SetAmmoPrice(ref TMP_Text AmmoPriceUIint, int PriceAfterQuanity)
    {
        AmmoPriceUIint.text = PriceAfterQuanity.ToString() + " " + "$";
    }

    public void ShowNotEnoughMoneyPanel()
    {
        _NotEnoughMoney.SetActive(true);
    }

    public void HideNotEnoughMoneyPanel()
    {
        _NotEnoughMoney.SetActive(false);
    }

    public void ShowMessageQuanityPanel()
    {
        _QuanityMessage.SetActive(true);
    }

    public void HideMessageQuanityPanel()
    {
        _QuanityMessage.SetActive(false);
    }

    public void ActivateSell()
    {
        _WeaponContentInUI.transform.GetChild(2).GetComponent<Button>().interactable = true;
    }

    public void DeactiveSell()
    {
        _WeaponContentInUI.transform.GetChild(2).GetComponent<Button>().interactable = false;
    }

    public void SwitchItemShopContentUI(int PressedContentIndex, int PreviousContentIndex)
    {
        GameObject scrollViewContent = _ItemStoreContents.transform.GetChild(PressedContentIndex).gameObject;
        _ItemStoreContents.transform.GetChild(PreviousContentIndex).gameObject.SetActive(false);
        scrollViewContent.SetActive(true);
        _ItemStoreContentsScrollView.content = scrollViewContent.GetComponent<RectTransform>();
    }

    public void ShowSaveGamePanel()
    {
        SaveGamePanel.SetActive(true);
    }

    public void HideSaveGamePanel()
    {
        SaveGamePanel.SetActive(false);
    }

    public void ShowAmmoContentInUI()
    {
        AmmoContentInUI.SetActive(true);
    }

    public void SetAmmoBulletsInUI(int Amount)
    {
        LiveAmmoBullets.text = Amount.ToString();
    }

    public void HideAmmoContentInUI()
    {
        AmmoContentInUI.SetActive(false);
    }

    public void ToggleGameQuest() //Button Event
    {
        onClick.Invoke();
        GameQuestsPanel.SetActive(GameQuestToggle);
        GameQuestToggle = !GameQuestToggle;
    }

    public void SwitchQuestContentUI(int PressedContentIndex, int PreviousContentIndex)
    {
        GameObject scrollViewContent = GameQuestsContent.transform.GetChild(PressedContentIndex).gameObject;
        GameQuestsContent.transform.GetChild(PreviousContentIndex).gameObject.SetActive(false);
        scrollViewContent.SetActive(true);
        GameQuestsContentScrollView.content = scrollViewContent.GetComponent<RectTransform>();
    }
    public void ShowInfoMenu()
    {
        onClick.Invoke();
        InfoMenu.SetActive(true);
    }
    public void HideInfoMenu(bool WithSound = true)
    {
        if (WithSound)
            onClick.Invoke();
        InfoMenu.SetActive(false);
    }
    public void ShowSkipCutscene()
    {
        SkipCutscene.SetActive(true);
    }
    public void HideSkipCutscene()
    {
        SkipCutscene.SetActive(false);
    }
}