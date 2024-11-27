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
    public int winrate;
    public int wincount;
    public int losecount;
    public bool ranking;
}

[System.Serializable]
public class PlayerListWrapper
{
    public List<PlayerData> playerDatas;
}
