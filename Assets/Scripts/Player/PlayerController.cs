using UnityEngine;
using MobileTanmak.Core;

namespace MobileTanmak.Player
{
    /// <summary>
    /// 터치 드래그로 플레이어를 이동시키는 컨트롤러
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerController : MonoBehaviour
    {
        #region Settings
        [Header("이동 설정")]
        [SerializeField] private float _moveSpeed = 10f;
        [SerializeField] private float _smoothTime = 0.1f;

        [Header(" boundaries (카메라 기준 자동 계산)")]
        [SerializeField] private Vector2 _minBounds = Vector2.zero;
        [SerializeField] private Vector2 _maxBounds = Vector2.zero;
        #endregion

        #region Private Fields
        private Camera _mainCamera;
        private Vector3 _targetPosition;
        private Vector3 _velocity = Vector3.zero;
        private bool _isTouching = false;

        // 화면 내 플레이어 패딩 (화면 가장자리에서 약간 떨어짐)
        private float _screenPadding = 0.5f;
        #endregion

        #region Properties
        public bool IsTouching => _isTouching;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                Debug.LogError("Main Camera를 찾을 수 없습니다!");
            }

            // 초기 위치 설정
            _targetPosition = transform.position;
        }

        private void Start()
        {
            // 화면 경계 계산
            CalculateScreenBounds();
        }

        private void Update()
        {
            HandleInput();
            UpdateMovement();
        }

        private void OnDrawGizmosSelected()
        {
            // 경계 가이즈모
            Gizmos.color = Color.green;
            Vector3 center = new Vector3((_minBounds.x + _maxBounds.x) * 0.5f, (_minBounds.y + _maxBounds.y) * 0.5f, 0);
            Vector3 size = new Vector3(_maxBounds.x - _minBounds.x, _maxBounds.y - _minBounds.y, 0);
            Gizmos.DrawWireCube(center, size);
        }
        #endregion

        #region Input Handling
        /// <summary>
        /// 터치/마우스 입력 처리
        /// </summary>
        private void HandleInput()
        {
            // 유니티 에디터에서는 마우스, 모바일에서는 터치 처리
            if (Input.touchCount > 0)
            {
                HandleTouchInput();
            }
            else
            {
                HandleMouseInput();
            }
        }

        /// <summary>
        /// 터치 입력 처리 (모바일)
        /// </summary>
        private void HandleTouchInput()
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _isTouching = true;
                    UpdateTargetPosition(touch.position);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (_isTouching)
                    {
                        UpdateTargetPosition(touch.position);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    _isTouching = false;
                    // 터치를 놓았을 때 현재 위치를 타겟으로 설정
                    _targetPosition = transform.position;
                    break;
            }
        }

        /// <summary>
        /// 마우스 입력 처리 (에디터 테스트용)
        /// </summary>
        private void HandleMouseInput()
        {
            if (Input.GetMouseButton(0))
            {
                _isTouching = true;
                UpdateTargetPosition(Input.mousePosition);
            }
            else
            {
                _isTouching = false;
                _targetPosition = transform.position;
            }
        }

        /// <summary>
        /// 화면 좌표를 월드 좌표로 변환하여 타겟 위치 업데이트
        /// </summary>
        private void UpdateTargetPosition(Vector3 screenPosition)
        {
            if (_mainCamera == null) return;

            Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0f; // 2D 게임이므로 Z는 0

            // 경계 내로 클램프
            worldPosition.x = Mathf.Clamp(worldPosition.x, _minBounds.x, _maxBounds.x);
            worldPosition.y = Mathf.Clamp(worldPosition.y, _minBounds.y, _maxBounds.y);

            _targetPosition = worldPosition;
        }
        #endregion

        #region Movement
        /// <summary>
        /// 부드러운 이동 업데이트
        /// </summary>
        private void UpdateMovement()
        {
            if (_isTouching)
            {
                // SmoothDamp를 사용한 부드러운 이동
                transform.position = Vector3.SmoothDamp(
                    transform.position,
                    _targetPosition,
                    ref _velocity,
                    _smoothTime,
                    _moveSpeed
                );
            }
        }

        /// <summary>
        /// 카메라를 기준으로 화면 경계 계산
        /// </summary>
        private void CalculateScreenBounds()
        {
            if (_mainCamera == null) return;

            // 카메라의 절두체 경계 계산
            float cameraHeight = _mainCamera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * _mainCamera.aspect;

            // 플레이어 스프라이트 크기 고려하여 패딩 조정
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
                _screenPadding = Mathf.Max(spriteSize.x, spriteSize.y) * 0.5f;
            }

            // 경계 설정
            _minBounds = new Vector2(
                -cameraWidth * 0.5f + _screenPadding,
                -cameraHeight * 0.5f + _screenPadding
            );

            _maxBounds = new Vector2(
                cameraWidth * 0.5f - _screenPadding,
                cameraHeight * 0.5f - _screenPadding
            );
        }

        /// <summary>
        /// 외부에서 경계를 갱신해야 할 때 호출 (카메라 설정 변경 등)
        /// </summary>
        public void RefreshBounds()
        {
            CalculateScreenBounds();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 플레이어를 지정된 위치로 순간 이동
        /// </summary>
        public void Teleport(Vector3 position)
        {
            transform.position = position;
            _targetPosition = position;
        }

        /// <summary>
        /// 플레이어를 초기 시작 위치로 리셋
        /// </summary>
        public void ResetPosition()
        {
            transform.position = Vector3.zero;
            _targetPosition = Vector3.zero;
        }
        #endregion
    }
}
