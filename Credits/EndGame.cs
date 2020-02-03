using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/* Credits Scene onGameEnded*/
public class EndGame : MonoBehaviour
{
    private CreditsCamera CameraTracker;

    public Image OnWayCredits;
    private Color AfterOnWayCredits;

    public Image EndGameCredits;
    private Color AfterEndGameCredits;

    private const float FadeTime = 3.0f;
    private void Awake()
    {
        CameraTracker = transform.parent.GetComponent<CreditsCamera>();

        Color OnWayCredits = this.OnWayCredits.color;
        OnWayCredits.a = 0f;
        AfterOnWayCredits = OnWayCredits;

        Color EndGameCredits = this.EndGameCredits.color;
        EndGameCredits.a = 1.0f;
        AfterEndGameCredits = EndGameCredits;

    }
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2.0f);
        yield return new WaitUntil(() => CameraTracker.ReachedEndOfGameCredits());
        yield return StartCoroutine(FadeGameCreditsOut());
        yield return StartCoroutine(FadeEndGameCreditsIn());
        yield return new WaitForSeconds(3.0f);
        int MainMenuSceneIndex = 0;
        PlayerPrefs.SetInt("Scene", MainMenuSceneIndex);
        yield return SceneManager.LoadSceneAsync("LoadingScreen");
    }
    private IEnumerator FadeGameCreditsOut()
    {
        while (OnWayCredits.color != AfterOnWayCredits)
        {
            OnWayCredits.color = Color.Lerp(OnWayCredits.color, AfterOnWayCredits, Time.deltaTime * FadeTime);
            yield return null;
        }
    }
    private IEnumerator FadeEndGameCreditsIn()
    {
        while (EndGameCredits.color != AfterEndGameCredits)
        {
            EndGameCredits.color = Color.Lerp(EndGameCredits.color, AfterEndGameCredits, Time.deltaTime * FadeTime);
            yield return null;
        }
    }
}