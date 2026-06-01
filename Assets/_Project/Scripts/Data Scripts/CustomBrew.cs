using System;
using UnityEngine;

[System.Serializable]
public class CustomBrew
{
    [Header("Identity")]
    [Tooltip("A unique string ID to track this specific brew in the save file.")]
    [SerializeField] private string _brewID;

    [Tooltip("The custom name the player typed in for their creation.")]
    [SerializeField] private string _customName;

    [Header("Progression")]
    [Tooltip("The current upgrade level of the drink (starts at 1).")]
    [SerializeField] private int _currentLevel;

    [Tooltip("The permanent baseline value calculated from the ingredients used.")]
    [SerializeField] private double _baseDrinkValue;

    [Header("Visual Design")]
    [Tooltip("An integer representing which 3D bottle shape the player chose.")]
    [SerializeField] private int _bottleShapeIndex;

    [Tooltip("The colour applied to the vessel material.")]
    [SerializeField] private Color _vesselColour;

    [Tooltip("An integer representing which label decal the player chose.")]
    [SerializeField] private int _decalIndex;

    [Header("State")]
    [Tooltip("Tracks whether this drink is currently active on the tavern menu to earn money.")]
    [SerializeField] private bool _isUnlockedOnTap;

    // ===================================================================================================

    // --- Public Read-Only Properties ---
    public string BrewID => _brewID;
    public string CustomName => _customName;
    public int CurrentLevel { get => _currentLevel; set => _currentLevel = value; }
    public double BaseDrinkValue => _baseDrinkValue;
    public int BottleShapeIndex => _bottleShapeIndex;
    public Color VesselColour => _vesselColour;
    public int DecalIndex => _decalIndex;
    public bool IsUnlockedOnTap { get => _isUnlockedOnTap; set => _isUnlockedOnTap = value; }

    // --- Constructor ---
    // This is called exactly when the player hits "Finish" in the design studio
    public CustomBrew(string name, double calculatedBase, int bottleShape, Color vesselColour, int decal)
    {
        _brewID = Guid.NewGuid().ToString(); // Automatically generate a unique ID for this brew
        _customName = name;
        _currentLevel = 1; // Always starts at level 1
        _baseDrinkValue = calculatedBase;
        _bottleShapeIndex = bottleShape;
        _vesselColour = vesselColour;
        _decalIndex = decal;
        _isUnlockedOnTap = true; // Automatically goes on tap when created
    }
}
