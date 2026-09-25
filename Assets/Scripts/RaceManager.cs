using UnityEngine;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject difficultyPanel;
    [SerializeField] private Button startButton;

    [Header("Player")]
    [SerializeField] private PlayerMovement player;

    private void Start()
    {
        // Al iniciar el juego mostramos la selección.
        difficultyPanel.SetActive(true);

        // El jugador todavía no puede moverse.
        player.PrepareForMenu();

        // Start comienza deshabilitado.
        startButton.interactable = false;

        ProgressManager.OnDifficultySelected += HandleDifficultySelected;

        UpdateStartButton();
    }

    private void OnDestroy()
    {
        ProgressManager.OnDifficultySelected -= HandleDifficultySelected;
    }

    private void HandleDifficultySelected(DifficultyTier tier)
    {
        UpdateStartButton();
    }

    private void UpdateStartButton()
    {
        startButton.interactable =
            ProgressManager.HasSelectedDifficulty;
    }

    public void StartRace()
    {
        if (!ProgressManager.HasSelectedDifficulty)
        {
            Debug.Log("Primero debes seleccionar una dificultad.");
            return;
        }

        DifficultyTier tier =
            ProgressManager.SelectedDifficulty;

        Debug.Log("Iniciando carrera en: " + tier);


        // trackGenerator.Generate(tier);

        difficultyPanel.SetActive(false);

        player.StartRace();
    }

    public void RaceWon()
    {
        DifficultyTier completedTier =
            ProgressManager.SelectedDifficulty;

        Debug.Log("Carrera completada: " + completedTier);

        // Registrar progreso.
        ProgressManager.Instance.RegisterCompletion(completedTier);

        // Regresar al menú de dificultad.
        difficultyPanel.SetActive(true);

        // El jugador deja de moverse.
        player.PrepareForMenu();

        // Mantener Start habilitado porque ya existe
        // una dificultad seleccionada.
        UpdateStartButton();
    }
}