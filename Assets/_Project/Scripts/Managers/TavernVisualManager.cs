using System.Collections.Generic;
using UnityEngine;

public class TavernVisualManager : MonoBehaviour
{
    // This struct creates a clean pairing in the Inspector
    [System.Serializable]
    public struct UpgradeVisualLink
    {
        public string UpgradeName; // MUST match the UpgradeName in TavernUpgradeData exactly to work
        public GameObject VisualGroup; // The GameObject to "Turn On"
    }

    [Header("Visual Upgrades")]
    [SerializeField] private List<UpgradeVisualLink> _visualLinks = new List<UpgradeVisualLink>(); // Link the name of a shop upgrade to the 3D objects it should turn on

    private void Start()
    {
        // When the scene loads, check the save file and turn on everything the player has unlocked so far
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        // SAFETY CHECK: Ensure the SaveSystem is loaded and ready before trying to update visuals
        if (SaveSystem.Instance == null || SaveSystem.Instance.CurrentProfile == null)
        {
            Debug.LogWarning("SaveSystem not ready. Cannot update tavern visuals.");
            return;
        }

        // Grab the list of string names bought
        List<string> purchasedUpgrades = SaveSystem.Instance.CurrentProfile.purchasedUpgradesList;

        // Loop through all the visual links and check if the player has unlocked each one, turning on the corresponding GameObjects if they have
        foreach (var link in _visualLinks)
        {
            if (link.VisualGroup != null)
            {
                bool isPurchased = purchasedUpgrades.Contains(link.UpgradeName);
                link.VisualGroup.SetActive(isPurchased);
            }
        }
    }
}
