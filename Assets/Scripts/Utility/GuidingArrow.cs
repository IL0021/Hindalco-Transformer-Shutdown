using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class GuidingArrow : MonoBehaviour
{
    public static GuidingArrow Instance { get; private set; }

    [Header("Targeting")]
    [SerializeField] private Transform target;
    [SerializeField] private Camera playerCamera;

    [Header("UI Settings")]
    [SerializeField] private RectTransform arrowIcon;
    [SerializeField] private float rotationOffset = -90f; // Adjust based on your arrow sprite's default orientation

    [Header("Visibility")]
    [SerializeField] private float fadeSpeed = 5f;
    [Range(0.01f, 0.5f)][SerializeField] private float screenMargin = 0.05f;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        // Singleton Initialization
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _canvasGroup = GetComponent<CanvasGroup>();
        if (playerCamera == null) playerCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (target == null || playerCamera == null)
        {
            _canvasGroup.alpha = 0;
            return;
        }

        // Get viewport position (x, y are 0-1, z is distance from camera)
        Vector3 viewportPoint = playerCamera.WorldToViewportPoint(target.position);

        // Target is off-screen if it's behind the camera (z < 0) 
        // or outside the specified viewport margins
        bool isOffScreen = viewportPoint.z < 0 ||
                           viewportPoint.x < screenMargin || viewportPoint.x > (1f - screenMargin) ||
                           viewportPoint.y < screenMargin || viewportPoint.y > (1f - screenMargin);

        // 1. Handle Visibility (Alpha)
        float targetAlpha = isOffScreen ? 1f : 0f;
        _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);

        // 2. Handle Rotation (Z-axis only)
        // We calculate rotation even while fading to ensure it's pointing the right way when it appears
        if (_canvasGroup.alpha > 0.01f)
        {
            UpdateArrowRotation(viewportPoint);
        }
    }

    /// <summary>
    /// Clears the current target for the guiding arrow.
    /// </summary>
    public void ClearTarget()
    {
        target = null;
    }

    /// <summary>
    /// Sets a new target for the guiding arrow globally.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void UpdateArrowRotation(Vector3 viewportPoint)
    {
        // Viewport center is (0.5, 0.5). 
        // We find the direction from the center of the screen to the target's viewport position.
        Vector2 screenCenter = new Vector2(0.5f, 0.5f);
        Vector2 targetDir = new Vector2(viewportPoint.x, viewportPoint.y) - screenCenter;

        // If the object is behind the player, viewport coordinates are inverted.
        // Negating the direction ensures the arrow points toward the edge the player needs to turn to.
        if (viewportPoint.z < 0)
        {
            targetDir = -targetDir;
        }

        // Calculate the 2D angle for the Z-axis
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

        // Rotate only on the Z-axis. This keeps the icon flat on its 2D plane (the Canvas).
        // Using localRotation ensures we respect the Canvas's orientation in front of the player.
        arrowIcon.localRotation = Quaternion.Euler(0, 0, angle + rotationOffset);
    }
}