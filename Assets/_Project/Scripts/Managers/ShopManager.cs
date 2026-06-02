using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<TavernUpgradeData> _availableUpgrades = new List<TavernUpgradeData>(); // A list of all possible upgrades that can be offered in the shop.
    [SerializeField] private Transform _shopContentContainer; // The parent transform where upgrade UI elements will be instantiated.
    [SerializeField] private GameObject _shopCardPrefab; // A prefab for the UI card that represents each upgrade in the shop.

    // A list of strings (UpgradeNames) the player has already purchased, saved in GameSaveData
    private List<string> _purchasedUpgrades = new List<string>();

    private void Start()
    {
        LoadPurchasedUpgrades();
        PopulateShop();
    }

    private void LoadPurchasedUpgrades()
    {
        // Load purchased upgrades from GameSaveData
        if (SaveSystem.Instance != null && SaveSystem.Instance.CurrentProfile != null)
        {
           _purchasedUpgrades = SaveSystem.Instance.CurrentProfile.purchasedUpgradesList;
        }
    }

    private void PopulateShop()
    {
        // Clear exisiting cards
        foreach (Transform child in _shopContentContainer)
        {
            Destroy(child.gameObject);
        }

        // Spawn a card for every upgrade in the database
        foreach (var upgrade in _availableUpgrades)
        {
            // Check prequisites and whether the upgrade has already been purchased
            if (upgrade.PrerequisiteUpgrade != null && !_purchasedUpgrades.Contains(upgrade.PrerequisiteUpgrade.UpgradeName))
            {
                continue; // Skip this upgrade if the prerequisite hasn't been purchased
            }

            GameObject newCard = Instantiate(_shopCardPrefab, _shopContentContainer);

            ShopCardUI ui = newCard.GetComponent<ShopCardUI>();
            ui.Setup(upgrade, this);
        }
    }

    public void AttemptPurchase(TavernUpgradeData upgradeToBuy)
    {
        // Check if the player has enough gold and meets any other requirements
        // If so, deduct gold, apply the upgrade effects, and add the upgrade to the purchased list
        // Then call PopulateShop() again to refresh the available upgrades based on new prerequisites
        if (_purchasedUpgrades.Contains(upgradeToBuy.UpgradeName))
        {
            Debug.Log("Upgrade already purchased!");
            return;
        }

        // Check against current gold balance
        if (SaveSystem.Instance.CurrentProfile.totalGoldBalance >= upgradeToBuy.UpgradeBaseCost)
        {
            // Deduct gold
            SaveSystem.Instance.CurrentProfile.totalGoldBalance -= upgradeToBuy.UpgradeBaseCost;

            // Add to purchased list
            _purchasedUpgrades.Add(upgradeToBuy.UpgradeName);

            // Apply the actual effect
            ApplyUpgradeEffect(upgradeToBuy);

            // Save the updated profile
            SaveSystem.Instance.SaveGameProgress();

            // Refresh shop
            PopulateShop();

            // Refresh the 3D Environment
            TavernVisualManager visualManager = FindAnyObjectByType<TavernVisualManager>();
            if (visualManager != null)
            {
                visualManager.UpdateVisuals();
            }

            Debug.Log($"Purchased upgrade: {upgradeToBuy.UpgradeName}");
        }
        else
        {
            Debug.Log("Not enough gold to purchase this upgrade!");
        }
    }

    private void ApplyUpgradeEffect(TavernUpgradeData upgrade)
    {
        // This method would contain the logic to apply the effects of the upgrade to the tavern's stats.
        // For example, if it's a Capacity upgrade, it would increase the tavern's capacity by the specified modifier value.
        // The actual implementation would depend on how the tavern's stats are structured in the game.
        // For example:
        // if (upgrade.Type == UpgradeType.Capacity) { PatronManager.Instance.MaxCapacity += (int)upgrade.ModifierValue; }

        if (upgrade.UpgradeType == UpgradeType.Capacity)
        {
            if (PatronSimulationManager.Instance != null)
            {
                PatronSimulationManager.Instance.RecalculatePatronCapacity();
            }
        }
    }
}
