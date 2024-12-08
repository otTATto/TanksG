using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System;

public class StartButton : MonoBehaviour
{
    public TMP_InputField userIdInputField;  // TMP_InputField（ユーザーIDの入力フィールド）
    public Button startButton;
    public TMP_Text warningText;  // TMP_Text（警告テキスト）
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

    private void Awake()
    {
        iDDisplayer = IDDisplayer.Instance.GetComponent<IDDisplayer>();
        startButton = GetComponent<Button>();
        startButton.onClick.AddListener(OnLoginButtonClicked);
        warningText.gameObject.SetActive(false);  // 初期は警告を非表示
    }

    private void OnLoginButtonClicked()
    {
        //int userId;

        if (string.IsNullOrEmpty(iDDisplayer.GetUserUuid()))
        {
            StartCoroutine(RegisterNewPlayer());
        }
        else
        {
            // ユーザーIDが正しい場合、APIを呼び出して状態を確認
            StartCoroutine(CheckAccountSuspended(iDDisplayer.GetPlayerID()));
        }
        //else
        //{
        //    // ユーザーIDが無効な場合
        //    warningText.text = "Invalid User ID";
        //    warningText.gameObject.SetActive(true);
        //}
    }

    private IEnumerator CheckAccountSuspended(int userId)
    {
        string url = $"http://localhost:8000/api/game-users/{userId}/check-suspended";
        UnityWebRequest request = UnityWebRequest.Get(url);

        // リクエストを送信
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // JSONのレスポンスを処理
            string jsonResponse = request.downloadHandler.text;
            var response = JsonUtility.FromJson<AccountSuspensionResponse>(jsonResponse);

            if (response.is_suspended)
            {
                // アカウント停止中のユーザー
                warningText.text = "Your account is suspended!";
                warningText.gameObject.SetActive(true);
            }
            else
            {
                // ログイン成功、ホーム画面に遷移
                SceneManager.LoadScene(SceneNames.HomeScene);
            }
        }
        else
        {
            // 通信エラーなど
            warningText.text = "Error connecting to the server";
            warningText.gameObject.SetActive(true);
        }
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

    // アカウント停止状態のレスポンス用クラス
    [System.Serializable]
    public class AccountSuspensionResponse
    {
        public bool is_suspended;
    }


}
