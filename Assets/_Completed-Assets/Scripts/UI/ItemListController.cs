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
    public GameObject itemTemplate;             // アイテム名を表示するテンプレート
    public Transform itemNameArea;              // アイテム名を表示するエリア
    public Transform itemQuantityArea;          // アイテム量を表示するエリア
    public TextMeshProUGUI emptyMessageArea;    // アイテムがない場合に表示するメッセージ

    private string apiURL = "http://localhost:8000/api/game-users/{0}/items";

    private void Start()
    { 
        // ユーザーのアイテムをロードして表示
        LoadUserItems();
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

            List<Item> itemList = JsonConvert.DeserializeObject<List<Item>>(jsonResponse);

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
                // アイテム名を表示
                GameObject itemNameObj = Instantiate(itemTemplate, itemNameArea);
                itemNameObj.SetActive(true);
                itemNameObj.GetComponent<TextMeshProUGUI>().text = item.name;
                // ログ
                Debug.Log($"Item Name: {item.name}");

                // アイテム量を表示
                GameObject itemQuantityObj = Instantiate(itemTemplate, itemQuantityArea);
                itemQuantityObj.SetActive(true);
                itemQuantityObj.GetComponent<TextMeshProUGUI>().text = item.quantity.ToString();
                // ログ
                Debug.Log($"Item Quantity: {item.quantity}");
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
        foreach (Transform child in itemNameArea)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in itemQuantityArea)
        {
            Destroy(child.gameObject);
        }
    }

    // アイテムのクラス
    [System.Serializable]
    public class Item
    {
        public string name;
        public int quantity;
    }
}