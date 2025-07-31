// Assets/Scripts/Saves/AutoSaveOnSceneChange.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSaveOnSceneChange : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        // Persist the player profile whenever a scene is unloaded
        ProfileManager.Instance?.SaveProfile();
    }
}
