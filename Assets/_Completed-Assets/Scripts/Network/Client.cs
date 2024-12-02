using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

public class Client
{
    private Socket clientSocket;
    private const int BUFFER_SIZE = 1024;
    private byte[] buffer;
    public int tankId = -1;
    public bool isReady = false;
    public int message = 0;
    public Rigidbody[] m_Rigidbodies;
    public GameObject[] m_Turrets;
    public Rigidbody m_Shell;
    public GameObject m_Mine;
    public GameObject shellCartridgePrefab, mineCartridgePrefab;

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
        // データの先頭はメッセージの種類を表す
        int message = data[0];

        int targetTankIndex;
        float x, y, z;
        float rotX, rotY, rotZ, rotW;
        switch (data[0])
        {
            // tankIdを付与
            case (byte)NetworkDataTypes.DataType.TANK_ID_1:
                tankId = 1;
                isReady = true;
                break;

            case (byte)NetworkDataTypes.DataType.TANK_ID_2:
                tankId = 2;
                isReady = true;
                break;

            case (byte)NetworkDataTypes.DataType.TANK_POSITION:
                targetTankIndex = (int)data[1] - 1;
                x = BitConverter.ToSingle(data, 2);
                y = BitConverter.ToSingle(data, 6);
                z = BitConverter.ToSingle(data, 10);
                Vector3 position = new Vector3(x, y, z);
                m_Rigidbodies[targetTankIndex].position = position;
                break;

            case (byte)NetworkDataTypes.DataType.TANK_ROTATION:
                targetTankIndex = (int)data[1] - 1;
                rotX = BitConverter.ToSingle(data, 2);
                rotY = BitConverter.ToSingle(data, 6);
                rotZ = BitConverter.ToSingle(data, 10);
                rotW = BitConverter.ToSingle(data, 14);
                Quaternion tankRotation = new Quaternion(rotX, rotY, rotZ, rotW);
                m_Rigidbodies[targetTankIndex].transform.rotation = tankRotation;
                break;

            case (byte)NetworkDataTypes.DataType.TURRET_ROTATION:
                targetTankIndex = (int)data[1] - 1;
                rotX = BitConverter.ToSingle(data, 2);
                rotY = BitConverter.ToSingle(data, 6);
                rotZ = BitConverter.ToSingle(data, 10);
                rotW = BitConverter.ToSingle(data, 14);
                Quaternion turretRotation = new Quaternion(rotX, rotY, rotZ, rotW);
                m_Turrets[targetTankIndex].transform.rotation = turretRotation;
                break;
            
            case (byte)NetworkDataTypes.DataType.SHELL_POSITION:
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
                x = BitConverter.ToSingle(data, 1);
                y = BitConverter.ToSingle(data, 5);
                z = BitConverter.ToSingle(data, 9);
                Vector3 minePosition = new Vector3(x, y, z);
                GameObject mineInstance = Object.Instantiate(m_Mine, minePosition, Quaternion.identity) as GameObject;
                mineInstance.GetComponent<MeshRenderer>().enabled = false; // レンダラーを無効化
                break;
            
            case (byte)NetworkDataTypes.DataType.S_CARTIDGE_POSITION:
                x = BitConverter.ToSingle(data, 1);
                y = BitConverter.ToSingle(data, 5);
                z = BitConverter.ToSingle(data, 9);
                Vector3 shellCartridgePosition = new Vector3(x, y, z);
                Object.Instantiate(shellCartridgePrefab, shellCartridgePosition, Quaternion.identity);
                break;
            
            case (byte)NetworkDataTypes.DataType.M_CARTIDGE_POSITION:
                x = BitConverter.ToSingle(data, 1);
                y = BitConverter.ToSingle(data, 5);
                z = BitConverter.ToSingle(data, 9);
                Vector3 mineCartridgePosition = new Vector3(x, y, z);
                Object.Instantiate(mineCartridgePrefab, mineCartridgePosition, Quaternion.identity);
                break;

            case (byte)NetworkDataTypes.DataType.GAME_END:
                UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.LobbyScene);
                break;
        }
    }
} 