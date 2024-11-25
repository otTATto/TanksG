using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using TMPro.Examples;
using UnityEngine.SceneManagement;

public class Resgister : MonoBehaviour
{
    private IDDisplayer iDDisplayer;
    //public InputField usernameInput;
    //public InputField passwordInput;
    private string registerURL = "http://localhost/register.php";
    private string id;

    void Start()
    {
        iDDisplayer = IDDisplayer.Instance.GetComponent<IDDisplayer>();
    }

    public void OnclickStart() 
    {
        if (iDDisplayer != null)
        {
            if (iDDisplayer.GetPlayerID() == 0)
            {
                StartCoroutine(ResgisterNewPlayer());
                //Debug.Log("Registration ");
            }
        }
    }
    public void ToUserResistration() 
    {
        if (iDDisplayer.GetPlayerID() != 0) iDDisplayer.GetPlayerNameData();
        SceneManager.LoadScene("UserRegistration");

    }
    public void ToRanking()
    {

        //SceneManager.LoadScene(SceneNames.Ranking);

    }
    public void ToExit()
    {


    }

    IEnumerator ResgisterNewPlayer()
    {
        WWWForm form = new WWWForm();
        using (UnityWebRequest www = UnityWebRequest.Post(registerURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                Debug.Log(response);

                if (response.StartsWith("success:"))
                {
                    id = response.Split(':')[1]; // ID‚ðŽæ“¾
                    Debug.Log("Registration successful! ID: " + id);

                    if (int.TryParse(id, out int playerId))
                    {
                        iDDisplayer.SetPlayerIDandName(playerId, "NONAME");
                        SceneManager.LoadScene(SceneNames.HomeScene);
                    }
                    else
                    {
                        Debug.LogError("Failed to parse player ID.");
                    }
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
