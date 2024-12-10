// シングルトンパターンで使用中のアイテムIDを保持するクラス

using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }
    public int CurrentItemID { get; private set; }

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

    public void SetItemID(int itemID)
    {
        CurrentItemID = itemID;
    }
}