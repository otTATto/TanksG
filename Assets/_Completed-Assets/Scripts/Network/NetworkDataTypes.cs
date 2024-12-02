using System;

public class NetworkDataTypes
{
    // データ種類の列挙型
    public enum DataType : byte
    {
        READY = 0x01,
        TANK_ID = 0x10,
        TANK_ID_1 = 0x11,
        TANK_ID_2 = 0x12,
        TANK_POSITION = 0x20,
        TANK_ROTATION = 0x21,
        TURRET_ROTATION = 0x22,
        SHELL_POSITION = 0x30,
        MINE_POSITION = 0x31,
        M_CARTIDGE_POSITION = 0x32,
        S_CARTIDGE_POSITION = 0x33,
        GAME_END = 0x40
    }

    public static byte[] TANK_IDs = new byte[] {
        0x01,
        0x02
    };

    // データ構造の定義
    // [Serializable]
    // public struct PlayerPositionData
    // {
    //     public float X;
    //     public float Y;
    //     public float Z;
    // }

    // [Serializable]
    // public struct PlayerRotationData
    // {
    //     public float X;
    //     public float Y;
    //     public float Z;
    //     public float W;
    // }

    // [Serializable]
    // public struct PlayerActionData
    // {
    //     public int ActionId;
    //     public float Timestamp;
    // }

    // [Serializable]
    // public struct GameStateData
    // {
    //     public int StateId;
    //     public float GameTime;
    // }

    // [Serializable]
    // public struct ChatMessageData
    // {
    //     public int PlayerId;
    //     public string Message;
    // }
} 