using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Slot Buttons (3 slots)")]
    [SerializeField] private Button[] slotButtons;   // assign 3 buttons in inspector

    [Header("Main menu buttons")]
    [SerializeField] private Button playButton;      // Start / Play
    [SerializeField] private Button deleteButton;    // Delete

    [Header("Profile Stats UI")]
    [SerializeField] private ProfileStatsUI profileStatsUI;  // Reference to the detailed stats UI

    public static ProfileManager Instance;

    private PlayerProfile currentProfile;
    public PlayerProfile CurrentProfile => currentProfile;

    public readonly string[] slotIds = { "Slot1", "Slot2", "Slot3" };

    private void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnProfileLoaded.AddListener(HandleProfileLoaded);
            EventManager.Instance.OnProfileDeleted.AddListener(HandleProfileDeleted);
        }

        ProfileManager.OnProfileDeleted += RefreshSlotUI;   // keep slot labels fresh
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnProfileLoaded.RemoveListener(HandleProfileLoaded);
            EventManager.Instance.OnProfileDeleted.RemoveListener(HandleProfileDeleted);
        }

        ProfileManager.OnProfileDeleted -= RefreshSlotUI;
    }

    private void Start()
    {
        // Play button
        playButton.interactable = false;
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(PlayGame);

        // Delete button
        deleteButton.interactable = false;
        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(DeleteProfile);

        SetupSlotButtons();
        RefreshSlotUI();
    }

    private void SetupSlotButtons()
    {
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int index = i; // closure-safe copy
            if (slotButtons[i] == null) continue;

            slotButtons[i].onClick.RemoveAllListeners();
            slotButtons[i].onClick.AddListener(() => OnSlotClicked(index));
        }
    }

    public void RefreshSlotUI()
    {
        for (int i = 0; i < slotButtons.Length; i++)
        {
            if (slotButtons[i] == null) continue;

            TMP_Text txt = slotButtons[i].GetComponentInChildren<TMP_Text>();
            if (txt == null) continue;

            PlayerProfile p = SaveManager.Load(slotIds[i], false);
            if (p == null)
            {
                txt.text = "Empty Slot";
            }
            else
            {
                string displayName = string.IsNullOrWhiteSpace(p.profileName)
                    ? "Game " + (i + 1)
                    : p.profileName;

                txt.text = displayName + "\nLevel: " + p.currentLevel;
            }
        }
    }

    public void OnSlotClicked(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slotIds.Length) return;

        string id = slotIds[slotIndex];

        bool loaded = ProfileManager.Instance.LoadProfile(id, true);
        if (!loaded)
            ProfileManager.Instance.CreateNewProfile(id);

        RefreshSlotUI();

        // Tell ProfileStatsUI to open with current profile details
        if (profileStatsUI != null)
        {
            profileStatsUI.OpenWithProfile(ProfileManager.Instance.CurrentProfile);
        }
    }

    private void PlayGame()
    {

        Debug.Log("Loading Next level");
        if (ProfileManager.Instance.CurrentProfile == null)
        {
            Debug.LogWarning("PlayGame clicked but no profile loaded.");
            return;
        }

        Debug.Log("Loading Next level 2");
        // Centralised load with fade + loading screen
        SceneLoader.Instance.LoadProfileLevel(LoadMode.WithFadeAndLoadingScreen);

    }

    private void DeleteProfile()
    {
        if (ProfileManager.Instance.CurrentProfile == null) return;

        ProfileManager.Instance.DeleteCurrentProfile();
        RefreshSlotUI();

        // Close the profile stats UI after deletion
        if (profileStatsUI != null)
        {
            profileStatsUI.Close();
        }
    }

    private void HandleProfileLoaded(PlayerProfile p)
    {
        playButton.interactable = true;
        deleteButton.interactable = true;
    }

    private void HandleProfileDeleted()
    {
        playButton.interactable = false;
        deleteButton.interactable = false;
    }
}
