using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System;

public class StartButton : MonoBehaviour
{
    public TMP_InputField userIdInputField;  // ユーザーID入力フィールド
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

    private void Start()
    {
        iDDisplayer = IDDisplayer.Instance.GetComponent<IDDisplayer>();
        // スタートボタンクリック時の処理を登録
        startButton = GetComponent<Button>();
        startButton.onClick.AddListener(() => {
            // ログインの処理
            OnLoginButtonClicked(); 
            // アイテムIDをリセット
            ItemManager.Instance.SetItemID(-1);
        });
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

    // アカウント停止確認コルーチン
    private IEnumerator CheckAccountSuspended(int userId)
    {
        string url = $"http://127.0.0.1:8000/api/game-users/{userId}/check-suspended";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            var response = JsonUtility.FromJson<AccountSuspensionResponse>(jsonResponse);

            if (response.is_suspended)
            {
                // アカウントが停止中
                warningText.text = "Your account is suspended!";
                warningText.gameObject.SetActive(true);
            }
            else
            {
                // アカウント停止でなければログイン状況確認
                StartCoroutine(CheckLoginStatus(userId));
            }
        }
        else
        {
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
                        StartCoroutine(ReceiveBonus(registerData.id));
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
    // ログイン状況確認コルーチン
    private IEnumerator CheckLoginStatus(int userId)
    {
        string url = $"http://127.0.0.1:8000/api/login/{userId}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            var response = JsonUtility.FromJson<LoginStatusResponse>(jsonResponse);

            if (response.can_receive_bonus)
            {
                // ログインボーナスを受け取れる場合
                StartCoroutine(ReceiveBonus(userId));
            }
            else
            {
                // 既に受け取り済みまたは対象外ならホーム画面へ
                SceneManager.LoadScene(SceneNames.HomeScene);
            }
        }
        else
        {
            warningText.text = "Error fetching login status.";
            warningText.gameObject.SetActive(true);
        }
    }

    // ログインボーナス受け取りコルーチン
    private IEnumerator ReceiveBonus(int userId)
    {
        string url = $"http://127.0.0.1:8000/api/bonus/receive/{userId}";
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            var response = JsonUtility.FromJson<BonusReceiveResponse>(jsonResponse);

            if (response.message == "Bonus received successfully.")
            {
                // 初回ボーナス取得成功ならログインボーナス画面へ遷移
                SceneManager.LoadScene(SceneNames.LoginBonusScene);
            }
            else
            {
                // 既に受け取り済み等の場合はホーム画面へ
                SceneManager.LoadScene(SceneNames.HomeScene);
            }
        }
        else
        {
            warningText.text = "Error receiving bonus.";
            warningText.gameObject.SetActive(true);
        }
    }

    // アカウント停止状態レスポンス用クラス
    [System.Serializable]
    public class AccountSuspensionResponse
    {
        public bool is_suspended;
    }

    // ログインステータス用レスポンス
    [System.Serializable]
    public class LoginStatusResponse
    {
        public bool can_receive_bonus;
        public int day;
    }

    // ボーナス受け取りレスポンス
    [System.Serializable]
    public class BonusReceiveResponse
    {
        public string message;
        public int item_id;
        public int quantity;
        public int next_day;
    }
}
