using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using Complete;

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

    // server method
    private void StartServer(int port)
    {
        server = new Server();
        server.Bind(port);
        server.Listen();
    }

    // client method
    private void StartClient(string ipAddress, int port)
    {
        client = new Client();
        client.Connect(ipAddress, port);
    }

    // server method
    public void SendFromServer(byte[] data, int clientId)
    {
        if (server != null)
        {
            server.Send(data, clientId);
        }
    }

    // client method
    public void SendFromClient(byte[] data)
    {
        if (client != null)
        {
            client.Send(data);
        }
    }

    // client method
    public int GetTankId()
    {
        if (client != null)
        {
            return client.tankId;
        }
        return -1;
    }

    public void SetRigidbodies(Rigidbody[] m_Rigidbodies)
    {
        if (isServer) server.m_Rigidbodies = m_Rigidbodies;
        if (isClient) client.m_Rigidbodies = m_Rigidbodies;
    }

    public void SetTurrets(GameObject[] m_Turrets)
    {
        if (isServer) server.m_Turrets = m_Turrets;
        if (isClient) client.m_Turrets = m_Turrets;
    }

    public void SetShell(Rigidbody m_Shell)
    {
        if (isServer) server.m_Shell = m_Shell;
        if (isClient) client.m_Shell = m_Shell;
    }

    public void SetMine(GameObject m_Mine)
    {
        if (isServer) server.m_Mine = m_Mine;
        if (isClient) client.m_Mine = m_Mine;
    }

    public void SetCartridgePrefabs(GameObject shellCartridgePrefab, GameObject mineCartridgePrefab)
    {
        // サーバ
        if (isServer) server.shellCartridgePrefab = shellCartridgePrefab;
        if (isServer) server.mineCartridgePrefab = mineCartridgePrefab;

        // クライアント
        if (isClient) client.shellCartridgePrefab = shellCartridgePrefab;
        if (isClient) client.mineCartridgePrefab = mineCartridgePrefab;
    }

    // client method (for debug)
    public void OnGUI()
    {
        if (isClient)
        {
            GUI.Label(new Rect(10, 30, 100, 20), $"client ready: {client.isReady}, message: 0x{client.message:X}", new GUIStyle() { normal = { textColor = Color.black } });
        }
    }
}
