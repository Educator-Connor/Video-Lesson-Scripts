using UnityEngine;

[System.Serializable]
public class PlayerProfile
{
    public string slotId;
    public string profileName;

    public string currentLevel = "Tutorial";
    public string lastSavedDate;
    public float playTime = 0f;

    public int score = 0;
    public int successfulHits = 0;
    public int deathCount = 0;
    public int injuryCount = 0;
    public int collectedGems = 0;

    public float CalculateHitMultiplier() => 1f + (successfulHits * 0.1f);
    public float CalculateDeathPenalty() => Mathf.Max(1f, 1f + (deathCount * 0.5f));
    public float CalculateInjuryPenalty() => Mathf.Max(1f, 1f + (injuryCount * 0.1f));

    public float CalculateTimePenalty(float timeTaken)
    {
        float maxBonusTime = 120f;
        if (timeTaken <= maxBonusTime) return 1f;
        float excess = timeTaken - maxBonusTime;
        return Mathf.Max(0.1f, 1f - (excess / maxBonusTime));
    }

    public float CalculateRewardMultiplier(float timeTaken)
    {
        float timePenalty = CalculateTimePenalty(timeTaken);
        return (CalculateHitMultiplier() * timePenalty) /
               (CalculateDeathPenalty() * CalculateInjuryPenalty());
    }
}
