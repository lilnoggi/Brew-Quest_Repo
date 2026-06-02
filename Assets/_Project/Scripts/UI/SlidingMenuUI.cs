using System.Collections;
using UnityEngine;

public class SlidingMenuUI : MonoBehaviour
{
    [Header("Menu References")]
    [SerializeField] private RectTransform _menuPanel; // The panel that will slide in and out

    [Header("Slide Settings")]
    [SerializeField] private float _hiddenPosY; // The Y position when the menu is hidden (off-screen)
    [SerializeField] private float _visiblePosY; // The Y position when the menu is visible (on-screen)
    [SerializeField] private float _slideDuration = 0.5f; // Duration of the slide animation

    private bool _isMenuVisible = false; // Tracks whether the menu is currently visible or hidden
    private Coroutine _slideCoroutine; // Reference to the slide coroutine so we can stop it if needed

    private void Start()
    {
        // Initialise the menu's position to be hidden at the start of the game
        Vector2 startPos = _menuPanel.anchoredPosition;
        startPos.y = _hiddenPosY;
        _menuPanel.anchoredPosition = startPos;
    }

    public void ToggleMenu()
    {
        _isMenuVisible = !_isMenuVisible; // Toggle the visibility state

        // Stop any current slide animation before starting a new one to prevent conflicts
        if (_slideCoroutine != null)
        {
            StopCoroutine(_slideCoroutine);
        }

        //Start sliding to the new target position
        float targetPosY = _isMenuVisible ? _visiblePosY : _hiddenPosY;
        _slideCoroutine = StartCoroutine(SlideTo(targetPosY));
    }

    private IEnumerator SlideTo(float targetPosY)
    {
        float elapsedTime = 0f;
        Vector2 startPos = _menuPanel.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, targetPosY);

        while (elapsedTime < _slideDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / _slideDuration);
            _menuPanel.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null; // Wait for the next frame before continuing the loop
        }

        // Ensure the menu ends up exactly at the target position after the animation completes
        _menuPanel.anchoredPosition = targetPos;
    }
}
