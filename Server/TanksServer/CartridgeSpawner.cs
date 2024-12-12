public class CartridgeSpawner
{
    public float shellSpawnInterval = 5f;
    public float mineSpawnInterval = 15f;
    public float spawnAreaWidth = 40f;
    public float spawnAreaHeight = 40f;
    public float heightOffset = 1f;

    // private void SpawnShellCartridge()
    // {
    //     Vector3 randomPosition = new Vector3(
    //         UnityEngine.Random.Range(-spawnAreaWidth/2, spawnAreaWidth/2),
    //         heightOffset,
    //         UnityEngine.Random.Range(-spawnAreaHeight/2, spawnAreaHeight/2)
    //     );
    // }

    public byte[] SpawnShellCartridge()
    {
        byte[] data = new byte[48];
        return data;
    }

    // private void SpawnMineCartridge()
    // {
    //     Vector3 randomPosition = new(
    //         UnityEngine.Random.Range(-spawnAreaWidth/2, spawnAreaWidth/2),
    //         heightOffset,
    //         UnityEngine.Random.Range(-spawnAreaHeight/2, spawnAreaHeight/2)
    //     );

    //     Instantiate(mineCartridgePrefab, randomPosition, Quaternion.identity);
    // }

    public byte[] SpawnMineCartridge()
    {
        byte[] data = new byte[48];
        return data;
    }
}
