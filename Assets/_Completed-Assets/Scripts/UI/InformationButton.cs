using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InformationButton : MonoBehaviour
{
    // Start is called before the first frame update
    private Button informationButton;

    private void Awake()
    {
        informationButton = GetComponent<Button>();
        informationButton.onClick.AddListener(() => {
            SceneManager.LoadScene(SceneNames.InformationScene);
        });
    }
}
