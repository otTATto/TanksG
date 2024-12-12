using System.Net;
using System.Net.Sockets;

new Server();
while (true) {};

public class Server
{
    private Socket serverSocket;
    private List<Socket> clientSockets = [];
    private const int BUFFER_SIZE = 1024;
    private byte[] buffer;
    public const int MAX_CLIENTS = 2;
    private bool[] isReady = new bool[MAX_CLIENTS];
    private const float SYNC_RATE = 0.05f; // 同期するレート(20Hz)
    private Dictionary<int, byte[]> syncObjectsData = []; // object id -> syncObjectData(for client, 48bytes)
    private Dictionary<Socket, List<byte>> receiveBuffers = new(); // 各クライアントの受信バッファ
    private readonly object lockObject = new();

    public Server()
    {
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        buffer = new byte[BUFFER_SIZE];
        Bind(8080);
        Listen();
        Console.WriteLine("server started.");
        SendDataASync();
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
                
                Console.WriteLine($"client {clientSockets.Count} connected.");
                clientSockets.Add(client);
                StartReceivingAsync(client);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Accept error: {e.Message}");
        }
    }

    public void Send(byte[] data, int clientId)
    {
        if (clientSockets[clientId] != null && clientSockets[clientId].Connected)
        {
            byte[] sendData = new byte[data.Length + 1];
            sendData[0] = (byte)data.Length;
            data.CopyTo(sendData, 1);
            clientSockets[clientId].Send(sendData);
        }
    }

    public void BroadCast(byte[] data)
    {
        byte[] sendData = new byte[data.Length + 1];
        sendData[0] = (byte)data.Length;
        data.CopyTo(sendData, 1);

        foreach (var client in clientSockets)
        {
            client.Send(sendData);
        }
    }

    private async void StartReceivingAsync(Socket client)
    {
        // 受信バッファがない場合は作成
        if (!receiveBuffers.ContainsKey(client))
        {
            receiveBuffers[client] = [];
        }

        while (client != null && client.Connected)
        {
            try
            {
                int received = await Task.Factory.FromAsync<int>(
                    client.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, null, null), client.EndReceive);

                // 受信したデータを処理する
                if (received > 0)
                {
                    lock (lockObject) receiveBuffers[client].AddRange(buffer[..received]);
                    ProcessReceiveBuffer(client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Receive error: {e.Message}");
                clientSockets.Remove(client);
                receiveBuffers.Remove(client);
                break;
            }
        }
    }

    private void ProcessReceiveBuffer(Socket client)
    {
        List<byte> clientBuffer = receiveBuffers[client];

        while (clientBuffer.Count > 0)
        {
            if (clientBuffer.Count < 1) return; // データタイプを含む最小サイズを確認

            int messageLength = clientBuffer[0]; // メッセージの長さを取得
            if (clientBuffer.Count < messageLength + 1) return; // 完全なメッセージが受信されるまで待つ

            byte[] message = [.. clientBuffer.GetRange(1, messageLength)];
            lock (lockObject) ProcessMessage(message, client);
            clientBuffer.RemoveRange(0, messageLength + 1);
        }
    }

    private void ProcessMessage(byte[] data, Socket client)
    {
        Console.WriteLine($"process message: {data.Length}bytes");
        switch (data[0])
        {
            case (byte)NetworkDataTypes.DataType.READY:
                isReady[clientSockets.IndexOf(client)] = true;
                Console.WriteLine($"client {clientSockets.IndexOf(client)} is ready");
                // クライアント全員がReadyを送ってきたらサーバからReadyを送信
                if (IsReady())
                {
                    BroadCast(data);
                    ResetReady();
                    Console.WriteLine("sent Ready to all clients");
                }
                break;

            case (byte)NetworkDataTypes.DataType.GET_PLAYER_ID:
                byte[] playerIdData = [(byte)NetworkDataTypes.DataType.PLAYER_ID, (byte)clientSockets.IndexOf(client)];
                Send(playerIdData, clientSockets.IndexOf(client));
                Console.WriteLine($"client {clientSockets.IndexOf(client)} requested player id");

                isReady[clientSockets.IndexOf(client)] = true;
                // クライアント全員がget playerIdを送ってきたらサーバからReadyを送信
                if (IsReady())
                {
                    BroadCast([(byte)NetworkDataTypes.DataType.READY]);
                    ResetReady();
                    Console.WriteLine("sent Ready to all clients");
                }
                break;

            case (byte)NetworkDataTypes.DataType.SYNC_OBJECT:
                ProcessSyncObject(data);
                break;

            case (byte)NetworkDataTypes.DataType.DESTROY_OBJECT:
                // サーバでidの削除を行うのみで、クライアントには送信しない
                int objectId = BitConverter.ToInt32(data[1..5]);
                syncObjectsData.Remove(objectId);
                break;
        }
    }

    private void ProcessSyncObject(byte[] data)
    {
        lock (lockObject)
        {
            // 2byte目から48bytesずつ処理
            for (int i = 1; i < data.Length; i += 48)
            {
                int objectId = BitConverter.ToInt32(data[i..(i + 4)]);

                // objectIdがsyncObjectsDataに存在する場合はそのデータを書き換え
                if (syncObjectsData.ContainsKey(objectId))
                {
                    syncObjectsData[objectId] = data[i..(i + 48)];
                }
                // objectIdがsyncObjectsDataに存在しない場合はデータに追加
                else
                {
                    syncObjectsData.Add(objectId, data[i..(i + 48)]);
                }
            }
        }
    }

    private async void SendDataASync()
    {
        while (true)
        {
            await Task.Delay((int)(SYNC_RATE * 1000));

            // 同期データがない場合はスキップ
            if (syncObjectsData.Count == 0) continue;

            // 全てのオブジェクトの同期データを1つのバイト配列にまとめる
            byte[] syncData = new byte[syncObjectsData.Count * 48 + 1];
            syncData[0] = (byte)NetworkDataTypes.DataType.SYNC_OBJECT;
            int index = 1;
            foreach (byte[] objectData in syncObjectsData.Values)
            {
                objectData.CopyTo(syncData, index);
                index += 48;
            }

            // 全クライアントに送信
            BroadCast(syncData);
            Console.WriteLine("sent objects data to all clients");
        }
    }

    public bool IsReady()
    {
        foreach (bool ready in isReady)
        {
            if (!ready) return false;
        }
        return true;
    }

    public void ResetReady()
    {
        for (int i = 0; i < isReady.Length; i++)
        {
            isReady[i] = false;
        }
    }
}
