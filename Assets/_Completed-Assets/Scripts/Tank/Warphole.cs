using System.Collections;
using UnityEngine;

public class Warphole : MonoBehaviour
{
    public LayerMask m_TankMask;
    public float warpDelay = 2f;    // ワープまでの待機時間
    public Transform transHole;     // ワープ先の位置
    private bool isWarping = false;  // ワープ中かどうかのフラグ

    // OnTriggerEnterにトリガーが入ったときに呼び出されるメソッド
    private void OnTriggerEnter(Collider other)
    {
        // タンクのレイヤーがm_TankMaskに含まれているかどうかをチェック
        if (((1 << other.gameObject.layer) & m_TankMask) != 0 && !isWarping)
        {
            TankWarp tankWarp = other.GetComponent<TankWarp>();

            if (tankWarp.canWarp)
            {
                // タンクの位置にワープを開始する
                tankWarp.StartBlinking();

                // ワープ中かどうかのフラグを設定
                isWarping = true;

                // 一定時間後にワープを開始するためにコルーチンを使用
                StartCoroutine(WarpAfterDelay(other.transform));
            }
        }
    }

    // ワープまでの待機時間の間に呼び出されるコルーチン
    private IEnumerator WarpAfterDelay(Transform target)
    {
        // 待機時間を待つ
        yield return new WaitForSeconds(warpDelay);

        // ワープを開始する
        StartWarp(target);

        // ワープが終了したことをフラグで設定
        isWarping = false;
    }

    // ワープを開始するメソッド
    private void StartWarp(Transform target)
    {
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
        if (transHole != null)
        {
            // ワープ先の位置にタンクを移動
            targetRigidbody.MovePosition(transHole.position);
        }
    }
}

