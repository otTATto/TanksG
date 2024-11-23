using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using TMPro.Examples;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class UserReistration : MonoBehaviour
{
    private IDDisplayer iDDisplayer;
    public TMP_InputField playernameInput;
    private string userReistrationURL = "http://localhost/userreistration.php";
    public TextMeshProUGUI errorTX;
    public TextMeshProUGUI informationTX;
    private bool isPlayerExsist = true;
    private int playerID;


    void Start()
    {
        GameObject existingObject = GameObject.FindWithTag("DontDistoryObject");
        iDDisplayer = existingObject.GetComponent<IDDisplayer>();
        playernameInput.onValueChanged.AddListener(ValidateInput);
        errorTX.text = "";
        informationTX.text = "";
        if (iDDisplayer != null)
        {
            if (iDDisplayer.GetPlayerID() == 0)
            {
                errorTX.text = "No Player Found";
                isPlayerExsist = false;
            }
            else {
                playerID = iDDisplayer.GetPlayerID();
                //informationTX.text =
            }
        }
    }
    public void BacktoTitle()
    {
        SceneManager.LoadScene("Login");
    }

    void ValidateInput(string input)
    {
        // 正規表現で無効な文字を除去
        string sanitizedInput = Regex.Replace(input, @"[^\w\u4E00-\u9FFF\u3040-\u30FF\uAC00-\uD7AF]", "");

        // 長さをチェック（3〜15文字）
        if (sanitizedInput.Length > 15)
        {
            sanitizedInput = sanitizedInput.Substring(0, 15);
        }

        // 修正した文字列を反映
        playernameInput.text = sanitizedInput;

        // エラー表示（3文字未満の場合）
        if (sanitizedInput.Length < 3)
        {
            errorTX.text = "More than 3 charactors please";
        }
        else
        {
            errorTX.text = "";
        }
    }

    public void OnNameChangeNameBottonPress()
    {
        if (playernameInput.text.Length < 3)
        {
                errorTX.text = "More than 3 charactors please";
        }
        else
        {
            
        
        if (isPlayerExsist) StartCoroutine(ChangeName());
        }
    }

    IEnumerator ChangeName() 
    {

        WWWForm form = new WWWForm();
        form.AddField("id", playerID);
        form.AddField("playername", playernameInput.text);
        using (UnityWebRequest www = UnityWebRequest.Post(userReistrationURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)

            {
                if (www.downloadHandler.text == "success")
                {
                    informationTX.text = "Name change successed";
                    iDDisplayer.GetPlayerNameData();
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);
                    //feedbackText.text = "Registration failed!";
                    Debug.Log("failed!");
                }
            }

            else
            {
                // feedbackText.text = "Error: " + www.error;
                Debug.Log("Error: " + www.error);
            }
        }

    }

}
