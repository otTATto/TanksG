using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Buttonを使用するために必要
using UnityEngine.Networking;
using TMPro;

public class InformationData : MonoBehaviour
{
    public Transform Content; // Scroll ViewのContentに対応するTransform
    public TextMeshProUGUI InfoPrehab; // 記事のタイトルを表示するPrefab
    public GameObject InfoPanel; // 詳細表示用のパネル
    public TextMeshProUGUI PanelTitle; // パネル内のタイトルTextMeshPro
    public TextMeshProUGUI PanelBody; // パネル内の本文TextMeshPro
    public Button CloseButton; // パネルを閉じるボタン

    [System.Serializable]
    public class Article
    {
        public int id;
        public string title;
        public string body;
        public string created_at;
    }

    [System.Serializable]
    public class ArticleList
    {
        public Article[] articles;
    }

    private string apiUrl = "http://127.0.0.1:8000/api/blog";

    void Start()
    {
        StartCoroutine(GetArticles());

        // CloseButtonにイベントリスナーを追加
        CloseButton.onClick.AddListener(() =>
        {
            InfoPanel.SetActive(false); // パネルを閉じる
        });
    }

    IEnumerator GetArticles()
    {
        UnityWebRequest www = UnityWebRequest.Get(apiUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            string json = www.downloadHandler.text;
            Debug.Log("Response: " + json);

            ArticleList articleList = JsonUtility.FromJson<ArticleList>("{\"articles\":" + json + "}");

            foreach (Article article in articleList.articles)
            {
                // TextMeshProUGUIを生成
                TextMeshProUGUI item = Instantiate(InfoPrehab, Content);
                item.text = article.title;

                // クリック時のイベントを登録
                item.GetComponent<Button>().onClick.AddListener(() =>
                {
                    
                    ShowArticleDetails(article); // 記事の詳細を表示
                });
            }
        }
    }

    void ShowArticleDetails(Article article)
    {
        InfoPanel.SetActive(true); // パネルをアクティブ化
        PanelTitle.text = article.title; // タイトルを設定
        PanelBody.text = article.body; // 本文を設定
    }
}
