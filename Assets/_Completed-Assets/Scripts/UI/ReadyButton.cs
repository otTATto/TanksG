using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReadyButton : MonoBehaviour
{
    private Button readyButton;

    private void Awake()
    {
        readyButton = GetComponent<Button>();
        readyButton.onClick.AddListener(OnClickReadyButton);
    }

    private void OnClickReadyButton()
    {
        if (NetworkManager.instance.isClient) // TODO: サーバに接続しているかどうかで確認
        {
            // クライアントからサーバに対して準備完了を通知する
            NetworkManager.instance.SendFromClient(new byte[] { (byte)NetworkDataTypes.DataType.READY });
        }
    }

    private void Update()
    {
        if (NetworkManager.instance.isClient && NetworkManager.instance.client.isReady)
        {
            // NetworkManager.instance.client.isReady = false;
            SceneManager.LoadScene(SceneNames.GameScene);
        }
        else if (NetworkManager.instance.isServer && NetworkManager.instance.server.IsReady())
        {
            // サーバからクライアントに対して準備完了を通知する
            for (int i = 0; i < Server.MAX_CLIENTS; i++)
            {
                byte message = (byte)(NetworkDataTypes.DataType.TANK_ID + NetworkDataTypes.TANK_IDs[i]);
                Debug.Log($"send tankId: 0x{message:X}");
                NetworkManager.instance.SendFromServer(new byte[] { message }, i);
            }
            Debug.Log("sent ready to all clients");

            NetworkManager.instance.server.ResetReady();
            SceneManager.LoadScene(SceneNames.GameScene);
        }
    }
}
