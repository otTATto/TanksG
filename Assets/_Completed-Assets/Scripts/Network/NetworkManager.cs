using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager instance;
    public Server server;
    public Client client;
    public bool isServer = false;
    public bool isClient = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (isServer)
        {
            StartServer(8000);
            Debug.Log("server started");
        }
        else if (isClient)
        {
            StartClient("127.0.0.1", 8000);
        }
    }

    private void StartServer(int port)
    {
        server = new Server();
        server.Bind(port);
        server.Listen();
    }

    private void StartClient(string ipAddress, int port)
    {
        client = new Client();
        client.Connect(ipAddress, port);
    }

    public void SendFromServer(byte[] data, int clientId)
    {
        if (server != null)
        {
            server.Send(data, clientId);
        }
    }

    public void SendFromClient(byte[] data)
    {
        if (client != null)
        {
            client.Send(data);
        }
    }

    public void OnGUI()
    {
        if (isClient)
        {
            string message = client.isReady ? "true" : "false";
            GUI.Label(new Rect(10, 30, 100, 20), $"client ready: {message}", new GUIStyle() { normal = { textColor = Color.black } });
        }
    }
}
