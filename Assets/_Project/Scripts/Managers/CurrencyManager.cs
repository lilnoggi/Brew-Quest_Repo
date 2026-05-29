using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Drag the Gold TextMeshPro UI element into this slot in the Inspector.")]
    [SerializeField] private TextMeshProUGUI _goldText;

    private void Update()
    {
        // SAFETY CHECK: Ensure the SaveSystem is loaded and ready before trying to count money
        if (SaveSystem.Instance == null || SaveSystem.Instance.CurrentProfile == null)
        {
            Debug.LogWarning("SaveSystem not ready. Cannot update gold display.");
            return;
        }

        double goldEarnedThisFrame = 0;

        // Calculate how much gold is made this exact frame
        foreach (var brew in SaveSystem.Instance.CurrentProfile.savedBrewsList)
        {
            if (brew.IsUnlockedOnTap)
            {
                double revenuePerSecond = IdleMathsBridge.CalculateDrinkRevenuePerSecond(brew.BaseDrinkValue, brew.CurrentLevel);

                // Multiply by Time.deltaTime to get the amount earned this frame, and add it to the total
                goldEarnedThisFrame += revenuePerSecond * Time.deltaTime;
            }
        }

        // Add to the total balance
        if (goldEarnedThisFrame > 0)
        {
            SaveSystem.Instance.CurrentProfile.totalGoldBalance += goldEarnedThisFrame;
        }

        // Update the UI using the formatter
        UpdateGoldUI();
    }

    private void UpdateGoldUI()
    {
        double currentGold = SaveSystem.Instance.CurrentProfile.totalGoldBalance;
        _goldText.text = $"Gold: {BigNumberFormatter.FormatValue(currentGold)}";
    }
}
