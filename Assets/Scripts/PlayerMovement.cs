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

    [SerializeField] private PlayerState currentState = PlayerState.Idle;

    private CharacterController controller;
    private float verticalVelocity;
    private float slideTimer;
    private Vector3 slideDirection;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool crouchPressed = Input.GetKey(KeyCode.LeftControl);
        bool crouchStarted = Input.GetKeyDown(KeyCode.LeftControl);

        Vector3 movement = new Vector3(
            horizontal,
            0f,
            vertical
        );

        movement *= moveSpeed;

        // Jump
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            Jump();
        }

        // Ground
        if (controller.isGrounded && verticalVelocity <= 0)
        {
            if (movement.magnitude > 0.1f)
            {
                ChangeState(PlayerState.Run);
            }
            else
            {
                ChangeState(PlayerState.Idle);
            }

            verticalVelocity = -2f;
        }

        // Gravity
        verticalVelocity += gravity * Time.deltaTime;

        // Falling
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

        verticalVelocity = Mathf.Sqrt(
            jumpHeight * -2f * gravity
        );
    }
    private void SetCrouch(bool crouching)
    {
        if (crouching)
        {
            controller.height = crouchHeight;
            controller.center = new Vector3(0f, crouchHeight / 2f, 0f);
        }
        else
        {
            controller.height = normalHeight;
            controller.center = new Vector3(0f, normalHeight / 2f, 0f);
        }
    }

    private void ChangeState(PlayerState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        Debug.Log("State changed to: " + currentState);
    }
}