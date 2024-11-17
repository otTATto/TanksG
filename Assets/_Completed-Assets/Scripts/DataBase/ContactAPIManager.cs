using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro; // TextMeshProを使用するための名前空間

public class ContactAPIManager : MonoBehaviour
{
    public string apiUrl = "http://127.0.0.1:8000/api/contact"; // Laravel APIのURL
    public GameObject contactItemPrefab; // 問い合わせ表示用のPrefab
    public GameObject ReplyPrefab; // 問い合わせ表示用のPrefab
    public Transform contentPanel; // Scroll ViewのContentに対応するTransform
    public GameObject detail; // 問い合わせ表示用のPrefab
    public Transform content; // Scroll ViewのContentに対応するTransform
    public TMP_InputField messageField; // 返信内容を入力するためのInputField
    public Button CloseButton; // 返信内容を入力するためのInputField

    public Button Send_Button;
    // 問い合わせデータのクラスを定義
    [System.Serializable]
    public class Contact
    {
        public int id;
        public int user_id;
        public string title;
        public string content;
        public string created_at;
        public string updated_at;
    }

    void Start()
    {
        StartCoroutine(GetContacts());
    }

    IEnumerator GetContacts()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Response: " + jsonResponse);

                // JSONデータをList<Contact>に変換
                List<Contact> contactList = JsonUtilityWrapper.FromJson<Contact>(jsonResponse);

                // 各問い合わせをリストに追加
                foreach (Contact contact in contactList)
                {
                    CreateContactItem(contact);
                }
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }

    void CreateContactItem(Contact contact)
    {
        // Prefabから問い合わせアイテムを生成
        GameObject contactItem = Instantiate(contactItemPrefab, contentPanel);
        DetailContact detailContact = contactItem.GetComponent<DetailContact>();
        if (detailContact != null)
        {
            detailContact.Contact_id = contact.id;
            detailContact.detailPanel = detail; // シーン内のPanelを割り当て
            detailContact.Content_detail = content;
            detailContact.SendButton1 = Send_Button;
            detailContact.messageField = messageField;
            detailContact.CloseButton = CloseButton;
            detailContact.ReplyPrefab= ReplyPrefab;
        }
        else
        {
            Debug.LogError("DetailContact component is missing on the ContactItem prefab.");
        }
        Debug.Log("Creating contact item for: " + contact.title);

        // 各TextMeshProUGUIコンポーネントを取得して内容を設定
        TextMeshProUGUI titleText = contactItem.transform.Find("TitleText").GetComponent<TextMeshProUGUI>();

        if (titleText != null)
        {
            titleText.text = contact.title;
            Debug.Log("Contact item created: " + titleText.text);
        }
        else
        {
            Debug.LogError("One or more TextMeshProUGUI components are missing in the ContactItem prefab.");
        }
    }

    // JSONデータのリストをデシリアライズするためのユーティリティクラス
    public static class JsonUtilityWrapper
    {
        public static List<T> FromJson<T>(string json)
        {
            string wrappedJson = "{\"Items\":" + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
            return wrapper.Items;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public List<T> Items;
        }
    }
}