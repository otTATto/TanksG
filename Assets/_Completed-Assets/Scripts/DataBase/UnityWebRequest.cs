using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ContactUsManager : MonoBehaviour
{
    public GameObject contactListDialog;
    public Text contactListText;

    private string apiUrl = "http://127.0.0.1:8000/api/contact";

    public void OpenContactList()
    {
        contactListDialog.SetActive(true);
        StartCoroutine(LoadContacts());
    }

    IEnumerator LoadContacts()
    {
        UnityWebRequest www = UnityWebRequest.Get(apiUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            contactListText.text = www.downloadHandler.text; // JSONのパースが必要
        }
        else
        {
            Debug.LogError("Error: " + www.error);
        }
    }
}
