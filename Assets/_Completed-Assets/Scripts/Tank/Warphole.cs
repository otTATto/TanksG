using System.Collections;
using UnityEngine;

public class Warphole : MonoBehaviour
{
    public LayerMask m_TankMask;
    public float warpDelay = 2f;    // ワープまでの待機時間
    public Transform transHole;     // ワープ先の位置
    private bool isWarping = false; // ワープ中かどうかのフラグ

    // OnTriggerEnterにタイマー処理とワープ処理を統合
    private void OnTriggerEnter(Collider other)
    {
        // 衝突したオブジェクトがm_TankMaskに含まれているかチェック
        if (((1 << other.gameObject.layer) & m_TankMask) != 0 && !isWarping)
        {
            TankWarp tankWarp = other.GetComponent<TankWarp>();

            if (tankWarp.canWarp)
            {
                // 戦車の点滅を開始し、ワープ準備
                tankWarp.StartBlinking();

                // ワープ中であることを示すフラグを設定
                isWarping = true;

                // コルーチンを開始して、タイマー後にワープを実行
                StartCoroutine(WarpAfterDelay(other.transform));
            }
        }
    }

    // ワープするまでの待機処理を行うコルーチン
    private IEnumerator WarpAfterDelay(Transform target)
    {
        // 指定した時間だけ待機
        yield return new WaitForSeconds(warpDelay);

        // ワープ処理を開始
        StartWarp(target);

        // ワープが完了したのでフラグをリセット
        isWarping = false;
    }

    // ワープ処理を行う
    private void StartWarp(Transform target)
    {
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
        if (transHole != null)
        {
            // ワープ先の位置にターゲットを移動
            targetRigidbody.MovePosition(transHole.position);
        }
    }
}

