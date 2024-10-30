using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class TankShooting : MonoBehaviour
    {
        public int m_PlayerNumber = 1;              // Used to identify the different players.
        public Rigidbody m_Shell;                   // Prefab of the shell.
        public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
        public Slider m_AimSlider;                  // A child of the tank that displays the current launch force.
        public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
        public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
        public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
        public float m_MinLaunchForce = 15f;        // The force given to the shell if the fire button is not held.
        public float m_MaxLaunchForce = 30f;        // The force given to the shell if the fire button is held for the max charge time.
        public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.

        public int m_RemainingShells = 10;          // 残弾数
        public int m_ShellCapacity = 50;            // 砲弾の最大数

        public GameObject m_MinePrefab;                // 地雷のプレハブ
        public int m_RemainingMines = 1;              // 持っている地雷の数
        public int m_MineCapacity = 3;                // 地雷の最大数
        public float m_MineSetupTime = 1.5f;          // 地雷設置に要する時間

        private string m_FireButton;                // 発射ボタン
        private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
        private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
        private bool m_Fired;                       // Whether or not the shell has been launched with this button press.

        private string m_MineButton;                   // 地雷設置ボタン
        private bool m_IsSettingMine = false;         // 地雷設置中かどうか
        private float m_MineSetupTimer = 0f;          // 地雷設置タイマー

        private void OnEnable()
        {
            // When the tank is turned on, reset the launch force and the UI
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_AimSlider.value = m_MinLaunchForce;
        }


        private void Start ()
        {
            // The fire axis is based on the player number.
            m_FireButton = "Fire" + m_PlayerNumber;

            // The rate that the launch force charges up is the range of possible forces by the max charge time.
            m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;

            m_MineButton = "Mine" + m_PlayerNumber;    // "Mine1" or "Mine2"
        }


        private void Update ()
        {
            // 砲弾がなくなったら発射できない
            if (m_RemainingShells <= 0) {
                return;
            }

            // The slider should have a default value of the minimum launch force.
            m_AimSlider.value = m_MinLaunchForce;

            // If the max force has been exceeded and the shell hasn't yet been launched...
            if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
            {
                // チャージを往復させる
                m_CurrentLaunchForce = m_MinLaunchForce;
                
                // Change the clip to the charging clip and start it playing.
                m_ShootingAudio.clip = m_ChargingClip;
                m_ShootingAudio.Play ();
            }
            // Otherwise, if the fire button has just started being pressed...
            else if (Input.GetButtonDown (m_FireButton))
            {
                // ... reset the fired flag and reset the launch force.
                m_Fired = false;
                m_CurrentLaunchForce = m_MinLaunchForce;

                // Change the clip to the charging clip and start it playing.
                m_ShootingAudio.clip = m_ChargingClip;
                m_ShootingAudio.Play ();
            }
            // Otherwise, if the fire button is being held and the shell hasn't been launched yet...
            else if (Input.GetButton (m_FireButton) && !m_Fired)
            {
                // Increment the launch force and update the slider.
                m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

                m_AimSlider.value = m_CurrentLaunchForce;
            }
            // Otherwise, if the fire button is released and the shell hasn't been launched yet...
            else if (Input.GetButtonUp (m_FireButton) && !m_Fired)
            {
                // ... launch the shell.
                Fire ();
                m_RemainingShells--;
            }

            #region 地雷設置の処理
            if (m_RemainingMines > 0 && Input.GetButtonDown(m_MineButton) && !m_IsSettingMine)
            {
                Debug.Log("地雷設置");
                StartSettingMine();
            }

            // 地雷設置中の処理
            if (m_IsSettingMine)
            {
                m_MineSetupTimer += Time.deltaTime;
                if (m_MineSetupTimer >= m_MineSetupTime)
                {
                    CompleteMineSetup();
                }
            }
            #endregion
        }


        private void Fire ()
        {
            // Set the fired flag so only Fire is only called once.
            m_Fired = true;

            // Create an instance of the shell and store a reference to it's rigidbody.
            Rigidbody shellInstance =
                Instantiate (m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

            // Set the shell's velocity to the launch force in the fire position's forward direction.
            shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; 

            // Change the clip to the firing clip and play it.
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play ();

            // Reset the launch force.  This is a precaution in case of missing button events.
            m_CurrentLaunchForce = m_MinLaunchForce;
        }

        private void StartSettingMine()
        {
            m_IsSettingMine = true;
            m_MineSetupTimer = 0f;

            // タンクの移動と射撃を無効化
            GetComponent<TankMovement>().enabled = false;
            enabled = false;
        }

        private void CompleteMineSetup()
        {
            // 地雷を設置
            Vector3 minePosition = new Vector3(
                transform.position.x,
                0.5f,  // 地面からの高さ
                transform.position.z
            );
            
            Instantiate(m_MinePrefab, minePosition, Quaternion.identity);
            m_RemainingMines--;

            // タンクの移動と射撃を再度有効化
            m_IsSettingMine = false;
            GetComponent<TankMovement>().enabled = true;
            enabled = true;
        }
    }
}