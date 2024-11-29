using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class RankingManager : MonoBehaviour
{
    public GameObject RankingBorder;
    public Transform RankingPoint;
    private const string BASE_URL = "127.0.0.1";
    private const string PORT = "8000";
    private const string API_PATH = "api/updateranking";
    private string updateAndGetRankingURL => $"http://{BASE_URL}:{PORT}/{API_PATH}";
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
        Debug.Log("UpdateAndFetch");
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
                try 
                {
                    PlayerListWrapper wrapper = JsonUtility.FromJson<PlayerListWrapper>(json);
                    if (wrapper != null && wrapper.playerDatas != null)
                    {
                        List<PlayerData> playerDataList = new List<PlayerData>(wrapper.playerDatas);
                        DisplayRanking(playerDataList);
                    }
                    else
                    {
                        Debug.LogError("Failed to parse JSON response");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"JSON Parse Error: {e.Message}");
                    Debug.LogError($"JSON content: {json}");
                }
            }
            else
            {
                Debug.LogError($"Error: {www.error}");
                Debug.LogError($"Response Code: {www.responseCode}");
                Debug.LogError($"Response: {www.downloadHandler.text}");
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
                if (wrapper != null && wrapper.playerDatas != null)
                {
                    List<PlayerData> playerDataList = new List<PlayerData>(wrapper.playerDatas);
                    DisplayRanking(playerDataList);
                }
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }

    void DisplayRanking(List<PlayerData> playerDatas)
    {
        foreach (Transform child in RankingPoint)
        {
            Destroy(child.gameObject);
        }

        foreach (var playerData in playerDatas)
        {
            GameObject rankingInstance = Instantiate(RankingBorder, RankingPoint);
            TMP_Text tmpText = rankingInstance.GetComponentInChildren<TMP_Text>();

            if (tmpText == null)
            {
                Debug.Log("Not found Text");
            }
            else
            {
                tmpText.text = $"#{playerData.ranking} " +
                              $"{playerData.playername} " +
                              $"Winrate: {playerData.winrate:F1}% " +
                              $"(W:{playerData.wincount} L:{playerData.losecount})";
            }
        }
    }
}
