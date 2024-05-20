using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShowLocation : MonoBehaviour
{
    private string IP;

    public GameObject capsulePrefab;
    private Dictionary<string, GameObject> instantiatedCapsules = new Dictionary<string, GameObject>();

    void Start()
    {
        IP = PlayerPrefs.GetString("IP");
    }

    // Start is called before the first frame update
    void Update()
    {
        StartCoroutine(GetLocation());
    }

    IEnumerator GetLocation()
    {
        // Send a GET request to your PHP script
        UnityWebRequest webRequest = UnityWebRequest.Get($"http://{IP}/UMEP/Get_location.php");
        // Send the request and wait for the response
        yield return webRequest.SendWebRequest();

        // Check for errors
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + webRequest.error);
        }
        else
        {
            // Log the response
            //Debug.Log("Received: " + webRequest.downloadHandler.text);

            // Parse the JSON data
            ParseLocationData(webRequest.downloadHandler.text);
        }
    }

    void ParseLocationData(string jsonData)
    {
        // Deserialize the JSON data into a list of LocationData objects
        LocationDataArray locationDataArray = JsonUtility.FromJson<LocationDataArray>("{\"locations\":" + jsonData + "}");

        foreach (LocationData location in locationDataArray.locations)
        {
            // Check if a capsulePrefab for this id_number already exists
            if (instantiatedCapsules.ContainsKey(location.id_number))
            {
                // Retrieve the existing capsule from the dictionary
                GameObject existingCapsule = instantiatedCapsules[location.id_number];

                // Update its position and rotation
                existingCapsule.transform.position = new Vector3(location.xPos, location.yPos, location.zPos);
                existingCapsule.transform.rotation = Quaternion.Euler(location.xRot, location.yRot, location.zRot);

                // Move to the next location data
                continue;
            }

            // Use the values from the LocationData object
            Vector3 position = new Vector3(location.xPos, location.yPos, location.zPos);
            Quaternion rotation = Quaternion.Euler(0, location.yRot, 0);

            // Instantiate the prefab at the given position and rotation
            GameObject capsule = Instantiate(capsulePrefab, position, rotation);

            // Add the instantiated capsule to the dictionary
            instantiatedCapsules.Add(location.id_number, capsule);
        }
    }


    [System.Serializable]
    public class LocationData
    {
        public int id;
        public string id_number;
        public float xPos;
        public float yPos;
        public float zPos;
        public float xRot;
        public float yRot;
        public float zRot;
    }

    [System.Serializable]
    private class LocationDataArray
    {
        public LocationData[] locations;
    }
}
