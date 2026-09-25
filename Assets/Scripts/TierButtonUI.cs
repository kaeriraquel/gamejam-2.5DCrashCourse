using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TierButtonUI : MonoBehaviour
{
    [SerializeField] private DifficultyTier tier;
    [SerializeField] private Button button;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private GameObject selectedHighlight;

    private void OnEnable()
    {
        ProgressManager.OnDifficultySelected += HandleSelectionChanged;
        ProgressManager.OnProgressChanged += RefreshUI;
    }

    private void OnDisable()
    {
        ProgressManager.OnDifficultySelected -= HandleSelectionChanged;
        ProgressManager.OnProgressChanged -= RefreshUI;
    }

    private void Start()
    {
        button.onClick.AddListener(OnClick);

        RefreshUI();

        // Al comenzar no debe aparecer ningún tier seleccionado.
        if (selectedHighlight != null)
        {
            selectedHighlight.SetActive(
                ProgressManager.HasSelectedDifficulty &&
                ProgressManager.SelectedDifficulty == tier
            );
        }
    }

    private void RefreshUI()
    {
        if (ProgressManager.Instance == null)
            return;

        bool unlocked = ProgressManager.Instance.IsUnlocked(tier);

        button.interactable = unlocked;

        if (lockIcon != null)
            lockIcon.SetActive(!unlocked);

        if (progressText != null)
        {
            if (unlocked)
            {
                progressText.text = "";
            }
            else
            {
                DifficultyTier previousTier =
                    tier == DifficultyTier.Tier2
                        ? DifficultyTier.Tier1
                        : DifficultyTier.Tier2;

                int current =
                    ProgressManager.Instance.GetCompletions(previousTier);

                int required =
                    ProgressManager.Instance.GetUnlockRequirement(tier);

                progressText.text = $"{current} / {required}";
            }
        }

        // Actualizar selección visual
        if (selectedHighlight != null)
        {
            selectedHighlight.SetActive(
                ProgressManager.HasSelectedDifficulty &&
                ProgressManager.SelectedDifficulty == tier
            );
        }
    }

    private void HandleSelectionChanged(DifficultyTier selected)
    {
        if (selectedHighlight != null)
        {
            selectedHighlight.SetActive(selected == tier);
        }
    }

    private void OnClick()
    {
        if (!ProgressManager.Instance.IsUnlocked(tier))
            return;

        ProgressManager.Instance.SelectDifficulty(tier);
    }
}