using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // DontDestroyOnLoad(gameObject);

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    public void FadeAndLoad(string sceneName, bool useLoadingScreen)
    {
        StartCoroutine(FadeAndLoadCoroutine(sceneName, useLoadingScreen));
    }

    private IEnumerator FadeAndLoadCoroutine(string sceneName, bool useLoadingScreen)
    {
        yield return FadeInCoroutine();

        if (useLoadingScreen)
        {
            SceneManager.LoadScene("LoadingScene");
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public IEnumerator FadeInCoroutine()
    {
        if (canvasGroup == null) yield break;

        canvasGroup.gameObject.SetActive(true);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    public IEnumerator FadeOutCoroutine()
    {
        if (canvasGroup == null) yield break;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.gameObject.SetActive(false);
    }
}
