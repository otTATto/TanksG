using System;

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
} 