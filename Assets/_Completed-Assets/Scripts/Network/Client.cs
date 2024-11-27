using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class Client
{
    private Socket clientSocket;
    private const int BUFFER_SIZE = 1024;
    private byte[] buffer;
    public bool isReady = false;
    public int tankId = -1;

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
                int received = await Task.Factory.FromAsync<int>(
                    clientSocket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, null, null),
                    clientSocket.EndReceive);

                if (received > 0)
                {
                    // ここでデータ処理を行う
                    UnityEngine.Debug.Log($"Received {received} bytes");

                    switch (received)
                    {
                        case (int)NetworkDataTypes.DataType.READY:
                            isReady = true;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Receive error: {e.Message}");
                break;
            }
        }
    }
} 