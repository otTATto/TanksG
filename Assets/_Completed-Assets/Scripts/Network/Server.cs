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
                    ProcessMessage(buffer, client);
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
        int targetTankIndex;
        float x, y, z;
        float rotX, rotY, rotZ, rotW;
        switch (data[0])
        {
            case (byte)NetworkDataTypes.DataType.READY:
                isReady[clientSockets.IndexOf(client)] = true;
                break;
            
            case (byte)NetworkDataTypes.DataType.TANK_POSITION:
                SendFromServerToOthers(data, client);
                // ターゲットのタンクの位置を更新
                targetTankIndex = (int)data[1] - 1;
                x = BitConverter.ToSingle(data, 2);
                y = BitConverter.ToSingle(data, 6);
                z = BitConverter.ToSingle(data, 10);
                Vector3 position = new Vector3(x, y, z);
                m_Rigidbodies[targetTankIndex].position = position;
                break;
            
            case (byte)NetworkDataTypes.DataType.TANK_ROTATION:
                SendFromServerToOthers(data, client);
                // ターゲットのタンクの回転を更新
                targetTankIndex = (int)data[1] - 1;
                rotX = BitConverter.ToSingle(data, 2);
                rotY = BitConverter.ToSingle(data, 6);
                rotZ = BitConverter.ToSingle(data, 10);
                rotW = BitConverter.ToSingle(data, 14);
                Quaternion tankRotation = new Quaternion(rotX, rotY, rotZ, rotW);
                m_Rigidbodies[targetTankIndex].transform.rotation = tankRotation;
                break;
            
            case (byte)NetworkDataTypes.DataType.TURRET_ROTATION:
                SendFromServerToOthers(data, client);
                // ターゲットのタンクの砲塔の回転を更新
                targetTankIndex = (int)data[1] - 1;
                rotX = BitConverter.ToSingle(data, 2);
                rotY = BitConverter.ToSingle(data, 6);
                rotZ = BitConverter.ToSingle(data, 10);
                rotW = BitConverter.ToSingle(data, 14);
                Quaternion turretRotation = new Quaternion(rotX, rotY, rotZ, rotW);
                m_Turrets[targetTankIndex].transform.rotation = turretRotation;
                break;
            
            case (byte)NetworkDataTypes.DataType.SHELL_POSITION:
                SendFromServerToOthers(data, client);
                // 位置
                x = BitConverter.ToSingle(data, 1);
                y = BitConverter.ToSingle(data, 5);
                z = BitConverter.ToSingle(data, 9);
                Vector3 shellPosition = new Vector3(x, y, z);

                // 回転
                rotX = BitConverter.ToSingle(data, 13);
                rotY = BitConverter.ToSingle(data, 17);
                rotZ = BitConverter.ToSingle(data, 21);
                rotW = BitConverter.ToSingle(data, 25);
                Quaternion shellRotation = new Quaternion(rotX, rotY, rotZ, rotW);

                // 速度
                float vX = BitConverter.ToSingle(data, 29);
                float vY = BitConverter.ToSingle(data, 33);
                float vZ = BitConverter.ToSingle(data, 37);
                Vector3 shellVelocity = new Vector3(vX, vY, vZ);

                // 砲弾を生成
                Rigidbody shellInstance = Object.Instantiate(m_Shell, shellPosition, shellRotation) as Rigidbody;
                shellInstance.velocity = shellVelocity;
                break;
            
            case (byte)NetworkDataTypes.DataType.MINE_POSITION:
                SendFromServerToOthers(data, client);
                x = BitConverter.ToSingle(data, 1);
                y = BitConverter.ToSingle(data, 5);
                z = BitConverter.ToSingle(data, 9);
                Vector3 minePosition = new Vector3(x, y, z);
                Object.Instantiate(m_Mine, minePosition, Quaternion.identity);
                break;
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