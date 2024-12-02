using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class Client
{
    private Socket clientSocket;
    private const int BUFFER_SIZE = 1024;
    private byte[] buffer;
    public int tankId = -1;
    public bool isReady = false;
    public int message = 0;

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
                    ProcessMessage(buffer[0]);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Receive error: {e.Message}");
                break;
            }
        }
    }

    private void ProcessMessage(byte data)
    {
        message = data; // TODO: デバッグ用
        switch (data)
        {
            // tankIdを付与
            case (int)NetworkDataTypes.DataType.TANK_ID_1:
                tankId = 1;
                isReady = true;
                break;

            case (int)NetworkDataTypes.DataType.TANK_ID_2:
                tankId = 2;
                isReady = true;
                break;
        }
    }
} 