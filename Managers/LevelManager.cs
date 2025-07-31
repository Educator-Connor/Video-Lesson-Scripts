using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelResults
{
    public int score;
    public int hits;
    public int deaths;
    public int injuries;
    public int gems;
    public float timeTaken;
    public string nextLevelName;
}

public class LevelManager : MonoBehaviour
{
    [Header("Completion Rules")]
    public bool requireAllGems = false;

    public int totalGems = 0;
    public int collectedGems = 0;

    [Header("Player Stats")]
    public int successfulHits = 0;
    public int deathCount = 0;
    public int injuryCount = 0;

    [Header("Level Score")]
    public int levelScore = 0;

    [Header("References")]
    public LevelTimer levelTimer;
    public GameObject player;

    private bool playerAtFinish;
    private bool levelCompleted;

    public UnityEvent<LevelResults> OnLevelCleared = new UnityEvent<LevelResults>();

    private void Start()
    {
        GameManager.Instance?.RegisterLevelManager(this);
        InitializeLevel();
    }

    private void Update()
    {
        if (!levelCompleted)
            CheckLevelCompletion();
    }

    public void InitializeLevel()
    {
        ResetStats();
        levelTimer?.StartTimer();
        UIManager.Instance?.UpdatePoints(levelScore);
    }

    public void ResetLevel()
    {
        ResetStats();
        if (levelTimer != null)
        {
            levelTimer.ResetTimer();
            levelTimer.StartTimer();
        }
        UIManager.Instance?.UpdatePoints(levelScore);
    }

    private void ResetStats()
    {
        collectedGems = 0;
        successfulHits = 0;
        deathCount = 0;
        injuryCount = 0;
        levelScore = 0;
        playerAtFinish = false;
        levelCompleted = false;

        UpdateTotalGems();
    }

    public void UpdateTotalGems()
    {
        totalGems = GameObject.FindGameObjectsWithTag("Gem").Length;
    }

    public void CollectGem()
    {
        collectedGems++;
        AddScore(1);
        CheckLevelCompletion();
    }

    public void AddScore(int amount)
    {
        levelScore += amount;
        UIManager.Instance?.UpdatePoints(levelScore);
    }

    public void SetPlayerAtFinish()
    {
        playerAtFinish = true;
        CheckLevelCompletion();
    }

    public void RegisterHit()
    {
        successfulHits++;
    }

    public void RegisterDeath()
    {
        deathCount++;
    }

    public void RegisterInjury()
    {
        injuryCount++;
    }

    private void CheckLevelCompletion()
    {
        if (playerAtFinish)
        {
            if (!requireAllGems || (requireAllGems && collectedGems >= totalGems))
            {
                LevelCompleted();
            }
        }
    }

    private void LevelCompleted()
    {
        if (levelCompleted) return;

        levelCompleted = true;
        levelTimer?.StopTimer();

            LevelResults results = new LevelResults
        {
            score = levelScore,
            hits = successfulHits,
            deaths = deathCount,
            injuries = injuryCount,
            gems = collectedGems,
            timeTaken = levelTimer?.GetElapsedTime() ?? 0f,
            nextLevelName = GetNextLevelName(), // Or assign next level
        };
        Debug.Log("Next level: " + GetNextLevelName());


        GameManager.Instance?.SetGameState(GameState.Victory);
        OnLevelCleared.Invoke(results);

        ProfileManager.Instance?.ProcessLevelResults(results);

        // Add this line to notify GameManager and trigger level finish UI:
        GameManager.Instance?.OnLevelFinished.Invoke();
    }

    public string GetNextLevelName()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Make sure the index is within range
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            return sceneName;
        }
        else
        {
            Debug.LogWarning("Next scene index is out of bounds.");
            return null;
        }
    }

}
