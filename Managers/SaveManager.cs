using UnityEngine;
using System.IO;

public static class SaveManager
{
    private static string GetPath(string slotId)
    {
        return Path.Combine(Application.persistentDataPath, "save_" + slotId + ".json");
    }

    /* --------------------------------------------------
       Save: convert PlayerProfile -> SaveData -> JSON
    -------------------------------------------------- */
    public static void Save(PlayerProfile profile)
    {
        SaveData data = new SaveData(profile.slotId)
        {
            profileName = profile.profileName,
            currentLevel = profile.currentLevel,
            score = profile.score,
            hits = profile.successfulHits,
            deaths = profile.deathCount,
            injuries = profile.injuryCount,
            gems = profile.collectedGems,
            playTime = profile.playTime,
            lastSavedDate = profile.lastSavedDate
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(profile.slotId), json);
        Debug.Log("Saved profile to slot " + profile.slotId);
    }

    /* --------------------------------------------------
       Load: JSON -> SaveData -> PlayerProfile
    -------------------------------------------------- */
    public static PlayerProfile Load(string slotId, bool logIfMissing = false)
    {
        string path = GetPath(slotId);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            PlayerProfile p = new PlayerProfile
            {
                slotId = data.slotId,
                profileName = data.profileName,
                currentLevel = data.currentLevel,
                score = data.score,
                successfulHits = data.hits,
                deathCount = data.deaths,
                injuryCount = data.injuries,
                collectedGems = data.gems,
                playTime = data.playTime,
                lastSavedDate = data.lastSavedDate
            };
            return p;
        }

        if (logIfMissing)
            Debug.LogWarning("No save found in slot " + slotId);

        return null;
    }

    /* --------------------------------------------------
       Misc helpers
    -------------------------------------------------- */
    public static bool SlotExists(string slotId)
    {
        return File.Exists(GetPath(slotId));
    }

    public static void DeleteSlot(string slotId)
    {
        string path = GetPath(slotId);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Deleted save file at " + path);
        }
    }
}
