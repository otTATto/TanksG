using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class IDDisplayer : MonoBehaviour
{
    public TextMeshProUGUI idText;
    private int playerID = 0;
    public static IDDisplayer Instance { get; private set; }
    private string playerName;
    private string fetchURL = "http://localhost/getplayerinformation.php";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void SetPlayerIDandName(int id, string name)
    {
        playerID = id;
        playerName = name;

    }
    public void SetPlayerName(string name)
    {
        playerName = name;

    }
    public int GetPlayerID()
    {
        return playerID;
    }
    public void GetPlayerNameData()
    {
        StartCoroutine(GetPlayerName(playerID));
    }

    void Update()
    {
        if (idText != null && playerID == 0) idText.text = "New Player";
        else idText.text = "Player ID: " + playerID.ToString() + "Player Name: " + playerName;
    }
    IEnumerator GetPlayerName(int id)
    {
        // POSTデータの準備
        WWWForm form = new WWWForm();
        form.AddField("id", id);

        // リクエスト送信
        using (UnityWebRequest www = UnityWebRequest.Post(fetchURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;

                if (response == "Player not found")
                {
                    Debug.Log("Player not found with ID: " + id);
                }
                else
                {
                    SetPlayerName(response);
                    Debug.Log("Player Name: " + response);
                }
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }

}
