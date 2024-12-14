
public class CartridgeSpawner
{
    private const float shellSpawnInterval = 5f;
    private const float mineSpawnInterval = 15f;
    private const float spawnAreaWidth = 40f;
    private const float spawnAreaHeight = 40f;
    private const float heightOffset = 1f;

    public static async Task<byte[]> SpawnShellCartridgeAsync(int objectId, Random rnd)
    {
        byte[] data = new byte[Server.OBJECT_DATA_SIZE];

        await Task.Delay((int)(shellSpawnInterval * 1000));

        // 弾丸のデータを生成
        // objectId
        BitConverter.GetBytes(objectId).CopyTo(data, 0);
        // objectType
        BitConverter.GetBytes((int)NetworkDataTypes.ObjectType.SHELL_CARTIDGE).CopyTo(data, 4);

        // position
        float x, y, z;
        x = (rnd.NextSingle() - 0.5f) * spawnAreaWidth;
        y = heightOffset;
        z = (rnd.NextSingle() - 0.5f) * spawnAreaHeight;
        BitConverter.GetBytes(x).CopyTo(data, 8);
        BitConverter.GetBytes(y).CopyTo(data, 12);
        BitConverter.GetBytes(z).CopyTo(data, 16);

        // rotation
        BitConverter.GetBytes(0).CopyTo(data, 20);
        BitConverter.GetBytes(0).CopyTo(data, 24);
        BitConverter.GetBytes(0).CopyTo(data, 28);
        BitConverter.GetBytes(1).CopyTo(data, 32);

        return data;
    }

    public static async Task<byte[]> SpawnMineCartridgeAsync(int objectId, Random rnd)
    {
        await Task.Delay((int)(mineSpawnInterval * 1000));
        byte[] data = new byte[Server.OBJECT_DATA_SIZE];

        // 地雷のデータを生成
        // objectId
        BitConverter.GetBytes(objectId).CopyTo(data, 0);
        // objectType
        BitConverter.GetBytes((int)NetworkDataTypes.ObjectType.MINE_CARTIDGE).CopyTo(data, 4);

        // position
        float x, y, z;
        x = (rnd.NextSingle() - 0.5f) * spawnAreaWidth;
        y = heightOffset;
        z = (rnd.NextSingle() - 0.5f) * spawnAreaHeight;
        BitConverter.GetBytes(x).CopyTo(data, 8);
        BitConverter.GetBytes(y).CopyTo(data, 12);
        BitConverter.GetBytes(z).CopyTo(data, 16);

        // rotation
        BitConverter.GetBytes(0).CopyTo(data, 20);
        BitConverter.GetBytes(0).CopyTo(data, 24);
        BitConverter.GetBytes(0).CopyTo(data, 28);
        BitConverter.GetBytes(1).CopyTo(data, 32);

        return data;
    }
}
