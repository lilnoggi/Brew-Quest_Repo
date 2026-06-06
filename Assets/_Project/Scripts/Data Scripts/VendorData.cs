using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VendorStockProbability
{
    // The ingredient that might appear
    public IngredientData Ingredient;

    // The percentage chance (0-100) this item is in stock today
    [Range(0, 100)] public float ChanceToAppear = 100f;

    // The minimum price it will cost if appears.
    public double MinPrice;

    // The maximum price it will cost if appears.
    public double MaxPrice;
}

[CreateAssetMenu(fileName = "NewVendor", menuName = "Brew Quest/Vendor")]
public class VendorData : ScriptableObject
{
    [Header("Vendor Information")]
    public string VendorName;
    public Sprite VendorPortrait;
    public string VendorDescription;
    [TextArea(3, 10)] public string WelcomeDialogue;

    [Header("Unlock Requirements")]
    public int RequiredPlayerLevel = 1;
    public double UnlockCost = 0; // Cost to unlock the vendor, if applicable

    [Header("Settings")]
    // The maximum number of unique items this vendor can display at one time.
    public int MaxStockDisplay = 5;

    [Header("Potential Inventory")]
    // The pool of items this vendor can potentially roll for their daily stock
    public List<VendorStockProbability> PotentialStockPool = new List<VendorStockProbability>();
}
