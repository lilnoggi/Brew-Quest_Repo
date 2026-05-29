using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The image component that displays the ingredient icon.")]
    [SerializeField] private Image _ingredientIcon;
    [SerializeField] private Button _slotButton;

    private IngredientData _ingredientData;
    private BrewingCellarManager _cellarManager;

    /// <summary>
    /// Called by the Manager to make this slot an empty background
    /// </summary>
    public void SetupEmptySlot()
    {
        _ingredientIcon.gameObject.SetActive(false); // Hide the icon
        _slotButton.interactable = false; // Disable the button
    }

    /// <summary>
    /// Called by the Manager to put an active ingredient into this slot
    /// </summary>
    public void SetupFilled(IngredientData data, BrewingCellarManager manager)
    {
        _ingredientData = data;
        _cellarManager = manager;

        // Set the actual sprite here once assets are ready
        // _ingredientIcon.sprite = data.Icon; // Set the icon to the ingredient's sprite

        _ingredientIcon.gameObject.SetActive(true); // Show the icon
        _slotButton.interactable = true; // Enable the button

        // Add a listener to the button to handle clicks
        _slotButton.onClick.RemoveAllListeners(); // Clear any existing listeners to prevent duplicates
        _slotButton.onClick.AddListener(OnSlotClicked);
    }

    private void OnSlotClicked()
    {
        // Tell the master manager that the player tapped this specific ingredient
        _cellarManager.SelectIngredient(_ingredientData);
    }
}
