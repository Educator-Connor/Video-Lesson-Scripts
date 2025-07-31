using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    public Slider progressBar;
    public TMP_Text progressText;
    public float minLoadTime = 2f;

    private void Start()
    {
        StartCoroutine(LoadAsync());
    }

    private IEnumerator LoadAsync()
    {
        string sceneToLoad = SceneLoader.TargetScene;

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("Target scene not set. Returning to Main Menu.");
            SceneManager.LoadScene("MainMenu");
            yield break;
        }

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneToLoad);
        op.allowSceneActivation = false;

        float elapsedAfterLoad = 0f;

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);

            if (progressBar != null)
                progressBar.value = progress;

            if (progressText != null)
                progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";

            if (op.progress >= 0.9f)
            {
                elapsedAfterLoad += Time.deltaTime;

                if (elapsedAfterLoad >= minLoadTime)
                {
                    Debug.Log("Minimum load time reached. Fading out and activating scene.");
                    if (TransitionManager.Instance != null)
                        yield return TransitionManager.Instance.FadeOutCoroutine();

                    op.allowSceneActivation = true;
                    yield break;
                }
            }

            yield return null;
        }
    }
}
