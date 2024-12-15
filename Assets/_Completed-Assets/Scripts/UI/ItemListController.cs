// 「Item List」に適切にアイテムを表示するためのスクリプト

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class ItemListController : MonoBehaviour
{
    public GameObject itemInfoPrefab;               // アイテムのPrefab
    public Button itemListButton;                   // アイテムリストボタン
    public GameObject itemListContent;              // アイテムリストの親オブジェクト
    public TextMeshProUGUI emptyMessageArea;        // アイテムがない場合に表示するメッセージ
    public GameObject itemUseDialog;                // アイテム使用ダイアログ
    public Button itemUseYesButton;                 // アイテム使用ダイアログの「はい」ボタン
    public GameObject itemCannotUseDialog;          // アイテム使用不可ダイアログ
    public GameObject itemUseSuccessDialog;         // アイテム使用成功ダイアログ
    public TextMeshProUGUI itemNameText;            // 使用中のアイテム名
    public TextMeshProUGUI itemIconText;            // 使用中のアイテムアイコンのテキスト
    public Image itemIconImage;                     // 使用中のアイテムアイコンの画像
    public GameObject staminaArea;                  // スタミナ表示エリア

    private List<Item> itemList = new List<Item>(); // アイテムリスト
    private int currentStamina;                     // 現在のスタミナ
    private IDDisplayer iDDisplayer;                //ユーザーIDのインスタンス

    // API URL
    private string gameUserItemApiURL = "http://localhost:8000/api/game-users/{0}/items";
    private string gameUserItemUseApiURL = "http://localhost:8000/api/game-users/{0}/items/{1}/use";
    private string gameUserStaminaApiURL = "http://localhost:8000/api/game-users/{0}/stamina";

    [System.Serializable]
    public class Item
    {
        public int id;              // アイテムID
        public string name;         // アイテム名
        public string description;  // アイテムの説明
        public string type;         // アイテムのタイプ
        public int quantity;        // アイテム量
    }

    private void Start()
    {
        iDDisplayer = IDDisplayer.Instance.GetComponent<IDDisplayer>();
        // ユーザーのアイテムをロードして表示
        LoadUserItems();

        // 現在使用中のアイテムIDを取得
        int currentItemId = ItemManager.Instance.CurrentItemID;
        if (currentItemId != -1)
        {
            // アイテムIDからアイテム名を取得
            string itemName = GetItemName(currentItemId);
            // 使用中のアイテム名を表示
            itemNameText.text = itemName;
            // 使用中のアイテムアイコンを表示
            string iconText = itemName.Substring(0, 2);
            itemIconText.text = iconText;
            // 使用中のアイテムアイコンの画像を表示
            itemIconImage.color = new Color32(59, 118, 255, 255);
        } else {
            itemNameText.text = "使用中のアイテムはありません";
        }

        // スタミナを取得してから表示
        StartCoroutine(GetStaminaAndShow());
    }

    // アイテムリストボタンがクリックされたときの処理
    public void OnItemListButtonClick()
    {
        // アイテムリストをロードして表示
        LoadUserItems();
    }

    // アイテムIDからアイテム名を取得
    private string GetItemName(int itemId)
    {
        // アイテムリストの各アイテムから，指定のアイテムIDに一致するアイテム名を取得
        foreach (var item in itemList)
        {
            if (item.id == itemId)
            {
                return item.name;
            }
        }
        return "アイテムが見つかりません";
    }

    // アイテムIDからアイテムの説明を取得
    public string GetItemDescription(int itemId)
    {
        // アイテムリストの各アイテムから，指定のアイテムIDに一致するアイテムの説明を取得
        foreach (var item in itemList)
        {
            if (item.id == itemId)
            {
                return item.description;
            }
        }
        return "アイテムの説明がありません";
    }

    // アイテムIDからアイテムのタイプを取得
    public string GetItemType(int itemId)
    {
        // アイテムリストの各アイテムから，指定のアイテムIDに一致するアイテムのタイプを取得
        foreach (var item in itemList)
        {
            if (item.id == itemId)
            {
                return item.type;
            }
        }
        return "instant";
    }
    
    // ユーザーのアイテムをロードして表示
    public void LoadUserItems ()
    {
        // ユーザーIDを取得
        int userId = iDDisplayer.GetPlayerID();
        Debug.Log($"get User ID: {userId}");

        // ユーザーのアイテムを取得
        StartCoroutine(GetUserItems(userId));
    }

    // ユーザーのアイテムを取得
    private IEnumerator GetUserItems(int userId)
    {
        // APIのURLを作成
        string url = string.Format(gameUserItemApiURL, userId);
        UnityWebRequest request = UnityWebRequest.Get(url);

        // リクエストを送信
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
        // JSONのレスポンスを処理
        string jsonResponse = request.downloadHandler.text;
        Debug.Log($"JSON Response: {jsonResponse}"); // JSONレスポンスをデバッグログに出力

            itemList = JsonConvert.DeserializeObject<List<Item>>(jsonResponse);

            // すでに表示されているアイテム名を削除
            ClearItemList();

            // アイテム数が1個以上の場合，EmptyMessage を非表示にする
            if (itemList.Count > 0)
            {
                // EmptyMessage を非表示にする
                emptyMessageArea.gameObject.SetActive(false);
            }

            // アイテムリストを表示
            foreach (var item in itemList)
            {
                // アイテム情報プレハブをインスタンス化
                GameObject itemInfo = Instantiate(itemInfoPrefab, itemListContent.transform);

                // アイテム名を表示
                itemInfo.transform.Find("ItemInfoCanvas/ItemNameText").GetComponent<TextMeshProUGUI>().text = item.name;

                // アイテム名の先頭2文字を取得
                string iconText = item.name.Substring(0, 2);
                // アイテムアイコンを表示
                itemInfo.transform.Find("ItemInfoCanvas/ItemIconText").GetComponent<TextMeshProUGUI>().text = iconText;

                // アイテム量を表示
                itemInfo.transform.Find("ItemInfoCanvas/ItemQuantityText").GetComponent<TextMeshProUGUI>().text = item.quantity.ToString();

                // 使用ボタンを取得
                Button useButton = itemInfo.transform.Find("ItemInfoCanvas/ItemUseButton").GetComponent<Button>();
                // 使用ボタンのクリックイベントを設定
                useButton.onClick.AddListener(() => OnUseButtonClick(item.id, item.name));
            }
        }
        else
        {
            Debug.LogError($"API Error: {request.error}");
        }
    }

    // 使用ボタンがクリックされたときの処理
    private void OnUseButtonClick(int itemId, string itemName)
    {
        // アイテム使用ダイアログを表示
        itemUseDialog.SetActive(true);
        // アイテム名を表示
        itemUseDialog.transform.Find("ItemNameText").GetComponent<TextMeshProUGUI>().text = itemName;

        // はいボタンの既存のリスナーを削除
        itemUseYesButton.onClick.RemoveAllListeners();
        // はいボタンのクリックイベントを設定
        itemUseYesButton.onClick.AddListener(() => StartCoroutine(OnYesButtonClick(itemId)));
    }

    // はいボタンがクリックされたときの処理
    private IEnumerator OnYesButtonClick(int itemId)
    {
        Debug.Log($"Yes Button Clicked: {itemId}");

        // アイテム数が1以上であるかどうかを確認
        int itemNum = itemList.Find(item => item.id == itemId).quantity;
        if (itemNum <= 0)
        {
            // アイテムがない場合は，その旨をダイアログで表示
            itemCannotUseDialog.SetActive(true);
            // 詳細を表示
            string detailMessage = "アイテムがありません。";
            itemCannotUseDialog.transform.Find("DetailMessage").GetComponent<TextMeshProUGUI>().text = detailMessage;
            yield break;
        }

        // スタミナ回復アイテムを使おうとしたとき
        if (itemId == 1)
        {
            // スタミナがMAXの場合は使用できない旨を表示
            StartCoroutine(GetStamina());   // スタミナを取得
            if (currentStamina >= 5)
            {
                // アイテム使用不可ダイアログを表示
                itemCannotUseDialog.SetActive(true);
                // 詳細を表示
                string detailMessage = "スタミナはこれ以上回復できません。";
                itemCannotUseDialog.transform.Find("DetailMessage").GetComponent<TextMeshProUGUI>().text = detailMessage;
                yield break;
            }
        }

        // すでに使用中の場合は使用できない旨を表示
        int currentItemId = ItemManager.Instance.CurrentItemID;
        if (currentItemId == itemId)
        {
            // アイテム使用不可ダイアログを表示
            itemCannotUseDialog.SetActive(true);
            // 詳細を表示
            string detailMessage = "既に効果が適用されています。";
            itemCannotUseDialog.transform.Find("DetailMessage").GetComponent<TextMeshProUGUI>().text = detailMessage;
            yield break;
        }

        // アイテムを使用する
        yield return StartCoroutine(UseItem(itemId));

        // アイテム使用ダイアログを非表示
        itemUseDialog.SetActive(false);
    }

    // アイテムを使用するときの処理
    private IEnumerator UseItem(int itemId)
    {
        // ユーザーIDを取得
        int userId = iDDisplayer.GetPlayerID();

        // APIのURLを作成
        string url = string.Format(gameUserItemUseApiURL, userId, itemId);
        Debug.Log($"Use Item URL: {url}");
        UnityWebRequest request = UnityWebRequest.Delete(url);

        // リクエストを送信
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // レスポンスコードを確認
            if (request.responseCode == 204)
            {
                // アイテムを使用した旨を表示
                itemUseSuccessDialog.SetActive(true);
                // アイテム名を表示
                itemUseSuccessDialog.transform.Find("ItemNameText").GetComponent<TextMeshProUGUI>().text = GetItemName(itemId);
                // アイテムの説明を表示
                itemUseSuccessDialog.transform.Find("ItemEffectText").GetComponent<TextMeshProUGUI>().text = GetItemDescription(itemId);

                // アイテムリストを更新
                LoadUserItems();

                // アイテムタイプが duration の場合のみ，使用中のアイテムIDを更新
                if (GetItemType(itemId) == "duration")
                {
                    // 使用中のアイテムIDを更新
                    ItemManager.Instance.SetItemID(itemId);

                    // 使用中のアイテム名を表示
                    itemNameText.text = GetItemName(itemId);

                    // 使用中のアイテムアイコンを表示
                    string iconText = GetItemName(itemId).Substring(0, 2);
                    itemIconText.text = iconText;
                    // 使用中のアイテムアイコンの画像を表示
                    itemIconImage.color = new Color32(59, 118, 255, 255);
                }

                // アイテムに応じて効果を発動
                ApplyItemEffect(itemId);

                // ログ
                Debug.Log("Item used successfully");
            }
            else
            {
                Debug.LogError($"API response code: {request.responseCode}");
                Debug.LogError($"API Error: {request.error}");
            }
        }
        else
        {
            Debug.LogError($"API Error: {request.error}");
        }
    }

    // アイテムリストをクリア
    private void ClearItemList()
    {
        foreach (Transform child in itemListContent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // アイテムに応じて効果を発動
    private void ApplyItemEffect(int itemId)
    {
        // アイテムIDに応じて効果を発動
        switch (itemId)
        {
            case 1:
                // スタミナを回復してから表示
                StartCoroutine(RecoverStaminaAndShow());
                break;
            case 2:
                // 装甲を強化

                break;
            default:
                break;
        }
    }

    // スタミナを取得してから表示するコルーチン
    private IEnumerator GetStaminaAndShow()
    {
        yield return StartCoroutine(GetStamina());
        ShowStamina();
    }

    // スタミナを回復してから表示するコルーチン
    private IEnumerator RecoverStaminaAndShow()
    {
        yield return StartCoroutine(RecoverStamina());
        ShowStamina();
    }

    // スタミナを取得
    private IEnumerator GetStamina()
    {
        // api URL
        int userId = iDDisplayer.GetPlayerID();
        string apiURL = gameUserStaminaApiURL + "/get";
        string url = string.Format(apiURL, userId);

        // リクエストを送信
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // JSONのレスポンスを処理
            string jsonResponse = request.downloadHandler.text;
            Debug.Log($"JSON Response: {jsonResponse}"); // JSONレスポンスをデバッグログに出力

            // スタミナを取得
            currentStamina = JsonConvert.DeserializeObject<int>(jsonResponse);
            Debug.Log($"Current Stamina: {currentStamina}");
        }
        else
        {
            Debug.LogError($"API Error: {request.error}");
        }
    }

    // スタミナを回復
    private IEnumerator RecoverStamina()
    {
        // api URL
        int userId = iDDisplayer.GetPlayerID();
        string apiURL = gameUserStaminaApiURL + "/recover";
        string url = string.Format(apiURL, userId);

        // リクエストを送信
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // JSONのレスポンスを処理
            string jsonResponse = request.downloadHandler.text;
            Debug.Log($"JSON Response: {jsonResponse}"); // JSONレスポンスをデバッグログに出力

            // スタミナを取得
            currentStamina = JsonConvert.DeserializeObject<int>(jsonResponse);
            Debug.Log($"Current Stamina: {currentStamina}");
        }
        else
        {
            Debug.LogError($"API Error: {request.error}");
        }
    }

    // スタミナを表示
    private void ShowStamina()
    {
        // スタミナありの表示形式
        string staminaText = "●";   // 「●」を有効なスタミナの記号として使用
        // スタミナの数だけ塗りつぶす
        for (int i = 1; i <= currentStamina; i++)
        {
            string num = i.ToString("D2");
            Debug.Log($"StaminaBall_{num}");
            staminaArea.transform.Find($"StaminaBall_{num}").GetComponent<TextMeshProUGUI>().text = staminaText;
            staminaArea.transform.Find($"StaminaBall_{num}").GetComponent<TextMeshProUGUI>().color = new Color32(253, 85, 85, 255);

        }
        // ログ
        Debug.Log($"Show Stamina: {currentStamina}");
    }
}