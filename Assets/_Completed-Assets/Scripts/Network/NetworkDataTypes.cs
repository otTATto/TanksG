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
        GAME_END = 0x4,
        SYNC_OBJECT = 0x5,
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
    public static byte[] EncodeSyncObjectData(SyncObjectData data)
    {
        byte[] bytes = new byte[48];
        // objectId
        BitConverter.GetBytes(data.objectId).CopyTo(bytes, 0);
        // objectType
        BitConverter.GetBytes(data.objectType).CopyTo(bytes, 4);

        // position
        BitConverter.GetBytes(data.position.x).CopyTo(bytes, 8);
        BitConverter.GetBytes(data.position.y).CopyTo(bytes, 12);
        BitConverter.GetBytes(data.position.z).CopyTo(bytes, 16);
        // rotation
        BitConverter.GetBytes(data.rotation.x).CopyTo(bytes, 20);
        BitConverter.GetBytes(data.rotation.y).CopyTo(bytes, 24);
        BitConverter.GetBytes(data.rotation.z).CopyTo(bytes, 28);
        BitConverter.GetBytes(data.rotation.w).CopyTo(bytes, 32);
        // velocity
        BitConverter.GetBytes(data.velocity.x).CopyTo(bytes, 36);
        BitConverter.GetBytes(data.velocity.y).CopyTo(bytes, 40);
        BitConverter.GetBytes(data.velocity.z).CopyTo(bytes, 44);
        return bytes;
    }

    // オブジェクトの同期データをデコード (48 bytes)
    public static SyncObjectData DecodeSyncObjectData(byte[] bytes)
    {
        UnityEngine.Debug.Log($"DecodeSyncObjectData: bytes Length: {bytes.Length}");
        SyncObjectData data = new SyncObjectData();
        data.objectId = BitConverter.ToInt32(bytes, 0);
        data.objectType = BitConverter.ToInt32(bytes, 4);
        UnityEngine.Debug.Log($"DecodeSyncObjectData: data.objectId: {data.objectId}, data.objectType: {data.objectType}");

        // position
        float x, y, z;
        x = BitConverter.ToSingle(bytes, 8);
        y = BitConverter.ToSingle(bytes, 12);
        z = BitConverter.ToSingle(bytes, 16);
        data.position = new Vector3(x, y, z);
        UnityEngine.Debug.Log($"DecodeSyncObjectData: data.position: {data.position}");

        // rotation
        float rx, ry, rz, rw;
        rx = BitConverter.ToSingle(bytes, 20);
        ry = BitConverter.ToSingle(bytes, 24);
        rz = BitConverter.ToSingle(bytes, 28);
        rw = BitConverter.ToSingle(bytes, 32);
        data.rotation = new Quaternion(rx, ry, rz, rw);
        UnityEngine.Debug.Log($"DecodeSyncObjectData: data.rotation: {data.rotation}");

        // velocity
        float vx, vy, vz;
        vx = BitConverter.ToSingle(bytes, 36);
        vy = BitConverter.ToSingle(bytes, 40);
        vz = BitConverter.ToSingle(bytes, 44);
        data.velocity = new Vector3(vx, vy, vz);
        return data;
    }
} 