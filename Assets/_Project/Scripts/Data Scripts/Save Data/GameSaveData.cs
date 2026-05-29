using System;
using System.Collections.Generic;

// Serializable so JsonUtility can convert it to text
[System.Serializable]
public class GameSaveData
{
    public double totalGoldBalance;
    public int totalPremiumCurrencyBalance;
    public int playerLevel;
    public string lastSavedTimestamp;

    // This list will hold every CustomBrew the player ever creates
    public List<CustomBrew> savedBrewsList;

    // Constructor sets the default values for a brand new player
    public GameSaveData()
    {
        totalGoldBalance = 0.0;
        totalPremiumCurrencyBalance = 0;
        playerLevel = 1;
        lastSavedTimestamp = DateTime.UtcNow.ToString();
        savedBrewsList = new List<CustomBrew>();
    }
}
