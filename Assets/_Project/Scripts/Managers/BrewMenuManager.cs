using UnityEngine;

public class BrewMenuManager : MonoBehaviour
{
    [Header("Menu Setup")]
    [Tooltip("Drag the Brew Card prefab into this slot in the Inspector.")]
    [SerializeField] private GameObject _brewCardPrefab;

    [Tooltip("Drag the 'Content' object from the Scroll View into this slot in the Inspector.")]
    [SerializeField] private Transform _contentContainer;

    private void Start()
    {
        // Populate tge menu right when the game starts
        RefreshMenu();
    }

    public void RefreshMenu()
    {
        // SAFETY CHECK: Ensure the SaveSystem is loaded and ready before trying to populate the menu
        if (SaveSystem.Instance == null || SaveSystem.Instance.CurrentProfile == null)
        {
            Debug.LogWarning("SaveSystem not ready. Cannot refresh brew menu.");
            return;
        }

        // Clear out any existing brew cards in the menu before repopulating it to prevent duplicates
        foreach (Transform child in _contentContainer)
        {
            Destroy(child.gameObject);
        }

        // Loop through the player's saved brews and create a new card for each one
        foreach (var brew in SaveSystem.Instance.CurrentProfile.savedBrewsList)
        {
            GameObject newCard = Instantiate(_brewCardPrefab, _contentContainer);
            BrewCardUI cardUI = newCard.GetComponent<BrewCardUI>();

            // SAFETY CHECK: Ensure the prefab has a BrewCardUI component before trying to set it up
            if (cardUI != null)
            {
                cardUI.InitialiseCard(brew);
            }
            else
            {
                Debug.LogWarning("The assigned brew card prefab does not have a BrewCardUI component.");
            }
        }
    }
}
