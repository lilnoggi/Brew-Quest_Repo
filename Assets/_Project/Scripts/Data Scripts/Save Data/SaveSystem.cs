using System;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    // --- SINGLETON SETUP ---
    private static SaveSystem _instance;
    public static SaveSystem Instance => _instance;

    [Header("Live Data")]
    [Tooltip("The current active save profile being played.")]
    [SerializeField] private GameSaveData _currentProfileString;

    private string _saveFilePath;

    public GameSaveData CurrentProfile => _currentProfileString;

    private void Awake()
    {
        // Enforce singleton
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes

        // Set the hidden file path on the user's mobile device or PC
        _saveFilePath = Path.Combine(Application.persistentDataPath, "brewquest_save.json");

        // Automatically load the game (and calculate offline earnings) the moment the app opens
        LoadGameProgress();
    }

    public void SaveGameProgress()
    {
        _currentProfileString.lastSavedTimestamp = DateTime.UtcNow.ToString(); // Update the timestamp before saving
        string jsonData = JsonUtility.ToJson(_currentProfileString, prettyPrint: true);
        File.WriteAllText(_saveFilePath, jsonData);
        Debug.Log($"Game saved to {_saveFilePath}");
    }

    public void LoadGameProgress()
    {
        if (!File.Exists(_saveFilePath))
        {
            Debug.Log("No save file found. Starting a new game.");
            _currentProfileString = new GameSaveData(); // Create a new profile with default values
            return;
        }

        // If a file exists, read it and convert it back into C# data
        string jsonData = File.ReadAllText(_saveFilePath);
        _currentProfileString = JsonUtility.FromJson<GameSaveData>(jsonData);

        CalculateOfflineEarnings();
    }

    private void CalculateOfflineEarnings()
    {
        if (DateTime.TryParse(_currentProfileString.lastSavedTimestamp, out DateTime lastSavedTime))
        {
            TimeSpan timeAway = DateTime.UtcNow - lastSavedTime;
            double secondsAway = timeAway.TotalSeconds;
            double totalOfflineEarnings = 0.0;

            // Calculate passive income for every drink currently on tap
            foreach (var brew in _currentProfileString.savedBrewsList)
            {
                if (brew.IsUnlockedOnTap)
                {
                    double revenuePerSecond = IdleMathsBridge.CalculateDrinkRevenuePerSecond(brew.BaseDrinkValue, brew.CurrentLevel);
                    totalOfflineEarnings += revenuePerSecond * secondsAway;
                }
            }

            // Cap offline progression time widnow (e.g., max 4 hours baseline)
            double fourHoursInSeconds = 4 * 60 * 60;
            if (secondsAway > fourHoursInSeconds)
            {
                totalOfflineEarnings = (totalOfflineEarnings / secondsAway) * fourHoursInSeconds; // Scale down earnings if away for too long
                Debug.Log($"You were away for over 4 hours. Offline earnings have been scaled down to {totalOfflineEarnings:F2} gold.");
            }

            _currentProfileString.totalGoldBalance += totalOfflineEarnings; // Add the offline earnings to the player's gold balance
            Debug.Log($"Welcome back! You were away for {timeAway.TotalMinutes:F1} minutes and earned {totalOfflineEarnings:F2} gold while you were gone.");
        }
        else
        {
            Debug.LogWarning("Failed to parse last saved timestamp. No offline earnings calculated.");
        }
    }

    public void AddIngredientToInventory(string ingredientName, int amount)
    {
        // Search to see if the ingredient already exists in the inventory
        foreach (var item in _currentProfileString.ownedIngredientsList)
        {
            if (item.ingredientName == ingredientName)
            {
                // If it does, just increase the quantity
                item.quantity += amount;
                SaveGameProgress();
                return;
            }
        }

        // If it doesn't exist, add a new entry to the inventory list
        _currentProfileString.ownedIngredientsList.Add(new IngredientInventoryItem(ingredientName, amount));
        SaveGameProgress();
    }
}
