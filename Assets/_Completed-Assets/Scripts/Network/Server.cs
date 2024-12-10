using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Server
{
    private Socket serverSocket;
    private List<Socket> clientSockets = new List<Socket>();
    private const int BUFFER_SIZE = 1024;
    private byte[] buffer;
    public const int MAX_CLIENTS = 2;
    private bool[] isReady = new bool[MAX_CLIENTS];
    public Rigidbody[] m_Rigidbodies;
    public GameObject[] m_Turrets;
    public Rigidbody m_Shell;
    public GameObject m_Mine;
    public GameObject shellCartridgePrefab, mineCartridgePrefab;

    public Server()
    {
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        buffer = new byte[BUFFER_SIZE];
    }

    public void Bind(int port)
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
        serverSocket.Bind(endPoint);
    }

    public void Listen(int backlog = 1)
    {
        serverSocket.Listen(backlog);
        AcceptAsync();
    }

    private async void AcceptAsync()
    {
        try
        {
            // クライアントがMAX_CLIENTSになるまで接続を待つ
            while (clientSockets.Count < MAX_CLIENTS)
            {
                Socket client = await Task.Factory.FromAsync(
                    serverSocket.BeginAccept,
                    serverSocket.EndAccept,
                    null);
                
                clientSockets.Add(client);
                UnityEngine.Debug.Log($"client connected: {clientSockets.Count}");
                StartReceiving(client);
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"Accept error: {e.Message}");
        }
    }

    public void Send(byte[] data, int clientId)
    {
        if (clientSockets[clientId] != null && clientSockets[clientId].Connected)
        {
            clientSockets[clientId].Send(data);
        }
    }

    private async void StartReceiving(Socket client)
    {
        while (client != null && client.Connected)
        {
            try
            {
                int received = await Task.Factory.FromAsync<int>(
                    client.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, null, null), client.EndReceive);

                // 受信したデータを処理する
                if (received > 0)
                {
                    UnityEngine.Debug.Log($"received: {received}");
                    ProcessMessage(buffer[0..received], client);
                    UnityEngine.Debug.Log($"ProcessMessage: buffer Length: {buffer.Length}");
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Receive error: {e.Message}");
                clientSockets.Remove(client);
                break;
            }
        }
    }

    private void ProcessMessage(byte[] data, Socket client)
    {
        switch (data[0])
        {
            case (byte)NetworkDataTypes.DataType.READY:
                isReady[clientSockets.IndexOf(client)] = true;
                UnityEngine.Debug.Log($"client {clientSockets.IndexOf(client)} is ready");
                break;

            case (byte)NetworkDataTypes.DataType.GET_PLAYER_ID:
                byte[] playerIdData = new byte[] { (byte)NetworkDataTypes.DataType.PLAYER_ID, (byte)clientSockets.IndexOf(client) };
                Send(playerIdData, clientSockets.IndexOf(client));
                UnityEngine.Debug.Log($"client {clientSockets.IndexOf(client)} requested player id");
                break;
            
            case (byte)NetworkDataTypes.DataType.SYNC_OBJECT:
                UnityEngine.Debug.Log("sync object received. Length: " + data.Length);
                ProcessSyncObject(data, client);
                UnityEngine.Debug.Log($"client {clientSockets.IndexOf(client)} sent sync object");
                break;
        }
    }

    private void ProcessSyncObject(byte[] data, Socket client)
    {
        // 1バイト目はデータタイプなので、2バイト目から48バイトずつ処理
        for (int i = 1; i < data.Length; i += 48)
        {
            // SyncObjectDataにデコード
            NetworkDataTypes.SyncObjectData syncData = NetworkDataTypes.DecodeSyncObjectData(data[i..(i + 48)]);
            
            // keyが存在するとき
            if (NetworkManager.instance.syncObjectData.ContainsKey(syncData.objectId))
            {
                NetworkManager.instance.syncObjectData[syncData.objectId] = syncData;
            }
            else
            {
                NetworkManager.instance.syncObjectData.Add(syncData.objectId, syncData);
            }
        }
    }

    // 指定したクライアント以外のクライアントにデータを送信
    public void SendFromServerToOthers(byte[] data, Socket client)
    {
        for (int i = 0; i < clientSockets.Count; i++)
        {
            if (clientSockets[i] != client) Send(data, i);
        }
    }

    public bool IsReady()
    {
        bool res = true;
        foreach (bool ready in isReady)
        {
            res &= ready;
        }
        return res;
    }

    public void ResetReady()
    {
        isReady = new bool[MAX_CLIENTS];
    }

    public int GetClientSocketsCount()
    {
        return clientSockets.Count;
    }
}