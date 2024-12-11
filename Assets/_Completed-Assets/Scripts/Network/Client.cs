using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Reflection;

public class Client: MonoBehaviour
{
    public static Client instance; // シングルトン

    private Socket clientSocket; // ソケット
    private const int BUFFER_SIZE = 1024; // バッファサイズ
    private byte[] buffer; // バッファ
    private float timer = 0.0f; // タイマー
    private const float SYNC_RATE = 0.05f; // 同期レート(20Hz)
    public bool isReady = false; // 準備完了の通知が来たかどうか
    public bool isGamePlaying = false; // ゲーム中かどうか
    public int playerId; // プレイヤーID
    [SerializeField]
    public List<GameObject> networkObjectPrefabs;
    public Dictionary<int, GameObject> networkObjects; // id -> ネットワークオブジェクト
    public Dictionary<int, int> networkObjectTypes; // id -> ネットワークオブジェクトのタイプ
    public List<int> myNetworkObjects; // 自分が所有するネットワークオブジェクトのidリスト
    public List<int> otherNetworkObjects; // 自分が所有しないネットワークオブジェクトのidリスト
    private List<byte> receiveBuffer = new(); // 受信バッファ
    private readonly object lockObject = new();

    public void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        buffer = new byte[BUFFER_SIZE];

        networkObjects = new Dictionary<int, GameObject>();
        networkObjectTypes = new Dictionary<int, int>();
        myNetworkObjects = new List<int>();
        otherNetworkObjects = new List<int>();

        Connect("127.0.0.1", 8080);
    }

    public void Update()
    {
        if (!isGamePlaying) return;
        timer += Time.deltaTime;
        if (timer > SYNC_RATE)
        {
            timer = 0.0f;
            List<byte> data = new() {(byte)NetworkDataTypes.DataType.SYNC_OBJECT };
            // 各オブジェクトの同期データを取得してデータに追加 
            foreach (var key in networkObjects.Keys)
            {
                byte[] syncObjectData = NetworkDataTypes.EncodeSyncObjectData(
                    key,
                    networkObjectTypes[key],
                    networkObjects[key].transform.position,
                    networkObjects[key].transform.rotation,
                    networkObjects[key].GetComponent<Rigidbody>().velocity
                );
                data.AddRange(syncObjectData);
            }
            Send(data.ToArray());
        }
    }

    public async void Connect(string ipAddress, int port)
    {
        try
        {
            IPEndPoint endPoint = new(IPAddress.Parse(ipAddress), port);
            await Task.Factory.FromAsync(
                clientSocket.BeginConnect(endPoint, null, null),
                clientSocket.EndConnect);

            StartReceiving();
        }
        catch (Exception e)
        {
            Debug.LogError($"Connect error: {e.Message}");
        }
    }

    public void Send(byte[] data)
    {
        if (clientSocket != null && clientSocket.Connected)
        {
            byte[] sendData = new byte[data.Length + 1];
            sendData[0] = (byte)data.Length;
            data.CopyTo(sendData, 1);
            clientSocket.Send(sendData);
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
                    receiveBuffer.AddRange(buffer[..received]);
                    ProcessReceiveBuffer();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Receive error: {e.Message}");
                break;
            }
        }
    }

    private void ProcessReceiveBuffer()
    {
        while (receiveBuffer.Count > 0)
        {
            if (receiveBuffer.Count < 1) return; // データタイプを含む最小サイズを確認

            int messageLength = receiveBuffer[0]; // メッセージの長さを取得
            if (receiveBuffer.Count < messageLength + 1) return; // 完全なメッセージが受信されるまで待つ

            byte[] message = receiveBuffer.GetRange(1, messageLength).ToArray();
            ProcessMessage(message);
            receiveBuffer.RemoveRange(0, messageLength + 1);
        }
    }

    private void ProcessMessage(byte[] data)
    {
        switch (data[0])
        {
            case (byte)NetworkDataTypes.DataType.PLAYER_ID:
                playerId = data[1];
                Debug.Log($"playerId: {playerId}, {data.Length}bytes");
                break;

            case (byte)NetworkDataTypes.DataType.READY:
                isReady = true;
                Debug.Log($"isReady: {data.Length}bytes");
                break;

            case (byte)NetworkDataTypes.DataType.SYNC_OBJECT:
                Debug.Log($"received sync object data: {data.Length}bytes");
                lock (lockObject) ProcessSyncObject(data);
                break;

            case (byte)NetworkDataTypes.DataType.GAME_END:
                SceneManager.LoadScene(SceneNames.LobbyScene);
                break;
        }
    }

    private void ProcessSyncObject(byte[] data)
    {
        if (!isGamePlaying) return;
        Assert.IsTrue(data.Length % 48 == 1, "data.Length is not 48n+1");

        lock (lockObject)
        {
            // 1バイト目はデータタイプなので、2バイト目から48バイトずつ処理
            for (int i = 1; i < data.Length; i += 48)
            {
                NetworkDataTypes.SyncObjectData syncObjectData = NetworkDataTypes.DecodeSyncObjectData(data[i..(i + 48)]);
                if (myNetworkObjects.Contains(syncObjectData.objectId))
                {
                    continue;
                }
                else if (otherNetworkObjects.Contains(syncObjectData.objectId))
                {
                    if (syncObjectData.objectType == (int)NetworkDataTypes.ObjectType.SHELL) return;

                    networkObjects[syncObjectData.objectId].transform.position = syncObjectData.position;
                    networkObjects[syncObjectData.objectId].transform.rotation = syncObjectData.rotation;
                }
                else
                {
                    // 登録されていないオブジェクトは生成
                    InstantiateNetworkObject(syncObjectData);
                }
            }
        }
    }

    // 所有権を持たないオブジェクトを生成
    public GameObject InstantiateNetworkObject(NetworkDataTypes.SyncObjectData syncObjectData) {
        lock (lockObject)
        {
            GameObject networkObjectPrefab = networkObjectPrefabs[syncObjectData.objectType];
            GameObject networkObject = Instantiate(networkObjectPrefab, syncObjectData.position, syncObjectData.rotation);
            networkObject.name = $"{networkObjectPrefab.name}_{syncObjectData.objectId}"; // オブジェクトIDをオブジェクトに設定
            networkObjects.Add(syncObjectData.objectId, networkObject);
            networkObjectTypes.Add(syncObjectData.objectId, syncObjectData.objectType);
            otherNetworkObjects.Add(syncObjectData.objectId);

            if (syncObjectData.objectType == (int)NetworkDataTypes.ObjectType.SHELL)
            {
                networkObject.GetComponent<Rigidbody>().velocity = syncObjectData.velocity;
            }

            Debug.Log($"instantiate other network object: {syncObjectData.objectId}, {syncObjectData.objectType}");
            return networkObject;
        }
    }

    // 所有権を持つオブジェクトを生成
    public GameObject InstantiateNetworkObject(int objectType, Vector3 position, Quaternion rotation) {
        lock (lockObject)
        {
            GameObject networkObjectPrefab = networkObjectPrefabs[objectType];
            GameObject networkObject = Instantiate(networkObjectPrefab, position, rotation);
            // 非同期的にオブジェクトを生成しても他のネットワークノードのオブジェクトIDが重複しないようにする
            int objectId = myNetworkObjects.Count * 3 + playerId;
            networkObject.name = $"{networkObjectPrefab.name}_{objectId}"; // オブジェクトIDをオブジェクトに設定
            networkObjects.Add(objectId, networkObject);
            networkObjectTypes.Add(objectId, objectType);
            myNetworkObjects.Add(objectId);

            Debug.Log($"instantiate my network object: {objectId}, {objectType}");
            return networkObject;
        }
    }

    public void DestroyNetworkObject(int objectId)
    {
        lock (lockObject)
        {
            networkObjects.Remove(objectId);
            networkObjectTypes.Remove(objectId);
            // 自分が所有するオブジェクトの場合はサーバに削除を通知
            if (myNetworkObjects.Contains(objectId))
            {
                myNetworkObjects.Remove(objectId);
                List<byte> destroyObjectData = new() {(byte)NetworkDataTypes.DataType.DESTROY_OBJECT};
                destroyObjectData.AddRange(BitConverter.GetBytes(objectId));
                Send(destroyObjectData.ToArray());
                return;
            }
            else if (otherNetworkObjects.Contains(objectId))
            {
                otherNetworkObjects.Remove(objectId);
                return;
            }
            else
            {
                Debug.LogError($"destroy network object: {objectId} not found");
            }
        }
    }
}