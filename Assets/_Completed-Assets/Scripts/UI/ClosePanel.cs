using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ClosePanel : MonoBehaviour
{
    public GameObject contactFormPanel;
    private Button Button1;

    private void Awake()
    {
        Button1 = GetComponent<Button>();
        Button1.onClick.AddListener(() =>
        {
            contactFormPanel.SetActive(false);
        });
    }
}