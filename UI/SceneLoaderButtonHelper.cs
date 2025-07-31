using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderButtonHelper : MonoBehaviour
{
    private void Awake()
    {
        UnpauseGame();
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UnpauseGame();
    }

    private static void UnpauseGame()
    {
        if (Time.timeScale != 1f)
        {
            Debug.Log("[ButtonHelper] Resetting Time.timeScale to 1");
            Time.timeScale = 1f;
        }

        PlayerMovement[] players = Object.FindObjectsOfType<PlayerMovement>();
        foreach (PlayerMovement pm in players)
        {
            pm.SetPaused(false);
        }
    }

    private void HideLevelFinishedPanel()
    {
        UIManager ui = Object.FindObjectOfType<UIManager>();
        if (ui != null)
        {
            ui.SetLevelFinishedPanel(false);
            Debug.Log("[ButtonHelper] Level-Finished panel hidden");
        }
    }

    public void LoadMainMenu()
    {
        Debug.Log("[ButtonHelper] LoadMainMenu() pressed");

        //ProfileManager.Instance?.SaveProfile();

        UnpauseGame();
        HideLevelFinishedPanel();
        SceneLoader.Instance?.LoadMainMenu();
    }

    public void ReloadCurrentScene()
    {
        Debug.Log("[ButtonHelper] ReloadCurrentScene() pressed");
        UnpauseGame();
        HideLevelFinishedPanel();
        SceneLoader.Instance?.ReloadScene();
    }

    public void LoadNextLevel()
    {
        Debug.Log("[ButtonHelper] LoadNextLevel() pressed");

        ProfileManager.Instance?.SaveProfile();

        UnpauseGame();
        HideLevelFinishedPanel();
        SceneLoader.Instance?.LoadNextLevel();
    }
}
