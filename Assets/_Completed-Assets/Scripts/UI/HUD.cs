using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class HUD : MonoBehaviour
    {
        private TankShooting tankShooting; // 砲弾ストック数を管理するコンポーネント

        private TankHealth playerHealth;
        private TankHealth[] opponetHealth;

        public GameObject gameManger;
        private GameManager manager;

        private static int tensMax = 5;
        private static int onesMax = 9;
        public GameObject[] tensObjects = new GameObject[tensMax];  // 10の位のアイコン
        public GameObject[] onesObjects = new GameObject[onesMax];  // 1の位のアイコン
        public Slider playerHealthSlider;
        public GameObject opponentSliderPlace;
        public Slider opponentSliderPrefab;
        private Slider[] opponentSlider;

        private static int maxMines = 3;
        private int opponentId;
        public GameObject[] minesObjects = new GameObject[maxMines];  // 地雷のアイコン
        public GameObject PlayerHealthSlider_2nd;  // 2つ目のプレイヤーの体力インジケータ

        private void Start()
        {
            // プレイヤーのTankShootingコンポーネントを取得
            manager = gameManger.GetComponent<GameManager>();
            opponetHealth = new TankHealth[manager.m_Tanks.Length - 1];
            opponentSlider = new Slider[manager.m_Tanks.Length - 1];
            int playerId = Client.instance.playerId;
            opponentId = (playerId == 0 ? 1 : 0);
            playerHealth = manager.m_Tanks[playerId].m_Instance.GetComponent<TankHealth>();
                
                
            opponetHealth[0] = manager.m_Tanks[opponentId].m_Instance.GetComponent<TankHealth>();
            opponentSlider[0] = Instantiate(opponentSliderPrefab, opponentSliderPlace.transform);
            opponentSlider[0].gameObject.SetActive(true);
                
            


            tankShooting = manager.m_Tanks[0].m_Instance.GetComponent<TankShooting>();

            // 10の位のアイコンを取得
            for (int i = 1; i <= tensMax; i++)
            {
                GameObject _tens = GameObject.Find("tens");
                GameObject shellCount = _tens.transform.Find($"Shell{i * 10}")?.gameObject;
                if (shellCount == null)
                {
                    Debug.LogError($"Shell{i * 10} not found");
                }
                tensObjects[i - 1] = shellCount;
            }


            // 1の位のアイコンを取得
            for (int i = 1; i <= onesMax; i++)
            {
                GameObject _ones = GameObject.Find("ones");
                GameObject shellCount = _ones.transform.Find($"Shell{i}")?.gameObject;
                if (shellCount == null)
                {
                    Debug.LogError($"Shell{i} not found");
                }
                onesObjects[i - 1] = shellCount;
            }


            // 地雷のアイコンを取得
            for (int i = 1; i <= maxMines; i++)
            {
                GameObject _mines = GameObject.Find("Mines");
                GameObject mineCount = _mines.transform.Find($"Mine{i}")?.gameObject;
                if (mineCount == null)
                {
                    Debug.LogError($"Mine{i} not found");
                }
                minesObjects[i - 1] = mineCount;
            }


            // 使用中のアイテムIDを取得
            int currentItemID = ItemManager.Instance.CurrentItemID;
            // アイテムIDに応じて効果を適用
            ApplyItemEffect(currentItemID);
        }

        private void Update()
        {
            UpdateShellCount();
            UpdateMineCount();
            UpdateHealth();
        }

        private void UpdateHealth()
        {
            // 相手の体力を更新
            for (int i = 0; i < manager.m_Tanks.Length - 1; i++)
            {
                opponentSlider[i].value = opponetHealth[i].GetHealth();
            }

            // アイテムIDを取得
            int currentItemID = ItemManager.Instance.CurrentItemID;
            // 装甲強化アイテムを使用中の場合
            if (currentItemID == 2)
            {
                // 2つ目のプレイヤーの体力インジケータを表示
                PlayerHealthSlider_2nd.SetActive(true);

                if (playerHealth.GetHealth() >= 100)
                {
                    // 残り HP が 100 を超えるとき
                    // 1つ目のプレイヤーの体力インジケータを更新
                    playerHealthSlider.value = playerHealth.GetHealth() - 100;
                    // 2つ目のプレイヤーの体力インジケータを更新
                    PlayerHealthSlider_2nd.GetComponent<Slider>().value = 100;
                }
                else
                {
                    // 残り HP が 100 未満のとき
                    // 1つ目のプレイヤーの体力インジケータを更新
                    playerHealthSlider.value = 0;
                    // 2つ目のプレイヤーの体力インジケータを更新
                    PlayerHealthSlider_2nd.GetComponent<Slider>().value = playerHealth.GetHealth();
                }


                // 自機の体力が 0 になったら，アイテム効果を解除
                if (playerHealth.GetHealth() <= 0)
                {
                    ItemManager.Instance.SetItemID(-1); // アイテムIDをリセット
                    RemoveArmorEnhancement();           // 装甲強化アイテムの効果を解除
                }
            }
            else
            {
                // 自機の体力を更新
                playerHealthSlider.value = playerHealth.GetHealth();
                Debug.Log($"HP: {playerHealth.GetHealth()}");

                // 2つ目のプレイヤーの体力インジケータを非表示
                PlayerHealthSlider_2nd.SetActive(false);
            }
        }

        private void UpdateShellCount()
        {
            int remainingShells = tankShooting.m_RemainingShells;

            // 10の位と1の位のアイコン表示を更新
            int tens = remainingShells / 10;
            int ones = remainingShells % 10;

            for (int i = 1; i <= tensMax; i++)
            {
                tensObjects[i - 1].SetActive(i <= tens);
            }

            for (int i = 1; i <= onesMax; i++)
            {
                onesObjects[i - 1].SetActive(i <= ones);
            }
        }

        private void UpdateMineCount()
        {
            int remainingMines = tankShooting.m_RemainingMines;

            for (int i = 1; i <= maxMines; i++)
            {
                minesObjects[i - 1].SetActive(i <= remainingMines);
            }
        }

        // アイテムIDに応じて効果を適用
        private void ApplyItemEffect(int itemID)
        {
            switch (itemID)
            {
                case 2:
                    // 装甲強化アイテム使用時
                    ApplyArmorEnhancement();    // 装甲強化アイテムの効果を適用
                    break;
                default:
                    break;
            }
        }

        // 装甲強化アイテムの効果を適用
        private void ApplyArmorEnhancement()
        {
            // プレイヤーの体力の最大値を 200 に設定
            playerHealth.m_StartingHealth = 200f;
            playerHealth.SetHealth(playerHealth.m_StartingHealth);
            // 2つ目のインジケータを表示

        }

        // 装甲強化アイテムの効果を解除
        private void RemoveArmorEnhancement()
        {
            // プレイヤーの体力の最大値を 100 に設定
            playerHealth.m_StartingHealth = 100f;
            playerHealth.SetHealth(playerHealth.m_StartingHealth);
            // 2つ目のインジケータを非表示
        }
    }
}