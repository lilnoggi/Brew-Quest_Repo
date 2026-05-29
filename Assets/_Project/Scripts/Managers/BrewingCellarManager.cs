using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool _isDrawerOpen = false;

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
        // This method will be called by the InventorySlotUI when a player taps an ingredient
        Debug.Log($"Selected ingredient: {ingredient.IngredientName}");

        // TODO: Add logic to put this ingredient into Slot 1 or Slot 2 for brewing, and update the UI accordingly
    }
}
