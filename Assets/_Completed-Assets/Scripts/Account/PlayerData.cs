using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class PlayerData
{
    public int id;
    public string playername;
    public int ranking;
    public float winrate;
    public int wincount;
    public int losecount;
}

[System.Serializable]
public class PlayerListWrapper
{
    public bool success;
    public PlayerData[] playerDatas;
    public PlayerData currentPlayer;
    public bool isRankUp;
}
