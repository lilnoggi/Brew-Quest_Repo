using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewVendor", menuName = "Brew Quest/Vendor")]
public class VendorData : ScriptableObject
{
    [Header("Vendor Information")]
    public string vendorName;
    public Sprite vendorPortrait;
    public string vendorDescription;

    [Header("Unlock Requirements")]
    public int RequiredPlayerLevel = 1;
    public double UnlockCost = 0; // Cost to unlock the vendor, if applicable

    [Header("Inventory")]
    // List of items the vendor sells
    public List<IngredientData> ItemsForSale = new List<IngredientData>();
}
