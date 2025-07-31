using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum LoadMode
{
    Direct,
    WithLoadingScreen,
    WithFade,
    WithFadeAndLoadingScreen
}

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    public static string TargetScene { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(InitializeSceneAfterLoad(scene));
    }

    private IEnumerator InitializeSceneAfterLoad(Scene scene)
    {
        yield return null;

        Time.timeScale = 1f;

        string sceneName = scene.name;

        if (sceneName == "MainMenu")
            GameManager.Instance?.SetGameState(GameState.MainMenu);
        else if (sceneName == "LoadingScene")
        {
            // Do nothing here
        }
        else if (sceneName == "GameOverScene")
            GameManager.Instance?.SetGameState(GameState.GameOver);
        else
            GameManager.Instance?.SetGameState(GameState.Playing);

        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            GameManager.Instance?.RegisterUIManager(uiManager);
            uiManager.SetPauseMenuPanel(false);
            uiManager.SetLevelFinishedPanel(false);
        }

        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            GameManager.Instance?.RegisterLevelManager(levelManager);
            levelManager.InitializeLevel();
        }

        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        GameObject spawn = GameObject.FindGameObjectWithTag("SpawnPoint");

        if (playerGO != null && spawn != null)
        {
            var playerRespawn = playerGO.GetComponent<PlayerRespawn>();
            if (playerRespawn != null)
            {
                playerRespawn.SetRespawnPoint(spawn.transform.position);
                playerRespawn.Respawn();
            }

            var movement = playerGO.GetComponent<PlayerMovement>();
            if (movement != null) movement.SetPaused(false);

            var animator = playerGO.GetComponent<Animator>();
            if (animator != null) animator.speed = 1f;
        }

        // Save currentLevel only if it's not MainMenu or LoadingScene
        if (sceneName != "MainMenu" && sceneName != "LoadingScene" && ProfileManager.Instance?.CurrentProfile != null)
        {
            if (sceneName != "GameOverScene")
            {
                // Update profile currentLevel and save
                ProfileManager.Instance.CurrentProfile.currentLevel = sceneName;
                SaveManager.Save(ProfileManager.Instance.CurrentProfile);
            }
        }
    }

    public void LoadNextScene(LoadMode mode = LoadMode.WithFadeAndLoadingScreen)
    {
        // Get current profile level - this is the base to find next scene
        string currentLevel = ProfileManager.Instance?.CurrentProfile?.currentLevel;

        if (string.IsNullOrEmpty(currentLevel))
        {
            Debug.LogWarning("[SceneLoader] Profile currentLevel invalid or empty, loading MainMenu.");
            Load("MainMenu", mode);
            return;
        }

        // Get build index of currentLevel scene
        int currentIndex = SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/" + currentLevel + ".unity");

        if (currentIndex == -1)
        {
            Debug.LogWarning($"[SceneLoader] Scene '{currentLevel}' not found in build settings, loading MainMenu.");
            Load("MainMenu", mode);
            return;
        }

        int nextIndex = currentIndex + 1;

        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("[SceneLoader] No next scene in build settings, loading GameOverScene.");
            Load("GameOver", mode);
            return;
        }

        string nextSceneName = System.IO.Path.GetFileNameWithoutExtension(
            SceneUtility.GetScenePathByBuildIndex(nextIndex));

        // Avoid loading LoadingScene or MainMenu as next scene
        if (nextSceneName == "LoadingScene" || nextSceneName == "MainMenu")
        {
            nextIndex++; // skip one more if possible
            if (nextIndex >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogWarning("[SceneLoader] No valid next scene after skipping, loading GameOverScene.");
                Load("GameOver", mode);
                return;
            }
            nextSceneName = System.IO.Path.GetFileNameWithoutExtension(
                SceneUtility.GetScenePathByBuildIndex(nextIndex));
        }

        Debug.Log($"[SceneLoader] Loading next scene: {nextSceneName}");
        Load(nextSceneName, mode);
    }

    // ** New method: load Main Menu cleanly **
    public void LoadMainMenu(LoadMode mode = LoadMode.WithFadeAndLoadingScreen)
    {
        Load("MainMenu", mode);
    }

    // ** New method: wrapper to load the next level **
    public void LoadNextLevel(LoadMode mode = LoadMode.WithFadeAndLoadingScreen)
    {
        LoadNextScene(mode);
    }

    public void Load(string sceneName, LoadMode mode = LoadMode.Direct)
    {
        Debug.Log("In Load");
        if (!SceneExists(sceneName))
        {
            Debug.LogError($"[SceneLoader] Scene '{sceneName}' not found in Build Settings.");
            return;
        }

        switch (mode)
        {
            case LoadMode.Direct:
                TargetScene = null;
                SceneManager.LoadScene(sceneName);
                break;

            case LoadMode.WithLoadingScreen:
                TargetScene = sceneName;
                SceneManager.LoadScene("LoadingScene");
                break;

            case LoadMode.WithFade:
                TransitionManager.Instance?.FadeAndLoad(sceneName, false);
                break;

            case LoadMode.WithFadeAndLoadingScreen:
                Debug.Log("In WithFadeAndLoadingScreen");
                TargetScene = sceneName;
                TransitionManager.Instance?.FadeAndLoad("LoadingScene", true);
                Debug.Log("after In WithFadeAndLoadingScreen");
                break;
        }
    }

    public void LoadProfileLevel(LoadMode mode = LoadMode.WithFadeAndLoadingScreen)
    {
        Debug.Log("We are in LoadProfileLevel");
        if (ProfileManager.Instance?.CurrentProfile == null)
        {
            Debug.LogWarning("[SceneLoader] No current profile loaded. Loading MainMenu instead.");
            Load("Tutorial", mode);
            return;
        }

        string profileLevel = ProfileManager.Instance.CurrentProfile.currentLevel;

        if (string.IsNullOrEmpty(profileLevel) || profileLevel == "MainMenu" || profileLevel == "LoadingScene" || !SceneExists(profileLevel))
        {
            Debug.LogWarning($"[SceneLoader] Profile currentLevel '{profileLevel}' invalid or disallowed. Loading MainMenu instead.");
            Load("MainMenu", mode);
            return;
        }
        Debug.Log("levelName" + profileLevel);
        SceneManager.LoadScene(profileLevel);
        Debug.Log("Loading Next level 3");

       // Load(profileLevel, mode);
    }

    public void ReloadScene()
    {
        Load(SceneManager.GetActiveScene().name, LoadMode.Direct);
    }

    public static bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName) return true;
        }
        return false;
    }
}
