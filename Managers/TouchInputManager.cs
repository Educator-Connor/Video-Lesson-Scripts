using UnityEngine;

public class TouchInputManager : MonoBehaviour
{
    private PlayerMovement player;

    private Vector2 startTouchPos;
    private Vector2 endTouchPos;
    private float minSwipeDist = 50f;
    private float maxTapTime = 0.2f;
    private float tapTimer;

    private float holdThreshold = 0.2f;
    private float holdTimer = 0f;
    private bool isHolding = false;
    private bool holdingLeft = false;

    private bool awaitingStomp = false;
    private float stompTimer = 0f;
    private float stompDelay = 0.4f;

    // Tap counting for pause controls
    private int tapCount = 0;
    private float tapCountResetTime = 0.5f; // time window to count taps
    private float tapCountTimer = 0f;

    void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
        if (player == null)
            Debug.LogError("TouchInputManager: PlayerMovement not found in scene.");
    }

    void Update()
    {
        if (player == null || Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                startTouchPos = touch.position;
                tapTimer = 0f;
                holdTimer = 0f;
                isHolding = true;
                holdingLeft = touch.position.x < Screen.width / 2;
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                holdTimer += Time.deltaTime;
                break;

            case TouchPhase.Ended:
                endTouchPos = touch.position;
                Vector2 swipeDelta = endTouchPos - startTouchPos;

                bool isTap = tapTimer < maxTapTime && swipeDelta.magnitude < minSwipeDist;

                if (GameManager.Instance.CurrentState == GameState.Playing)
                {
                    // Playing state input
                    if (isTap)
                    {
                        player.OnMobileJump();
                        RegisterTap();
                    }
                    else if (swipeDelta.magnitude >= minSwipeDist)
                    {
                        float x = swipeDelta.x;
                        float y = swipeDelta.y;

                        if (Mathf.Abs(x) < Mathf.Abs(y))
                        {
                            if (y < 0)
                            {
                                if (awaitingStomp)
                                {
                                    player.PerformStompGesture();
                                    awaitingStomp = false;
                                }
                                else
                                {
                                    player.OnMobileCrouch(true);
                                }
                            }
                            else if (y > 0)
                            {
                                awaitingStomp = true;
                                stompTimer = 0f;
                            }
                        }
                    }
                    isHolding = false;
                    player.OnMobileCrouch(false);
                }
                else if (GameManager.Instance.CurrentState == GameState.Paused)
                {
                    // Pause menu input
                    if (isTap)
                    {
                        RegisterTap();
                    }
                }
                break;
        }

        tapTimer += Time.deltaTime;

        if (awaitingStomp)
        {
            stompTimer += Time.deltaTime;
            if (stompTimer > stompDelay)
                awaitingStomp = false;
        }

        if (isHolding && holdTimer > holdThreshold && GameManager.Instance.CurrentState == GameState.Playing)
        {
            player.SetMobileMove(holdingLeft ? -1 : 1);
        }
        else
        {
            player.SetMobileMove(0);
        }

        UpdateTapCountTimer();
    }

    private void RegisterTap()
    {
        tapCount++;
        tapCountTimer = 0f;

        if (GameManager.Instance.CurrentState == GameState.Playing)
        {
            if (tapCount == 2)
            {
                GameManager.Instance.PauseGame();
                ResetTapCount();
            }
        }
        else if (GameManager.Instance.CurrentState == GameState.Paused)
        {
            if (tapCount == 2)
            {
                GameManager.Instance.ResumeGame();
                ResetTapCount();
            }
            else if (tapCount == 3)
            {
                GameManager.Instance.RestartLevel();
                ResetTapCount();
            }
        }
    }

    private void UpdateTapCountTimer()
    {
        if (tapCount > 0)
        {
            tapCountTimer += Time.deltaTime;
            if (tapCountTimer > tapCountResetTime)
            {
                ResetTapCount();
            }
        }
    }

    private void ResetTapCount()
    {
        tapCount = 0;
        tapCountTimer = 0f;
    }
}
