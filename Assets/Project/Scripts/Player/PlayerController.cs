using UnityEngine;
using UnityEngine.InputSystem;

namespace RedVeil.Player
{
    /// <summary>
    /// 1인칭 플레이어 이동 및 카메라 회전 컨트롤러.
    /// CharacterController 기반, 새 Input System 사용.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float gravity = -9.81f;

        [Header("Look")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float lookSensitivity = 0.15f;
        [SerializeField] private float maxLookAngle = 85f;

        private CharacterController _controller;
        private PlayerInputActions _input;
        private Vector2 _moveInput;
        private Vector2 _lookInput;
        private float _verticalVelocity;
        private float _cameraPitch;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _input = new PlayerInputActions();

            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;
        }

        private void OnEnable()
        {
            _input.Player.Enable();
            _input.Player.Move.performed += OnMove;
            _input.Player.Move.canceled += OnMove;
            _input.Player.Look.performed += OnLook;
            _input.Player.Look.canceled += OnLook;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            _input.Player.Move.performed -= OnMove;
            _input.Player.Move.canceled -= OnMove;
            _input.Player.Look.performed -= OnLook;
            _input.Player.Look.canceled -= OnLook;
            _input.Player.Disable();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void OnMove(InputAction.CallbackContext ctx) => _moveInput = ctx.ReadValue<Vector2>();
        private void OnLook(InputAction.CallbackContext ctx) => _lookInput = ctx.ReadValue<Vector2>();

        private void Update()
        {
            HandleLook();
            HandleMove();
        }

        private void HandleLook()
        {
            // 좌우 회전: 플레이어 본체를 돌림
            transform.Rotate(Vector3.up * _lookInput.x * lookSensitivity);

            // 상하 회전: 카메라만 돌림 (피치 제한)
            _cameraPitch -= _lookInput.y * lookSensitivity;
            _cameraPitch = Mathf.Clamp(_cameraPitch, -maxLookAngle, maxLookAngle);
            if (cameraTransform != null)
                cameraTransform.localEulerAngles = new Vector3(_cameraPitch, 0f, 0f);
        }

        private void HandleMove()
        {
            // 입력을 월드 방향으로 변환
            Vector3 direction = transform.right * _moveInput.x + transform.forward * _moveInput.y;
            Vector3 horizontal = direction * moveSpeed;

            // 중력 처리
            if (_controller.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;
            else
                _verticalVelocity += gravity * Time.deltaTime;

            Vector3 velocity = horizontal + Vector3.up * _verticalVelocity;
            _controller.Move(velocity * Time.deltaTime);
        }
    }
}