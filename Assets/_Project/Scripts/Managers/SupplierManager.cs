using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SupplierManager : MonoBehaviour
{
    [Header("VendorList UI Components (Left Page)")]
    [SerializeField] private Transform _vendorListContentContainer;
    [SerializeField] private GameObject _vendorCardPrefab;

    [Header("Vendor UI Components (Right Page)")]
    [SerializeField] private Image _vendorPortrait;
    [SerializeField] private TextMeshProUGUI _vendorNameText;
    [SerializeField] private TextMeshProUGUI _vendorGreetingText;
    [SerializeField] private Transform _inventoryContentContainer;
    [SerializeField] private GameObject _itemSlotPrefab;

    // --- INVENTORY GRID SIZE VARIABLE ---
    [SerializeField] private int _totalGridSize = 10; // The total number of slots to display (inventory size)

    [Header("UI Animations")]
    [SerializeField] private SlidingMenuUI _shopSlidingPanel;

    [Header("Vendor Master Database")]
    [SerializeField] private List<VendorData> _allVendorsDatabase = new List<VendorData>();

    private VendorData _currentVendor;

    private void Start()
    {
        PopulateVendorList();
    }

    private void PopulateVendorList()
    {
        // Clear any placeholder items
        foreach (Transform child in _vendorListContentContainer)
        {
            Destroy(child.gameObject);
        }

        // Spawn a card for each vendor in the database
        foreach (VendorData vendor in _allVendorsDatabase)
        {
            // FUTURE: Check if the player level is high enough to unlock this vendor before spawning their card
            if (SaveSystem.Instance.CurrentProfile.playerLevel >= vendor.RequiredPlayerLevel)
            {
                GameObject newCard = Instantiate(_vendorCardPrefab, _vendorListContentContainer);
                SupplierSelectCardUI cardUI = newCard.GetComponent<SupplierSelectCardUI>();

                if (cardUI != null)
                {
                    cardUI.SetupVendorCard(vendor, this);
                }
                else
                {
                    Debug.LogWarning("SupplierManager: The vendor card prefab is missing the SupplierSelectCardUI component.");
                }
            }
        }
    }

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
        for (int i = 0; i < _totalGridSize; i++)
        {
            GameObject newSlot = Instantiate(_itemSlotPrefab, _inventoryContentContainer);
            ItemSlotUI slotUI = newSlot.GetComponent<ItemSlotUI>();

            // Pass the item data and "this" manager so the button knows who to talk to
            if (slotUI != null)
            {
                // If vendor has not run out of items for sale yet, set it up as a shop item
                if (i < vendor.ItemsForSale.Count)
                {
                    slotUI.SetupForShop(vendor.ItemsForSale[i], this);
                }
                else
                {
                    // Otherwise, set it up as an empty inventory slot
                    slotUI.SetupEmptySlot();
                }
            }
        }

        // --- TRIGGER THE SLIDE ANIMATION ---
        if (_shopSlidingPanel != null)
        {
            // Use OpenMenu() instead so it doesn't close of the same card is tapped.
            _shopSlidingPanel.OpenMenu();
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
