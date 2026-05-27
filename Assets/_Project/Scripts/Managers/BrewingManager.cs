using UnityEngine;

public class BrewingManager : MonoBehaviour
{
    [Header("Test Ingredients")]
    [Tooltip("Drag two ingredients from ScriptableObjects folder into here")]
    [SerializeField] private IngredientData _slot1;
    [SerializeField] private IngredientData _slot2;

    [Header("Test UI Hook")]
    [Tooltip("Type a test name for the drink before the mix button is pressed.")]
    [SerializeField] private string _testDrinkName = "My First Brew";

    /// <summary>
    /// This will be hooked up to a temporary "MIX" button
    /// </summary>
    public void MixTestDrink()
    {
        // SAFETY CHECK: Ensure both ingredient slots are filled before allowing the mix
        if (_slot1 == null || _slot2 == null)
        {
            Debug.LogWarning("Please assign ingredients to both slots before mixing.");
            return;
        }

        // Run the maths engine to calculate the synergy and value
        double calculatedBaseValue = IdleMathsBridge.CalculateBaseDrinkValue(_slot1, _slot2);

        // Create the brand new drink (using 0 for the visual design indexes for now)
        CustomBrew newDrink = new CustomBrew(_testDrinkName, calculatedBaseValue, 0, 0);

        // Add it to the active save profile
        SaveSystem.Instance.CurrentProfile.savedBrewsList.Add(newDrink);

        // Force a hard drive save so it doesn't get lost
        SaveSystem.Instance.SaveGameProgress();

        // Print a success message to the console
        Debug.Log($"SUCCESS! Brewed '{newDrink.CustomName}' with a base value of {newDrink.BaseDrinkValue} gold.");
    }
}
