using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using TMPro.Examples;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class UserReistration : MonoBehaviour
{
    private IDDisplayer iDDisplayer;
    public TMP_InputField playernameInput;
    private const string BASE_URL = "127.0.0.1";
    private const string PORT = "8000";
    private const string API_PATH = "api/gameusers/update-name";
    private string userReistrationURL => $"http://{BASE_URL}:{PORT}/{API_PATH}";
    public TextMeshProUGUI errorTX;
    public TextMeshProUGUI informationTX;
    private bool isPlayerExsist = true;
    private int playerID;


    void Start()
    {
        iDDisplayer = IDDisplayer.Instance.GetComponent<IDDisplayer>();
        playernameInput.onValueChanged.AddListener(ValidateInput);
        errorTX.text = "";
        informationTX.text = "";
        if (iDDisplayer != null)
        {
                playerID = iDDisplayer.GetPlayerID();
        }
    }
    public void BacktoTitle()
    {
        SceneManager.LoadScene(SceneNames.HomeScene);
    }

    void ValidateInput(string input)
    {
        // 正規表現で無効な文字を除去
        string sanitizedInput = Regex.Replace(input, @"[^\w\u4E00-\u9FFF\u3040-\u30FF\uAC00-\uD7AF]", "");

        // 長さをチェック（3〜15文字）
        if (sanitizedInput.Length > 15)
        {
            sanitizedInput = sanitizedInput.Substring(0, 15);
        }

        // 修正した文字列を反映
        playernameInput.text = sanitizedInput;

        // エラー表示（3文字未満の場合）
        if (sanitizedInput.Length < 3)
        {
            errorTX.text = "More than 3 charactors please";
        }
        else
        {
            errorTX.text = "";
        }
    }

    public void OnNameChangeNameBottonPress()
    {
        if (playernameInput.text.Length < 3)
        {
                errorTX.text = "More than 3 charactors please";
        }
        else
        {
            
        
        if (isPlayerExsist) StartCoroutine(ChangeName());
        }
    }

    IEnumerator ChangeName() 
    {
        WWWForm form = new WWWForm();
        form.AddField("id", playerID);
        form.AddField("playername", playernameInput.text);

        Debug.Log($"Sending request to: {userReistrationURL}");
        Debug.Log($"Data: id={playerID}, name={playernameInput.text}");

        using (UnityWebRequest www = UnityWebRequest.Post(userReistrationURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                Debug.Log($"Response: {response}");
                
                try
                {
                    var responseData = JsonUtility.FromJson<ChangeNameResponse>(response);
                    if (responseData.success)
                    {
                        informationTX.text = "Name change succeeded";
                        iDDisplayer.SetPlayerName(playernameInput.text);
                    }
                    else
                    {
                        errorTX.text = responseData.error ?? "Name change failed";
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSON parse error: {e.Message}");
                    errorTX.text = "Error processing response";
                }
            }
            else
            {
                Debug.LogError($"Error: {www.error}");
                Debug.LogError($"Response Code: {www.responseCode}");
                Debug.LogError($"Response Text: {www.downloadHandler.text}");
                errorTX.text = "Connection error";
            }
        }
    }

    [System.Serializable]
    private class ChangeNameResponse
    {
        public bool success;
        public string error;
    }

}
