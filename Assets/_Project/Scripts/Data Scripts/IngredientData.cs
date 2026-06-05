using UnityEngine;

// public enum IngredientTier { Poor, Common, Uncommon, Rare, Epic, Mythic, Legendary }
public enum IngredientTier { Poor, Common, Rare, Mythic } // The quality tier of the ingredient, which will act as a multiplier for the final drink value.
public enum FlavourProfile { Sweet, Bitter, Fiery, Arcane } // The primary flavour, used to calculate synergy bonuses or clashing penalties when mixed.

[CreateAssetMenu(fileName = "NewIngredient", menuName = "Brew Quest/Ingredient")]
public class IngredientData : ScriptableObject
{
    [Header("Basic Information")]
    [Tooltip("The display name of the ingredient in the UI.")]
    [SerializeField] private string _ingredientName;

    [Tooltip("The visual icon representing the ingredient.")]
    [SerializeField] private Sprite _icon;
    
    [Header("Economic Statistics")]
    [Tooltip("Determines the base quality multiplier.")]
    [SerializeField] private IngredientTier _tier;

    [Tooltip("Used to check if ingredients mix well together.")]
    [SerializeField] private FlavourProfile _flavourProfile;

    [Tooltip("The flat gold value this ingredient adds to the baseline price of a custom brew.")]
    [SerializeField] private int _baseValueContribution; 

    // How much Gold it costs to buy this ingredient from a vendor.
    [SerializeField] private double _purchaseCost;

    // ===================================================================================================

    // --- Public Read-Only Properties ---
    // These allow other scripts to read the data without accidentally modifying the master template.
    public string IngredientName => _ingredientName;
    public IngredientTier Tier => _tier;
    public FlavourProfile FlavourProfile => _flavourProfile;
    public int BaseValueContribution => _baseValueContribution;
    public Sprite Icon => _icon;
    public double PurchaseCost => _purchaseCost;
}
