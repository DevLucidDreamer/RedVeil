using UnityEngine;
using UnityEngine.InputSystem;
using RedVeil.Core;

namespace RedVeil.Player
{
    /// <summary>
    /// 1인칭 총기 컨트롤러. 레이캐스트(hitscan) 방식으로 발사한다.
    /// 카메라의 forward 방향으로 레이를 쏘고, IDamageable이 있으면 데미지 적용.
    /// 손 애니메이션과 머즐 플래시는 별도 컴포넌트가 OnFired 이벤트를 듣고 처리.
    /// </summary>
    public class WeaponController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("레이캐스트 시작점이 될 카메라 (보통 PixelCamera)")]
        [SerializeField] private Camera firingCamera;

        [Header("Weapon Stats")]
        [SerializeField] private float damage = 50f;
        [SerializeField] private float range = 50f;
        [Tooltip("초당 최대 발사 횟수")]
        [SerializeField] private float fireRate = 2f;

        [Header("Layers")]
        [Tooltip("총알이 충돌할 레이어. 보통 Default + Enemy")]
        [SerializeField] private LayerMask hitMask = ~0; // 기본은 모든 레이어

        // Events: 외부 시스템(애니메이터, 사운드, UI 등)이 구독
        public event System.Action OnFired;
        public event System.Action<RaycastHit> OnHit;
        public event System.Action OnMissed;

        private PlayerInputActions _input;
        private float _nextFireTime;

        private void Awake()
        {
            if (firingCamera == null && Camera.main != null)
                firingCamera = Camera.main;

            _input = new PlayerInputActions();
        }

        private void OnEnable()
        {
            _input.Player.Enable();
            _input.Player.Fire.performed += OnFirePerformed;
        }

        private void OnDisable()
        {
            _input.Player.Fire.performed -= OnFirePerformed;
            _input.Player.Disable();
        }

        private void OnFirePerformed(InputAction.CallbackContext ctx)
        {
            TryFire();
        }

        private void TryFire()
        {
            if (Time.time < _nextFireTime) return;
            if (firingCamera == null) return;

            _nextFireTime = Time.time + (1f / fireRate);

            // 카메라 중앙에서 forward 방향으로 레이캐스트
            Ray ray = new Ray(firingCamera.transform.position, firingCamera.transform.forward);

            OnFired?.Invoke();

            if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
            {
                OnHit?.Invoke(hit);

                Debug.DrawLine(ray.origin, hit.point, Color.red, 0.5f);
                Debug.Log($"Hit: {hit.collider.name} at {hit.point}");

                // IDamageable이 있는지 확인하고 데미지 적용
                if (hit.collider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(damage, hit.point, ray.direction);
                }
            }
            else
            {
                OnMissed?.Invoke();
                Debug.DrawRay(ray.origin, ray.direction * range, Color.yellow, 0.5f);
            }
        }
    }
}