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
            NetworkManager.instance.client.isReady = false;
            NetworkManager.instance.isGameStart = true;
            SceneManager.LoadScene(SceneNames.GameScene);
        }
        else if (NetworkManager.instance.isServer && NetworkManager.instance.server.IsReady())
        {
            // サーバからクライアントに対して準備完了を通知する
            for (int i = 0; i < Server.MAX_CLIENTS; i++)
            {
                NetworkManager.instance.SendFromServer(new byte[] { (byte)NetworkDataTypes.DataType.READY }, i);
            }
            Debug.Log("sent ready to all clients");

            NetworkManager.instance.isGameStart = true;

            NetworkManager.instance.server.ResetReady();
            SceneManager.LoadScene(SceneNames.GameScene);
        }
    }
}
