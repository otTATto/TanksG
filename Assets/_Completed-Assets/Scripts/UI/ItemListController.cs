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
    public GameObject itemListContent;              // アイテムリストの親オブジェクト
    public TextMeshProUGUI emptyMessageArea;        // アイテムがない場合に表示するメッセージ
    public GameObject itemUseDialog;                // アイテム使用ダイアログ
    public Button itemUseYesButton;                 // アイテム使用ダイアログの「はい」ボタン
    public GameObject itemCannotUseDialog;          // アイテム使用不可ダイアログ
    public GameObject itemUseSuccessDialog;         // アイテム使用成功ダイアログ
    public TextMeshProUGUI itemNameText;            // 使用中のアイテム名

    private int currentItemId;                      // 使用中のアイテムID
    private List<Item> itemList = new List<Item>(); // アイテムリスト

    private string apiURL = "http://localhost:8000/api/game-users/{0}/items";

    [System.Serializable]
    public class Item
    {
        public int id;          // アイテムID
        public string name;     // アイテム名
        public int quantity;    // アイテム量
    }

    private void Start()
    { 
        // ユーザーのアイテムをロードして表示
        LoadUserItems();

        // 現在使用中のアイテムIDを取得
        currentItemId = ItemManager.Instance.CurrentItemID;
        if (currentItemId != -1)
        {
            // アイテムIDからアイテム名を取得
            string itemName = GetItemName(currentItemId);
            // 使用中のアイテム名を表示
            itemNameText.text = ItemManager.Instance.CurrentItemID.ToString();
        } else {
            itemNameText.text = "使用中のアイテムはありません";
        }
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
    
    // ユーザーのアイテムをロードして表示
    public void LoadUserItems ()
    {
        // ユーザーIDを取得
        int userId = UserManager.Instance.CurrentUserID;
        Debug.Log($"get User ID: {userId}");

        // ユーザーのアイテムを取得
        StartCoroutine(GetUserItems(userId));
    }

    // ユーザーのアイテムを取得
    private IEnumerator GetUserItems(int userId)
    {
        // APIのURLを作成
        string url = string.Format(apiURL, userId);
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
                // アイテムプレハブをインスタンス化
                GameObject itemInfo = Instantiate(itemInfoPrefab, itemListContent.transform);

                // アイテム名を表示
                itemInfo.transform.Find("ItemInfoCanvas/ItemNameText").GetComponent<TextMeshProUGUI>().text = item.name;
                // ログ
                Debug.Log($"Item Name: {item.name}");

                // アイテム量を表示
                itemInfo.transform.Find("ItemInfoCanvas/ItemQuantityText").GetComponent<TextMeshProUGUI>().text = item.quantity.ToString();
                // ログ
                Debug.Log($"Item Quantity: {item.quantity}");

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
        int userId = UserManager.Instance.CurrentUserID;

        // APIのURLを作成
        string itemUseApiURL = "http://localhost:8000/api/game-users/{0}/items/{1}/use";
        string url = string.Format(itemUseApiURL, userId, itemId);
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
                Debug.Log("Item used successfully");

                // アイテムリストを更新
                LoadUserItems();

                // 使用中のアイテムIDを更新
                ItemManager.Instance.SetItemID(itemId);

                // 使用中のアイテム名を表示
                itemNameText.text = GetItemName(itemId);
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
}