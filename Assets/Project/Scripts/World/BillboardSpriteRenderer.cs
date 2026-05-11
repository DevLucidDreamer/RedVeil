using UnityEngine;

namespace RedVeil.World
{
    /// <summary>
    /// SpriteRenderer가 항상 카메라를 향하도록 Y축 회전.
    /// 환경 빌보드 셰이더(S_BillboardY)와 달리, 이 컴포넌트는 캐릭터/적용으로
    /// SpriteRenderer + Animator 조합과 함께 사용한다.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class BillboardSpriteRenderer : MonoBehaviour
    {
        [Tooltip("Y축만 회전. 끄면 풀 빌보드(상하 회전 포함).")]
        [SerializeField] private bool yAxisOnly = true;

        [Tooltip("스프라이트가 좌측 향함 (대부분 도트는 우측 기본). 체크하면 좌우 자동 반전.")]
        [SerializeField] private bool autoFlipBasedOnCamera = true;

        [Tooltip("targetCamera 미지정 시 Camera.main 사용. PixelCamera를 직접 지정하는 게 정확.")]
        [SerializeField] private Camera targetCamera;

        private SpriteRenderer _spriteRenderer;
        private Transform _cachedTransform;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _cachedTransform = transform;

            if (targetCamera == null)
                targetCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (targetCamera == null) return;

            // 카메라 → 본인 방향 벡터
            Vector3 toCamera = targetCamera.transform.position - _cachedTransform.position;

            if (yAxisOnly)
                toCamera.y = 0f;

            if (toCamera.sqrMagnitude < 0.0001f) return;

            // 카메라를 향하는 회전 (스프라이트의 -Z가 카메라 향하도록)
            _cachedTransform.rotation = Quaternion.LookRotation(-toCamera, Vector3.up);

            // 카메라 위치에 따라 좌우 반전 (선택)
            if (autoFlipBasedOnCamera)
            {
                Vector3 localToCamera = _cachedTransform.parent != null
                    ? _cachedTransform.parent.InverseTransformDirection(toCamera)
                    : toCamera;

                // 자동 반전은 일단 비활성 — 캐릭터 방향성은 상태 머신에서 처리하는 게 더 깔끔
                // 필요시 여기 로직 추가
            }
        }
    }
}