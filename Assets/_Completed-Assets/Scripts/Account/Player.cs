using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class Player
{
    public int id;
    public string playername;
    public int winrate;
    public int wincount;
    public int losecount;
}

[System.Serializable]
public class PlayerListWrapper
{
    public List<Player> players;
}
