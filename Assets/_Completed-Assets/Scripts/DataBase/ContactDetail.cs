/*
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class ContactDetail : MonoBehaviour
{
    public static ContactDetail instance;
    public Text titleText;
    public Text contentText;
    public Transform repliesPanel;
    public GameObject replyPrefab;
    public InputField replyField;
    public string apiUrl = "http://127.0.0.1:8000/api/contact/";

    private int currentContactId;

    private void Awake()
    {
        instance = this;
    }

    public void LoadContactDetail(int contactId)
    {
        currentContactId = contactId;
        StartCoroutine(GetContactDetail());
    }

    private IEnumerator GetContactDetail()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl + currentContactId);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ContactResponse contact = JsonUtility.FromJson<ContactResponse>(request.downloadHandler.text);
            titleText.text = contact.title;
            contentText.text = contact.content;
            // repliesPanelの過去のやりとりを表示
        }
        else
        {
            Debug.LogError("エラー: " + request.error);
        }
    }

    public void SendReply()
    {
        StartCoroutine(PostReply());
    }

    private IEnumerator PostReply()
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", "1"); // ユーザーIDを適宜設定
        form.AddField("message", replyField.text);

        UnityWebRequest request = UnityWebRequest.Post(apiUrl + currentContactId + "/reply", form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("返信が送信されました");
            replyField.text = "";
            // 新しい返信を表示
        }
        else
        {
            Debug.LogError("エラー: " + request.error);
        }
    }
}
*/