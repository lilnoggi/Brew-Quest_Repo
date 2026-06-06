using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image _itemIcon;
    [SerializeField] private Image _rarityIndicator;
    [SerializeField] private GameObject _pricingGroup;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private TextMeshProUGUI _quantityText;

    private IngredientData _currentIngredient;
    private SupplierManager _supplierManager;

    // Sets up the slot for a SHOP view (Shows price)
    public void SetupForShop(IngredientData ingredient, SupplierManager supplier)
    {
        _currentIngredient = ingredient;
        _supplierManager = supplier;

        _itemIcon.sprite = ingredient.Icon;
        _itemIcon.enabled = true;

        // Turn ON the pricing group and set the cost text
        _pricingGroup.SetActive(true);
        _costText.text = ingredient.PurchaseCost.ToString();

        SetRarityIndicator(ingredient.Tier);
    }

    // Sets up the slot for an INVENTORY view (Hides price)
    public void SetupForInventory(IngredientData ingredient, int currentQuantity)
    {
        _currentIngredient = ingredient;

        _itemIcon.sprite = ingredient.Icon;
        _itemIcon.enabled = true;

        // Turn OFF the pricing group
        _pricingGroup.SetActive(false);

        // Turn ON the quantity text and set the number
        if (_quantityText != null)
        {
            _quantityText.gameObject.SetActive(true);
            _quantityText.text = "x" + currentQuantity;
        }

        SetRarityIndicator(ingredient.Tier);

        // Ensure the button is interactable if there is more than 0
        GetComponent<Button>().interactable = currentQuantity > 0;
    }

    private void SetRarityIndicator(IngredientTier tier)
    {
        switch (tier)
        {
            case IngredientTier.Poor:
            _rarityIndicator.color = Color.gray;
                break;
            case IngredientTier.Common:
                _rarityIndicator.color = Color.gray;
                break;
            // case IngredientTier.Uncommon:
            //     _rarityIndicator.color = Color.green;
            //     break;
            case IngredientTier.Rare:
                _rarityIndicator.color = Color.blue;
                break;
            // case IngredientTier.Epic:
                // _rarityIndicator.color = Color.magenta;
                // break;
            // case IngredientTier.Legendary:
                // _rarityIndicator.color = Color.yellow;
                // break;
            default:
                _rarityIndicator.color = Color.white;
                break;
        }
    }

    // Attach this to the Button component on the prefab
    public void OnSlotClicked()
    {
        if (_pricingGroup.activeSelf && _currentIngredient != null)
        {
            // This is a shop slot, attempt to purchase
            _supplierManager.AttemptPurchase(_currentIngredient);
        }
        else if (_currentIngredient != null)
        {
            // This is an inventory slot, show details or allow selling
            // Implement inventory interaction logic here
        }
    }

    // Sets up the slot as an empty, un-interactable background
    public void SetupEmptySlot()
    {
        _currentIngredient = null;
        _itemIcon.sprite = null;
        _itemIcon.enabled = false;
        _pricingGroup.SetActive(false);
        _rarityIndicator.color = Color.clear; // Hide rarity indicator
        GetComponent<Button>().interactable = false; // Disable button interaction
        _quantityText.gameObject.SetActive(false); // Hide quantity text if it exists
    }
}
