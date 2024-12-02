using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance; // シングルトンインスタンス
    public Server server;
    public Client client;
    public bool isServer = false;
    public bool isClient = false;

    void Awake()
    {
        // シングルトンインスタンスを作成
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

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

    public int GetTankId()
    {
        if (client != null)
        {
            return client.tankId;
        }
        return -1;
    }

    public void OnGUI()
    {
        if (isClient)
        {
            GUI.Label(new Rect(10, 30, 100, 20), $"client ready: {client.isReady}, message: 0x{client.message:X}", new GUIStyle() { normal = { textColor = Color.black } });
        }
    }
}
