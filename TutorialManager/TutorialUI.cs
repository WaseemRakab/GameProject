using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    private AudioManager _AudioManager;
    public Controls ControlsData;
    public TMP_Text TutorialText;

    public PlayerTutorial _PlayerTutorial;
    public EnemyDummyTutorial _EnemyTutorial;

    public GameObject TutorialMessages;
    public OnNextTutorial _onNextTutorial;

    public PushableBoxTutorial _PushableBoxTutorial;
    public SecretChestTutorial _SecrertChestTutorial;

    private readonly float MessageWaitTime = 0.07f;


    public GameObject TutorialFinishedPanel;

    public GameObject EndTutorialConfirmationMessage;

    private Button.ButtonClickedEvent onClick;


    private void Awake()
    {
        if (_PlayerTutorial == null)
            GameObject.FindGameObjectWithTag("Player")?.TryGetComponent(out _PlayerTutorial);
        if (_PushableBoxTutorial == null)
            GameObject.FindGameObjectWithTag("PushableBox")?.TryGetComponent(out _PushableBoxTutorial);
        if (_EnemyTutorial == null)
            GameObject.FindGameObjectWithTag("EnemyTutorial")?.TryGetComponent(out _EnemyTutorial);

        GameObject.FindGameObjectWithTag("AudioManager")?.TryGetComponent(out _AudioManager);
        onClick = new Button.ButtonClickedEvent();
        onClick.AddListener(() => _AudioManager.ClickSound());
    }
    private void Start()
    {
        StartCoroutine(TutorialInstructions());
    }
    private IEnumerator TutorialInstructions()
    {
        yield return StartCoroutine(TextTyping("Welcome To Game Tutorial !", MessageWaitTime));
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(TextTyping("Let's Start.", MessageWaitTime));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(TextTyping("Press " + ControlsData.Crouch + " To Crouch", MessageWaitTime));
        _PlayerTutorial.CanCrouch = true;
        yield return new WaitUntil(() => _PlayerTutorial.isCrouching);
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(TextTyping("Press " + ControlsData.Forward + " To Move Forward Or Press " + ControlsData.Backward + "To Move Backward", MessageWaitTime));
        _PlayerTutorial.CanMoveLeft = true;
        _PlayerTutorial.CanMoveRight = true;
        yield return new WaitUntil(() => _PlayerTutorial.MovedRight && _PlayerTutorial.MovedLeft);
        yield return StartCoroutine(TextTyping("Press " + ControlsData.Sprint + " While Holding " + ControlsData.Forward + " Or " + ControlsData.Backward + " To Run", MessageWaitTime));
        _PlayerTutorial.CanSprint = true;
        yield return new WaitUntil(() => _PlayerTutorial.isRunning);
        yield return StartCoroutine(TextTyping("Open The Gate To Go To The Next Step", MessageWaitTime));
        yield return new WaitUntil(() => _onNextTutorial.OnNextTutorialCheck);
        SetNextStepPositions(new Vector3(30.0f, 0f, 0f), new Vector3(33.0f, 0f, 0f));
        yield return StartCoroutine(TextTyping("Press " + ControlsData.Jump + " To Jump", MessageWaitTime));
        _PlayerTutorial.CanJump = true;
        yield return new WaitUntil(() => _PlayerTutorial.isJumping);
        yield return StartCoroutine(TextTyping("Open The Gate To Go To The Next Step", MessageWaitTime));
        yield return new WaitUntil(() => _onNextTutorial.OnNextTutorialCheck);
        SetNextStepPositions(new Vector3(65.0f, 5f, 0f), new Vector3(65.0f, 0f, 0f));
        yield return StartCoroutine(TextTyping("Hold " + ControlsData.Interact + " To Push The Box", MessageWaitTime));
        yield return new WaitUntil(() => _PushableBoxTutorial.PressedInteractPush);
        yield return StartCoroutine(TextTyping("Open The Gate To Go To The Next Step", MessageWaitTime));
        yield return new WaitUntil(() => _onNextTutorial.OnNextTutorialCheck);
        SetNextStepPositions(new Vector3(95.0f, 0f, 0f), new Vector3(95.0f, 0f, 0f));
        yield return StartCoroutine(TextTyping("Press " + ControlsData.Attack + " to Melee Attack", MessageWaitTime));
        _PlayerTutorial.CanAttack = true;
        yield return new WaitUntil(() => _EnemyTutorial.GotAttacked);
        _PlayerTutorial.SwitchToGunMode();
        yield return StartCoroutine(TextTyping("Now You Have A Weapon.", MessageWaitTime));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(TextTyping("Press " + ControlsData.Shoot + " To Shoot", MessageWaitTime));
        _PlayerTutorial.CanShoot = true;
        yield return new WaitUntil(() => _EnemyTutorial.EnemyIsDead);
        yield return StartCoroutine(TextTyping("Open The Gate To Go To The Next Step", MessageWaitTime));
        yield return new WaitUntil(() => _onNextTutorial.OnNextTutorialCheck);
        SetNextStepPositions(new Vector3(122.0f, 0f, 0f), new Vector3(114.0f, 0f, 0f));
        _PlayerTutorial.ReachedSecretChest = true;
        yield return StartCoroutine(TextTyping("Press " + ControlsData.Interact + " On The Chest To Get The Key", MessageWaitTime));
        yield return new WaitUntil(() => _SecrertChestTutorial.ChestOpened);
        _PlayerTutorial.ReachedSecretChest = false;
        yield return StartCoroutine(TextTyping("Enter The Door So You Can Finish This Tutorial", MessageWaitTime));
        yield return new WaitUntil(() => _onNextTutorial.OnNextTutorialCheck);
        yield return StartCoroutine(TextTyping("Congratulations!, Now You Can Start The Game.", MessageWaitTime));
        yield return new WaitForSeconds(2.0f);
        SetMessageOnFinishingTutorial();
    }
    private void SetMessageOnFinishingTutorial()
    {
        _AudioManager.PauseAudioOnPauseGame();
        TutorialFinishedPanel.SetActive(true);
    }
    public void EndThisTutorial()
    {
        onClick.Invoke();
        _AudioManager.PauseAudioOnPauseGame();
        ShowConfirmationMessage();
    }
    private void ShowConfirmationMessage()
    {
        EndTutorialConfirmationMessage.SetActive(true);
        Time.timeScale = 0f; // Paused
    }
    private void HideConfirmationMessage()
    {
        EndTutorialConfirmationMessage.SetActive(false);
        Time.timeScale = 1f;// UnPaused
    }
    public void ConfirmAnswerToEndTutorial(Button AnswerBtn)//SkipTutorial
    {
        onClick.Invoke();
        if (AnswerBtn.name == "Yes")
        {
            StopCoroutine(TutorialInstructions());
            StartGame();
        }
        else//Pressed No
        {
            HideConfirmationMessage();
        }
        _AudioManager.UnPauseAudioOnResumeGame();
    }
    public void TutorialFinishedConfirmAnswer(Button ButtonAnsw)
    {
        onClick.Invoke();
        if (ButtonAnsw.name == "Yes")
        {
            StartGame();
        }
        else
        {
            GoToMainMenu();
        }
        _AudioManager.UnPauseAudioOnResumeGame();
    }
    private void GoToMainMenu()
    {
        PlayerPrefs.SetInt("Scene", 0);//Main Menu
        SceneManager.LoadSceneAsync("LoadingScreen");
    }
    private void StartGame()
    {
        SceneManager.LoadSceneAsync(10);//Intro Scene
    }
    private void SetNextStepPositions(Vector3 TutorialMessagePos, Vector3 OnNextTutorialPos)
    {
        TutorialMessages.transform.position = TutorialMessagePos;
        _onNextTutorial.transform.position = OnNextTutorialPos;
        _onNextTutorial.OnNextTutorialCheck = false;
    }
    private IEnumerator TextTyping(string message, float WaitTime)
    {
        TutorialText.text = string.Empty;
        for (int i = 0; i < message.Length; ++i)
        {
            TutorialText.text += message[i];
            yield return new WaitForSeconds(WaitTime);
        }
    }
}