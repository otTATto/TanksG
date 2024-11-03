using UnityEngine;
using System.Collections;

namespace Complete
{
    public class MineExplosion : MonoBehaviour
    {
        public LayerMask m_TankMask;
        public ParticleSystem m_ExplosionParticles;
        public AudioSource m_ExplosionAudio;
        public float m_MaxDamage = 80f;
        public float m_ExplosionForce = 1000f;
        public float m_ExplosionRadius = 10f;
        public float m_ActivationDelay = 1f; // Delay before the mine becomes active

        private Collider m_Collider;

        private void Start()
        {
            m_Collider = GetComponent<Collider>();
            m_Collider.enabled = false; // Disable the collider initially
            StartCoroutine(ActivateMine());
        }

        private IEnumerator ActivateMine()
        {
            yield return new WaitForSeconds(m_ActivationDelay);
            m_Collider.enabled = true; // Enable the collider after the delay
        }

        private void OnTriggerEnter(Collider other)
        {
            // 砲弾か戦車が接触したら爆発
            if (other.CompareTag("Shell") || other.CompareTag("Player"))
            {
                Explode();
            }
        }

        private void Explode()
        {
            // 爆発範囲内のタンクにダメージを与える
            Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

            for (int i = 0; i < colliders.Length; i++)
            {
                Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
                if (!targetRigidbody)
                    continue;

                targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

                TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();
                if (!targetHealth)
                    continue;

                float damage = CalculateDamage(targetRigidbody.position);
                targetHealth.TakeDamage(damage);
            }

            // エフェクトの再生
            m_ExplosionParticles.transform.parent = null;
            m_ExplosionParticles.Play();
            m_ExplosionAudio.Play();

            ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;
            Destroy(m_ExplosionParticles.gameObject, mainModule.duration);
            Destroy(gameObject);
        }

        private float CalculateDamage(Vector3 targetPosition)
        {
            Vector3 explosionToTarget = targetPosition - transform.position;
            float explosionDistance = explosionToTarget.magnitude;
            if (explosionDistance > m_ExplosionRadius) return 0;

            float relativeDistance = explosionDistance / m_ExplosionRadius; // [0,1]に収める
            float damage = m_MaxDamage * Mathf.Exp(-relativeDistance);
            return damage;
        }
    }
} 