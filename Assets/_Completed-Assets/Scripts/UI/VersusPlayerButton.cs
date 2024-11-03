using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VersusPlayerButton : MonoBehaviour
{
    private Button versusPlayerButton;

    private void Awake()
    {
        versusPlayerButton = GetComponent<Button>();
        versusPlayerButton.onClick.AddListener(() => {
            SceneManager.LoadScene(SceneNames.GameScene);
        });
    }
}