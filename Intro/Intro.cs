using UnityEngine;
using UnityEngine.SceneManagement;
/*Scene Introduction Manager When new game Starts*/
public class Intro : StateMachineBehaviour
{
    private bool EnteringGame;

    private void Awake()
    {
        Time.timeScale = 1.0f;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        StartGame();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (!EnteringGame && Input.GetKey(KeyCode.E))//Skipping Intro
        {
            StartGame();
            EnteringGame = true;
        }
    }
    private void StartGame()
    {
        int StartGameIndex = 2;
        PlayerPrefs.SetInt("Scene", StartGameIndex);
        SceneManager.LoadSceneAsync("LoadingScreen");
    }
}