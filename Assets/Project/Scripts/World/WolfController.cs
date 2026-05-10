using UnityEngine;

namespace RedVeil.World
{
    /// <summary>
    /// 늑대 적 통합 컨트롤러.
    /// EnemyHealth의 이벤트를 받아 Animator에 트리거/파라미터를 전달.
    /// 부모 Wolf GameObject에 EnemyHealth와 함께 부착.
    /// 자식의 SpriteRenderer GameObject에 있는 Animator를 참조.
    /// </summary>
    [RequireComponent(typeof(EnemyHealth))]
    public class WolfController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("자식 WolfSprite의 Animator")]
        [SerializeField] private Animator animator;

        [Header("Death Settings")]
        [Tooltip("죽은 후 콜라이더를 비활성화할지 (총알이 시체에 박히지 않게)")]
        [SerializeField] private bool disableColliderOnDeath = true;

        [Tooltip("죽은 후 시체로 남길 시간(초). 0이면 영원히 남음.")]
        [SerializeField] private float despawnDelay = 0f;

        [Header("Animator Parameters")]
        [SerializeField] private string hitTriggerName = "Hit";
        [SerializeField] private string deadBoolName = "IsDead";

        private EnemyHealth _health;
        private Collider _collider;
        private int _hitTriggerHash;
        private int _deadBoolHash;

        private void Awake()
        {
            _health = GetComponent<EnemyHealth>();
            _collider = GetComponent<Collider>();
            _hitTriggerHash = Animator.StringToHash(hitTriggerName);
            _deadBoolHash = Animator.StringToHash(deadBoolName);

            if (animator == null)
            {
                Debug.LogError($"[{nameof(WolfController)}] Animator가 할당되지 않았습니다.", this);
            }
        }

        private void OnEnable()
        {
            _health.OnDamaged.AddListener(HandleDamaged);
            _health.OnDeath.AddListener(HandleDeath);
        }

        private void OnDisable()
        {
            _health.OnDamaged.RemoveListener(HandleDamaged);
            _health.OnDeath.RemoveListener(HandleDeath);
        }

        private void HandleDamaged(float amount)
        {
            if (!_health.IsAlive) return;
            if (animator != null) animator.SetTrigger(_hitTriggerHash);
        }

        private void HandleDeath()
        {
            if (animator != null) animator.SetBool(_deadBoolHash, true);

            if (disableColliderOnDeath && _collider != null)
                _collider.enabled = false;

            if (despawnDelay > 0f)
                Destroy(gameObject, despawnDelay);
        }
    }
}