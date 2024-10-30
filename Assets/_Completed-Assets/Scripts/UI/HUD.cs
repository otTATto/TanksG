using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class HUD : MonoBehaviour
    {   
        private TankShooting tankShooting; // 砲弾ストック数を管理するコンポーネント

        private static int tensMax = 5;
        private static int onesMax = 9;
        public GameObject[] tensObjects = new GameObject[tensMax];  // 10の位のアイコン
        public GameObject[] onesObjects = new GameObject[onesMax];  // 1の位のアイコン

        private static int maxMines = 3;
        public GameObject[] minesObjects = new GameObject[maxMines];  // 地雷のアイコン

        private void Start()
        {
            // プレイヤーのTankShootingコンポーネントを取得
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
        }

        private void Update()
        {
            UpdateShellCount();
            UpdateMineCount();
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
    }
}