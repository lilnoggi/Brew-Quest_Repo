using System.Collections.Generic;
using UnityEngine;

public class PatronSimulationManager : MonoBehaviour
{
    // --- SINGLETON SETUP ---
    private static PatronSimulationManager _instance;
    public static PatronSimulationManager Instance => _instance;

    [Header("Simulation Stats")]
    [Tooltip("The base number of patrons the tavern can hold before any upgrades.")]
    [SerializeField] private int _basePatronCapacity = 2;

    private int _currentMaxPatronCapacity;
    private int _activePatrons;

    public int CurrentMaxPatronCapacity => _currentMaxPatronCapacity;

    private void Awake()
    {
        // Singleton pattern enforcement
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        RecalculatePatronCapacity();
    }

    // Reads the player's purchased upgrades and calcualtes the current max capacity
    public void RecalculatePatronCapacity()
    {
        _currentMaxPatronCapacity = _basePatronCapacity;

        if (SaveSystem.Instance != null && SaveSystem.Instance.CurrentProfile != null)
        {
            List<string> purchasedUpgrades = SaveSystem.Instance.CurrentProfile.purchasedUpgradesList;

            // NOTE: Later, iterate thrrough all the TavernUpgradeData assets,
            // check if they are purchased, and add their ModifierValue.
            // For the MVP, hardcode the checks for simplicity.

            // Example upgrade checks 
            if (purchasedUpgrades.Contains("Basic Table"))
            {
                _currentMaxPatronCapacity += 2; // Increase capacity by 2 for this upgrade
            }
            if (purchasedUpgrades.Contains("Wooden Chair"))
            {
                _currentMaxPatronCapacity += 1; // Increase capacity by 1 for this upgrade
            }
        }

        Debug.Log($"Tavern Capacity Recalculated: {_currentMaxPatronCapacity} patrons.");

        // NOTE: Later, this will trigger the spawning of 3D patron models in the tavern. For now, just log the capacity.
    }

    // Use this for when the 3D models are spawned/despawned
    public bool CanSpawnPatron()
    {
        return _activePatrons < _currentMaxPatronCapacity;
    }

    public void AddActivePatron()
    {
        _activePatrons++;
    }

    public void RemoveActivePatron()
    {
        _activePatrons--;
    }
}
