using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The image component that displays the ingredient icon.")]
    [SerializeField] private Image _slotIcon;
    [SerializeField] private Button _slotButton;

    // --- VARIABLES FOR THE CELLAR ---
    private IngredientData _ingredientData;
    private BrewingCellarManager _cellarManager;

    // --- VARIABLES FOR THE DESIGN STUDIO ---
    private Sprite _decalSprite;
    private int _decalDatabaseIndex;
    private DesignStudioManager _designManager;

    /// <summary>
    /// Called by the Manager to make this slot an empty background
    /// </summary>
    public void SetupEmptySlot()
    {
        _slotIcon.gameObject.SetActive(false); // Hide the icon
        _slotButton.interactable = false; // Disable the button
    }

// ==========================================
    // INGREDIENT LOGIC (For the Cellar)
    // ==========================================
    public void SetupFilled(IngredientData data, BrewingCellarManager manager)
    {
        _ingredientData = data;
        _cellarManager = manager;

        _slotIcon.sprite = data.Icon; // Set the icon to the ingredient's sprite

        _slotIcon.gameObject.SetActive(true); // Show the icon
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

    // ==========================================
    // DECAL LOGIC (For the Design Studio)
    // ==========================================
    public void SetupForDecal(Sprite decal, int index, DesignStudioManager manager)
    {
        _decalSprite = decal;
        _decalDatabaseIndex = index;
        _designManager = manager;

        _slotIcon.sprite = decal;
        _slotIcon.gameObject.SetActive(true);
        _slotButton.interactable = true;

        _slotButton.onClick.RemoveAllListeners();
        _slotButton.onClick.AddListener(OnDecalClicked);
    }

    private void OnDecalClicked()
    {
        if (_designManager != null)
        {
            _designManager.SelectDecal(_decalSprite, _decalDatabaseIndex);
        }
    }
}
