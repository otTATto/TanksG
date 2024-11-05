using System.Collections;
using UnityEngine;

public class ShellCartridge : MonoBehaviour
{
    public float lifeTime = 20f;        // カートリッジの生存時間
    public float blinkStartTime = 15f;    // 点滅開始時間
    public float cycleTime = 0.5f;      // 点滅周期
    public int shellAmount = 10;         // 補給される砲弾数

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
            var repeatValue = Mathf.Repeat((float)timer, cycleTime);
            meshRenderer.enabled = (repeatValue >= cycleTime * 0.5f);
        }

        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var tankShooting = other.GetComponent<Complete.TankShooting>();
            if (tankShooting != null && tankShooting.m_RemainingShells < tankShooting.m_ShellCapacity)
            {
                // 砲弾の残量を補給
                tankShooting.m_RemainingShells = Mathf.Min(
                    tankShooting.m_RemainingShells + shellAmount,
                    tankShooting.m_ShellCapacity
                );
                Destroy(gameObject);
                // Debug.Log(tankShooting.m_RemainingShells);
            }
        }
    }
}