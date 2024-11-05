using System.Collections;
using UnityEngine;

public class ShellCartridgeSpawner : MonoBehaviour
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
            Random.Range(-spawnAreaWidth/2, spawnAreaWidth/2),
            heightOffset,
            Random.Range(-spawnAreaHeight/2, spawnAreaHeight/2)
        );

        Instantiate(shellCartridgePrefab, randomPosition, Quaternion.identity);
    }

    private void SpawnMineCartridge()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnAreaWidth/2, spawnAreaWidth/2),
            heightOffset,
            Random.Range(-spawnAreaHeight/2, spawnAreaHeight/2)
        );

        Instantiate(mineCartridgePrefab, randomPosition, Quaternion.identity);
    }
}