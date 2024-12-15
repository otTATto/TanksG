// 汎用的な，ダイアログの表示・非表示を制御するスクリプト

using UnityEngine;

public class DialogController : MonoBehaviour
{
    public GameObject dialogPanel;

    // ダイアログを表示する
    public void ShowDialog()
    {
        dialogPanel.SetActive(true);
    }

    // ダイアログを非表示にする
    public void HideDialog()
    {
        dialogPanel.SetActive(false);
    }
}
