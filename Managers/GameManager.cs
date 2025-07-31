using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver,
    Victory
}

[System.Serializable]
public class GameStateEvent : UnityEvent<GameState> { }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState CurrentState { get; private set; }
    public bool IsGameOver { get; private set; }

    private LevelManager currentLevelManager;
    private UIManager currentUIManager;

    public GameStateEvent OnGameStateChanged = new GameStateEvent();
    public UnityEvent OnPause = new UnityEvent();
    public UnityEvent OnResume = new UnityEvent();
    public UnityEvent OnRestart = new UnityEvent();
    public UnityEvent OnLevelFinished = new UnityEvent();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        IsGameOver = false;

        // Set the initial game state based on the current scene
        string scene = SceneManager.GetActiveScene().name.ToLower();
        SetGameState(scene.Contains("menu") ? GameState.MainMenu : GameState.Playing);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name.ToLower();

        if (sceneName.Contains("menu"))
        {
            SetGameState(GameState.MainMenu);
        }
        else
        {
            SetGameState(GameState.Playing);
        }
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) && !IsGameOver)
        {
            if (CurrentState == GameState.Playing)
                SetGameState(GameState.Paused);
            else if (CurrentState == GameState.Paused)
                SetGameState(GameState.Playing);
        }

        if (Input.GetKeyDown(KeyCode.R) && CurrentState == GameState.Paused)
            RestartLevel();
    }

    public void RegisterLevelManager(LevelManager lm)
    {
        currentLevelManager = lm;
    }

    public void RegisterUIManager(UIManager ui)
    {
        currentUIManager = ui;
    }

    public void SetGameState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;

        switch (CurrentState)
        {
            case GameState.MainMenu:
                Time.timeScale = 0f;
                IsGameOver = false;
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                IsGameOver = false;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
            case GameState.GameOver:
            case GameState.Victory:
                Time.timeScale = 0f;
                IsGameOver = true;
                break;
        }

        OnGameStateChanged.Invoke(CurrentState);

        if (CurrentState == GameState.Paused)
        {
            AudioManager.Instance?.PlayUIClick();
            OnPause.Invoke();
        }
        else if (CurrentState == GameState.Playing)
        {
            AudioManager.Instance?.PlayUIClick();
            OnResume.Invoke();
        }
    }

    public void PauseGame()
    {
        SetGameState(GameState.Paused);
    }

    public void ResumeGame()
    {
        SetGameState(GameState.Playing);
    }

    public void RestartLevel()
    {
        AudioManager.Instance?.PlayUIClick();

        IsGameOver = false;
        Time.timeScale = 1f;
        OnRestart.Invoke();

        currentLevelManager?.ResetLevel();

        if (SceneLoader.Instance != null)
        {
            SceneLoader.TargetScene = null;
            SceneLoader.Instance.ReloadScene();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public LevelManager GetCurrentLevelManager()
    {
        return currentLevelManager;
    }
}
