using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DecalCategory { Nature, Arcane, Crests }

[System.Serializable]
public struct DecalOption
{
    public Sprite DecalSprite;
    public DecalCategory category;
}

public class DesignStudioManager : MonoBehaviour
{
    // Data holding variables
    private string _pendingBrewName;
    private double _pendingBaseValue;

    [Header("3D Models (Vessles)")]
    [Tooltip("Drag the 3D models here from the DesignStudio_3DEnvironment.")]
    [SerializeField] private List<GameObject> _vesselModels = new List<GameObject>();

    [Header("Decal System")]
    [Tooltip("Add all the decal sprite here and assign them a category.")]
    [SerializeField] private List<DecalOption> _masterDecalDatabase = new List<DecalOption>();
    [SerializeField] private Image _appliedDecalImage;

    [Tooltip("The parent object holding the grid slots.")]
    [SerializeField] private Transform _decalGridContent;
    [SerializeField] private GameObject _decalSlotPrefab;

    [Header("Flow Control UI")]
    [SerializeField] private GameObject _containerVesselArrows; // Parent holding the arrow buttons
    [SerializeField] private GameObject _buttonConfirmVessel; // Button to confirm cup type
    [SerializeField] private GameObject _containerDecalSystem; // Parent holding the decal tabs and decal inventory
    [SerializeField] private GameObject _containerColourPalette; // Parent holding the colour swatch buttons

    private int _selectedDecalIndex = 0; // Tracks the player's final choice for the CustomBrew save data

    // This tracks exactly which shape the player is looking at.
    // It will directly map to CustomBrew._bottleShapeIndex
    private int _currentVesselIndex = 0;

    // Hold the chosen colour temporarily
    private Color _selectedVesselColour = Color.white;

    // =======================================================================================================

    private void Start()
    {
        // Make sure only the first model is visible when the game starts
        UpdateVesselVisibility();
    }

    // Attatch to the "-->" Next Button
    public void CycleNextVessel()
    {
        _currentVesselIndex++;

        // If the end of the list, loop back to start (0)
        if (_currentVesselIndex >= _vesselModels.Count)
        {
            _currentVesselIndex = 0;
        }

        UpdateVesselVisibility();
    }

    // Attatch to the "<--" Previous Button
    public void CyclePreviousVessel()
    {
        _currentVesselIndex--;

        // If below 0, loop back to the last item in the list
        if (_currentVesselIndex < 0)
        {
            _currentVesselIndex = _vesselModels.Count -1;
        }

        UpdateVesselVisibility();
    }

    private void UpdateVesselVisibility()
    {
        // Loop through all the models. Turn ON the one that matches the index, turn OFF the rest
        for (int i = 0; i < _vesselModels.Count; i++)
        {
            if (i == _currentVesselIndex)
            {
                _vesselModels[i].SetActive(true);
            }
            else
            {
                _vesselModels[i].SetActive(false);
            }
        }
    }

    // =====================================================================
    // --- DECAL GRID LOGIC ---
    // =====================================================================

    // Attatch this to the Category Tab buttons.
    // For the "Nature" button, select, "Nature" from the dropdown
    public void LoadDecalCategory(DecalCategory selectedCategory)
    {
        // Clear the current grid
        foreach (Transform child in _decalGridContent)
        {
            Destroy(child.gameObject);
        }

        // Loop through the database and spawn ONLY the ones that match the tab
        for (int i = 0; i < _masterDecalDatabase.Count; i++)
        {
            if (_masterDecalDatabase[i].category == selectedCategory)
            {
                GameObject newSlot = Instantiate(_decalSlotPrefab, _decalGridContent);

                InventorySlotUI slotUI = newSlot.GetComponent<InventorySlotUI>();
                if (slotUI != null)
                {
                    // Pass the sprite, its index in the master list, and a reference to this manager.
                    slotUI.SetupForDecal(_masterDecalDatabase[i].DecalSprite, i, this);
                }
            }
        }
    }

    public void SelectDecal(Sprite pressedSprite, int indexInDatabase)
    {
        _selectedDecalIndex = indexInDatabase;

        // Show the decal on the bottle
        _appliedDecalImage.sprite = pressedSprite;
        _appliedDecalImage.color = Color.white;
    }

    // =====================================================================
    // --- UI BUTTON HOOKS ---
    // =====================================================================

    public void OnClickTabNature()
    {
        LoadDecalCategory(DecalCategory.Nature);
    }

    public void OnClickTabArcane()
    {
        LoadDecalCategory(DecalCategory.Arcane);
    }

    public void OnClickTabCrests()
    {
        LoadDecalCategory(DecalCategory.Crests);
    }

    // --- COLOUR SWATCH HOOKS ---
    public void OnClickColourRed()
    {
        SetVesselColour(Color.red);
    }

    public void OnClickColourGreen()
    {
        SetVesselColour(Color.green);
    }

    public void OnClickColourBlue()
    {
        SetVesselColour(Color.blue);
    }

    // =====================================================================
    // --- PIPELINE LOGIC ---
    // =====================================================================

    // The cellar manager will call this when the player confirms the name
    public void StartDesignProcess(string brewName, double baseValue)
    {
        _pendingBrewName = brewName;
        _pendingBaseValue = baseValue;

        // Reset the UI to Step 1: Vessel Selection
        _currentVesselIndex = 0;
        UpdateVesselVisibility();

        _selectedDecalIndex = 0;
        _appliedDecalImage.sprite = null;
        _appliedDecalImage.color = new Color(1,1,1,0); // Hide decal

        // Setup the UI Flow
        _containerVesselArrows.SetActive(true);
        _buttonConfirmVessel.SetActive(true);
        _containerColourPalette.SetActive(true); // Show the colour options
        _containerDecalSystem.SetActive(false); // Hide the decals

        // Ensure spinning is ON
        AutoSpinner[] spinners = FindObjectsByType<AutoSpinner>();
        foreach(var spinner in spinners)
        {
            spinner.enabled = true;
        }
    }

    // Attatch to select cup button
    public void ConfirmVesselSelection()
    {
        // Move to Step 2: Decal Selection
        _containerVesselArrows.SetActive(false);
        _buttonConfirmVessel.SetActive(false);
        _containerColourPalette.SetActive(false); // Hide the colour options
        _containerDecalSystem.SetActive(true);

        // Stop the spinning so the decal can be placed
        AutoSpinner[] spinners = FindObjectsByType<AutoSpinner>();
        foreach(var spinner in spinners)
        {
            spinner.enabled = false;
        }

        // Load the default decal tab
        LoadDecalCategory(DecalCategory.Nature);
    }

    // Attach this to the final "FINISH BREW" in the design studio
    public void FinaliseBrew()
    {
        double finalDrinkValue = _pendingBaseValue * CalculateDesignMultiplier();

        // Create the final drink with ALL the data
        CustomBrew newDrink = new CustomBrew(_pendingBrewName, finalDrinkValue, _currentVesselIndex, _selectedVesselColour, _selectedDecalIndex);
    
        // Save it
        SaveSystem.Instance.CurrentProfile.savedBrewsList.Add(newDrink);
        SaveSystem.Instance.SaveGameProgress();

        // Refresh the Tavern Menu
        BrewMenuManager menuManager = FindAnyObjectByType<BrewMenuManager>();
        if (menuManager != null)
        {
            menuManager.RefreshMenu();
        }

        Debug.Log($"DESIGN COMPLETE! Added {_pendingBrewName} to the Tavern!");

        // ---  TRANSITION LOGIC ---
        // Use the UIManager to switch to return the the Tavern Floor
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SwitchView((int)ViewState.TavernFloor);
        }
    }

    // --- COLOUR LOGIC ---
    // Call this from UI Buttons in the Design Studio
    // Pass the specific colour wanted (e.g., Red, Blue, Green) via the Inspector
    public void SetVesselColour(Color chosenColour)
    {
        _selectedVesselColour = chosenColour;

        // Find the currently active vessel model and change its colour
        GameObject activeVessel = _vesselModels[_currentVesselIndex];

        // Grab the MeshRenderer and change the material colour
        MeshRenderer renderer = activeVessel.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            // Assuming the material has a "_BaseColor" property (like URP Lit Shader)
            renderer.material.SetColor("_BaseColor", chosenColour);
        }
    }

    // --- VALUE CALCULATION LOGIC ---
    private double CalculateDesignMultiplier()
    {
        double multiplier = 1.0; // Start with a base multiplier of 1 (no change)

        // Vessel Multipliers (e.g., Index 0 is Tankard, Index 1 is Bottle)
        switch (_currentVesselIndex)
        {
            case 0: // e.g., Standard Tankard
                multiplier += 0.0; // No change for the basic tankard
                break;
            case 1: // e.g., Standard Bottle
                multiplier += 0.0; // No change for the basic bottle
                break;
            case 2: // e.g., Standard Goblet
                multiplier += 0.05;
                break;
            default:
                multiplier += 0.1;
                break;
        }

        // Decal Multipliers (e.g., check the category of the chosen decal)
        if (_selectedDecalIndex >= 0 && _selectedDecalIndex < _masterDecalDatabase.Count)
        {
            DecalCategory chosenCategory = _masterDecalDatabase[_selectedDecalIndex].category;

            switch (chosenCategory)
            {
                case DecalCategory.Nature:
                    multiplier += 0.1; // + 10%
                    break;
                case DecalCategory.Crests:
                    multiplier += 0.2; // + 20%
                    break;
                case DecalCategory.Arcane:
                    multiplier += 0.15; // + 15%
                    break;
            }
        }

        return multiplier;
    }
}
