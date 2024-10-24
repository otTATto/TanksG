using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TankWarp : MonoBehaviour
{
    private Renderer[]  _target;  // 点滅させるオブジェクトのRenderer
    public float cycle = 0.5f;  // 点滅の周期
    public float blinkingTime = 4f;
    private float timer = 0f;    // 内部の時間を追跡
    private bool isBlinking = false; // 点滅中かどうかを示すフラグ
    public bool canWarp = true;
    
     void Start()
    {
        // 子オブジェクトのRendererを取得
        _target = GetComponentsInChildren<Renderer>();
    }
        private void Update()
    {
        if (isBlinking)　//点滅の処理
        {

            timer += Time.deltaTime;

            var repeatValue = Mathf.Repeat((float)timer, cycle);
            foreach (Renderer renderer in _target) renderer.enabled = repeatValue >= cycle * 0.5f;

            if (timer >= blinkingTime) StopBlinking();　//時間になったら点滅が終わる
        }
    }

    public void StartBlinking()　//点滅処理を開始する
    {
        isBlinking = true;
        timer = 0f;
        canWarp = false;     
    }


    public void StopBlinking()  //点滅処理を終了する
    {
        isBlinking = false;
        foreach (Renderer renderer in _target) renderer.enabled = true;
        canWarp = true;
    }

}

