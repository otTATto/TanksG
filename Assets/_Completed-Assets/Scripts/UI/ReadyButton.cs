using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReadyButton : MonoBehaviour
{
    private Button readyButton;
    private NetworkManager networkManager;

    private void Awake()
    {
        readyButton = GetComponent<Button>();
        readyButton.onClick.AddListener(OnClickReadyButton);
        networkManager = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>();
    }

    private void OnClickReadyButton()
    {
        if (networkManager.isClient) // TODO: サーバに接続しているかどうかで確認
        {
            // クライアントからサーバに対して準備完了を通知する
            networkManager.SendFromClient(new byte[] { (byte)NetworkDataTypes.DataType.READY });
        }
    }

    private void Update()
    {
        if (networkManager.isClient && networkManager.client.isReady)
        {
            // networkManager.client.isReady = false;
            SceneManager.LoadScene(SceneNames.GameScene);
        }
        else if (networkManager.isServer && networkManager.server.IsReady())
        {
            // サーバからクライアントに対して準備完了を通知する
            for (int i = 0; i < Server.MAX_CLIENTS; i++)
            {
                networkManager.SendFromServer(new byte[] { (byte)NetworkDataTypes.DataType.READY }, i);
            }
            Debug.Log("sent ready to all clients");

            networkManager.server.ResetReady();
            SceneManager.LoadScene(SceneNames.GameScene);
        }
    }
}
