using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class StartButton : MonoBehaviour
{
    public TMP_InputField userIdInputField;  // TMP_InputField（ユーザーIDの入力フィールド）
    public Button startButton;
    public TMP_Text warningText;  // TMP_Text（警告テキスト）

    private void Awake()
    {
        startButton = GetComponent<Button>();
        startButton.onClick.AddListener(OnLoginButtonClicked);
        warningText.gameObject.SetActive(false);  // 初期は警告を非表示
    }

    private void OnLoginButtonClicked()
    {
        int userId;
        if (int.TryParse(userIdInputField.text, out userId))
        {
            // ユーザーIDが正しい場合、APIを呼び出して状態を確認
            StartCoroutine(CheckAccountSuspended(userId));
        }
        else
        {
            // ユーザーIDが無効な場合
            warningText.text = "Invalid User ID";
            warningText.gameObject.SetActive(true);
        }
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

    // アカウント停止状態のレスポンス用クラス
    [System.Serializable]
    public class AccountSuspensionResponse
    {
        public bool is_suspended;
    }
}
