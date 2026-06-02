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

    // This list will hold the names of all purchased upgrades, which can be used to determine which upgrades are active and which should be shown in the shop
    public List<string> purchasedUpgradesList;

    // Constructor sets the default values for a brand new player
    public GameSaveData()
    {
        totalGoldBalance = 0.0;
        totalPremiumCurrencyBalance = 0;
        playerLevel = 1;
        lastSavedTimestamp = DateTime.UtcNow.ToString();
        savedBrewsList = new List<CustomBrew>();
        purchasedUpgradesList = new List<string>();
    }
}
