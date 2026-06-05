using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SupplierSelectCardUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image _vendorPortrait;
    [SerializeField] private TextMeshProUGUI _vendorNameText;

    private VendorData _assignedVendor;
    private SupplierManager _supplierManager;

    // Called by the SupplierManager when this card is spawned into the Supplier Ledger
    public void SetupVendorCard(VendorData vendor, SupplierManager supplierManager)
    {
        _assignedVendor = vendor;
        _supplierManager = supplierManager;

        // Populate visuals
        _vendorPortrait.sprite = vendor.VendorPortrait;
        _vendorNameText.text = vendor.VendorName;
    }

    // Attatch this to the Button component on the prefab
    public void OnVendorCardClicked()
    {
        if (_assignedVendor != null && _supplierManager != null)
        {
            _supplierManager.OpenVendorShop(_assignedVendor);
        }
        else
        {
            Debug.LogWarning("SupplierSelectCardUI: Vendor or SupplierManager reference is missing.");
        }
    }
}
