using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        switch (newScene.name)
        {
            case "MainMenu":
                AudioManager.Instance.PlayMusic(AudioManager.Instance.mainMenuMusic);
                break;

            case "GameOver":
                AudioManager.Instance.PlayMusic(AudioManager.Instance.gameOverMusic);
                break;

            default:
                // For gameplay levels: pick first music or by index (simplest version just plays first)
                if (AudioManager.Instance.levelMusics.Length > 0)
                {
                    AudioManager.Instance.PlayMusic(AudioManager.Instance.levelMusics[0]);
                }
                else
                {
                    AudioManager.Instance.StopMusic();
                }
                break;
        }
    }
}
