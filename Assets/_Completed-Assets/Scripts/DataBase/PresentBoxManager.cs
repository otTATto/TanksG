using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class PresentBoxManager : MonoBehaviour
{
    [SerializeField] private GameObject presentBoxDialog; // プレゼントボックスのダイアログ
    [SerializeField] private GameObject presentListContent; // プレゼントリストの親オブジェクト
    [SerializeField] private GameObject presentItemPrefab; // プレゼントアイテムのPrefab
    [SerializeField] private TextMeshProUGUI emptyMessage; // プレゼントがない場合のメッセージ
    [SerializeField] private Button closeButton; // 閉じるボタン
    [SerializeField] private Button openButton; // 開くボタン

    private int user_id; // 動的に取得できる場合は設定を変更
    private List<PresentData> presents = new List<PresentData>();
    private IDDisplayer iDDisplayer;

    public static class JsonHelper
    {
        public static T[] FromJsonArray<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }

    [System.Serializable]
    public class PresentData
    {
        public int gift_id;          // プレゼントID
        public string item_name;     // アイテム名
        public int quantity;         // 配布数
        public string distributed_at; // 配布日時
    }

    [System.Serializable]
    public class PresentsResponse
    {
        public List<PresentData> presents;
    }

    private void Awake()
    {
        Debug.Log("PresentBoxManager Awake called.");
        closeButton.onClick.AddListener(ClosePresentBox);
        openButton.onClick.AddListener(OpenPresentBox);
        iDDisplayer = IDDisplayer.Instance.GetComponent<IDDisplayer>();
        user_id = iDDisplayer.GetPlayerID();
    }

    // プレゼントBOXを開く
    public void OpenPresentBox()
    {
        Debug.Log("OpenPresentBox called.");
        StartCoroutine(FetchPresentsFromServer(response =>
        {
            if (response != null)
            {
                presents = response.presents ?? new List<PresentData>();
            }
            else
            {
                presents = new List<PresentData>();
                Debug.LogError("Failed to fetch presents or response was null.");
            }

            UpdatePresentBoxUI();
            presentBoxDialog.SetActive(true);
            Debug.Log("PresentBoxDialog set to active.");
        }));
    }

    // プレゼントBOXのUIを更新
    private void UpdatePresentBoxUI()
    {
        // リストをクリア
        foreach (Transform child in presentListContent.transform)
        {
            Destroy(child.gameObject);
        }

        // プレゼントがない場合
        if (presents.Count == 0)
        {
            emptyMessage.gameObject.SetActive(true);
            return;
        }
        emptyMessage.gameObject.SetActive(false);

        // プレゼントをリストに表示
        foreach (var present in presents)
        {
            GameObject item = Instantiate(presentItemPrefab, presentListContent.transform);

            // 各UI要素にデータを設定
            item.transform.Find("Canvas/Name").GetComponent<TextMeshProUGUI>().text = present.item_name;
            item.transform.Find("Canvas/Number").GetComponent<TextMeshProUGUI>().text = $"×{present.quantity}";
            item.transform.Find("Canvas/Date").GetComponent<TextMeshProUGUI>().text = present.distributed_at;

            // 「受け取る」ボタンの設定
            Button receiveButton = item.transform.Find("Canvas/Button").GetComponent<Button>();
            receiveButton.onClick.AddListener(() => ReceivePresent(present.gift_id));
        }
    }

    // アイテム受け取り処理
    private void ReceivePresent(int presentId)
    {
        Debug.Log($"ReceivePresent called for Present ID: {presentId}");
        StartCoroutine(SendReceiveRequest(presentId, () =>
        {
            presents.RemoveAll(p => p.gift_id == presentId);
            UpdatePresentBoxUI();
        }));
    }

    private IEnumerator SendReceiveRequest(int presentId, System.Action onComplete)
    {
        Debug.Log($"Sending receive request for Present ID: {presentId}");
        string apiUrl = $"http://localhost:8000/api/presents/received/{user_id}/{presentId}";

        UnityWebRequest request = UnityWebRequest.PostWwwForm(apiUrl, string.Empty); // POSTリクエストを送信
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Failed to receive item: {request.error}");
        }
        else
        {
            Debug.Log("Item received successfully.");
            onComplete?.Invoke();
        }
    }

    // プレゼントBOXを閉じる
    public void ClosePresentBox()
    {
        Debug.Log("ClosePresentBox called.");
        presentBoxDialog.SetActive(false);
    }

    // サーバーからプレゼントを取得 (型指定されたコルーチン)
private IEnumerator FetchPresentsFromServer(System.Action<PresentsResponse> onComplete)
{
    string apiUrl = $"http://localhost:8000/api/presents/{user_id}";

    UnityWebRequest request = UnityWebRequest.Get(apiUrl);
    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    {
        Debug.LogError($"Failed to fetch presents: {request.error}");
        onComplete?.Invoke(null);
    }
    else
    {
        string jsonResponse = request.downloadHandler.text;
        Debug.Log($"Server response: {jsonResponse}");

        try
        {
            // サーバーからのレスポンスが配列である場合
            // プレゼントデータを直接配列として取得
            PresentData[] presentsArray = JsonHelper.FromJsonArray<PresentData>(jsonResponse);

            // 配列をListに変換
            List<PresentData> presentsList = new List<PresentData>(presentsArray);

            if (presentsList != null && presentsList.Count > 0)
            {
                // プレゼントが存在する場合
                PresentsResponse response = new PresentsResponse { presents = presentsList };
                onComplete?.Invoke(response);
            }
            else
            {
                // プレゼントがない場合
                Debug.Log("No presents found in response.");
                onComplete?.Invoke(new PresentsResponse { presents = new List<PresentData>() });
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error parsing presents: {ex.Message}");
            onComplete?.Invoke(null);
        }
    }
}


    // アイコンのロード（仮実装: URLからスプライトを作成）
    private Sprite LoadIcon(string url)
    {
        Debug.Log($"Loading icon from URL: {url}");
        // TODO: 実際の画像読み込み処理
        return null;
    }
}
