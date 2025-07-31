using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    // ---------- Music ----------
    [Header("Music")]
    public AudioClip mainMenuMusic;
    public AudioClip gameOverMusic;
    public AudioClip[] levelMusics;
    [Range(0f, 1f)] public float musicVolume = 1f;

    // ---------- UI SFX ----------
    [Header("UI SFX")]
    public AudioClip uiClickClip;

    // ---------- Shared SFX ----------
    [Header("Shared SFX")]
    public AudioClip[] sfxClips; // 0 gem, 1 enemy, 2 box, 3 checkpoint, 4 finish
    [Range(0f, 1f)] public float sfxVolume = 1f;

    // ---------- Player SFX ----------
    [Header("Player SFX")]
    public AudioClip[] footstepClips;
    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip hurtClip;
    public AudioClip respawnClip;          // NEW
    [Range(0f, 0.2f)] public float footstepPitchVariance = 0.05f;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = musicVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;

        SceneManager.activeSceneChanged += OnSceneChanged;
    }
    void OnDestroy() => SceneManager.activeSceneChanged -= OnSceneChanged;

    // ---------- Scene music switching ----------
    private void OnSceneChanged(Scene _, Scene newScene)
    {
        string s = newScene.name;
        if (s == "SceneLoading") { StopMusic(); return; }
        if (s == "MainMenu") PlayMusic(mainMenuMusic);
        else if (s == "GameOver") PlayMusic(gameOverMusic);
        else if (levelMusics.Length > 0) PlayMusic(levelMusics[0]);
        else StopMusic();
    }

    // ---------- Music helpers ----------
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) { Debug.LogWarning("AudioManager: null music clip."); return; }
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }
    public void StopMusic() => musicSource.Stop();
    public void SetMusicVolume(float v) => musicSource.volume = musicVolume = Mathf.Clamp01(v);

    // ---------- Generic SFX ----------
    public void PlaySFX(AudioClip clip, float pitch = 1f)
    {
        if (clip == null) { Debug.LogWarning("AudioManager: null SFX clip."); return; }
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, sfxVolume);
        sfxSource.pitch = 1f;
    }
    public void PlaySFXByIndex(int i) { if (i >= 0 && i < sfxClips.Length) PlaySFX(sfxClips[i]); }
    public void PlaySFXByIndexWithPitch(int i, float p) { if (i >= 0 && i < sfxClips.Length) PlaySFX(sfxClips[i], p); }
    public void SetSFXVolume(float v) => sfxSource.volume = sfxVolume = Mathf.Clamp01(v);

    // ---------- Player SFX ----------
    public void PlayRandomFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0) return;
        int r = Random.Range(0, footstepClips.Length);
        float p = Random.Range(1f - footstepPitchVariance, 1f + footstepPitchVariance);
        PlaySFX(footstepClips[r], p);
    }
    public void PlayJump() => PlaySFX(jumpClip);
    public void PlayLand() => PlaySFX(landClip);
    public void PlayHurt() => PlaySFX(hurtClip);
    public void PlayRespawn() => PlaySFX(respawnClip);   // NEW

    // ---------- UI click ----------
    public void PlayUIClick() => PlaySFX(uiClickClip);
}
