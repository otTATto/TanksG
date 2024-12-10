using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json; // Newtonsoft.Jsonを使用する場合

public class LoginBonus : MonoBehaviour
{
    [SerializeField] private TMP_Text[] bonusTexts; 
    [SerializeField] private TMP_Text dayText; // 「Day」用のTMP_Textを割り当てる
    private int currentDay; // 現在のログイン日
    private List<LoginBonusData> bonusDataList;

    [System.Serializable]
    public class LoginBonusData
    {
        public int id;
        public int day;
        public long item_id;
        public int quantity;
        public string item_name;    // item_nameフィールドを追加
        public string created_at;
        public string updated_at;
    }

    void Start()
    {
        // 例として userId=15 を仮定
        int userId = UserManager.Instance.CurrentUserID;
        StartCoroutine(SetupLoginBonusUI(userId));
    }

    private IEnumerator SetupLoginBonusUI(int userId)
    {
        // 現在のログイン日を取得
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

        // 全ログインボーナス情報を取得
        string bonusUrl = "http://127.0.0.1:8000/api/login-bonuses";
        using (UnityWebRequest bonusRequest = UnityWebRequest.Get(bonusUrl))
        {
            yield return bonusRequest.SendWebRequest();

            if (bonusRequest.result == UnityWebRequest.Result.Success)
            {
                string bonusJson = bonusRequest.downloadHandler.text;
                bonusDataList = JsonConvert.DeserializeObject<List<LoginBonusData>>(bonusJson);

                // bonusDataListには1日目から7日目までのログインボーナスデータが day 順で入っている想定
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
        // currentDayまでを表示、それ以降は空欄
        // 一旦全てクリア
        foreach (var text in bonusTexts)
        {
            text.text = "";
        }

        // currentDayをdayTextに表示
        // 例えば「今日のログイン日: {currentDay}日目」などと表示したい場合:
        dayText.text = $"Day: {currentDay}";

        // 1日目からcurrentDay日目まで表示
        for (int i = 0; i < currentDay; i++)
        {
            int dayIndex = i; // 0-based index
            var data = bonusDataList.Find(d => d.day == dayIndex + 1);
            if (data != null)
            {
                // 「アイテム名 x 個数」の形式で表示
                bonusTexts[dayIndex].text = $"{data.item_name} x {data.quantity}";
            }
        }
        // currentDay+1日目～7日目は空欄のまま
    }

    [System.Serializable]
    public class LoginStatusResponse
    {
        public bool can_receive_bonus;
        public int day;
    }
}
