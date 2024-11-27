using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using NetworkDataTypes;

public class Server
{
    private Socket serverSocket;
    private List<Socket> clientSockets = new List<Socket>();
    private const int BUFFER_SIZE = 1024;
    private byte[] buffer;
    public const int MAX_CLIENTS = 2;
    private bool[] isReady = new bool[MAX_CLIENTS];

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
                    client.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, null, null),
                    client.EndReceive);

                // 受信したデータを処理する
                if (received > 0)
                {
                    UnityEngine.Debug.Log($"Received {received} bytes");
                    
                    switch (received)
                    {
                        case (int)NetworkDataTypes.DataType.READY:
                            isReady[clientSockets.IndexOf(client)] = true;
                            break;
                    }
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