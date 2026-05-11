using System;
using UnityEngine;
using UnityEngine.Events;
using RedVeil.Core;

namespace RedVeil.World
{
    /// <summary>
    /// 적의 체력을 관리하는 컴포넌트. IDamageable 구현체.
    /// 죽음 처리는 OnDeath 이벤트로 외부 시스템(애니메이터, 사운드 등)에 위임한다.
    /// </summary>
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [Header("Health")]
        [SerializeField] private float maxHealth = 100f;

        [Header("Events")]
        public UnityEvent<float> OnDamaged;        // 인자: 받은 데미지
        public UnityEvent OnDeath;
        public UnityEvent<float, float> OnHealthChanged; // 인자: current, max

        private float _currentHealth;
        public bool IsAlive => _currentHealth > 0f;
        public float HealthRatio => Mathf.Clamp01(_currentHealth / maxHealth);

        private void Awake()
        {
            _currentHealth = maxHealth;
        }

        public void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitDirection)
        {
            if (!IsAlive || amount <= 0f) return;

            _currentHealth = Mathf.Max(0f, _currentHealth - amount);
            OnDamaged?.Invoke(amount);
            OnHealthChanged?.Invoke(_currentHealth, maxHealth);

            Debug.Log($"[{name}] took {amount} damage. HP: {_currentHealth}/{maxHealth}");

            if (_currentHealth <= 0f)
            {
                OnDeath?.Invoke();
                Debug.Log($"[{name}] died.");
            }
        }
    }
}