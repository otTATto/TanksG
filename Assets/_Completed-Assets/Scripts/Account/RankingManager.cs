using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RankingManager : MonoBehaviour
{
    public GameObject rankingBorder;
    public GameObject playerRankingBorder;
    public GameObject playerRankingBorderUp;
    public Transform rankingPoint;
    public Transform playerRankingPoint; 
    private const string BASE_URL = "127.0.0.1";
    private const string PORT = "8000";
    private const string API_PATH = "api/updateranking";
    private const string GET_PLAYER_RANKING_PATH = "api/getplayerranking";
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

    public void ToTitle() 
    {
        SceneManager.LoadScene(SceneNames.Login);
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
                        DisplayRanking(playerDataList, wrapper);
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
                    DisplayRanking(playerDataList, wrapper);
                }
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }

    void DisplayRanking(List<PlayerData> playerDatas, PlayerListWrapper wrapper)
    {
        foreach (Transform child in rankingPoint)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in playerRankingPoint)
        {
            Destroy(child.gameObject);
        }

        // トップ10のランキング表示
        foreach (var playerData in playerDatas)
        {   
            GameObject rankingInstance = Instantiate(rankingBorder, rankingPoint);
            TMP_Text tmpText = rankingInstance.GetComponentInChildren<TMP_Text>();

            if (tmpText != null)
            {
                tmpText.text = $"#{playerData.ranking} " +
                $"id:{playerData.id} "+
                              $"{playerData.playername} " +
                              $"Winrate: {playerData.winrate:F1}% " +
                              $"(W:{playerData.wincount} L:{playerData.losecount})";
            }
        }

        // 現在のプレイヤーのランキング表示
        if (wrapper.currentPlayer != null)
        {
            Image imageComponent = rankingBorder.GetComponent<Image>();
            imageComponent.color = new Color32(255, 0, 0, 255);
            GameObject playerRankingInstance = Instantiate(rankingBorder, playerRankingPoint);
            TMP_Text playerTmpText = playerRankingInstance.GetComponentInChildren<TMP_Text>();

            if (playerTmpText != null)
            {
                if(wrapper.isRankUp)
                {
                    Instantiate(playerRankingBorderUp, 
                        new Vector3(-270f, playerRankingPoint.position.y, playerRankingPoint.position.z),
                        Quaternion.Euler(0f, 0f, 90f), playerRankingPoint);
                }
                if(wrapper.currentPlayer.ranking > 10 || wrapper.currentPlayer.wincount + wrapper.currentPlayer.losecount < 10)
                {
                    playerTmpText.text = $"#OUT " +
                                    $"id:{wrapper.currentPlayer.id} "+
                                      $"{wrapper.currentPlayer.playername} " +
                                      $"Winrate: {wrapper.currentPlayer.winrate:F1}% " +
                                      $"(W:{wrapper.currentPlayer.wincount} L:{wrapper.currentPlayer.losecount})";
                }
                else{
                    playerTmpText.text = $"#{wrapper.currentPlayer.ranking} " +
                                    $"id:{wrapper.currentPlayer.id} "+
                                      $"{wrapper.currentPlayer.playername} " +
                                      $"Winrate: {wrapper.currentPlayer.winrate:F1}% " +
                                      $"(W:{wrapper.currentPlayer.wincount} L:{wrapper.currentPlayer.losecount})";
                }
            }
        }
    }
}               
