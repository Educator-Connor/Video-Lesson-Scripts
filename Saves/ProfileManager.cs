using System;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance;

    private PlayerProfile currentProfile;
    public PlayerProfile CurrentProfile => currentProfile;

    public static event Action OnProfileDeleted;
    public static event Action<PlayerProfile> OnProfileLoaded;
    public static event Action<PlayerProfile> OnProfileSaved;

    private DateTime sessionStartTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool LoadProfile(string id, bool raiseEvent = false)
    {
        PlayerProfile p = SaveManager.Load(id, false);
        if (p == null) return false;

        currentProfile = p;
        sessionStartTime = DateTime.Now;

        if (raiseEvent)
            OnProfileLoaded?.Invoke(currentProfile);

        return true;
    }

    public void CreateNewProfile(string id)
    {
        string displayName = id switch
        {
            "Slot1" => "Game A",
            "Slot2" => "Game B",
            "Slot3" => "Game C",
            _ => "Game"
        };

        PlayerProfile p = new PlayerProfile
        {
            slotId = id,
            profileName = displayName,
            currentLevel = "Tutorial",
            lastSavedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            playTime = 0f,
            score = 0,
            successfulHits = 0,
            deathCount = 0,
            injuryCount = 0,
            collectedGems = 0
        };

        currentProfile = p;
        sessionStartTime = DateTime.Now;

        SaveManager.Save(p);
        OnProfileLoaded?.Invoke(currentProfile);
    }

    public void ProcessLevelResults(LevelResults r)
    {
        if (r == null || currentProfile == null) return;

        currentProfile.score += r.score;
        currentProfile.successfulHits += r.hits;
        currentProfile.deathCount += r.deaths;
        currentProfile.injuryCount += r.injuries;
        currentProfile.collectedGems += r.gems;
        currentProfile.currentLevel = r.nextLevelName;

        Debug.Log("currentProfile.currentLevel being saved as: " + currentProfile.currentLevel);

        SaveProfile();
    }

    public void SaveProfile()
    {
        if (currentProfile == null) return;

        TimeSpan elapsed = DateTime.Now - sessionStartTime;
        currentProfile.playTime += (float)elapsed.TotalSeconds;
        sessionStartTime = DateTime.Now;

        currentProfile.lastSavedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        SaveManager.Save(currentProfile);
        OnProfileSaved?.Invoke(currentProfile);
    }

    public void DeleteCurrentProfile()
    {
        if (currentProfile == null) return;

        SaveManager.DeleteSlot(currentProfile.slotId);
        OnProfileDeleted?.Invoke();
        currentProfile = null;
    }
}
