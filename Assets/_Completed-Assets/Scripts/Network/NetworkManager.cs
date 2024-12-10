using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance; // シングルトンインスタンス
    public Server server;
    public Client client;
    public bool isServer = false;
    public bool isClient = false;
    private float syncRate = 0.1f; // 同期間隔
    private float syncTimer = 0f; // 同期タイマ

    [SerializeField]
    public List<GameObject> syncObjectRefs;

    public Dictionary<int, NetworkDataTypes.SyncObjectData> syncObjectData;
    public List<int> syncObjectIds;

    public bool isGameStart = false;

    public void Awake()
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

        syncObjectData = new Dictionary<int, NetworkDataTypes.SyncObjectData>();
        syncObjectIds = new List<int>();
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
        client.Send(new byte[] { (byte)NetworkDataTypes.DataType.GET_PLAYER_ID });
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

    public void Update()
    {
        if (!isGameStart || syncObjectData.Count == 0) return;

        syncTimer += Time.deltaTime;
        if (syncTimer >= syncRate)
        {
            syncTimer = 0f;
            SendSyncObject();
        }

        foreach (var key in syncObjectData.Keys)
        {
            if (!syncObjectIds.Contains(key))
            {
                RegisterSyncObject(syncObjectData[key]);
            }
        }
    }

    private void SendSyncObject()
    {
        // データを格納するバッファを作成
        byte[] data = new byte[48 * syncObjectData.Count + 1];
        data[0] = (byte)NetworkDataTypes.DataType.SYNC_OBJECT;
        int offset = 1;

        Debug.Log($"syncObjectData Count: {syncObjectData.Count}");

        // 各SyncObjectDataをバッファにコピー
        foreach (var syncData in syncObjectData.Values)
        {
            byte[] encodedData = NetworkDataTypes.EncodeSyncObjectData(syncData);
            Debug.Log($"encodedData Length: {encodedData.Length}");
            Buffer.BlockCopy(encodedData, 0, data, offset, encodedData.Length);
            offset += encodedData.Length;
        }

        // データを送信
        if (isClient) SendFromClient(data);
        if (isServer)
        {
            for (int i = 0; i < server.GetClientSocketsCount(); i++)
            {
                SendFromServer(data, i);
            }
        }
    }

    private void RegisterSyncObject(NetworkDataTypes.SyncObjectData data)
    {
        GameObject obj = Instantiate(syncObjectRefs[data.objectType], data.position, data.rotation) as GameObject;
        SyncObject newSyncObject = obj.AddComponent<SyncObject>();
        newSyncObject.CopyFrom(data);
        syncObjectIds.Add(data.objectId);
    }

    // client method (for debug)
    public void OnGUI()
    {
        if (isClient)
        {
            GUI.Label(new Rect(10, 30, 100, 20), $"client id: {client.playerId}, message: {client.messageString}", new GUIStyle() { normal = { textColor = Color.black } });
        }
    }
}