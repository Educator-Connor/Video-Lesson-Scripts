using System;

[Serializable]
public class SaveData
{
    public string slotId;          // "Slot1", "Slot2", "Slot3"
    public string profileName;     // Game A, B, C
    public string currentLevel;    // Level the player should load next

    public int score;
    public int hits;
    public int deaths;
    public int injuries;
    public int gems;

    public float playTime;
    public string lastSavedDate;

    public SaveData(string id)
    {
        slotId = id;
        profileName = "Game";
        currentLevel = "Tutorial";
        score = 0;
        hits = 0;
        deaths = 0;
        injuries = 0;
        gems = 0;
        playTime = 0f;
        lastSavedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
