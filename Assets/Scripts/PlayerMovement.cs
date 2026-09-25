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
    [SerializeField] private bool isCrouching = false;
    [SerializeField] private Transform visualTransform;

    private CharacterController controller;
    private float verticalVelocity;
    private float slideTimer;
    private Vector3 slideDirection;

private void Awake()
{
    controller = GetComponent<CharacterController>();
    SetCrouch(false); 
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

        // Solo actualiza el collider cuando el estado realmente cambia
        if (crouchPressed != isCrouching)
        {
            isCrouching = crouchPressed;
            SetCrouch(isCrouching);
            // Debug.Log("Crouch: " + isCrouching);
        }

        Vector3 movement = new Vector3(horizontal, 0f, vertical);
        bool isMoving = movement.magnitude > 0.1f;

        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;
        movement *= currentSpeed;

        if (Input.GetButtonDown("Jump") && controller.isGrounded && !isCrouching)
        {
            Jump();
        }

        if (controller.isGrounded && verticalVelocity <= 0)
        {
            if (isCrouching)
                ChangeState(isMoving ? PlayerState.CrouchWalk : PlayerState.Idle);
            else if (isMoving)
                ChangeState(PlayerState.Run);
            else
                ChangeState(PlayerState.Idle);

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

        verticalVelocity = Mathf.Sqrt(
            jumpHeight * -2f * gravity
        );
    }

    private void SetCrouch(bool crouching)
    {
        float targetHeight = crouching ? crouchHeight : normalHeight;

        // Collider (colisión física, invisible)
        controller.height = targetHeight;
        controller.center = new Vector3(0f, targetHeight / 2f, 0f);

        // Visual (lo que se ve enpantalla)
        if (visualTransform != null)
        {
            float scaleRatio = targetHeight / normalHeight;
            visualTransform.localScale = new Vector3(1f, scaleRatio, 1f);
            visualTransform.localPosition = new Vector3(0f, targetHeight / 2f, 0f);
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