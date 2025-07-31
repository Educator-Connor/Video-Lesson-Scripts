using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerProfileEvent : UnityEvent<PlayerProfile> { }

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    /* --------------------------------------------------
       Menu / Profile Events
    -------------------------------------------------- */
    public PlayerProfileEvent OnProfileLoaded = new PlayerProfileEvent();
    public UnityEvent OnProfileDeleted = new UnityEvent();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        SubscribeToGameManager();

        // Forward static ProfileManager events
        ProfileManager.OnProfileLoaded += RaiseProfileLoaded;
        ProfileManager.OnProfileDeleted += RaiseProfileDeleted;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged.RemoveListener(HandleGameStateChanged);

        ProfileManager.OnProfileLoaded -= RaiseProfileLoaded;
        ProfileManager.OnProfileDeleted -= RaiseProfileDeleted;
    }

    /* --------------------------------------------------
       Game state plumbing
    -------------------------------------------------- */
    private void SubscribeToGameManager()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("EventManager: GameManager instance not found.");
            return;
        }

        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    private void HandleGameStateChanged(GameState state)
    {
        // Optional: react here if you want to close menus on play, etc.
    }

    /* --------------------------------------------------
       Profile-related helpers
    -------------------------------------------------- */
    public void RaiseProfileLoaded(PlayerProfile profile)
    {
        OnProfileLoaded.Invoke(profile);
    }

    public void RaiseProfileDeleted()
    {
        OnProfileDeleted.Invoke();
    }
}
