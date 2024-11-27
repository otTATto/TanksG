using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ContactUsButton : MonoBehaviour
{
    private Button contactButton;

    private void Awake()
    {
        contactButton = GetComponent<Button>();
        contactButton.onClick.AddListener(() => {
            SceneManager.LoadScene(SceneNames.InquiriesScene);
        });
    }
}
