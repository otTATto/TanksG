using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.IO;

public class IDDisplayer : MonoBehaviour
{
    public TextMeshProUGUI idText;
    private int playerID = 0;
    public static IDDisplayer Instance { get; private set; }
    private string playerName;
    private const string BASE_URL = "127.0.0.1";
    private const string PORT = "8000";
    private const string API_PATH = "api/getplayerinformation";
    private string fetchURL => $"http://{BASE_URL}:{PORT}/{API_PATH}";
    private const string USER_DATA_FILENAME = "user_data.json";
    private string userUuid;

    


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            Debug.Log($"Constructed URL: {fetchURL}");
            
            if (LoadLocalUuid())
            {
                Debug.Log($"Loaded UUID: {userUuid}");
                StartCoroutine(GetPlayerIDData(userUuid));
            }
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    private string GetUuidFilePath()
    {
        return Path.Combine(Application.persistentDataPath, USER_DATA_FILENAME);
    }

    private bool LoadLocalUuid()
    {
        string filePath = GetUuidFilePath();
        if (File.Exists(filePath))
        {
            try
            {
                userUuid = File.ReadAllText(filePath).Trim();
                return !string.IsNullOrEmpty(userUuid);
            }
            catch (Exception e)
            {
                Debug.LogError($"UUID読み込みエラー: {e.Message}");
                return false;
            }
        }
        return false;
    }

    private void SaveUuid(string uuid)
    {
        try
        {
            File.WriteAllText(GetUuidFilePath(), uuid);
            userUuid = uuid;
        }
        catch (Exception e)
        {
            Debug.LogError($"UUID保存エラー: {e.Message}");
        }
    }

    public string GetUserUuid()
    {
        return userUuid;
    }

    public void SetUserUuid(string uuid)
    {
        userUuid = uuid;
        SaveUuid(uuid);
        StartCoroutine(GetPlayerIDData(userUuid));
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

    void Update()
    {
        if (idText != null && !LoadLocalUuid()) idText.text = "New Player";
        else idText.text = "Player ID: " + playerID.ToString() + " " + "Player Name: " + playerName;
    }
     IEnumerator GetPlayerIDData(string uuid)
    {
        if (string.IsNullOrEmpty(uuid))
        {
            Debug.LogError("UUID is null or empty");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("uuid", uuid);

        Debug.Log($"Sending request to: {fetchURL} with UUID: {uuid}");

        using (UnityWebRequest www = UnityWebRequest.Post(fetchURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                Debug.Log($"Response: {response}");
                try 
                {
                    PlayerData playerData = JsonUtility.FromJson<PlayerData>(response);
                    SetPlayerIDandName(playerData.id, playerData.playername);
                    Debug.Log($"プレイヤー情報を取得: ID={playerData.id}, Name={playerData.playername}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSONパースエラー: {e.Message}");
                    Debug.LogError($"Response: {www.downloadHandler.text}");
                }
            }
            else
            {
                Debug.LogError($"Error: {www.error}");
                Debug.LogError($"Response Code: {www.responseCode}");
                Debug.LogError($"Response Text: {www.downloadHandler.text}");
            }
        }
    }
}
