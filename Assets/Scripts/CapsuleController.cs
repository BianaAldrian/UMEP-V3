using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Net;
using System.Net.Sockets;

public class CapsuleController : MonoBehaviour
{
    private string IP;
    private string id_number;

    [SerializeField]
    private Camera xrCamera;

    void Start()
    {
        IP = PlayerPrefs.GetString("IP");
        id_number = GetLocalIPAddress();
    }

    public string GetLocalIPAddress()
    {
        string localIP = string.Empty;
        try
        {
            foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error getting local IP address: " + ex.Message);
        }

        return localIP;
    }

    void Update()
    {
       /* if (xrCamera != null)
        {
            // Get the camera's rotation
            Quaternion cameraRotation = xrCamera.transform.rotation;

            // Keep only the Y rotation
            Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

            // Set the capsule's rotation to match only the Y rotation of the camera's rotation
            transform.rotation = yRotation;
        }*/

        // Get the position of the capsule
        Vector3 position = transform.position;
        // Get the rotation of the capsule
        Quaternion rotation = transform.rotation;   

        // You can access individual components of position and rotation if needed
        float xPos = position.x;
        float yPos = position.y;
        float zPos = position.z;

        // You can also get Euler angles from the rotation if you need them
        Vector3 eulerAngles = rotation.eulerAngles;
        float xRot = eulerAngles.x;
        float yRot = eulerAngles.y;
        float zRot = eulerAngles.z;

        // Now you can use these values as needed
        /*Debug.Log("Position: (" + xPos + ", " + yPos + ", " + zPos + ")");
        Debug.Log("Rotation: (" + xRot + ", " + yRot + ", " + zRot + ")");*/

        // Call SetLocation to update the data
        StartCoroutine(SetLocation(id_number, xPos, yPos, zPos, xRot, yRot, zRot));

    }

   /* private void OnDestroy()
    {
        // If the app is destroyed, reset position and rotation to (0, 0, 0)
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }*/

    IEnumerator SetLocation(string id_number, float xPos, float yPos, float zPos, float xRot, float yRot, float zRot)
    {
        // Create a new form to store the data
        WWWForm form = new WWWForm();
        form.AddField("id_number", id_number);
        form.AddField("xPos", xPos.ToString());
        form.AddField("yPos", yPos.ToString());
        form.AddField("zPos", zPos.ToString());
        form.AddField("xRot", xRot.ToString());
        form.AddField("yRot", yRot.ToString());
        form.AddField("zRot", zRot.ToString());

        // Send a POST request to your PHP script
        UnityWebRequest www = UnityWebRequest.Post($"http://{IP}/UMEP/Users_location.php", form);
        //UnityWebRequest webRequest = UnityWebRequest.Post("$http://{IP}/UMEP/Users_location.php", form);
        // Send the request and wait for the response
        yield return www.SendWebRequest();

        // Check for errors
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            // Log the response
            //Debug.Log("Received: " + www.downloadHandler.text);
        }
    }

}
