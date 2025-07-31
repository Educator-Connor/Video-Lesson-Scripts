using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    public TMP_Text timerText;

    private float elapsedTime;
    private bool isRunning;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPause.AddListener(PauseTimer);
            GameManager.Instance.OnResume.AddListener(ResumeTimer);
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPause.RemoveListener(PauseTimer);
            GameManager.Instance.OnResume.RemoveListener(ResumeTimer);
        }
    }

    private void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;

        int hours = Mathf.FloorToInt(elapsedTime / 3600f);
        int minutes = Mathf.FloorToInt((elapsedTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        if (timerText != null)
        {
            timerText.text = hours > 0
                ? string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds)
                : string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }


    /* ------------------------------------------------------------------ */
    public void StartTimer()
    {
        elapsedTime = 0f;
        isRunning = true;
    }

    public void SetTimerRunning(bool running)
    {
        isRunning = running;
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        isRunning = true;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunning = false;
        if (timerText != null) timerText.text = "00:00";
    }

    public float GetFinalTime()
    {
        return elapsedTime;
    }

    // Added to fix your errors:
    public void StopTimer()
    {
        isRunning = false;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
