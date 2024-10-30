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

        private void Start()
        {
            // プレイヤーのTankShootingコンポーネントを取得
            manager = gameManger.GetComponent<GameManager>();
            opponetHealth = new TankHealth[manager.m_Tanks.Length - 1];
            opponentSlider = new Slider[manager.m_Tanks.Length - 1];
            for (int i = 0; i < manager.m_Tanks.Length; i++)
            {
                if (i == 0)
                {
                    playerHealth = manager.m_Tanks[i].m_Instance.GetComponent<TankHealth>();
                }
                else
                {
                    opponetHealth[i - 1] = manager.m_Tanks[i].m_Instance.GetComponent<TankHealth>();
                    opponentSlider[i - 1] = Instantiate(opponentSliderPrefab, opponentSliderPlace.transform);
                    opponentSlider[i - 1].gameObject.SetActive(true);
                }
            }

            tankShooting = GameObject.FindWithTag("Player").GetComponent<TankShooting>();

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
        }

        private void Update()
        {
            UpdateShellCount();
            UpdateHealth();
        }

        private void UpdateHealth() {
            playerHealthSlider.value = playerHealth.GetHealth();
            for (int i = 0; i < manager.m_Tanks.Length - 1; i++)
            {
                opponentSlider[i].value = opponetHealth[i].GetHealth();
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
    }
}