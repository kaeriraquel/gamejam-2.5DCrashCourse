using UnityEngine;

// Estados del personaje
public enum PlayerState
{
    Idle,       // Quieto
    Ready,      // Estado previo a iniciar la pista, aun no lo hago xd
    Run,        // Corriendo  automáticamente
    Jump,       // Saltar
    Slide,      // Deslizándose rápido por el suelo con shift
    CrouchWalk, // Caminando agachado (Ctrl)
    Fall,       // Cayendo
    Win         // Meta
}

public class PlayerMovement : MonoBehaviour
{
    // ====== MOVIMIENTO NORMAL ======
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;      // Altura ddel slto

    // ======agacharse ======
    [SerializeField] private float crouchHeight = 1f;   //altura del collider agachado
    [SerializeField] private float normalHeight = 2f;    //altura del collider normal
    [SerializeField] private float crouchSpeed = 2.5f;   // Velocidad  mientras está agachado

    // ====== deslizarse ======
    [SerializeField] private float slideSpeed = 8f;      // Velocidadmás rápido que correr
    [SerializeField] private float slideDuration = 0.6f;
    [SerializeField] private KeyCode slideKey = KeyCode.LeftShift;

    // ====== DOBLE SALTO ======
    [SerializeField] private int maxJumps = 2;           // Saltos totales
    private int jumpsRemaining;                          // Saltos que le quedan antes de tocar suelo de nuevo
    private bool raceStarted = false;


    [SerializeField] private Transform visualTransform;

    [SerializeField] private PlayerState currentState = PlayerState.Idle;

    private CharacterController controller;
    private float verticalVelocity; // salto + gravedad
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
        if (!raceStarted)
            return;

        if (isSliding)
            Slide();
        else
            Move();
    }

    // movimientos principales
    private void Move()
    {
        bool crouchPressed = Input.GetKey(KeyCode.LeftControl); // Ctrl = agachado
        bool slideStarted = Input.GetKeyDown(slideKey);          // Shift = iniciar slide

        // --- SLIDE: solo se puede iniciar corriendo y tocando el suelo ---
        if (slideStarted && currentState == PlayerState.Run && controller.isGrounded)
        {
            StartSlide();
            return;
        }

        // --- CROUCH
        if (crouchPressed != isCrouching)
        {
            isCrouching = crouchPressed;
            SetCrouch(isCrouching);
        }


        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;


        Vector3 movement = transform.forward * currentSpeed;

        // --- JUMP / DOBLE SALTO
        if (Input.GetButtonDown("Jump") && jumpsRemaining > 0 && !isCrouching)
        {
            Jump();
        }

        // al tocar suelo, resetea saltos y decide el estado (Run o CrouchWalk)
        if (controller.isGrounded && verticalVelocity <= 0)
        {
            jumpsRemaining = maxJumps;

            if (isCrouching)
                ChangeState(PlayerState.CrouchWalk);
            else
                ChangeState(PlayerState.Run);

            verticalVelocity = -2f; // fuerza para mantenerlo "pegado" al suelo
        }

        // --- GRAVEDAD
        verticalVelocity += gravity * Time.deltaTime;

        // --- FALL
        if (!controller.isGrounded && verticalVelocity < 0)
        {
            ChangeState(PlayerState.Fall);
        }

        movement.y = verticalVelocity;
        controller.Move(movement * Time.deltaTime);
    }

    // deslizamiento
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

        Vector3 movement = transform.forward * slideSpeed; // avanza rápido, siempre hacia adelante

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

    // jump
    private void Jump()
    {
        ChangeState(PlayerState.Jump);

        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        jumpsRemaining--;
    }

    // crouch
    private void SetCrouch(bool crouching)
    {
        float targetHeight = crouching ? crouchHeight : normalHeight;

        // Collider físic
        controller.height = targetHeight;
        controller.center = new Vector3(0f, targetHeight / 2f, 0f);


        // para que los "pies" sigan tocando el piso en vez de flotar o hundirse
        if (visualTransform != null)
        {
            float scaleRatio = targetHeight / normalHeight;
            visualTransform.localScale = new Vector3(1f, scaleRatio, 1f);
            visualTransform.localPosition = new Vector3(0f, targetHeight / 2f, 0f);
        }
    }

    // meta win
    public void TriggerWin()
    {
        if (!raceStarted)
            return;

        hasWon = true;
        raceStarted = false;

        isSliding = false;
        verticalVelocity = 0f;

        ChangeState(PlayerState.Win);

        // Avisamos al RaceManager.
        RaceManager raceManager = FindFirstObjectByType<RaceManager>();

        if (raceManager != null)
        {
            raceManager.RaceWon();
        }
    }


    private void ChangeState(PlayerState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        // Debug.Log("State changed to: " + currentState);
    }

    public void StartRace()
    {
        raceStarted = true;

        hasWon = false;

        enabled = true;

        ChangeState(PlayerState.Run);
    }

    public void PrepareForMenu()
    {
        raceStarted = false;

        isSliding = false;
        isCrouching = false;
        hasWon = false;

        verticalVelocity = 0f;
        jumpsRemaining = maxJumps;

        SetCrouch(false);

        ChangeState(PlayerState.Idle);
    }
}