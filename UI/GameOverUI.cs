using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public TMP_Text totalScoreText;
    public TMP_Text totalHitsText;
    public TMP_Text totalGemsText;
    public TMP_Text totalInjuriesText;
    public TMP_Text totalDeaths;
    public TMP_Text totalPlayTimeText;
    

    void Start()
    {
        var profile = ProfileManager.Instance?.CurrentProfile;
        if (profile == null)
        {
            Debug.LogWarning("No current profile found in ProfileManager.");
            return;
        }

        totalScoreText.text = $"Total Score: {profile.deathCount}";
        totalScoreText.text = $"Total Score: {profile.score}";
        totalHitsText.text = $"Total Hits: {profile.successfulHits}";
        totalGemsText.text = $"Gems Collected: {profile.collectedGems}";
        totalInjuriesText.text = $"Injuries: {profile.injuryCount}";

        int hours = Mathf.FloorToInt(profile.playTime / 3600f);
        int minutes = Mathf.FloorToInt((profile.playTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(profile.playTime % 60f);

        totalPlayTimeText.text = hours > 0
            ? $"Total Time Played: {hours:00}:{minutes:00}:{seconds:00}"
            : $"Total Time Played: {minutes:00}:{seconds:00}";
    }
}
