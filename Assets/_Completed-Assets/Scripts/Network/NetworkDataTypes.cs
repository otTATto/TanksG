using System;
using UnityEngine;

public class NetworkDataTypes
{
    // データ種類の列挙型
    public enum DataType : byte
    {
        PLAYER_ID = 0x0,
        GET_PLAYER_ID = 0x1,
        READY = 0x2,
        GAME_START = 0x3,
        GAME_STOP = 0x4,
        GAME_END = 0x5,
        SYNC_OBJECT = 0x6,
        DESTROY_OBJECT = 0x7,
    }

    // オブジェクト種類の列挙型
    // 必ずこの順番で定義すること
    public enum ObjectType : int
    {
        TANK = 0,
        SHELL = 1,
        MINE = 2,
        SHELL_CARTIDGE = 3,
        MINE_CARTIDGE = 4,
    }

    public struct SyncObjectData
    {
        public int objectId;
        public int objectType;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
    }

    // オブジェクトの同期データをエンコード (48 bytes)
    public static byte[] EncodeSyncObjectData(int objectId, int objectType, Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        byte[] bytes = new byte[48];
        // objectId
        BitConverter.GetBytes(objectId).CopyTo(bytes, 0);
        // objectType
        BitConverter.GetBytes(objectType).CopyTo(bytes, 4);

        // position
        BitConverter.GetBytes(position.x).CopyTo(bytes, 8);
        BitConverter.GetBytes(position.y).CopyTo(bytes, 12);
        BitConverter.GetBytes(position.z).CopyTo(bytes, 16);
        // rotation
        BitConverter.GetBytes(rotation.x).CopyTo(bytes, 20);
        BitConverter.GetBytes(rotation.y).CopyTo(bytes, 24);
        BitConverter.GetBytes(rotation.z).CopyTo(bytes, 28);
        BitConverter.GetBytes(rotation.w).CopyTo(bytes, 32);
        // velocity
        BitConverter.GetBytes(velocity.x).CopyTo(bytes, 36);
        BitConverter.GetBytes(velocity.y).CopyTo(bytes, 40);
        BitConverter.GetBytes(velocity.z).CopyTo(bytes, 44);
        return bytes;
    }

    // オブジェクトの同期データをデコード (48 bytes)
    public static SyncObjectData DecodeSyncObjectData(byte[] bytes)
    {
        SyncObjectData data = new SyncObjectData();
        data.objectId = BitConverter.ToInt32(bytes, 0);
        data.objectType = BitConverter.ToInt32(bytes, 4);

        // position
        float x, y, z;
        x = BitConverter.ToSingle(bytes, 8);
        y = BitConverter.ToSingle(bytes, 12);
        z = BitConverter.ToSingle(bytes, 16);
        data.position = new Vector3(x, y, z);

        // rotation
        float rx, ry, rz, rw;
        rx = BitConverter.ToSingle(bytes, 20);
        ry = BitConverter.ToSingle(bytes, 24);
        rz = BitConverter.ToSingle(bytes, 28);
        rw = BitConverter.ToSingle(bytes, 32);
        data.rotation = new Quaternion(rx, ry, rz, rw);

        // velocity
        float vx, vy, vz;
        vx = BitConverter.ToSingle(bytes, 36);
        vy = BitConverter.ToSingle(bytes, 40);
        vz = BitConverter.ToSingle(bytes, 44);
        data.velocity = new Vector3(vx, vy, vz);
        return data;
    }
} 