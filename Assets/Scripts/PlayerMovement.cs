using UnityEngine;

public enum PlayerState
{
    Idle,
    Ready,
    Run,
    Jump,
    Slide,
    CrouchWalk,
    Fall,
    Win
}

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;

    // CROUCH
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float normalHeight = 2f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float slideSpeed = 8f;
    [SerializeField] private float slideDuration = 0.6f;
    [SerializeField] private Transform visualTransform;
    [SerializeField] private KeyCode slideKey = KeyCode.LeftShift;

    [SerializeField] private PlayerState currentState = PlayerState.Idle;
    [SerializeField] private int maxJumps = 2;
    private int jumpsRemaining;
    private CharacterController controller;
    private float verticalVelocity;
    private float slideTimer;
    private bool isCrouching = false;
    private bool isSliding = false;
    private bool hasWon = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        SetCrouch(false);
        jumpsRemaining = maxJumps;
    }

    private void Update()
    {
        if (isSliding)
            Slide();
        else
            Move();
    }

    private void Move()
    {
        bool crouchPressed = Input.GetKey(KeyCode.LeftControl);
        bool slideStarted = Input.GetKeyDown(slideKey);

        if (slideStarted && currentState == PlayerState.Run && controller.isGrounded)
        {
            StartSlide();
            return;
        }

        if (crouchPressed != isCrouching)
        {
            isCrouching = crouchPressed;
            SetCrouch(isCrouching);
        }

        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;
        Vector3 movement = transform.forward * currentSpeed;

        // Salto: permite mientras queden saltos disponibles, sin exigir estar en el suelo
        if (Input.GetButtonDown("Jump") && jumpsRemaining > 0 && !isCrouching)
        {
            Jump();
        }

        if (controller.isGrounded && verticalVelocity <= 0)
        {
            jumpsRemaining = maxJumps; // resetea saltos al tocar suelo

            if (isCrouching)
                ChangeState(PlayerState.CrouchWalk);
            else
                ChangeState(PlayerState.Run);

            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        if (!controller.isGrounded && verticalVelocity < 0)
        {
            ChangeState(PlayerState.Fall);
        }

        movement.y = verticalVelocity;
        controller.Move(movement * Time.deltaTime);
    }

    private void Jump()
    {
        ChangeState(PlayerState.Jump);
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        jumpsRemaining--;
    }
    private void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;

        SetCrouch(true);
        ChangeState(PlayerState.Slide);
    }

    private void Slide()
    {
        slideTimer -= Time.deltaTime;

        Vector3 movement = transform.forward * slideSpeed;

        if (controller.isGrounded && verticalVelocity <= 0)
        {
            verticalVelocity = -2f;
        }
        verticalVelocity += gravity * Time.deltaTime;
        movement.y = verticalVelocity;

        controller.Move(movement * Time.deltaTime);

        if (slideTimer <= 0f)
        {
            EndSlide();
        }
    }

    private void EndSlide()
    {
        isSliding = false;

        bool crouchStillHeld = Input.GetKey(KeyCode.LeftControl);
        isCrouching = crouchStillHeld;
        SetCrouch(crouchStillHeld);

        ChangeState(crouchStillHeld ? PlayerState.CrouchWalk : PlayerState.Run);
    }

    private void SetCrouch(bool crouching)
    {
        float targetHeight = crouching ? crouchHeight : normalHeight;

        controller.height = targetHeight;
        controller.center = new Vector3(0f, targetHeight / 2f, 0f);

        if (visualTransform != null)
        {
            float scaleRatio = targetHeight / normalHeight;
            visualTransform.localScale = new Vector3(1f, scaleRatio, 1f);
            visualTransform.localPosition = new Vector3(0f, targetHeight / 2f, 0f);
        }
    }

    public void TriggerWin()
    {
        hasWon = true;
        isSliding = false;
        verticalVelocity = 0f;
        ChangeState(PlayerState.Win);
        enabled = false;
    }

    private void ChangeState(PlayerState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        Debug.Log("State changed to: " + currentState);
    }
}