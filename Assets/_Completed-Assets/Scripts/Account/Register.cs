using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using TMPro.Examples;
using UnityEngine.SceneManagement;

public class Register : MonoBehaviour
{
    private IDDisplayer iDDisplayer;
    private const string BASE_URL = "localhost";
    private const string PORT = "8000";
    private const string API_PATH = "api/gameusers";
    private string registerURL => $"http://{BASE_URL}:{PORT}/{API_PATH}";
    
    [System.Serializable]
    private class RegisterResponse
    {
        public bool success;
        public int id;
        public string uuid;
        public string error;
    }

    void Start()
    {
        iDDisplayer = IDDisplayer.Instance.GetComponent<IDDisplayer>();
    }

    public void OnclickStart() 
    {
        if (iDDisplayer != null)
        {
            if (string.IsNullOrEmpty(iDDisplayer.GetUserUuid()))
            {
                StartCoroutine(RegisterNewPlayer());
            }
            SceneManager.LoadScene(SceneNames.Login);     
        }
    }
    
    public void ToExit()
    {
        Application.Quit();
    }

    IEnumerator RegisterNewPlayer()
    {
        Debug.Log($"Attempting to connect to: {registerURL}");

        WWWForm form = new WWWForm();
        form.AddField("name", "NONAME");

        using (UnityWebRequest www = UnityWebRequest.Post(registerURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                Debug.Log($"Response: {response}");
                try
                {
                    RegisterResponse registerData = JsonUtility.FromJson<RegisterResponse>(response);
                    
                    if (registerData.success)
                    {
                        iDDisplayer.SetUserUuid(registerData.uuid);
                        iDDisplayer.SetPlayerIDandName(registerData.id, "NONAME");
                        
                        Debug.Log($"登録成功！ ID: {registerData.id}, UUID: {registerData.uuid}");
                        SceneManager.LoadScene(SceneNames.HomeScene);
                    }
                    else
                    {
                        Debug.LogError($"登録エラー: {registerData.error}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSONパースエラー: {e.Message}");
                    Debug.LogError($"Response: {response}");
                }
            }
            else
            {
                Debug.LogError($"通信エラー: {www.error}");
                Debug.LogError($"Response Code: {www.responseCode}");
                Debug.LogError($"Response Text: {www.downloadHandler.text}");
            }
        }
    }
}