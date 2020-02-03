using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * In-Game Quests Manager
 * Handling which Quest to-do
 * inorder to complete the level
 */
public class GameQuests : MonoBehaviour
{
    public UIManager _UIManager;
    public AudioManager _AudioManager;
    public CoinChestsManager _CoinChestsManager;
    public Player CurrentPlayer;
    public ScriptableSpawnObjects _SpawnObjects;

    private Button.ButtonClickedEvent onClick;

    public GameObject OptionalQuest1;
    public GameObject OptionalQuest2;
    public GameObject MainQuestsContent;

    public MainQuestsMethodInvoker _MainQuestsMethodInvoker;

    private int _CurrentActiveQuestContent;

    private int LevelNumber;
    private int StageNumber;

    public TMP_Text CoinChestsTakenValue;
    public TMP_Text RewardOnKillingAllEnemiesPrice;

    public TMP_Text CurrentEnemiesKilledValue;

    private int AllEnemiesInCurrentLevelCount;
    private int EnemiesKilledCount;

    private int CoinChestsTaken;

    private int OnKillingAllEnemiesPrice;

    //Main Quests , Initialized
    public GameObject FirstQuest;
    public GameObject SecondQuest;
    public GameObject ThirdQuest;

    private void Awake()
    {
        if (_UIManager == null)
            GameObject.FindGameObjectWithTag("UiSystem")?.TryGetComponent(out _UIManager);
        if (_AudioManager == null)
            GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);
        if (_CoinChestsManager == null)
            GameObject.FindGameObjectWithTag("CoinChestsManager")?.TryGetComponent(out _CoinChestsManager);
        if (CurrentPlayer == null)
            GameObject.FindGameObjectWithTag("Player")?.TryGetComponent(out CurrentPlayer);
        if (_MainQuestsMethodInvoker == null)
            GameObject.FindGameObjectWithTag("MainQuestsMethodInvoker")?.TryGetComponent(out _MainQuestsMethodInvoker);

        onClick = new Button.ButtonClickedEvent();
        onClick.AddListener(new UnityAction(() => _AudioManager.ClickSound()));

        int CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        LevelNumber = (CurrentSceneIndex + 1) % 3 + 1;
        StageNumber = (CurrentSceneIndex - LevelNumber) / 3 + 1;

        _MainQuestsMethodInvoker.WhichMethod.Invoke();

        RefreshRewardOnKillingAllEnemiesPrice();
        RefreshEnemiesKilledCount();
        RefreshCoinChestsTakenValue();
    }
    //Event Invoke
    public void RefreshFirstStageWithoutBossLevels()
    {
        FirstQuest.transform.GetChild(0).gameObject.SetActive(true); //First Quest Text Enable
        SecondQuest.transform.GetChild(2).gameObject.SetActive(true);
        if (CurrentPlayer.HasBeenInSecretChest)
        {
            FirstQuest.transform.GetChild(4).GetChild(0).gameObject.SetActive(true);//Enabling the CheckMark , Quest Is Already Done
                                                                                    //By SaveGame Etc..
            SecondQuest.transform.GetChild(5).gameObject.SetActive(false);
            //Unlocking The Second Quest , If Player Already got Secret Chest on SaveGame
        }
        FirstQuest.SetActive(true);
        SecondQuest.SetActive(true);
    }
    //Event Invoke
    public void RefreshBossLevels(bool FinalBoss = false)
    {
        FirstQuest.transform.GetChild(3).gameObject.SetActive(true); //First Quest Text Enable
        FirstQuest.SetActive(true);
        if (FinalBoss)
        {
            SecondQuest.transform.GetChild(3).gameObject.SetActive(true); // Setting Hidden Artifact
            SecondQuest.SetActive(true);
        }
    }
    //Event Invoke
    public void RefreshSecondStageWithoutBossLevels()
    {
        FirstQuest.transform.GetChild(0).gameObject.SetActive(true);
        SecondQuest.transform.GetChild(1).gameObject.SetActive(true);//Setting the Text On The GameObject
        ThirdQuest.transform.GetChild(2).gameObject.SetActive(true);//Setting the Text On The GameObject(Activating)

        if (CurrentPlayer.HasBeenInSecretChest)
        {
            //Enabling the CheckMark , Quest Is Already Done
            //By SaveGame Etc..
            FirstQuest.transform.GetChild(4).GetChild(0).gameObject.SetActive(true);

            SecondQuest.transform.GetChild(5).gameObject.SetActive(false);
            //Unlocking The Second Quest , If Player Already got Secret Chest on SaveGame

            if (CurrentPlayer.HasBeenInDarkAreaTrigger)
            {
                SecondQuest.transform.GetChild(4).GetChild(0).gameObject.SetActive(true);

                ThirdQuest.transform.GetChild(5).gameObject.SetActive(false);
            }
        }
        FirstQuest.SetActive(true);
        SecondQuest.SetActive(true);
        ThirdQuest.SetActive(true);
    }
    private void RefreshEnemiesKilledCount()
    {
        AllEnemiesInCurrentLevelCount =
            _SpawnObjects._SpawnObjects.EnemiesWithPistolAlive.Length +
            _SpawnObjects._SpawnObjects.EnemiesWithAutoGunWithoutBunkerAlive.Length +
            _SpawnObjects._SpawnObjects.EnemiesWithAutoGunWithBunkerAlive.Length +
            _SpawnObjects._SpawnObjects.TurrentLaunchersAlive.Length +
            _SpawnObjects._SpawnObjects.TurrentLaunchersWallAlive.Length;

        EnemiesKilledCount =
            _SpawnObjects._SpawnObjects.EnemiesWithPistolAlive.Count((val) => CheckIfEnemyDead(val)) +
            _SpawnObjects._SpawnObjects.EnemiesWithAutoGunWithoutBunkerAlive.Count((val) => CheckIfEnemyDead(val)) +
            _SpawnObjects._SpawnObjects.EnemiesWithAutoGunWithBunkerAlive.Count((val) => CheckIfEnemyDead(val)) +
            _SpawnObjects._SpawnObjects.TurrentLaunchersAlive.Count((val) => CheckIfEnemyDead(val)) +
            _SpawnObjects._SpawnObjects.TurrentLaunchersWallAlive.Count((val) => CheckIfEnemyDead(val));

        CurrentEnemiesKilledValue.text = EnemiesKilledCount + "/" + AllEnemiesInCurrentLevelCount;
        if (AllEnemiesInCurrentLevelCount == EnemiesKilledCount)
            OptionalQuest2.SetActive(true);
    }
    private bool CheckIfEnemyDead(bool value)
    {
        return value == false;
    }
    private void RefreshRewardOnKillingAllEnemiesPrice()
    {
        OnKillingAllEnemiesPrice = 150 * StageNumber + (LevelNumber * 200);
        RewardOnKillingAllEnemiesPrice.text = OnKillingAllEnemiesPrice.ToString() + " $";
    }
    public void SwitchContent(Button QuestType)
    {
        onClick.Invoke();
        int ContentIndex = QuestType.transform.GetSiblingIndex();
        if (ContentIndex != _CurrentActiveQuestContent)
        {
            _UIManager.SwitchQuestContentUI(ContentIndex, _CurrentActiveQuestContent);
            _CurrentActiveQuestContent = ContentIndex;
        }
    }
    private void RefreshCoinChestsTakenValue()
    {
        CoinChestsTaken = CurrentPlayer.HasBeenInCoinChests.Count((val) => val == true);
        CoinChestsTakenValue.text = CoinChestsTaken + "/" + _CoinChestsManager.CoinChests;
        if (CoinChestsTaken == _CoinChestsManager.CoinChests)
            OptionalQuest1.SetActive(true);
    }
    //Event Invoke
    public void OnEnemyKilled()
    {
        EnemiesKilledCount++;
        CurrentEnemiesKilledValue.text = EnemiesKilledCount + "/" + AllEnemiesInCurrentLevelCount;
        if (AllEnemiesInCurrentLevelCount == EnemiesKilledCount)
        {
            OptionalQuest2.SetActive(true);
            CurrentPlayer.AddCash(OnKillingAllEnemiesPrice);
            _UIManager.SetCash(CurrentPlayer._Stats._PlayerStats.MyCash);
        }
    }
    public void OnFinalBossKilled()
    {
        FirstQuest.transform.GetChild(4).GetChild(0).gameObject.SetActive(true);
        SecondQuest.transform.GetChild(5).gameObject.SetActive(false);
    }
    public void OnArtifactOpened()
    {
        SecondQuest.transform.GetChild(4).GetChild(0).gameObject.SetActive(true);
    }
    //Event Invoke
    public void OnCoinChestOpened()
    {
        CoinChestsTaken++;
        CoinChestsTakenValue.text = CoinChestsTaken + "/" + _CoinChestsManager.CoinChests;
        if (CoinChestsTaken == _CoinChestsManager.CoinChests)
            OptionalQuest1.SetActive(true);
    }
    //Event Invoke
    public void OnSecretChestOpened()
    {
        FirstQuest.transform.GetChild(4).GetChild(0).gameObject.SetActive(true);


        SecondQuest.transform.GetChild(5).gameObject.SetActive(false);
    }
    //Event Invoke
    public void OnFinishDoorOpenedOnFirstStage()
    {
        SecondQuest.transform.GetChild(4).GetChild(0).gameObject.SetActive(true);
    }
    public void OnFinishDoorOpenedOnSecondStage()
    {
        ThirdQuest.transform.GetChild(4).GetChild(0).gameObject.SetActive(true);
    }
    //Event Invoke
    public void OnFinishingBossLevel()
    {
        FirstQuest.transform.GetChild(4).GetChild(0).gameObject.SetActive(true);
    }
    //Event Invoke
    public void OnDarkAreaOpened()
    {
        SecondQuest.transform.GetChild(4).GetChild(0).gameObject.SetActive(true);

        ThirdQuest.transform.GetChild(5).gameObject.SetActive(false);
    }
}