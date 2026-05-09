using UnityEngine;

namespace RedVeil.Player
{
    /// <summary>
    /// 1인칭 카메라 헤드 보브 효과.
    /// CharacterController의 이동 속도에 따라 사인파로 카메라를 흔든다.
    /// PlayerController가 붙어있는 GameObject에 두고, 카메라 트랜스폼을 연결해서 사용.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class HeadBob : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraTransform;

        [Header("Bob Settings")]
        [Tooltip("초당 보폭 수. 클수록 빠르게 흔들림.")]
        [SerializeField] private float bobFrequency = 8f;

        [Tooltip("위아래 흔들림 크기 (미터).")]
        [SerializeField] private float verticalAmplitude = 0.04f;

        [Tooltip("좌우 흔들림 크기 (미터).")]
        [SerializeField] private float horizontalAmplitude = 0.025f;

        [Tooltip("정지에서 흔들림 시작까지 걸리는 부드러움.")]
        [SerializeField] private float smoothing = 8f;

        [Tooltip("이 속도 이상일 때만 흔들림 시작 (정지 판정).")]
        [SerializeField] private float minSpeed = 0.1f;

        private CharacterController _controller;
        private Vector3 _cameraDefaultLocalPos;
        private float _bobTimer;
        private float _currentIntensity;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();

            if (cameraTransform == null)
            {
                Debug.LogError($"[{nameof(HeadBob)}] Camera Transform이 할당되지 않았습니다.", this);
                enabled = false;
                return;
            }

            _cameraDefaultLocalPos = cameraTransform.localPosition;
        }

        private void LateUpdate()
        {
            // 수평 이동 속도만 측정 (낙하/점프 무시)
            Vector3 horizontalVelocity = _controller.velocity;
            horizontalVelocity.y = 0f;
            float speed = horizontalVelocity.magnitude;

            bool isMoving = _controller.isGrounded && speed > minSpeed;

            // 흔들림 강도 보간 (걷기 시작/멈출 때 부드럽게)
            float targetIntensity = isMoving ? 1f : 0f;
            _currentIntensity = Mathf.Lerp(_currentIntensity, targetIntensity, Time.deltaTime * smoothing);

            // 움직일 때만 타이머 진행
            if (isMoving)
                _bobTimer += Time.deltaTime * bobFrequency;

            // 사인파 계산
            float verticalOffset = Mathf.Sin(_bobTimer) * verticalAmplitude * _currentIntensity;
            float horizontalOffset = Mathf.Cos(_bobTimer * 0.5f) * horizontalAmplitude * _currentIntensity;

            // 카메라 로컬 포지션에 적용
            cameraTransform.localPosition = _cameraDefaultLocalPos + new Vector3(horizontalOffset, verticalOffset, 0f);
        }
    }
}