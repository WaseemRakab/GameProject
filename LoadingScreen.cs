using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * LoadaingScreen Handler For Waitting Time 
 * Until Next Level (Scene)
 * Actually Loaded in Memory
 */
public class LoadingScreen : MonoBehaviour
{
    private readonly float WaitTime = 1f;

    private int SceneIndex;
    private void Awake()
    {
        Time.timeScale = 1f;
    }
    void Start()
    {
        SceneIndex = PlayerPrefs.GetInt("Scene");
        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel()
    {
        yield return new WaitForSeconds(WaitTime);
        AsyncOperation sync = SceneManager.LoadSceneAsync(SceneIndex);
        sync.allowSceneActivation = false;
        while (!sync.isDone)
        {
            if (sync.progress >= 0.9f)
            {
                sync.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}