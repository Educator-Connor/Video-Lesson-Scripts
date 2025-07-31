using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
    public GameObject levelFinishedPanel;
    public GameObject gameOverPanel;

    [Header("UI Elements")]
    public TMP_Text pointsText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Optional: persist across scenes
        }
        else
        {
            Debug.LogWarning("UIManager: Duplicate instance detected, destroying this one.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        WarnIfUnassigned(pauseMenuPanel, "pauseMenuPanel");
        WarnIfUnassigned(levelFinishedPanel, "levelFinishedPanel");
        WarnIfUnassigned(gameOverPanel, "gameOverPanel");
        WarnIfUnassigned(pointsText, "pointsText");

        HideAllPanels();

        // Subscribing to GameManager events in Start (ensures GameManager.Instance is ready)
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("UIManager: GameManager.Instance is still null in Start.");
            return;
        }

        GameManager.Instance.OnPause?.AddListener(HandlePause);
        GameManager.Instance.OnResume?.AddListener(HandleResume);
        GameManager.Instance.OnLevelFinished?.AddListener(ShowLevelFinishedUI);
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.OnPause?.RemoveListener(HandlePause);
        GameManager.Instance.OnResume?.RemoveListener(HandleResume);
        GameManager.Instance.OnLevelFinished?.RemoveListener(ShowLevelFinishedUI);
    }

    public void UpdatePoints(int points)
    {
        if (pointsText != null)
        {
            pointsText.text = $"Points: {points}";
        }
    }

    public void HandlePause() => SetPauseMenuPanel(true);
    public void HandleResume() => SetPauseMenuPanel(false);
    private void ShowLevelFinishedUI() => SetLevelFinishedPanel(true);

    public void SetPauseMenuPanel(bool isActive) => SetPanelActive(pauseMenuPanel, isActive, "pauseMenuPanel");
    public void SetLevelFinishedPanel(bool isActive) => SetPanelActive(levelFinishedPanel, isActive, "levelFinishedPanel");
    public void SetGameOverPanel(bool isActive) => SetPanelActive(gameOverPanel, isActive, "gameOverPanel");

    public void HideAllPanels()
    {
        SetPauseMenuPanel(false);
        SetLevelFinishedPanel(false);
        SetGameOverPanel(false);
    }

    public void PlayUIClickSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUIClick();
        }
        else
        {
            Debug.LogWarning("UIManager: AudioManager is missing — skipping UI click sound.");
        }
    }

    private void SetPanelActive(GameObject panel, bool isActive, string panelName)
    {
        if (panel != null)
        {
            panel.SetActive(isActive);
        }
        else
        {
            Debug.LogWarning($"UIManager: {panelName} is not assigned — cannot change active state.");
        }
    }

    private void WarnIfUnassigned(Object obj, string name)
    {
        if (obj == null)
            Debug.LogWarning($"UIManager: {name} is not assigned.");
    }
}
