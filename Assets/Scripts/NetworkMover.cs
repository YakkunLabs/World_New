using UnityEngine;
using System.Net.Sockets;
using System.Text;

public class NetworkMover : MonoBehaviour
{
    private UdpClient udpClient;
    private string serverIP = "127.0.0.1";
    private int port = 7777;

    void Start()
    {
        udpClient = new UdpClient();
        udpClient.Connect(serverIP, port);
    }

    void Update()
    {
        // 1. Create a string with our position
        // Format: "POS: X, Y, Z"
        string posData = $"POS:{transform.position.x:F2},{transform.position.y:F2},{transform.position.z:F2}";

        // 2. Convert string to bytes
        byte[] data = Encoding.UTF8.GetBytes(posData);

        // 3. Send to Rust Server
        udpClient.Send(data, data.Length);
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}