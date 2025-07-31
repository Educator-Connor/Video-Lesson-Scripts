using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompletePanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panelRoot = null;

    [SerializeField] private TMP_Text txtScore = null;
    [SerializeField] private TMP_Text txtHits = null;
    [SerializeField] private TMP_Text txtDeaths = null;
    [SerializeField] private TMP_Text txtInjuries = null;
    [SerializeField] private TMP_Text txtGems = null;
    [SerializeField] private TMP_Text txtPlaytime = null;

    [Header("Buttons")]
    [SerializeField] private Button btnNextLevel = null;
    [SerializeField] private Button btnMainMenu = null;

    private void Awake()
    {
        if (btnNextLevel != null)
            btnNextLevel.onClick.AddListener(OnNextLevelClicked);

        if (btnMainMenu != null)
            btnMainMenu.onClick.AddListener(OnMainMenuClicked);

        if (panelRoot != null)
            panelRoot.SetActive(false);
        else
            Debug.LogWarning("[LevelCompletePanel] PanelRoot is not assigned.");
    }

    private void OnEnable()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.OnLevelCleared.AddListener(LevelFinished);
        }
        else
        {
            Debug.LogWarning("[LevelCompletePanel] LevelManager not found in scene.");
        }
    }

    private void OnDisable()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.OnLevelCleared.RemoveListener(LevelFinished);
        }
    }

    private void LevelFinished(LevelResults results)
    {
        if (results == null)
        {
            Debug.LogWarning("[LevelCompletePanel] LevelResults is null. Cannot display stats.");
            return;
        }

        if (txtScore != null) txtScore.text = "Score: " + results.score;
        if (txtHits != null) txtHits.text = "Hits: " + results.hits;
        if (txtDeaths != null) txtDeaths.text = "Deaths: " + results.deaths;
        if (txtInjuries != null) txtInjuries.text = "Injuries: " + results.injuries;
        if (txtGems != null) txtGems.text = "Gems: " + results.gems;
        if (txtPlaytime != null) txtPlaytime.text = $"Playtime: {results.timeTaken:F2} s";

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("[LevelCompletePanel] panelRoot not assigned — cannot show panel.");
        }
    }

    private void OnNextLevelClicked()
    {
        Debug.Log("[LevelCompletePanel] Next Level clicked");

        ProfileManager.Instance?.SaveProfile();

        Time.timeScale = 1f;

        if (panelRoot != null)
            panelRoot.SetActive(false);

        SceneLoader.Instance?.LoadNextLevel();
    }

    private void OnMainMenuClicked()
    {
        Debug.Log("[LevelCompletePanel] Main Menu clicked");

       // ProfileManager.Instance?.SaveProfile();

       // Time.timeScale = 1f;
       //
       // if (panelRoot != null)
       //     panelRoot.SetActive(false);
       //
       // SceneLoader.Instance?.LoadMainMenu();
    }
}
