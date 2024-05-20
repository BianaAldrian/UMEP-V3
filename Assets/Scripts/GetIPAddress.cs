using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class GetIPAddress : MonoBehaviour
{
    void Start()
    {
        string localIP = GetLocalIPAddress();
        Debug.Log("Local IP Address: " + localIP);
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
}
