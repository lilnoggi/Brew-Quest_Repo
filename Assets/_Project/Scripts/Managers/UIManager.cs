using System.Collections.Generic;
using UnityEngine;

// UI States - this enum will be used to track which screen the player is currently on, 
//so we can show/hide the appropriate UI elements and trigger the correct game logic.
public enum ViewState { TavernFloor, BrewingCellar, DesignStudio, QuestBoard }

public class UIManager : MonoBehaviour
{
    // --- SINGLETON SETUP ---
    private static UIManager _instance;
    public static UIManager Instance => _instance;

    [Header("UI Configuration")]
    [Tooltip("Drag the four UI Panels (Tavern, Cellar, Design, Quests) into this list in the Inspector, in the same order as the ViewState enum.")]
    [SerializeField] private List<CanvasGroup> _viewPanels = new List<CanvasGroup>();

    [Tooltip("The screen that opens when the game first launches.")]
    [SerializeField] private ViewState _initialState = ViewState.TavernFloor;

    private ViewState _currentViewState;

    private void Awake()
    {
        // Enforce singleton
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        // Set the initial view state and update the UI accordingly
        SwitchView((int)_initialState);
    }

    /// <summary>
    /// Swaps the active UI screen. Put this directly onto the Bottom Navigation UI Button's OnClick() events in inspector
    /// </summary>
    public void SwitchView(int targetViewIndex)
    {
        // SAFETY CHECK: Ensure the index is within the bounds of the panels list
        if (targetViewIndex < 0 || targetViewIndex >= _viewPanels.Count)
        {
            Debug.LogWarning($"Invalid view index: {targetViewIndex}. Please check the UIManager configuration.");
            return;
        }

        for (int i = 0; i < _viewPanels.Count; i++)
        {
            // CanvasGroup is used and toggle the alpha and interactable rather than turning the GameObject on/off, to allow for smooth fade transitions in the future if desired.
            if (i == targetViewIndex)
            {
                _viewPanels[i].alpha = 1f;
                _viewPanels[i].interactable = true;
                _viewPanels[i].blocksRaycasts = true;
            }
            else
            {
                _viewPanels[i].alpha = 0f;
                _viewPanels[i].interactable = false;
                _viewPanels[i].blocksRaycasts = false;
            }
        }

        _currentViewState = (ViewState)targetViewIndex;
        // Debug.Log($"Switched to view: {_currentViewState}");
    }
}
