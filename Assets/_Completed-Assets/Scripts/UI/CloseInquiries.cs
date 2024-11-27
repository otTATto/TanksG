using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class CloseInquires : MonoBehaviour
{
    public GameObject contactFormPanel;
    private Button Button1;
    public Transform content; // ScrollViewのContentオブジェクト

    private void Awake()
    {
        Button1 = GetComponent<Button>();
        Button1.onClick.AddListener(() =>
        {
            // Content内のすべての子オブジェクトを削除
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }
            contactFormPanel.SetActive(false);
        });
    }
}