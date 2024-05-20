using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;
using TMPro;

public class QrCodeRecenter : MonoBehaviour
{
    [SerializeField]
    private ARSession session;
    [SerializeField]
    private XROrigin sessionOrigin;
    [SerializeField]
    private ARCameraManager cameraManager;
    [SerializeField]
    private List<Target> navigationTargetObjects = new List<Target>();

    [SerializeField]
    private Button Teleport;

    //[SerializeField]
    //private TextMeshProUGUI Distance;

    private Texture2D cameraImageTexture;
    private IBarcodeReader reader = new BarcodeReader();

    public bool isActivated = false;

    //public bool IsActivated { get; internal set; }

    private void OnEnable()
    {
        cameraManager.frameReceived += OnCameraFrameReceived;
    }

    private void OnDisable()
    {
        cameraManager.frameReceived -= OnCameraFrameReceived;
    }

    private void Update()
    {
        Teleport.onClick.AddListener(setQR);
    }

    private void setQR()
    {
        SetQRCodeRecenterTarget("1005", 1);
    }

    private void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            return;
        }


        var conversionParams = new XRCpuImage.ConversionParams
        {
            // Get entire Image.
            inputRect = new RectInt(0, 0, image.width, image.height),

            // Downsample by 2.
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),

            // Choose RGBA Format.
            outputFormat = TextureFormat.RGBA32,

            // Flip across
            transformation = XRCpuImage.Transformation.MirrorY
        };

        // See how many bytes you need to store the final image.
        int size = image.GetConvertedDataSize(conversionParams);

        // Allocate a buffer to store the image.
        var buffer = new NativeArray<byte>(size, Allocator.Temp);

        // Extract the image data.
        image.Convert(conversionParams, buffer);

        // The image was converted to RGBA32 format and written into the provided buffer
        // so you can dispose of the XRCpuImage. You must do this or it will leak resources.
        image.Dispose();

        // Calculate camera resolution
        int cameraWidth = conversionParams.outputDimensions.x;
        int cameraHeight = conversionParams.outputDimensions.y;

        // At this point, you can process the image, pass it to a computer vision algorithm, etc.
        // In this example, you apply it to a texture to visualize it.

        // You've got the data; let's put it into a texture so you can visualize it.
        cameraImageTexture = new Texture2D(
            conversionParams.outputDimensions.x,
            conversionParams.outputDimensions.y,
            conversionParams.outputFormat,
            false);

        cameraImageTexture.LoadRawTextureData(buffer);
        cameraImageTexture.Apply();

        // Done with your temporary data, so you can dispose it.
        buffer.Dispose();

        // Detect and decode the barcode inside the bitmap.
        var result = reader.Decode(cameraImageTexture.GetPixels32(), cameraImageTexture.width, cameraImageTexture.height);

        // Do something with the result.
        if (result != null)
        {
            // Calculate distance based on physical dimensions of QR code and camera FOV
            float qrCodeWidthInMeters = 0.125f; // 12.5 cm converted to meters
            float cameraHorizontalFOV = 78.734f; // Horizontal FOV in degrees
            float cameraVerticalFOV = 120f; // Vertical FOV in degrees

            // Estimate QR code size in pixels
            float qrCodeWidthInPixels = result.ResultPoints[1].X - result.ResultPoints[0].X;
            float qrCodeHeightInPixels = result.ResultPoints[2].Y - result.ResultPoints[1].Y;

            // Calculate horizontal and vertical angles in radians
            float angleWidth = Mathf.Deg2Rad * cameraHorizontalFOV * (qrCodeWidthInPixels / cameraWidth);
            float angleHeight = Mathf.Deg2Rad * cameraVerticalFOV * (qrCodeHeightInPixels / cameraHeight);

            // Calculate distance
            float distanceHorizontal = (qrCodeWidthInMeters / 2) / Mathf.Tan(angleWidth / 2);
            float distanceVertical = (qrCodeWidthInMeters / 2) / Mathf.Tan(angleHeight / 2);

            // You can choose to use either horizontal or vertical distance, or combine them depending on your requirement
            // For simplicity, let's just take the average
            float distance = (distanceHorizontal + distanceVertical) / 2;

            Debug.Log("Distance to QR code: " + distance + " meters");
            //Distance.text = distance + " meters";

            if (distance != 0.0)
            {
                SetQRCodeRecenterTarget(result.Text, distance);
            }
            
        }
    }

    private void SetQRCodeRecenterTarget(string v, float distance)
    {
        Target currentTarget = navigationTargetObjects.Find(x => x.Name.ToLower().Equals(v.ToLower()));
        if (currentTarget != null)
        {
            // Reset position and rotation of ARSession
            session.Reset();

            // Calculate offset direction based on the sessionOrigin's forward direction
            Vector3 offsetDirection = -sessionOrigin.transform.forward; // Assuming you want to move in the opposite direction of forward

            // Add offset for recentering
            float offsetDistance = distance; // Adjust this value as needed
            Vector3 newPosition = currentTarget.PositionObject.transform.position + offsetDirection * offsetDistance;
            sessionOrigin.transform.position = newPosition;
            sessionOrigin.transform.rotation = currentTarget.PositionObject.transform.rotation;
            isActivated = true;
        }
    }

}
