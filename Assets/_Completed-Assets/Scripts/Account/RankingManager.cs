using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class RankingManager : MonoBehaviour
{
    public GameObject RankingBorder;
    public Transform RankingPoint;
    private string updateAndGetRankingURL = "http://localhost/updateranking.php";
    private IDDisplayer iDDisplayer;





    public void UpdateScoreAndFetchRanking(bool isPlayerWin)
    {
        iDDisplayer = IDDisplayer.Instance.GetComponent<IDDisplayer>();
        if (iDDisplayer != null)
        {
            int result = isPlayerWin ? 1 : 0;
            StartCoroutine(UpdateAndFetch(iDDisplayer.GetPlayerID(), result));
        }
    }

    IEnumerator UpdateAndFetch(int playerId, int isPlayerWin)
    {
        Debug.Log(" UpdateAndFetch");
        WWWForm form = new WWWForm();
        form.AddField("id", playerId);
        form.AddField("ifplayerwin", isPlayerWin);

        using (UnityWebRequest www = UnityWebRequest.Post(updateAndGetRankingURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                Debug.Log("Received JSON: " + json);
                PlayerListWrapper wrapper = JsonUtility.FromJson<PlayerListWrapper>(json);
                List<Player> players = wrapper.players;
                DisplayRanking(players);
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }

    IEnumerator UpdateOnly(int playerId)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", playerId);

        using (UnityWebRequest www = UnityWebRequest.Post(updateAndGetRankingURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                Debug.Log("Received JSON: " + json);
                PlayerListWrapper wrapper = JsonUtility.FromJson<PlayerListWrapper>(json);
                List<Player> players = wrapper.players;
                DisplayRanking(players);
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }

    void DisplayRanking(List<Player> players)
    {
        foreach (var player in players)
        {
            GameObject rankingInstance = Instantiate(RankingBorder, RankingPoint);
            TMP_Text tmpText = rankingInstance.GetComponentInChildren<TMP_Text>();

            if (tmpText == null)
            {
                Debug.Log("Not found Text");
            }
            else
            {
                tmpText.text = "Ranking:" + player.id + " playername:" + player.playername +
                               " Winrate:" + player.winrate + "%" +
                               " Win:" + player.wincount + " lose:" + player.losecount;
            }
        }
    }
}
