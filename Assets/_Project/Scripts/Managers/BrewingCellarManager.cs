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
    [SerializeField] private RectTransform _drawerPanel;
    [SerializeField] private Transform _gridContentContainer;
    [SerializeField] private GameObject _inventorySlotPrefab;
    [Tooltip("How many slots should make up the empty grid (e.g., 50 for a 5x10 grid).")]
    [SerializeField] private int _totalGridSize = 50;
    [Tooltip("The Y position when the drawer is fully OPEN (usually 0)")]
    [SerializeField] private float _drawerOpenYPosition = 0f;
    [Tooltip("The Y position when the drawer is fully CLOSED (e.g. -1118)")]
    [SerializeField] private float _drawerClosedYPosition = -1118f;

    [Header("Selected Ingredient Slots")]
    [SerializeField] private Image _uiSlot1Image; // Drag "Slot 1" Image here
    [SerializeField] private Image _uiSlot2Image; // Drag "Slot 2" Image here
    [SerializeField] private Button _mixButton; // Drag the central "Mix" Button here

    [Header("Naming Popup UI")]
    [SerializeField] private GameObject _namingPopupPanel;
    [SerializeField] private TMP_InputField _nameInputField;

    // Temporarly hold the value of the mixed brew while the player types the name
    private double _pendingBaseValue;

    private bool _isDrawerOpen = false;

    // --- TRACK WHAT INGREDIENTS THE PLAYER HAS SELECTED FOR BREWING ---
    private IngredientData _selectedIngredient1;
    private IngredientData _selectedIngredient2;

    // ------------------------------------------------------------------------------------------------

    private void Start()
    {
        // Build the inventory grid the movement the game starts
        InitialiseInventoryGrid();
    }

    private void InitialiseInventoryGrid()
    {
        // Loop 50 times to create the rigid grid
        for (int i = 0; i < _totalGridSize; i++)
        {
            GameObject newSlot = Instantiate(_inventorySlotPrefab, _gridContentContainer);
            InventorySlotUI slotUI = newSlot.GetComponent<InventorySlotUI>();

            if (slotUI != null)
            {
                // If we still have ingredients in the database, fill the slot
                if (i < _masterIngredientDatabase.Count)
                {
                    slotUI.SetupFilled(_masterIngredientDatabase[i], this);
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
    // --- DRAWER ANIMATION LOGIC ---
    // ===================================================================== 

    /// <summary>
    /// Attatch this to the "Open Ingredients" Button
    /// </summary>
    public void ToggleDrawer()
    {
        _isDrawerOpen = !_isDrawerOpen; // Flip the bool to track the new state

        // Stop any current sliding animations
        StopAllCoroutines();

        // Target Y position: 0 is fully on screen, -800 is hidden off the bottom
        float targetY = _isDrawerOpen ? _drawerOpenYPosition : _drawerClosedYPosition;

        StartCoroutine(SlideDrawerRoutine(targetY));
    }

    private IEnumerator SlideDrawerRoutine(float targetY)
    {
        float duration = 0.3f; // Duration of the slide in 0.3 seconds
        float timeElapsed = 0f;

        Vector2 startingPos = _drawerPanel.anchoredPosition;
        Vector2 targetPos = new Vector2(startingPos.x, targetY);

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            
            // Lerp smoothly moves the panel from its start position to the target
            _drawerPanel.anchoredPosition = Vector2.Lerp(startingPos, targetPos, timeElapsed / duration);

            yield return null; // Wait until next frame
        }

        // Ensure it ends exactly at the target position
        _drawerPanel.anchoredPosition = targetPos;
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
            if (_isDrawerOpen)
            {
                ToggleDrawer();
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

            // FUTURE: Depending on the ingredients used, give a name that relates to the brew.
        }

        // Create the final drink using the held value
        CustomBrew newDrink = new CustomBrew(finalName, _pendingBaseValue, 0, 0);

        // Save the new drink to the save system (this will be expanded later to include the actual recipe and not just the resulting drink)
        SaveSystem.Instance.CurrentProfile.savedBrewsList.Add(newDrink);
        SaveSystem.Instance.SaveGameProgress(); // Save immediately to ensure the new brew is stored

        // Tell the Tavern Menu to refresh so the new brew appears on tap right away
        BrewMenuManager menuManager = FindAnyObjectByType<BrewMenuManager>();
        if (menuManager != null)
        {
            menuManager.RefreshMenu();
        }
        
        Debug.Log($"SUCCESS! Brewed {newDrink.CustomName} with {_selectedIngredient1.IngredientName} and {_selectedIngredient2.IngredientName}. Base Value: {newDrink.BaseDrinkValue}");

        // Hide the popup and clear the center slots so the player can brew again
        _namingPopupPanel.SetActive(false);
        ClearSelectedSlots();
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
