using UnityEngine;

public enum DifficultyTier
{
    Tier1 = 0,
    Tier2 = 1,
    Tier3 = 2
}

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    [Header("Desbloqueo")]
    [SerializeField] private int tier2UnlockCount = 10;
    [SerializeField] private int tier3UnlockCount = 10;

    public static DifficultyTier SelectedDifficulty { get; private set; }

    public static bool HasSelectedDifficulty { get; private set; }

    public static event System.Action<DifficultyTier> OnDifficultySelected;
    public static event System.Action OnProgressChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Al iniciar no hay dificultad seleccionada todavía.
        HasSelectedDifficulty = false;
        SelectedDifficulty = DifficultyTier.Tier1;
    }

    public int GetCompletions(DifficultyTier tier)
    {
        return PlayerPrefs.GetInt(CompletionsKey(tier), 0);
    }

    public void RegisterCompletion(DifficultyTier tier)
    {
        int count = GetCompletions(tier) + 1;

        PlayerPrefs.SetInt(CompletionsKey(tier), count);
        PlayerPrefs.Save();

        OnProgressChanged?.Invoke();
    }

    public bool IsUnlocked(DifficultyTier tier)
    {
        switch (tier)
        {
            case DifficultyTier.Tier1:
                return true;

            case DifficultyTier.Tier2:
                return GetCompletions(DifficultyTier.Tier1) >= tier2UnlockCount;

            case DifficultyTier.Tier3:
                return GetCompletions(DifficultyTier.Tier2) >= tier3UnlockCount;

            default:
                return false;
        }
    }

    public int GetUnlockRequirement(DifficultyTier tier)
    {
        if (tier == DifficultyTier.Tier2)
            return tier2UnlockCount;

        if (tier == DifficultyTier.Tier3)
            return tier3UnlockCount;

        return 0;
    }

    public void SelectDifficulty(DifficultyTier tier)
    {
        if (!IsUnlocked(tier))
            return;

        SelectedDifficulty = tier;
        HasSelectedDifficulty = true;

        OnDifficultySelected?.Invoke(SelectedDifficulty);
    }

    private string CompletionsKey(DifficultyTier tier)
    {
        return $"Completions_{tier}";
    }
}