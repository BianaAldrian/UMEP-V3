using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class CameraOverlayController : MonoBehaviour
{
    [SerializeField]
    private ARCameraManager ARCameraManager;

    [SerializeField]
    private Camera secondCamera;

    [SerializeField]
    private Button btn;

    private bool isSecondCameraActive = false;

    void Start()
    {
        // Add an onClick listener to the button
        btn.onClick.AddListener(ToggleCameras);

        // Set the clear flags of the second camera to Depth Only to make it transparent
        secondCamera.clearFlags = CameraClearFlags.Depth;
    }

    public void ToggleCameras()
    {
        // Toggle the active state of the secondCamera
        isSecondCameraActive = !isSecondCameraActive;

        // Enable/disable the secondCamera based on the toggle state
        secondCamera.gameObject.SetActive(isSecondCameraActive);

        // Set the depth of the second camera to render on top of other cameras
        if (isSecondCameraActive)
        {
            secondCamera.depth = ARCameraManager.GetComponent<Camera>().depth + 1;
        }
        else
        {
            secondCamera.depth = -1; // Set it to a value lower than the ARCamera to render behind it
        }
    }
}
