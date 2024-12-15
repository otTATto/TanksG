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
        // サーバに対してplayerIdを要求する
        Client.instance.Send(new byte[] { (byte)NetworkDataTypes.DataType.GET_PLAYER_ID });   
    }

    private void Update()
    {
        if (Client.instance.isReady)
        {
            Client.instance.isReady = false;
            SceneManager.LoadScene(SceneNames.GameScene);
        }
    }
}
