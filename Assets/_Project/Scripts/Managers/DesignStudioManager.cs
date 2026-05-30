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

    private int _selectedDecalIndex = 0; // Tracks the player's final choice for the CustomBrew save data

    // This tracks exactly which shape the player is looking at.
    // It will directly map to CustomBrew._bottleShapeIndex
    private int _currentVesselIndex = 0;

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

    // =====================================================================
    // --- PIPELINE LOGIC ---
    // =====================================================================

    // The cellar manager will call this when the player confirms the name
    public void StartDesignProcess(string brewName, double baseValue)
    {
        _pendingBrewName = brewName;
        _pendingBaseValue = baseValue;

        // Reset the UI to defaults
        _currentVesselIndex = 0;
        UpdateVesselVisibility();

        _selectedDecalIndex = 0;
        _appliedDecalImage.sprite = null;
        _appliedDecalImage.color = new Color(1,1,1,0); // Hide decal

        // Load default tab
        LoadDecalCategory(DecalCategory.Nature);
    }

    // Attach this to the final "FINISH BREW" in the design studio
    public void FinaliseBrew()
    {
        // Create the final drink with ALL the data
        CustomBrew newDrink = new CustomBrew(_pendingBrewName, _pendingBaseValue, _currentVesselIndex, _selectedDecalIndex);
    
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
    }
}
