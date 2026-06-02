using System;
using UnityEngine;

// Static class does not need to be attached to a GameObject and can be accessed globally without instantiation.
public static class IdleMathsBridge
{
    // The cost multiplier scalar (e.g., 1.07 = 7% increase per upgrade level)
    // Kept private to prevent accidental modification by other scripts
    private static double _costGrowthMultiplier = 1.07;

    // ========================================
    // --- CORE ECONOMIC CALCULATIONS ---
    // ========================================

    /// <summary>
    /// Calculates exponential cost: Cost = BaseCost * (GrowthMultiplier ^ Owned)
    /// </summary>
    public static double CalculateUpgradeCost(double baseDrinkValue, int currentLevel)
    {
        return baseDrinkValue * Math.Pow(_costGrowthMultiplier, currentLevel - 1);
    }

    /// <summary>
    /// Calculates profit: Production = (BaseDrinkValue * Level) * MilestoneMultipliers
    /// </summary>
    public static double CalculateDrinkRevenuePerSecond(double baseDrinkValue, int currentLevel)
    {
        double baseProduction = baseDrinkValue * currentLevel;
        double multiplier = 1.0;

        // Milestone multi triggers: Stacking x2 at level 25, 50, and x4 at 100
        if (currentLevel >= 25) multiplier *= 2.0;
        if (currentLevel >= 50) multiplier *= 2.0;
        if (currentLevel >= 100) multiplier *= 4.0;

        // Apply the capacity multiplier: More steats = more revenue
        // Ensure it defaults to at least 1 so it doesn't break if the manager is not loaded
        int currentCapacity = PatronSimulationManager.Instance != null ? PatronSimulationManager.Instance.CurrentMaxPatronCapacity : 1;

        return (baseProduction * multiplier) * currentCapacity;
    }
    
    // ========================================
    // --- FLAVOUR SYNERGY ENGINE ---
    // ========================================

    /// <summary>
    /// Calculates the permanent baseline value of a newly created custom brew.
    /// </summary>
    public static double CalculateBaseDrinkValue(IngredientData ingredientA, IngredientData ingredientB)
    {
        // Add flat ingredient values together
        double totalValue = ingredientA.BaseValueContribution + ingredientB.BaseValueContribution;

        double tierMultiplier = GetTierMultiplier(ingredientA.Tier) * GetTierMultiplier(ingredientB.Tier);
        totalValue *= tierMultiplier;

        // Flavour Matrix Synergy Check
        double synergyModifier = 1.0;

        // Example clashing rule: Fiery + Bitter tastes terrible
        if ((ingredientA.FlavourProfile == FlavourProfile.Fiery && ingredientB.FlavourProfile == FlavourProfile.Bitter) ||
            (ingredientA.FlavourProfile == FlavourProfile.Bitter && ingredientB.FlavourProfile == FlavourProfile.Fiery))
        {
            synergyModifier = 0.50; // 50% "Poor Quality Brew" penalty reduction
        }
        // Example matching rule: Arcane + Sweet goes together incredibly well
        else if ((ingredientA.FlavourProfile == FlavourProfile.Arcane && ingredientB.FlavourProfile == FlavourProfile.Sweet) ||
                 (ingredientA.FlavourProfile == FlavourProfile.Sweet && ingredientB.FlavourProfile == FlavourProfile.Arcane))
        {
            synergyModifier = 1.35; // 35% Flavor Synergy bonus increase
        }

        return totalValue * synergyModifier;
    }

    private static double GetTierMultiplier(IngredientTier tier)
    {
        switch (tier)
        {
            case IngredientTier.Poor: return 1.0;
            case IngredientTier.Common: return 1.2;
            case IngredientTier.Rare: return 1.5;
            case IngredientTier.Mythic: return 2.5;
            default: return 1.0;
        }
    }
}
