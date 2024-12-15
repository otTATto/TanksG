using System.Collections;
using UnityEngine;

public class MineCartridge : MonoBehaviour
{
    public float lifeTime = 20f;        // カートリッジの生存時間
    public float blinkStartTime = 15f;    // 点滅開始時間
    public float cycleTime = 0.5f;      // 点滅周期
    public int mineAmount = 1;         // 補給される砲弾数

    private MeshRenderer meshRenderer;
    private float timer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= blinkStartTime)
        {
            // 点滅開始
            var repeatValue = Mathf.Repeat(timer, cycleTime);
            meshRenderer.enabled = repeatValue >= cycleTime * 0.5f;
        }

        if (timer >= lifeTime)
        {
            int objectId = int.Parse(gameObject.name.Split('_')[1]);
            Client.instance.DestroyNetworkObject(objectId);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var tankShooting = other.GetComponent<Complete.TankShooting>();
            if (tankShooting != null && tankShooting.m_RemainingMines < tankShooting.m_MineCapacity)
            {
                // 砲弾の残量を補給
                tankShooting.m_RemainingMines = Mathf.Min(
                    tankShooting.m_RemainingMines + mineAmount,
                    tankShooting.m_MineCapacity
                );
                int objectId = int.Parse(gameObject.name.Split('_')[1]);
                // 取得したのが自分のtankなら削除データを送信
                bool sendDestroyData = tankShooting.m_PlayerNumber == Client.instance.playerId + 1;
                Client.instance.DestroyNetworkObject(objectId, sendDestroyData);
            }
        }
    }
}