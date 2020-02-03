using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/**
 * ItemShop InGame , Handling Shopping
 * Items Ingame,Weapon,Health,Armor,Etc
 * 
 */
public class ItemShop : MonoBehaviour
{
    public UIManager _UiManager;
    public Player _CurrentPlayer;
    public AudioManager _AudioManager;

    public ScriptableWeapon _WeaponInStore;
    public ScriptablePlayerStats _PlayerStats;

    public int _CurrentActiveItemStoreContent;

    public TMP_Text _CurrentCashTextInStore;

    public TMP_Text CurrentAmmoValueInUI;
    public Image CurrentAmmoBar;

    public TMP_Text _AmmoPriceInUI;
    private int _CurrentAmmoPrice;
    private int AmmoQuanityAmount;

    public Button _WeaponBuyButton;

    public Image CurrentHealthBar;
    public Image[] CurrentPlayerLives;

    public TMP_Text MedKitPrice;
    public TMP_Text LiveAddPrice;

    public TMP_Text SellWeaponPrice;

    public TMP_Text _UpgradeDamageTextInUI;
    public Image _UpgradeDamageBarInUI;
    public TMP_Text _UpgradeDamagePriceInUI;

    public TMP_Text _UpgradeFireRateTextInUI;
    public Image _UpgradeFireRateBarInUI;
    public TMP_Text _UpgradeFireRatePriceInUI;

    public TMP_Text _UpgradeClipSizeTextInUI;
    public Image _UpgradeClipSizeBarInUI;
    public TMP_Text _UpgradeClipSizePriceInUI;

    public TMP_Text _ShieldPrice;

    public Button[] WeaponUpgradesButtons;

    public Button[] ShieldUpgradesButtons;
    public Button ShieldBuyButton;
    public Button ShieldUpgradeCapacityButton;
    public Button ShieldFillButton;

    public Image _ShieldCapacityBarInUI;
    public TMP_Text _ShieldCapacityTextInUI;
    public TMP_Text _ShieldUpgradePriceInUI;

    public Image _ShieldBarInUI;
    private readonly int _ShieldFillPrice = 200;

    private Button.ButtonClickedEvent onClick;

    private UnityEvent OnBoughtShieldEvent;

    private void Awake()
    {
        if (_UiManager == null)
            _UiManager = GameObject.FindGameObjectWithTag("UiSystem").GetComponent<UIManager>();
        if (_CurrentPlayer == null)
            _CurrentPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        onClick = new Button.ButtonClickedEvent();
        onClick.AddListener(new UnityAction(() => _AudioManager.PlayItemShopClickSound()));

        OnBoughtShieldEvent = new UnityEvent();
        OnBoughtShieldEvent.AddListener(new UnityAction(() => _CurrentPlayer.AddMyShieldEffect()));
    }
    private void OnEnable()
    {
        _AudioManager.PlayItemShopSound();
        _UiManager.UpdateCashInItemShop();
        SetCurrentAmmoInUI();
        SetCurrentHealthInUI();
        SetCurrentLivesInUI();
        if (_PlayerStats._PlayerStats.ShieldCapacity > 0)
        {
            EnableShieldButtons();
            SetCurrentShieldInUI();
            SetCurrentShieldCapacityInUI();
        }
        if (_PlayerStats._PlayerStats.HasWeapon)
        {
            EnableWeaponUpgradeButtons();
            SetCurrentWeaponDamageInUI();
            SetCurrentWeaponFireRateInUI();
            SetCurrentWeaponClipSizeInUI();
            SetWeapon();
            SetAmmoContentInUI();
        }
    }
    private void OnDisable()
    {
        _AudioManager.StopPlayingItemShopSound();
    }
    private void SetAmmoContentInUI()
    {
        _UiManager.ShowAmmoContentInUI();
        _UiManager.SetAmmoBulletsInUI(_WeaponInStore._Weapon.Ammo);
    }
    private void SetCurrentWeaponDamageInUI()
    {
        _UpgradeDamageTextInUI.text = _WeaponInStore._Weapon._Damage.ToString();
        if (_WeaponInStore._Weapon._UpgradeDamageLevel != _WeaponInStore._Weapon._MaxUpgradeDamageLevel)
            _UpgradeDamagePriceInUI.text = _WeaponInStore._Weapon.UpgradeDamagePrices[_WeaponInStore._Weapon._UpgradeDamageLevel] + " $";
        else
        {
            _UpgradeDamagePriceInUI.text = "Max Upgrade";
            WeaponUpgradesButtons[0].interactable = false;
        }
        _UpgradeDamageBarInUI.fillAmount = 1 / 5f * _WeaponInStore._Weapon._UpgradeDamageLevel;
    }
    private void SetCurrentWeaponFireRateInUI()
    {
        _UpgradeFireRateTextInUI.text = _WeaponInStore._Weapon.FireRate.ToString();
        if (_WeaponInStore._Weapon.UpgradeFireRateLevel != _WeaponInStore._Weapon._MaxUpgradeFireRateLevel)
            _UpgradeFireRatePriceInUI.text = _WeaponInStore._Weapon.UpgradeFireRatePrices[_WeaponInStore._Weapon.UpgradeFireRateLevel] + " $";
        else
        {
            _UpgradeFireRatePriceInUI.text = "Max Upgrade";
            WeaponUpgradesButtons[1].interactable = false;
        }
        _UpgradeFireRateBarInUI.fillAmount = 1 / 4f * _WeaponInStore._Weapon.UpgradeFireRateLevel;
    }
    private void SetCurrentWeaponClipSizeInUI()
    {
        _UpgradeClipSizeTextInUI.text = _WeaponInStore._Weapon.ClipSize.ToString();
        if (_WeaponInStore._Weapon._UpgradeClipSizeLevel != _WeaponInStore._Weapon._MaxUpgradeClipSizeLevel)
            _UpgradeClipSizePriceInUI.text = _WeaponInStore._Weapon.UpgradeClipSizePrices[_WeaponInStore._Weapon._UpgradeClipSizeLevel] + " $";
        else
        {
            _UpgradeClipSizePriceInUI.text = "Max Upgrade";
            WeaponUpgradesButtons[2].interactable = false;
        }
        _UpgradeClipSizeBarInUI.fillAmount = 1 / 4f * _WeaponInStore._Weapon._UpgradeClipSizeLevel;
    }
    private void SetCurrentLivesInUI()
    {
        for (int i = _PlayerStats._PlayerStats.Lives - 1; i >= 0; --i)
        {
            CurrentPlayerLives[i].sprite = _UiManager.Live.sprite;
        }
        for (int i = _PlayerStats._PlayerStats.Lives; i < 3; ++i)
        {
            CurrentPlayerLives[i].sprite = _UiManager.noLive.sprite;
        }
    }
    private void SetCurrentShieldInUI()
    {
        _ShieldBarInUI.fillAmount = _PlayerStats._PlayerStats.Shield / 100f;
    }
    private void SetCurrentShieldCapacityInUI()
    {
        if (_PlayerStats._PlayerStats.ShieldCapacity == 100)
        {
            FullShieldCapacity();
        }
        else
        {
            _ShieldUpgradePriceInUI.text = (_PlayerStats._PlayerStats.ShieldCapacity / 25 * 100) + " $";
            ShieldBuyButton.interactable = false;
            _ShieldCapacityBarInUI.fillAmount = _PlayerStats._PlayerStats.ShieldCapacity / 100f;
            _ShieldCapacityTextInUI.text = _PlayerStats._PlayerStats.ShieldCapacity.ToString();
        }
    }
    private void SetCurrentHealthInUI()
    {
        CurrentHealthBar.fillAmount = _PlayerStats._PlayerStats.Health / 100f;
    }
    public void BuyWeapon()
    {
        onClick.Invoke();
        int PriceOfWeapon = _UiManager.GetCashFromUI(_WeaponBuyButton.transform.GetChild(0).GetComponent<TMP_Text>().text);
        if (_PlayerStats._PlayerStats.MyCash - PriceOfWeapon >= 0)
        {
            _CurrentPlayer.LoseCash(PriceOfWeapon);
            _UiManager.SetCash(_PlayerStats._PlayerStats.MyCash);
            _AudioManager.PlayBoughtCashSound();
            _UiManager.UpdateCashInItemShop();
            _CurrentPlayer.AttachWeapon();
            SetWeapon();
            EnableWeaponUpgradeButtons();
            SetCurrentWeaponDamageInUI();
            SetCurrentWeaponFireRateInUI();
            SetCurrentWeaponClipSizeInUI();
            SetAmmoContentInUI();
        }
        else
        {
            //Can not Buy the weapon
            _UiManager.ShowNotEnoughMoneyPanel();
        }
    }
    public void SellWeapon()
    {
        onClick.Invoke();
        _UiManager.ShowSellWeaponConfirmation();
    }
    public void SellConfirmAnswer(Button YesOrNoBtn)
    {
        onClick.Invoke();
        if (YesOrNoBtn.name == "Yes")
        {
            int sellPrice = _UiManager.GetCashFromUI(SellWeaponPrice.text);
            _CurrentPlayer.AddCash(sellPrice);
            _UiManager.SetCash(_PlayerStats._PlayerStats.MyCash);
            _AudioManager.PlayGotCashSound();
            _UiManager.UpdateCashInItemShop();
            UnsetWeapon();
            _CurrentPlayer.DetachWeapon();
            _UiManager.HideAmmoContentInUI();
            DisableWeaponUpgradeButtons();
        }
        //Pressed No
        _UiManager.HideSellWeaponConfirmation();
    }
    public void BuyMedKit()//Health
    {
        onClick.Invoke();
        int MedkitPrice = _UiManager.GetCashFromUI(MedKitPrice.text);
        if (_PlayerStats._PlayerStats.MyCash - MedkitPrice >= 0)
        {
            if (_PlayerStats._PlayerStats.Health < 100)
            {
                _CurrentPlayer.LoseCash(MedkitPrice);
                _UiManager.SetCash(_PlayerStats._PlayerStats.MyCash);
                _AudioManager.PlayBoughtCashSound();
                _UiManager.UpdateCashInItemShop();
                _PlayerStats._PlayerStats.Health = 100;
                CurrentHealthBar.fillAmount = 1f;
                _UiManager.RefreshHealth();
            }
        }
        else
        {
            _UiManager.ShowNotEnoughMoneyPanel();
        }
    }
    public void BuyLive()
    {
        onClick.Invoke();
        int ToAddLivePrice = _UiManager.GetCashFromUI(LiveAddPrice.text);
        if (_PlayerStats._PlayerStats.MyCash - ToAddLivePrice >= 0)
        {
            if (_PlayerStats._PlayerStats.Lives < 3)
            {
                _CurrentPlayer.LoseCash(ToAddLivePrice);
                _UiManager.SetCash(_PlayerStats._PlayerStats.MyCash);
                _AudioManager.PlayBoughtCashSound();
                _UiManager.UpdateCashInItemShop();
                _PlayerStats._PlayerStats.Lives++;
                SetCurrentLivesInUI();
                _UiManager.RefreshLives();
                _AudioManager.PlayGotLiveSound();
            }
        }
        else
        {
            _UiManager.ShowNotEnoughMoneyPanel();
        }
    }
    private void SetWeapon()
    {
        _WeaponBuyButton.interactable = false;
        _UiManager.ActivateSell();
    }
    private void UnsetWeapon()
    {
        _WeaponBuyButton.interactable = true;
        _UiManager.DeactiveSell();
    }
    public void ReadNotEnoughtMoney()
    {
        onClick.Invoke();
        _UiManager.HideNotEnoughMoneyPanel();
    }
    public void ReadMessageQuanity()
    {
        onClick.Invoke();
        _UiManager.HideMessageQuanityPanel();
    }
    //Weapon , Health , Armor
    public void SwitchContent(Button ItemType)
    {
        onClick.Invoke();
        int ContentIndex = ItemType.transform.GetSiblingIndex();
        if (ContentIndex != _CurrentActiveItemStoreContent)
        {
            _UiManager.SwitchItemShopContentUI(ContentIndex, _CurrentActiveItemStoreContent);
            _CurrentActiveItemStoreContent = ContentIndex;
        }
    }
    public void IncreaseQuanity(TMP_Text quanityValue)
    {
        onClick.Invoke();
        AmmoQuanityAmount = Convert.ToInt32(quanityValue.text);
        if (AmmoQuanityAmount < _WeaponInStore._Weapon.ClipSize)
        {
            AmmoQuanityAmount++;
            quanityValue.text = AmmoQuanityAmount.ToString();
            _CurrentAmmoPrice = AmmoQuanityAmount * 5;
            _UiManager.SetAmmoPrice(ref _AmmoPriceInUI, _CurrentAmmoPrice);
        }
    }
    public void DecreaseQuanity(TMP_Text quanityValue)
    {
        onClick.Invoke();
        AmmoQuanityAmount = Convert.ToInt32(quanityValue.text);
        if (AmmoQuanityAmount > 0)
        {
            AmmoQuanityAmount--;
            quanityValue.text = AmmoQuanityAmount.ToString();
            _CurrentAmmoPrice = AmmoQuanityAmount * 5;
            _UiManager.SetAmmoPrice(ref _AmmoPriceInUI, _CurrentAmmoPrice);
        }
    }
    public void BuyAmmo(TMP_Text quanityValue)
    {
        onClick.Invoke();
        if (_PlayerStats._PlayerStats.MyCash - _CurrentAmmoPrice >= 0)
        {
            if (AmmoQuanityAmount != 0)
            {
                if (AmmoQuanityAmount + _WeaponInStore._Weapon.Ammo <= _WeaponInStore._Weapon.ClipSize)
                {
                    _WeaponInStore._Weapon.Ammo += AmmoQuanityAmount;
                    SetCurrentAmmoInUI();
                    _CurrentPlayer.LoseCash(_CurrentAmmoPrice);
                    _UiManager.SetAmmoBulletsInUI(_WeaponInStore._Weapon.Ammo);
                    _UiManager.SetCash(_PlayerStats._PlayerStats.MyCash);
                    _AudioManager.PlayBoughtCashSound();
                    _UiManager.UpdateCashInItemShop();
                }
                else
                {
                    quanityValue.text = 0.ToString();
                }
            }
            else
            {
                _UiManager.ShowMessageQuanityPanel();
            }
        }
        else
        {
            _UiManager.ShowNotEnoughMoneyPanel();
        }
    }
    private void SetCurrentAmmoInUI()
    {
        CurrentAmmoValueInUI.text = _WeaponInStore._Weapon.Ammo.ToString();
        CurrentAmmoBar.fillAmount = _WeaponInStore._Weapon.Ammo / _WeaponInStore._Weapon.ClipSize;
    }
    public void UpgradeWeaponDamage()
    {
        onClick.Invoke();
        if (_WeaponInStore._Weapon._UpgradeDamageLevel < _WeaponInStore._Weapon._MaxUpgradeDamageLevel)
            UpgradeWeapon(ref _UpgradeDamageTextInUI, ref _UpgradeDamageBarInUI, ref _UpgradeDamagePriceInUI, ref _WeaponInStore._Weapon._UpgradeDamageLevel, ref _WeaponInStore._Weapon._MaxUpgradeDamageLevel, ref _WeaponInStore._Weapon.UpgradeDamagePrices, 1 / 5f, 5f, ref _WeaponInStore._Weapon._Damage, ref WeaponUpgradesButtons[0]);
    }
    public void UpgradeWeaponFireRate()
    {
        onClick.Invoke();
        if (_WeaponInStore._Weapon.UpgradeFireRateLevel < _WeaponInStore._Weapon._MaxUpgradeFireRateLevel)
            UpgradeWeapon(ref _UpgradeFireRateTextInUI, ref _UpgradeFireRateBarInUI, ref _UpgradeFireRatePriceInUI, ref _WeaponInStore._Weapon.UpgradeFireRateLevel, ref _WeaponInStore._Weapon._MaxUpgradeFireRateLevel, ref _WeaponInStore._Weapon.UpgradeFireRatePrices, 1 / 4f, -0.2f, ref _WeaponInStore._Weapon.FireRate, ref WeaponUpgradesButtons[1]);
    }
    public void UpgradeWeaponClipSize()
    {
        onClick.Invoke();
        if (_WeaponInStore._Weapon._UpgradeClipSizeLevel < _WeaponInStore._Weapon._MaxUpgradeClipSizeLevel)
        {
            UpgradeWeapon(ref _UpgradeClipSizeTextInUI, ref _UpgradeClipSizeBarInUI, ref _UpgradeClipSizePriceInUI, ref _WeaponInStore._Weapon._UpgradeClipSizeLevel, ref _WeaponInStore._Weapon._MaxUpgradeClipSizeLevel, ref _WeaponInStore._Weapon.UpgradeClipSizePrices, 1 / 5f, 10f, ref _WeaponInStore._Weapon.ClipSize, ref WeaponUpgradesButtons[2]);
            SetCurrentAmmoInUI();
        }
    }
    private void UpgradeWeapon(ref TMP_Text _UpgradeTextInUI,
        ref Image _UpgradeBarInUI,
        ref TMP_Text _UpgradePriceInUI,
        ref int _UpgradeLevel,
        ref int _MaxUpgradeLevel,
        ref int[] _UpgradePrices,
        float _UpgradeFillAmount,
        float _UpgradeAmount,
        ref float _UpdatedSpecificWeaponStat,
        ref Button UpgradeButton)
    {
        int CashToLose = _UpgradePrices[_UpgradeLevel];
        if (_PlayerStats._PlayerStats.MyCash - CashToLose >= 0)
        {
            _UpgradeLevel++;
            _UpdatedSpecificWeaponStat = float.Parse(_UpgradeTextInUI.text) + _UpgradeAmount;
            _CurrentPlayer.LoseCash(CashToLose);
            _UiManager.SetCash(_PlayerStats._PlayerStats.MyCash);
            _AudioManager.PlayBoughtCashSound();
            _UiManager.UpdateCashInItemShop();
            _UpgradeBarInUI.fillAmount += _UpgradeFillAmount;
            _UpgradeTextInUI.text = _UpdatedSpecificWeaponStat.ToString();
            if (_UpgradeLevel != _MaxUpgradeLevel)
            {
                _UpgradePriceInUI.text = _UpgradePrices[_UpgradeLevel] + " $";
            }
            else
            {
                _UpgradePriceInUI.text = "Max Upgrade";
                UpgradeButton.interactable = false;
            }
        }
        else
        {
            _UiManager.ShowNotEnoughMoneyPanel();
        }
    }
    private void EnableWeaponUpgradeButtons()
    {
        for (int i = 0; i < WeaponUpgradesButtons.Length; ++i)
        {
            WeaponUpgradesButtons[i].interactable = true;
        }
    }
    private void DisableWeaponUpgradeButtons()
    {
        for (int i = 0; i < WeaponUpgradesButtons.Length; ++i)
        {
            WeaponUpgradesButtons[i].interactable = false;
        }
    }
    public void BuyShield()
    {
        onClick.Invoke();
        int ShieldPrice = _UiManager.GetCashFromUI(_ShieldPrice.text);
        if (_PlayerStats._PlayerStats.MyCash - ShieldPrice >= 0)
        {
            _CurrentPlayer.LoseCash(ShieldPrice);
            _UiManager.SetCash(_PlayerStats._PlayerStats.MyCash);
            _AudioManager.PlayBoughtCashSound();
            _UiManager.UpdateCashInItemShop();
            EnableShieldButtons();
            ShieldBuyButton.interactable = false;
            _AudioManager.PlayGotShieldSound();
            EnableShieldAfterBuy();
        }
        else
        {
            _UiManager.ShowNotEnoughMoneyPanel();
        }
    }
    private void EnableShieldAfterBuy()
    {
        _ShieldCapacityBarInUI.fillAmount = 0.25f;
        _ShieldBarInUI.fillAmount = 0.25f;
        _ShieldCapacityTextInUI.text = "25";
        _PlayerStats._PlayerStats.Shield = 25;
        _PlayerStats._PlayerStats.ShieldCapacity = 25;
        _UiManager.FillShield(_PlayerStats._PlayerStats.ShieldCapacity);
        OnBoughtShieldEvent.Invoke();
    }
    private void EnableShieldButtons()
    {
        for (int i = 0; i < ShieldUpgradesButtons.Length; ++i)
        {
            ShieldUpgradesButtons[i].interactable = true;
            ShieldUpgradesButtons[i].GetComponent<Image>().color = Color.white;
        }
    }
    public void UpgradeShieldCapacity()
    {
        onClick.Invoke();
        int shieldcapPrice = _UiManager.GetCashFromUI(_ShieldUpgradePriceInUI.text);
        if (_PlayerStats._PlayerStats.MyCash - shieldcapPrice >= 0)
        {
            int ShieldCapacityValue = int.Parse(_ShieldCapacityTextInUI.text);
            _CurrentPlayer.LoseCash(shieldcapPrice);
            _UiManager.SetCash(_PlayerStats._PlayerStats.MyCash);
            _AudioManager.PlayBoughtCashSound();
            _UiManager.UpdateCashInItemShop();
            if (ShieldCapacityValue < 100)
            {
                ShieldCapacityValue += 25;
                _PlayerStats._PlayerStats.ShieldCapacity = ShieldCapacityValue;
                _ShieldUpgradePriceInUI.text = shieldcapPrice + 100 + " $";
                _ShieldCapacityTextInUI.text = ShieldCapacityValue.ToString();
                _ShieldCapacityBarInUI.fillAmount += 1 / 4f;
                if (ShieldCapacityValue == 100)
                {
                    FullShieldCapacity();
                }
            }
        }
        else
        {
            _UiManager.ShowNotEnoughMoneyPanel();
        }
    }
    private void FullShieldCapacity()
    {
        ShieldBuyButton.interactable = false;
        ShieldUpgradeCapacityButton.interactable = false;
        ShieldUpgradeCapacityButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Max Upgraded";
        ShieldUpgradeCapacityButton.GetComponent<Image>().color = Color.gray;
        _ShieldUpgradePriceInUI.text = "Full";
        _ShieldCapacityTextInUI.text = _PlayerStats._PlayerStats.ShieldCapacity.ToString();
        _ShieldCapacityBarInUI.fillAmount = _PlayerStats._PlayerStats.ShieldCapacity / 100f;
    }
    public void FillShield()
    {
        onClick.Invoke();
        if (_PlayerStats._PlayerStats.Shield != _PlayerStats._PlayerStats.ShieldCapacity)
        {
            if (_PlayerStats._PlayerStats.MyCash - _ShieldFillPrice >= 0)
            {
                _CurrentPlayer.LoseCash(_ShieldFillPrice);
                _UiManager.SetCash(_PlayerStats._PlayerStats.MyCash);
                _AudioManager.PlayBoughtCashSound();
                _UiManager.UpdateCashInItemShop();
                _ShieldBarInUI.fillAmount = _PlayerStats._PlayerStats.ShieldCapacity / 100f;
                _PlayerStats._PlayerStats.Shield = _PlayerStats._PlayerStats.ShieldCapacity;
                _UiManager.FillShield(_PlayerStats._PlayerStats.ShieldCapacity);
                OnBoughtShieldEvent.Invoke();
            }
            else
            {
                _UiManager.ShowNotEnoughMoneyPanel();
            }
        }
    }
}