using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class MultiplayerClient : MonoBehaviour
{
    private TcpClient tcpClient;
    private NetworkStream stream;
    private byte[] receiveBuffer = new byte[4096];
    private const string SERVER_ADDRESS = "192.168.0.118";
    private const int PORT = 8888;

    void Start()
    {
        ConnectToServer();
    }

    void Update()
    {
        // Check for F key press
        if (Input.GetKeyDown(KeyCode.F))
        {
            SendKeyPressToServer();
        }
    }

    void ConnectToServer()
    {
        tcpClient = new TcpClient(SERVER_ADDRESS, PORT);
        stream = tcpClient.GetStream();

        stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveCallback, null);
    }

    void SendKeyPressToServer()
    {
        // Send a message to the server indicating the F key press
        string message = "F Key Pressed";
        byte[] data = Encoding.ASCII.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    void ReceiveCallback(IAsyncResult ar)
    {
        int bytesRead = stream.EndRead(ar);
        if (bytesRead <= 0)
        {
            Debug.Log("Disconnected from server");
            return;
        }

        string receivedData = Encoding.ASCII.GetString(receiveBuffer, 0, bytesRead);
        Debug.Log("Received data: " + receivedData);

        // Handle the received data as needed

        stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveCallback, null);
    }
}
