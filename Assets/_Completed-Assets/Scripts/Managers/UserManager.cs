// シングルトンパターンでユーザーIDを保持するクラス

using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance { get; private set; }
    public int CurrentUserID { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーン間でオブジェクトを保持
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetUserID(int userID)
    {
        CurrentUserID = userID;
    }
}
