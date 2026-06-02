using UnityEngine;

public enum UpgradeType { Capacity, Speed, ProfitMultiplier } // The type of upgrade, which will determine how it affects the tavern's operations.

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Brew Quest/TavernUpgrade")]
public class TavernUpgradeData : ScriptableObject
{
    [SerializeField] private string _upgradeName; // The display name of the upgrade in the UI.
    [SerializeField] private string _upgradeDescription; // A brief description of the upgrade's effects, shown in the UI.
    [SerializeField] private double _upgradeBaseCost; // The initial cost of the upgrade, which will increase with each purchase.
    [SerializeField] private UpgradeType _upgradeType; // The type of upgrade, which will determine how it affects the tavern's operations.
    [SerializeField] private float _upgradeModifierValue; // The specific value that modifies the tavern's stats (e.g., +1 capacity, -10% brew time, +20% profit).
    [SerializeField] private TavernUpgradeData _prerequisiteUpgrade; // An optional reference to another upgrade that must be purchased first.

    // --- Public Read-Only Properties ---
    public string UpgradeName => _upgradeName;
    public string UpgradeDescription => _upgradeDescription;
    public double UpgradeBaseCost => _upgradeBaseCost;
    public UpgradeType UpgradeType => _upgradeType;
    public float UpgradeModifierValue => _upgradeModifierValue;
    public TavernUpgradeData PrerequisiteUpgrade => _prerequisiteUpgrade;
}
