using System.Collections;
using UnityEngine;
using System;

public class CartridgeSpawner : MonoBehaviour
{
    public GameObject shellCartridgePrefab;
    public GameObject mineCartridgePrefab;
    public float shellSpawnInterval = 5f;
    public float mineSpawnInterval = 15f;
    public float spawnAreaWidth = 40f;
    public float spawnAreaHeight = 40f;
    public float heightOffset = 1f;

    private float mineTimer;
    private float shellTimer;

    private void Update()
    {
        if (!NetworkManager.instance.isServer) return;

        mineTimer += Time.deltaTime;
        shellTimer += Time.deltaTime;
        
        if (shellTimer >= shellSpawnInterval)
        {
            SpawnShellCartridge();
            shellTimer = 0f;
        }

        if (mineTimer >= mineSpawnInterval)
        {
            SpawnMineCartridge();
            mineTimer = 0f;
        }
    }

    private void SpawnShellCartridge()
    {
        Vector3 randomPosition = new Vector3(
            UnityEngine.Random.Range(-spawnAreaWidth/2, spawnAreaWidth/2),
            heightOffset,
            UnityEngine.Random.Range(-spawnAreaHeight/2, spawnAreaHeight/2)
        );

        SendShellCartridge(randomPosition);
        Instantiate(shellCartridgePrefab, randomPosition, Quaternion.identity);
    }

    private void SendShellCartridge(Vector3 position)
    {
        byte[] data = new byte[13];
        data[0] = (byte)NetworkDataTypes.DataType.S_CARTIDGE_POSITION;
        BitConverter.GetBytes(position.x).CopyTo(data, 1);
        BitConverter.GetBytes(position.y).CopyTo(data, 5);
        BitConverter.GetBytes(position.z).CopyTo(data, 9);
        for (int i = 0; i < NetworkManager.instance.server.GetClientSocketsCount(); i++)
        {
            NetworkManager.instance.SendFromServer(data, i);
        }
    }

    private void SpawnMineCartridge()
    {
        Vector3 randomPosition = new Vector3(
            UnityEngine.Random.Range(-spawnAreaWidth/2, spawnAreaWidth/2),
            heightOffset,
            UnityEngine.Random.Range(-spawnAreaHeight/2, spawnAreaHeight/2)
        );

        SendMineCartridge(randomPosition);
        Instantiate(mineCartridgePrefab, randomPosition, Quaternion.identity);
    }

    private void SendMineCartridge(Vector3 position)
    {
        byte[] data = new byte[13];
        data[0] = (byte)NetworkDataTypes.DataType.M_CARTIDGE_POSITION;
        BitConverter.GetBytes(position.x).CopyTo(data, 1);
        BitConverter.GetBytes(position.y).CopyTo(data, 5);
        BitConverter.GetBytes(position.z).CopyTo(data, 9);
        for (int i = 0; i < NetworkManager.instance.server.GetClientSocketsCount(); i++)
        {
            NetworkManager.instance.SendFromServer(data, i);
        }
    }
}
