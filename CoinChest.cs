using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
/**
 * Coin Chest In-Game Behaviour
 */
public class CoinChest : MonoBehaviour, IComparable<CoinChest>
{
    public Player _CurrentPlayer;
    public UIManager _UIManager;
    private AudioManager _AudioManager;
    public GameQuests _GameQuestsManager;

    private int RandomCashAmount;

    public Sprite ChestOpenedSprite;
    public Sprite ChestTookCashSprite;

    public int CoinChestID;

    private UnityEvent OnCoinChestOpenedEvent;

    private void Awake()
    {
        if (_CurrentPlayer == null)
            _CurrentPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (_UIManager == null)
            _UIManager = GameObject.FindGameObjectWithTag("UiSystem").GetComponent<UIManager>();
        GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);
        if (_GameQuestsManager == null)
            GameObject.FindGameObjectWithTag("GameQuest")?.TryGetComponent(out _GameQuestsManager);

        OnCoinChestOpenedEvent = new UnityEvent();
        OnCoinChestOpenedEvent = new UnityEvent();
        OnCoinChestOpenedEvent.AddListener(() => _GameQuestsManager.OnCoinChestOpened());
    }

    /*give the chest a random amount of cash inside*/
    private void Start()
    {
        RandomCashAmount = UnityEngine.Random.Range(1200, 1500);
    }
    /*the chest was opened now add cash to the player*/
    public void OpenChest()
    {
        StartCoroutine(OnTakingChestCash());
    }
    public void OnChestOpened()
    {
        GetComponent<SpriteRenderer>().sprite = ChestTookCashSprite;
    }

    private IEnumerator OnTakingChestCash()
    {
        GetComponent<SpriteRenderer>().sprite = ChestOpenedSprite;
        yield return new WaitForSeconds(1f);
        _AudioManager?.PlayGotCashSound();
        _CurrentPlayer.AddCash(RandomCashAmount);
        _UIManager.SetCash(_CurrentPlayer._Stats._PlayerStats.MyCash);
        GetComponent<SpriteRenderer>().sprite = ChestTookCashSprite;
        OnCoinChestOpenedEvent.Invoke();
    }
    public int CompareTo(CoinChest other)
    {
        return CoinChestID - other.CoinChestID;
    }
}