using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopCardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _upgradeNameText;
    [SerializeField] private TextMeshProUGUI _upgradeDescriptionText;
    [SerializeField] private TextMeshProUGUI _upgradeCostText;
    [SerializeField] private Button _purchaseButton;

    private TavernUpgradeData _upgradeData;
    private ShopManager _shopManager;

    // Called by ShopManager when instantiating the card prefab
    public void Setup(TavernUpgradeData upgradeData, ShopManager shopManager)
    {
        _upgradeData = upgradeData;
        _shopManager = shopManager;

        // Update UI elements with data from the upgrade
        _upgradeNameText.text = _upgradeData.UpgradeName;
        _upgradeDescriptionText.text = _upgradeData.UpgradeDescription;
        
        // Format cost
        _upgradeCostText.text = _upgradeData.UpgradeBaseCost.ToString("F2") + " Gold";

        // Add listener to the purchase button
        _purchaseButton.onClick.RemoveAllListeners(); // Clear any existing listeners to prevent multiple calls
        _purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
    }

    private void OnPurchaseButtonClicked()
    {
        if (_shopManager != null && _upgradeData != null)
        {
            // Attempt to purchase the upgrade through the ShopManager, which will handle validation and applying effects
            _shopManager.AttemptPurchase(_upgradeData);
        }
    }
}
