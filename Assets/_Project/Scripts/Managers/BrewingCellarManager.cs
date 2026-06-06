using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Needed for UI components like Image and Button

public class BrewingCellarManager : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private List<IngredientData> _masterIngredientDatabase = new List<IngredientData>();

    [Header("Drawer UI Setup")]
    [SerializeField] private SlidingMenuUI _ingredientDrawerSlider; // Reference to the SlidingMenuUI component that controls the drawer animation
    [SerializeField] private Transform _gridContentContainer;
    [SerializeField] private GameObject _inventorySlotPrefab;
    [Tooltip("How many slots should make up the empty grid (e.g., 50 for a 5x10 grid).")]
    [SerializeField] private int _totalGridSize = 50;

    [Header("Selected Ingredient Slots")]
    [SerializeField] private Image _uiSlot1Image; // Drag "Slot 1" Image here
    [SerializeField] private Image _uiSlot2Image; // Drag "Slot 2" Image here
    [SerializeField] private Button _mixButton; // Drag the central "Mix" Button here

    [Header("Naming Popup UI")]
    [SerializeField] private GameObject _namingPopupPanel;
    [SerializeField] private TMP_InputField _nameInputField;

    // Temporarly hold the value of the mixed brew while the player types the name
    private double _pendingBaseValue;

    // --- TRACK WHAT INGREDIENTS THE PLAYER HAS SELECTED FOR BREWING ---
    private IngredientData _selectedIngredient1;
    private IngredientData _selectedIngredient2;

    // ------------------------------------------------------------------------------------------------

    private void Start()
    {
        // Build the inventory grid the movement the game starts
        RefreshInventoryGrid();
    }

    public void RefreshInventoryGrid()
    {
        // Clear any placeholder items
        foreach (Transform child in _gridContentContainer)
        {
            Destroy(child.gameObject);
        }

        // Loop 50 times to create the rigid grid
        for (int i = 0; i < _totalGridSize; i++)
        {
            GameObject newSlot = Instantiate(_inventorySlotPrefab, _gridContentContainer);
            ItemSlotUI slotUI = newSlot.GetComponent<ItemSlotUI>();

            if (slotUI != null)
            {
                // Check if there is an item in the saved inventory for this slot index
                if (i < SaveSystem.Instance.CurrentProfile.ownedIngredientsList.Count)
                {
                    IngredientInventoryItem savedItem = SaveSystem.Instance.CurrentProfile.ownedIngredientsList[i];

                    // The string is saved. Find the actual IngredientData object
                    IngredientData foundIngredient = null;
                    foreach (var ingredient in _masterIngredientDatabase)
                    {
                        if (ingredient.IngredientName == savedItem.ingredientName)
                        {
                            foundIngredient = ingredient;
                            break;
                        }
                    }

                    // If the ingredient was found set up the slot
                    if (foundIngredient != null)
                    {
                        slotUI.SetupForInventory(foundIngredient, savedItem.quantity, this);
                    }
                    else
                    {
                        Debug.LogWarning($"Ingredient '{savedItem.ingredientName}' not found in master database. Check for typos or ensure the ingredient is added to the database.");
                        slotUI.SetupEmptySlot(); // Set up as empty if not found
                    }
                }
                else
                {
                    // Otherwise, make it an empty slot
                    slotUI.SetupEmptySlot();
                }
            }
        }
    }

    // =====================================================================
    // --- GAMEPLAY LOGIC ---
    // =====================================================================

    public void SelectIngredient(IngredientData ingredient)
    {
        // Check if Slot 1 is empty, if it is, put the ingredient there
        if (_selectedIngredient1 == null)
        {
            _selectedIngredient1 = ingredient;
            _uiSlot1Image.sprite = ingredient.Icon; // Update the UI image to show the selected ingredient
            _uiSlot1Image.color = Color.white; // Ensure the image is visible (in case it was greyed out)
        }
        // Otherwise, check if Slot 2 is empty, if it is, put the ingredient there
        else if (_selectedIngredient2 == null)
        {
            _selectedIngredient2 = ingredient;
            _uiSlot2Image.sprite = ingredient.Icon; // Update the UI image to show the selected ingredient
            _uiSlot2Image.color = Color.white; // Ensure the image is visible (in case it was greyed out)

            // Both slots are filled, close the drawer automatically
            if (_ingredientDrawerSlider != null)
            {
                _ingredientDrawerSlider.CloseMenu();
            }
        }
        else
        {
            Debug.Log("Both ingredient slots are already filled!");
        }
    }

    // Attach to the main "MIX" button
    public void AttemptToMixDrink()
    {
        // SAFETY CHECK: Ensure both slots are filled before attempting to mix
        if (_selectedIngredient1 == null || _selectedIngredient2 == null)
        {
            Debug.Log("Please select two ingredients before mixing!");
            return;
        }

        // Calculate the resulting drink based on the two selected ingredients
        _pendingBaseValue = IdleMathsBridge.CalculateBaseDrinkValue(_selectedIngredient1, _selectedIngredient2);

        // Prepare the popup
        _nameInputField.text = ""; // Clear out any old text

        // Show the naming popup
        _namingPopupPanel.SetActive(true);
    }

    /// <summary>
    /// Attatch this to the "Confirm" button inside the Naming Popup Panel
    /// </summary>
    public void ConfirmDrinkName()
    {
        // Get the typed name, or give a default if left blank
        string finalName = _nameInputField.text;
        if (string.IsNullOrWhiteSpace(finalName))
        {
            finalName = "Nameless Brew";

            // FUTURE: Depending on the ingredients used, give a default name that relates to the brew.
        }

        // Find the Design Studio Manager and pass the data over
        DesignStudioManager designManager = FindAnyObjectByType<DesignStudioManager>();
        if (designManager != null)
        {
            designManager.StartDesignProcess(finalName, _pendingBaseValue);
        }

        SaveSystem.Instance.RemoveIngredientFromInventory(_selectedIngredient1.IngredientName, 1);
        SaveSystem.Instance.RemoveIngredientFromInventory(_selectedIngredient2.IngredientName, 1);

        // Hide the popup and clear the center slots so the player can brew again
        _namingPopupPanel.SetActive(false);
        ClearSelectedSlots();

        // Refresh the grid so the numbers update visually
        RefreshInventoryGrid();

        // --- TRANSITION LOGIC ---
        // Use the UIManager to switch to the design studio
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SwitchView((int)ViewState.DesignStudio);
        }
    }

    private void ClearSelectedSlots()
    {
        // Wipe the data
        _selectedIngredient1 = null;
        _selectedIngredient2 = null;

        _uiSlot1Image.sprite = null; // Clear the image
        _uiSlot1Image.color = new Color(1, 1, 1, 0); // Make it invisible

        _uiSlot2Image.sprite = null; // Clear the image
        _uiSlot2Image.color = new Color(1, 1, 1, 0); // Make it invisible
    }
}
