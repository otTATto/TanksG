using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json; 
using UnityEngine.EventSystems; // 追加
using UnityEngine.SceneManagement; // シーン遷移する場合

public class LoginBonus : MonoBehaviour, IPointerClickHandler // IPointerClickHandlerを実装
{
    [SerializeField] private TMP_Text[] bonusTexts; 
    [SerializeField] private TMP_Text dayText; 
    private int currentDay; 
    private List<LoginBonusData> bonusDataList;

    [System.Serializable]
    public class LoginBonusData
    {
        public int id;
        public int day;
        public long item_id;
        public int quantity;
        public string item_name; 
        public string created_at;
        public string updated_at;
    }

    [System.Serializable]
    public class LoginStatusResponse
    {
        public bool can_receive_bonus;
        public int day;
    }

    void Start()
    {
        int userId = UserManager.Instance.CurrentUserID;
        StartCoroutine(SetupLoginBonusUI(userId));
    }

    private IEnumerator SetupLoginBonusUI(int userId)
    {
        string loginUrl = $"http://127.0.0.1:8000/api/login/{userId}";
        using (UnityWebRequest loginRequest = UnityWebRequest.Get(loginUrl))
        {
            yield return loginRequest.SendWebRequest();

            if (loginRequest.result == UnityWebRequest.Result.Success)
            {
                string loginJson = loginRequest.downloadHandler.text;
                var loginStatus = JsonConvert.DeserializeObject<LoginStatusResponse>(loginJson);
                currentDay = loginStatus.day;
            }
            else
            {
                Debug.LogError("Failed to fetch login status");
                yield break;
            }
        }

        string bonusUrl = "http://127.0.0.1:8000/api/login-bonuses";
        using (UnityWebRequest bonusRequest = UnityWebRequest.Get(bonusUrl))
        {
            yield return bonusRequest.SendWebRequest();

            if (bonusRequest.result == UnityWebRequest.Result.Success)
            {
                string bonusJson = bonusRequest.downloadHandler.text;
                bonusDataList = JsonConvert.DeserializeObject<List<LoginBonusData>>(bonusJson);

                UpdateBonusTexts();
            }
            else
            {
                Debug.LogError("Failed to fetch login bonus data");
            }
        }
    }

    private void UpdateBonusTexts()
    {
        foreach (var text in bonusTexts)
        {
            text.text = "";
        }

        if (--currentDay == 0) currentDay = 7;
        dayText.text = $"Day: {currentDay}";

        for (int i = 0; i < currentDay; i++)
        {
            int dayIndex = i;
            var data = bonusDataList.Find(d => d.day == dayIndex + 1);
            if (data != null)
            {
                bonusTexts[dayIndex].text = $"{data.item_name} x {data.quantity}";
            }
        }
    }

    // IPointerClickHandlerの実装
    public void OnPointerClick(PointerEventData eventData)
    {
        // ここでシーン遷移や別の処理を行う
        // 例: "NextScene" へ遷移する
        SceneManager.LoadScene("NextScene");
    }
}
