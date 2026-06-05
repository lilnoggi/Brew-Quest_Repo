using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SupplierManager : MonoBehaviour
{
    [Header("Vendor UI Components")]
    [SerializeField] private Image _vendorPortrait;
    [SerializeField] private TextMeshProUGUI _vendorNameText;
    [SerializeField] private TextMeshProUGUI _vendorGreetingText;
    [SerializeField] private Transform _inventoryContentContainer;
    [SerializeField] private GameObject _itemSlotPrefab;

    private VendorData _currentVendor;

    // Called when the player clicks a Vendor Name from the Supplier Selection UI
    public void OpenVendorShop(VendorData vendor)
    {
        _currentVendor = vendor;

        // Update the UI with the vendor's info
        _vendorPortrait.sprite = vendor.VendorPortrait;
        _vendorNameText.text = vendor.VendorName;
        _vendorGreetingText.text = vendor.WelcomeDialogue;

        // Clear old items out of the grid
        foreach (Transform child in _inventoryContentContainer)
        {
            Destroy(child.gameObject);
        }

        // Spawn the specific items this vendor sells
        foreach (IngredientData item in vendor.ItemsForSale){
            GameObject newSlot = Instantiate(_itemSlotPrefab, _inventoryContentContainer);
            ItemSlotUI slotUI = newSlot.GetComponent<ItemSlotUI>();

            // Pass the item data and "this" manager so the button knows who to talk to
            if (slotUI != null)
            {
                slotUI.SetupForShop(item, this);
            }
        }
    }

    // Called by the ItemSlotUI when the player taps an item
    public void AttemptPurchase(IngredientData itemToBuy)
    {
        // Check if the player has enough gold
        if (SaveSystem.Instance.CurrentProfile.totalGoldBalance >= itemToBuy.PurchaseCost)
        {
            // Deduct gold and add item to inventory
            SaveSystem.Instance.CurrentProfile.totalGoldBalance -= itemToBuy.PurchaseCost;

            // Add the ingredient name to the player's save file inventory
            SaveSystem.Instance.CurrentProfile.ownedIngredients.Add(itemToBuy.IngredientName);

            // Save the game
            SaveSystem.Instance.SaveGameProgress();

            // NOTE: remove the item from the vendor's list if it's a one-time purchase
            // _currentVendor.ItemsForSale.Remove(itemToBuy);
        }
        else
        {
            Debug.Log("Not enough gold to purchase " + itemToBuy.IngredientName);
        }
    }
}
