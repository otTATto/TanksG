using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class ContactForm : MonoBehaviour
{
    public InputField titleField;
    public InputField contentField;
    public string apiUrl = "http://127.0.0.1:8000/api/contact";
    public Button Button1;

    private IDDisplayer iDDisplayer;

    private void Awake()
    {
        Button1.onClick.AddListener(() => {
            SendContactRequest();
        });
        iDDisplayer = IDDisplayer.Instance.GetComponent<IDDisplayer>();
    }

    public void SendContactRequest()
    {
        StartCoroutine(PostContact());
    }

    private IEnumerator PostContact()
    {
        string title = titleField.text;
        string content = contentField.text;
        string userID = iDDisplayer.GetPlayerID().ToString();

        WWWForm form = new WWWForm();
        form.AddField("title", title);
        form.AddField("content", content);
        form.AddField("user_id", userID); // ユーザーIDを適宜設定

        UnityWebRequest request = UnityWebRequest.Post(apiUrl, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("お問い合わせが送信されました");
            // フォームのリセット処理など
        }
        else
        {
            Debug.LogError("エラー: " + request.error);
        }
    }
}
