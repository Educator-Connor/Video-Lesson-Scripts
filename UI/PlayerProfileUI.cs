using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileUI : MonoBehaviour
{
    public GameObject panelRoot;

    public TMP_Text nameText;
    public TMP_Text levelText;
    public TMP_Text scoreText; // Will hold score, hits, deaths, injuries, gems together
    public TMP_Text dateText;
    public TMP_Text timeText;

    public Button deleteButton;

    public MainMenuManager mainMenuManager;

    private PlayerProfile currentProfile;

    private void OnEnable()
    {
        ProfileManager.OnProfileDeleted += Close;
    }

    private void OnDisable()
    {
        ProfileManager.OnProfileDeleted -= Close;
    }

    private void Start()
    {
        deleteButton.onClick.AddListener(OnDeleteClicked);
    }

    public void OpenWithProfile(PlayerProfile profile)
    {
        currentProfile = profile;

        panelRoot.SetActive(true);

        nameText.text = profile.profileName;
        levelText.text = "Current Level: " + profile.currentLevel;
        dateText.text = "Last Played: " + profile.lastSavedDate;
        timeText.text = "Play Time: " + FormatTime(profile.playTime);

        // Show score, hits, deaths, injuries, and gems in one text field
        scoreText.text =
            $"Score: {profile.score}\n" +
            $"Hits: {profile.successfulHits}\n" +
            $"Deaths: {profile.deathCount}\n" +
            $"Injuries: {profile.injuryCount}\n" +
            $"Gems: {profile.collectedGems}";
    }

    public void Close()
    {
        panelRoot.SetActive(false);

        if (mainMenuManager != null)
        {
            mainMenuManager.RefreshSlotUI();
        }
    }

    private void OnDeleteClicked()
    {
        if (currentProfile != null)
        {
            ProfileManager.Instance.DeleteCurrentProfile();
            Close();
        }
    }

    private string FormatTime(float seconds)
    {
        int totalSeconds = Mathf.FloorToInt(seconds);
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int secs = totalSeconds % 60;

        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, secs);
    }
}


