using UnityEngine;

public class TouchInput : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _focalPoint;

    private bool isDragging = false;

    private float zoomSpeed = 2.0f;
    private float rotationSpeed = 2.0f;
    private float swipeSpeed = 0.5f;
    private bool pinchOutEnabled = true;
    private Vector2 lastDragPosition;

    void Update()
    {
        HandleTouch();
    }

    private void HandleTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    isDragging = true;
                    // Store the initial touch position
                    lastDragPosition = touch.position;
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {

                        // Calculate the delta movement since the last frame
                        Vector2 delta = touch.position - lastDragPosition;

                        // Calculate rotation angles based on touch delta
                        float verticalInput = delta.y * rotationSpeed * Time.deltaTime;
                        float horizontalInput = delta.x * rotationSpeed * Time.deltaTime;

                        // Rotate the camera accordingly
                        _focalPoint.transform.Rotate(Vector3.right, verticalInput);
                        _focalPoint.transform.Rotate(Vector3.up, horizontalInput, Space.World);

                        // Update the last drag position for the next frame
                        lastDragPosition = touch.position;
                    }
                    break;

                case TouchPhase.Ended:
                    if (isDragging)
                    {
                        isDragging = false;
                    }
                    break;
            }
        }

        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
            {
                Vector2 touch1Delta = touch1.deltaPosition.normalized;
                Vector2 touch2Delta = touch2.deltaPosition.normalized;

                if (Vector2.Dot(touch1Delta, touch2Delta) > 0.7f)
                {
                    Vector2 swipeDelta = touch1.deltaPosition + touch2.deltaPosition;

                    float swipeX = swipeDelta.x * swipeSpeed * Time.deltaTime;
                    float swipeY = -swipeDelta.y * swipeSpeed * Time.deltaTime;

                    _focalPoint.transform.Translate(Vector3.right * swipeX, Space.World);
                    _focalPoint.transform.Translate(Vector3.up * swipeY, Space.World);

                }
                else
                {
                    Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
                    Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;
                    float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
                    float touchDeltaMag = (touch1.position - touch2.position).magnitude;
                    float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

                    if (deltaMagnitudeDiff > 0)
                    {
                        OnPinchOut(deltaMagnitudeDiff);
                    }
                    else if (deltaMagnitudeDiff < 0)
                    {
                        OnPinchIn(deltaMagnitudeDiff);
                    }
                }
            }
        }
    }


    private void OnPinchIn(float deltaMagnitudeDiff)
    {
        Debug.Log("Pinch In detected with magnitude difference: " + deltaMagnitudeDiff);

        // Move the camera closer or farther along its forward axis
        _camera.transform.Translate(Vector3.forward * deltaMagnitudeDiff * zoomSpeed * Time.deltaTime);
        pinchOutEnabled = true;
    }

    private void OnPinchOut(float deltaMagnitudeDiff)
    {
        Debug.Log("Pinch Out detected with magnitude difference: " + deltaMagnitudeDiff);

        // Check if pinch-out functionality is enabled
        if (!pinchOutEnabled)
            return;

        // Calculate the new position after translation
        Vector3 newPosition = _camera.transform.position + Vector3.forward * deltaMagnitudeDiff * zoomSpeed * Time.deltaTime;

        // Check if the new position crosses the boundary (z = 0)
        if (newPosition.z >= 0)
        {
            // Move the camera closer or farther along its forward axis
            _camera.transform.Translate(Vector3.forward * deltaMagnitudeDiff * zoomSpeed * Time.deltaTime);
        }
        else
        {
            // Set the camera position to the boundary (z = 0)
            _camera.transform.position = new Vector3(_camera.transform.position.x, _camera.transform.position.y, 0);
            // Disable pinch-out functionality
            pinchOutEnabled = false;
        }
    }
}
