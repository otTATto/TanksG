using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BacktoHome : MonoBehaviour
{
    // Start is called before the first frame update
    private Button BackButton;

    private void Awake()
    {
        BackButton = GetComponent<Button>();
        BackButton.onClick.AddListener(() => {
            SceneManager.LoadScene(SceneNames.HomeScene);
        });
    }
}
