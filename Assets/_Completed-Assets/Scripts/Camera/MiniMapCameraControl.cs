using UnityEngine;

namespace Complete
{
    public class MiniMapCameraControl : MonoBehaviour
    {
        public float m_DampTime = 0.2f;
        public float m_ScreenEdgeBuffer = 4f;
        public float m_MinSize = 6.5f;  
        [HideInInspector] public Transform[] m_Targets;
        private Camera m_Camera;

        private void Awake ()
        {
            if (NetworkManager.instance.isServer) return;
            m_Camera = GetComponentInChildren<Camera> ();
        }

        private void FixedUpdate ()
        {
            if (NetworkManager.instance.isServer) return;
            Move ();
        }

        private void Move ()
        {
            // カメラの追跡対象が存在する場合
            if (m_Targets.Length > 0)
            {
                int playerId = NetworkManager.instance.client.playerId;
                // カメラの位置を設定
                transform.position = m_Targets[playerId].position;
                // カメラの向きを設定
                transform.rotation = m_Targets[playerId].rotation;
            }
            else
            {
                Debug.LogWarning("m_Targets array is empty.");
            }
        }
    }
}