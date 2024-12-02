using UnityEngine;

namespace Complete
{
    public class CameraControl : MonoBehaviour
    {
        public float m_DampTime = 0.2f;                 // Approximate time for the camera to refocus.
        public float m_ScreenEdgeBuffer = 4f;           // Space between the top/bottom most target and the screen edge.
        public float m_MinSize = 6.5f;                  // The smallest orthographic size the camera can be.
        [HideInInspector] public Transform[] m_Targets; // All the targets the camera needs to encompass.


        private Camera m_Camera;                        // Used for referencing the camera.
        private float m_ZoomSpeed;                      // Reference speed for the smooth damping of the orthographic size.
        private Vector3 m_MoveVelocity;                 // Reference velocity for the smooth damping of the position.
        private Vector3 m_DesiredPosition;              // The position the camera is moving towards.


        private void Awake ()
        {
            if (NetworkManager.instance.isServer) return;
            m_Camera = GetComponentInChildren<Camera> ();
        }


        private void FixedUpdate ()
        {
            // Move the camera towards a desired position.
            if (NetworkManager.instance.isServer) return;
            Move ();
        }


        private void Move ()
        {
            // Find the average position of the targets.
            //FindAveragePosition ();
            // TODO: 通信が確率されていることを確認
            if (m_Targets.Length > 0) // 配列に要素が存在するか確認
            {
                int tankId = NetworkManager.instance.GetTankId();
                transform.rotation = m_Targets[tankId - 1].rotation;
                transform.position = m_Targets[tankId - 1].TransformPoint(new Vector3(0, 2, -5));
            }
            else
            {
                Debug.LogWarning("m_Targets array is empty.");
            }
        }

        public void OnGUI()
        {
            if (NetworkManager.instance.isServer) return;
            GUI.Label(new Rect(10, 80, 100, 20), $"tankId: {NetworkManager.instance.GetTankId()}", new GUIStyle() { normal = { textColor = Color.black } });
        }
    }
}
