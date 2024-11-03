using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    private Button startButton;

    private void Awake()
    {
        startButton = GetComponent<Button>();
        startButton.onClick.AddListener(() => {
            SceneManager.LoadScene(SceneNames.HomeScene);
        });
    }
}