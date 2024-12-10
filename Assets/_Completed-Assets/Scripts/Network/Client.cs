using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class Client
{
    private Socket clientSocket;
    private const int BUFFER_SIZE = 1024;
    private byte[] buffer;
    public bool isReady = false;
    public int playerId;
    public int message;
    public string messageString;

    public Client()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        buffer = new byte[BUFFER_SIZE];
    }

    public async void Connect(string ipAddress, int port)
    {
        try
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            await Task.Factory.FromAsync(
                clientSocket.BeginConnect(endPoint, null, null),
                clientSocket.EndConnect);

            StartReceiving();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"Connect error: {e.Message}");
        }
    }

    public void Send(byte[] data)
    {
        if (clientSocket != null && clientSocket.Connected)
        {
            clientSocket.Send(data);
        }
    }

    private async void StartReceiving()
    {
        while (clientSocket != null && clientSocket.Connected)
        {
            try
            {
                // 受信したデータ容量を取得
                int received = await Task.Factory.FromAsync<int>(
                    clientSocket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, null, null),
                    clientSocket.EndReceive);

                if (received > 0)
                {
                    ProcessMessage(buffer);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Receive error: {e.Message}");
                break;
            }
        }
    }

    private void ProcessMessage(byte[] data)
    {
        message = data[0];
        switch (data[0])
        {
            case (byte)NetworkDataTypes.DataType.PLAYER_ID:
                playerId = data[1];
                messageString = "PLAYER_ID";
                break;

            case (byte)NetworkDataTypes.DataType.READY:
                isReady = true;
                messageString = "READY";
                break;

            case (byte)NetworkDataTypes.DataType.SYNC_OBJECT:
                ProcessSyncObject(data);
                messageString = "SYNC_OBJECT";
                break;

            case (byte)NetworkDataTypes.DataType.GAME_END:
                UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.LobbyScene);
                messageString = "GAME_END";
                break;
        }
    }

    private void ProcessSyncObject(byte[] data)
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
} 