using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro; // TextMeshProを使用するための名前空間

public class DetailContact : MonoBehaviour
{
    public int User_id;
    public int Contact_id; // 返信を追加する対象の問い合わせID
    public Transform Content_detail; // Scroll ViewのContentに対応するTransform
    public GameObject detailPanel; // 詳細表示用のパネル
    public GameObject ReplyPrefab; // 問い合わせ表示用のPrefab
    public TMP_InputField messageField; // 返信内容を入力するためのInputField
    public Button SendButton1; // 返信送信ボタン
    public Button CloseButton; // 返信送信ボタン
    public string apiUrl = "http://127.0.0.1:8000/api/contact"; // ベースURL
    private bool isButtonPressed = false;  // ボタンが押されたかどうかを示すフラグ


    [System.Serializable]
    public class Reply
    {
        public int id;
        public int contact_id;
        public int user_id;
        public string message;
        public string created_at;
        public string updated_at;
    }

    [System.Serializable]
    public class Contact
    {
        public int id;
        public int user_id;
        public string title;
        public string content;
        public string created_at;
        public string updated_at;
        public List<Reply> replies;
    }

    private void Start()
    {
        // Detail Panelを開くボタンにイベントリスナーを追加
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                isButtonPressed=true;
                Debug.Log($"Opening details for Contact ID: {Contact_id}");
                detailPanel.SetActive(true);
                OnContactItemClicked();
            });
        }

        // SendButtonにイベントリスナーを設定
        if (SendButton1 != null)
        {
            SendButton1.onClick.AddListener(() =>
            {
                if(isButtonPressed){
                    Debug.Log($"SendButton clicked for Contact ID: {Contact_id}");
                    SendContactRequest();
                }
            });
        }
        else
        {
            Debug.LogError("SendButton1 is not assigned in the Inspector!");
        }
        CloseButton.onClick.AddListener(() =>
        {
            isButtonPressed=false;
            // Content内のすべての子オブジェクトを削除
            foreach (Transform child in Content_detail)
            {
                Destroy(child.gameObject);
            }
            detailPanel.SetActive(false);
        });
    }

    public void SendContactRequest()
    {
        if (string.IsNullOrEmpty(messageField?.text))
        {
            Debug.LogWarning("Message field is empty. Please enter a reply.");
            return;
        }

        StartCoroutine(PostReply());
    }

    private IEnumerator PostReply()
    {
        string message = messageField.text; // 返信メッセージを取得

        WWWForm form = new WWWForm();
        form.AddField("message", message); // 返信内容を追加
        form.AddField("user_id", User_id.ToString()); // ユーザーID
        form.AddField("contact_id", Contact_id.ToString()); // 返信対象の問い合わせID

        string replyUrl = $"{apiUrl}/{Contact_id}/replies"; // 返信用エンドポイント

        using (UnityWebRequest request = UnityWebRequest.Post(replyUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Reply sent successfully for Contact ID: {Contact_id}");
                // UI更新処理など
            }
            else
            {
                Debug.LogError($"Error sending reply: {replyUrl}{request.error}");
            }
        }
    }

    public void OnContactItemClicked()
    {
        string url = $"{apiUrl}/{Contact_id}";
        StartCoroutine(GetContactDetails(url));
    }

    private IEnumerator GetContactDetails(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"Contact Details Response for Contact ID {Contact_id}: " + jsonResponse);

                Contact contact = JsonUtility.FromJson<Contact>(jsonResponse);

                Debug.Log($"Contact ID: {contact.id}, Title: {contact.title}, Content: {contact.content}");

                foreach (Reply reply in contact.replies)
                {
                    Debug.Log($"Reply: {reply.message}");
                    DisplayContactDetails(reply);
                }
            }
            else
            {
                Debug.LogError($"Error retrieving details for Contact ID {Contact_id}: {request.error}");
                Debug.LogError($"{url}");
            }
        }
    }

    private void DisplayContactDetails(Reply reply)
    {
        // Prefabから問い合わせアイテムを生成
        GameObject contactItem = Instantiate(ReplyPrefab, Content_detail);

        // TextMeshProUGUIコンポーネントを取得して内容を設定
        TextMeshProUGUI textComponent = contactItem.GetComponent<TextMeshProUGUI>();

        if (textComponent != null)
        {
            textComponent.text = reply.message; // メッセージを設定
            Debug.Log($"Created reply item: {reply.message}");
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component is missing in the ReplyPrefab.");
        }
    }
}

