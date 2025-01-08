using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 offset;
    private float zCoordinate;

    // Snapping variables
    [SerializeField] private Transform snapTarget; // The target position to snap the piece to
    [SerializeField] private float snapThreshold = 0.001f; // Distance threshold for snapping
    private bool isSnapped = false;

    // Reference to game start state
    public static bool gameStarted = false;

    // Rotation angle for 90-degree rotation
    private float rotationAngle = 90f;

    void Start()
    {
        // Make pieces invisible at the start (before the game starts)
        gameObject.SetActive(false);
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        // Only allow dragging if the game has started and the piece is not snapped
        if (!gameStarted || isSnapped)
            return;

        isDragging = true;

        // Get the offset for dragging
        zCoordinate = mainCamera.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPosition();
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            // Update position while dragging
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    void OnMouseUp()
    {
        // Stop dragging when mouse is released
        isDragging = false;

        // Check snapping conditions: close enough and correct rotation
        if (snapTarget != null &&
            Vector3.Distance(transform.position, snapTarget.position) < snapThreshold &&
            AreRotationsAligned())
        {
            SnapToPosition();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Convert mouse position from screen space to world space
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoordinate;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    private void SnapToPosition()
    {
        // Snap the piece to the target position and mark it as snapped
        transform.position = snapTarget.position;
        transform.rotation = snapTarget.rotation; // Align rotation with the snap target
        isSnapped = true;
    }

    private bool AreRotationsAligned()
    {
        // Check if the piece's rotation is aligned with the target rotation
        return Quaternion.Angle(transform.rotation, snapTarget.rotation) < 1f; // Adjust threshold if needed
    }

    void Update()
    {
        // Perform raycast on right-click for rotation
        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            RotateObjectWithRaycast();
        }
    }

    public bool IsSnapped()
    {
        return isSnapped;
    }

    void RotateObjectWithRaycast()
    {
        if (isSnapped) return; // Do not rotate snapped pieces

        // Raycast to check which object is being clicked
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Only rotate the object that is clicked
            if (hit.transform == transform)
            {
                // Rotate the clicked object 90 degrees around the Y-axis
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, rotationAngle, 0));
            }
        }
    }
}
