using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;

    [Header("Movement")]
    public float runSpeed = 40f;
    public float stompForce = -20f;
    public float stompCooldown = 0.5f;
    public float climbSpeed = 5f;

    [Header("Footstep SFX")]
    public float baseFootstepInterval = 0.25f;
    private float footstepTimer;

    [HideInInspector] public bool isHurt = false;

    private float horizontalMove;
    private float verticalMove;
    private bool jump;
    private bool crouch;
    private bool isStomping;
    private float lastStompTime;
    private Rigidbody2D rb;

    private bool isClimbing;
    private Transform currentLadder;
    private float defaultGravity;

    private bool isPaused;
    private bool isExitingLadder;
    private float ladderExitTimer;
    public float ladderExitDelay = 0.2f;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;

    private int groundLayerMask;

    // Mobile
    private int mobileMoveInput = 0;
    private bool mobileCrouching = false;

    // Smooth speed multiplier for air control smoothing
    private float speedMultiplier = 1f;
    public float airControlLerpSpeed = 10f; // You can tweak this for smoothness

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPause.AddListener(OnPauseGame);
            GameManager.Instance.OnResume.AddListener(OnResumeGame);
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPause.RemoveListener(OnPauseGame);
            GameManager.Instance.OnResume.RemoveListener(OnResumeGame);
        }
    }

    private void OnPauseGame() => SetPaused(true);
    private void OnResumeGame() => SetPaused(false);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;

        if (groundCheckPoint == null)
            Debug.LogWarning("GroundCheckPoint is NOT assigned!");

        groundLayerMask = LayerMask.GetMask("Ground");
        if (groundLayerMask == 0)
            Debug.LogWarning("Ground layer 'Ground' not found.");
    }

    void Update()
    {
        if (isPaused || isHurt) return;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        bool keyboardCrouch = Input.GetButton("Crouch");

        bool grounded = IsGrounded();

        crouch = keyboardCrouch || mobileCrouching;

        if ((Input.GetButtonDown("Jump") || jump) && !isClimbing)
        {
            jump = true;
            PlayJumpSound();
        }

        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) &&
            Time.time >= lastStompTime + stompCooldown)
            PerformStomp();

        if (isExitingLadder)
        {
            ladderExitTimer -= Time.deltaTime;
            if (ladderExitTimer <= 0f) isExitingLadder = false;
        }

        if (isClimbing && !isExitingLadder)
        {
            verticalMove = verticalInput * climbSpeed;
            horizontalMove = 0f;

            if (currentLadder != null)
            {
                Vector3 pos = transform.position;
                pos.x = Mathf.Lerp(pos.x, currentLadder.position.x, Time.deltaTime * 20f);
                transform.position = pos;
            }
        }
        else
        {
            // Smoothly interpolate speedMultiplier between grounded (1f) and air (0.7f)
            float targetSpeedMultiplier = grounded ? 1f : 0.7f;
            speedMultiplier = Mathf.Lerp(speedMultiplier, targetSpeedMultiplier, Time.deltaTime * airControlLerpSpeed);

            // Calculate smooth horizontal movement
            float targetHorizontalMove = (horizontalInput + mobileMoveInput) * runSpeed * speedMultiplier;
            horizontalMove = Mathf.Lerp(horizontalMove, targetHorizontalMove, Time.deltaTime * airControlLerpSpeed);

            verticalMove = 0f;
        }

        HandleFootsteps(grounded);
        UpdateAnimatorStates(grounded);
    }

    void FixedUpdate()
    {
        if (isPaused) return;

        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(horizontalMove, verticalMove);
        }
        else
        {
            rb.gravityScale = defaultGravity;
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
            jump = false;
        }
    }

    private bool IsGrounded()
    {
        if (groundCheckPoint == null || groundLayerMask == 0) return false;
        return Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayerMask);
    }

    private void HandleFootsteps(bool grounded)
    {
        if (!grounded || Mathf.Abs(horizontalMove) < 0.1f || isPaused || isClimbing)
        {
            footstepTimer = 0f;
            return;
        }

        float speedRatio = Mathf.Clamp01(Mathf.Abs(horizontalMove) / runSpeed);
        float intervalCurrent = baseFootstepInterval / Mathf.Max(0.1f, speedRatio);

        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0f)
        {
            AudioManager.Instance?.PlayRandomFootstep();
            footstepTimer = intervalCurrent;
        }
    }

    private void UpdateAnimatorStates(bool grounded)
    {
        bool jumping = !grounded && !isClimbing;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        animator.SetBool("isJumping", jumping);
        animator.SetBool("isClimbing", isClimbing);
        animator.SetBool("isCrouching", crouch);
        animator.SetBool("isHurt", isHurt);
    }

    void PerformStomp()
    {
        isStomping = true;
        lastStompTime = Time.time;
        rb.velocity = new Vector2(rb.velocity.x, stompForce);
    }

    public void OnLanding()
    {
        animator.SetBool("isJumping", false);
        isStomping = false;
        PlayLandSound();
    }

    public void OnCrouching(bool isCrouching) => animator.SetBool("isCrouching", isCrouching);

    public void EnterLadder(Transform ladderTransform)
    {
        currentLadder = ladderTransform;
        isClimbing = true;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
        animator.SetBool("isClimbing", true);
        animator.SetBool("isJumping", false);
        transform.position = new Vector3(ladderTransform.position.x, transform.position.y);
    }

    public void ExitLadder()
    {
        isClimbing = false;
        isExitingLadder = true;
        ladderExitTimer = ladderExitDelay;
        currentLadder = null;
        rb.gravityScale = defaultGravity;
        animator.SetBool("isClimbing", false);
    }

    private void PlayJumpSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayJump();
        }
    }

    private void PlayLandSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLand();
        }
    }

    public void SetPaused(bool paused) => isPaused = paused;
    public bool IsStomping() => isStomping;

    // --- MOBILE SUPPORT ---
    public void SetMobileMove(int dir) => mobileMoveInput = dir;
    public void OnMobileJump() { if (!isClimbing && !jump) { jump = true; PlayJumpSound(); } }
    public void OnMobileCrouch(bool state) => mobileCrouching = state;
    public void PerformStompGesture() { if (Time.time >= lastStompTime + stompCooldown) PerformStomp(); }
}
