using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BrewCardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private Button _upgradeButton;

    // This hold the exact data for this specific drink
    private CustomBrew _brewData;

    /// <summary>
    /// Called by the Manager when this card is spawned to give it data.
    /// </summary>
    public void InitialiseCard(CustomBrew brew)
    {
        _brewData = brew;
        
        // Connect the button click to the code automatically
        _upgradeButton.onClick.RemoveAllListeners(); // Clear any old listeners to avoid duplicates
        _upgradeButton.onClick.AddListener(AttemptUpgrade);

        UpdateCardVisuals();
    }

    private void UpdateCardVisuals()
    {
        _nameText.text = _brewData.CustomName;
        _levelText.text = $"Lvl {_brewData.CurrentLevel}";

        // Calculate how much it costs to reach the NEXT level
        double cost = IdleMathsBridge.CalculateUpgradeCost(_brewData.BaseDrinkValue, _brewData.CurrentLevel);
        _costText.text = $"Upgrade Cost: {BigNumberFormatter.FormatValue(cost)} Gold";

        // Calculate and display how much this drink earns per second
        double value = IdleMathsBridge.CalculateDrinkRevenuePerSecond(_brewData.BaseDrinkValue, _brewData.CurrentLevel);
        _valueText.text = $"+{BigNumberFormatter.FormatValue(value)} G/sec";
    }

    private void AttemptUpgrade()
    {
        // SAFETY CHECK: Ensure the SaveSystem is loaded and ready before trying to upgrade
        if (SaveSystem.Instance == null || SaveSystem.Instance.CurrentProfile == null)
        {
            Debug.LogWarning("SaveSystem not ready. Cannot attempt upgrade.");
            return;
        }

        double cost = IdleMathsBridge.CalculateUpgradeCost(_brewData.BaseDrinkValue, _brewData.CurrentLevel);

        // Check if the player has enough gold to upgrade
        if (SaveSystem.Instance.CurrentProfile.totalGoldBalance >= cost)
        {
            // Deduct the cost
            SaveSystem.Instance.CurrentProfile.totalGoldBalance -= cost;

            // Upgrade the brew's level
            _brewData.CurrentLevel++;

            // Save to hard drive immediately to avoid losing progress
            SaveSystem.Instance.SaveGameProgress();

            // Update the visuals to reflect the new level and next cost
            UpdateCardVisuals();
        }
        else
        {
            Debug.Log($"Not enough gold to upgrade this brew. You need {BigNumberFormatter.FormatValue(cost)} gold.");
        }
    }
}
