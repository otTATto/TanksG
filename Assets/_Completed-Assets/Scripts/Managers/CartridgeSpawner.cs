using System.Collections;
using UnityEngine;

public class ShellCartridgeSpawner : MonoBehaviour
{
    public GameObject cartridgePrefab;
    public float spawnInterval = 5f;
    public float spawnAreaWidth = 40f;
    public float spawnAreaHeight = 40f;
    public float heightOffset = 1f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= spawnInterval)
        {
            SpawnCartridge();
            timer = 0f;
        }
    }

    private void SpawnCartridge()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnAreaWidth/2, spawnAreaWidth/2),
            heightOffset,
            Random.Range(-spawnAreaHeight/2, spawnAreaHeight/2)
        );

        Instantiate(cartridgePrefab, randomPosition, Quaternion.identity);
    }
}