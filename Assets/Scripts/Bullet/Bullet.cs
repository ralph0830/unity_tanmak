using UnityEngine;

namespace MobileTanmak.Bullet
{
    /// <summary>
    /// 개별 탄막 동작을 제어하는 클래스
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Bullet : MonoBehaviour
    {
        #region Settings
        [Header("이동 설정")]
        [SerializeField] private float _speed = 5f;
        [SerializeField] private Vector2 _direction = Vector2.down;

        [Header("가속도 설정")]
        [SerializeField] private bool _useAcceleration = false;
        [SerializeField] private float _acceleration = 1f;
        [SerializeField] private float _maxSpeed = 10f;

        [Header("회전 설정")]
        [SerializeField] private bool _rotateToDirection = true;
        [SerializeField] private float _rotationSpeed = 0f;

        [Header("생명주기")]
        [SerializeField] private float _lifetime = 10f;
        #endregion

        #region Private Fields
        private float _currentSpeed;
        private float _age = 0f;
        private Collider2D _collider;
        private SpriteRenderer _spriteRenderer;

        // 탄막 타입 (나중에 패턴별로 다른 효과 적용 가능)
        public enum BulletType
        {
            Normal,
            Spiral,
            Targeted,
            Accelerating
        }

        private BulletType _type = BulletType.Normal;
        #endregion

        #region Properties
        public float Speed => _currentSpeed;
        public Vector2 Direction => _direction.normalized;
        public BulletType Type => _type;
        public bool IsActive { get; private set; } = true;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            // 초기화
            _age = 0f;
            _currentSpeed = _speed;
            IsActive = true;

            // 콜라이더 활성화
            if (_collider != null) _collider.enabled = true;
            if (_spriteRenderer != null) _spriteRenderer.enabled = true;

            // 방향으로 회전
            if (_rotateToDirection && _direction != Vector2.zero)
            {
                float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
            }
        }

        private void Update()
        {
            if (!IsActive) return;

            // 나이 증가
            _age += Time.deltaTime;

            // 생명주기 종료 체크
            if (_age >= _lifetime)
            {
                Deactivate();
                return;
            }

            // 가속도 적용
            if (_useAcceleration)
            {
                _currentSpeed = Mathf.Min(_currentSpeed + _acceleration * Time.deltaTime, _maxSpeed);
            }

            // 이동
            Move();

            // 회전
            if (_rotationSpeed != 0f)
            {
                transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);
            }
        }

        private void OnBecameInvisible()
        {
            // 화면 밖으로 나가면 비활성화 (ObjectPool 재사용을 위해)
            Deactivate();
        }
        #endregion

        #region Movement
        /// <summary>
        /// 탄막 이동
        /// </summary>
        private void Move()
        {
            transform.position += (Vector3)_direction * _currentSpeed * Time.deltaTime;
        }

        /// <summary>
        /// 이동 방향 설정
        /// </summary>
        public void SetDirection(Vector2 direction)
        {
            _direction = direction.normalized;

            // 방향으로 회전
            if (_rotateToDirection && _direction != Vector2.zero)
            {
                float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
            }
        }

        /// <summary>
        /// 속도 설정
        /// </summary>
        public void SetSpeed(float speed)
        {
            _speed = speed;
            _currentSpeed = speed;
        }

        /// <summary>
        /// 가속도 설정
        /// </summary>
        public void SetAcceleration(float acceleration, float maxSpeed = 10f)
        {
            _useAcceleration = true;
            _acceleration = acceleration;
            _maxSpeed = maxSpeed;
        }
        #endregion

        #region Lifecycle Management
        /// <summary>
        /// 탄막 비활성화 (ObjectPool에서 재사용)
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 탄막 초기화 설정
        /// </summary>
        public void Initialize(Vector2 position, Vector2 direction, float speed, BulletType type = BulletType.Normal)
        {
            transform.position = position;
            _direction = direction.normalized;
            _speed = speed;
            _currentSpeed = speed;
            _type = type;
            _age = 0f;
            IsActive = true;

            // 방향으로 회전
            if (_rotateToDirection && _direction != Vector2.zero)
            {
                float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
            }
        }

        /// <summary>
        /// 탄막 타입 설정
        /// </summary>
        public void SetType(BulletType type)
        {
            _type = type;
        }
        #endregion

        #region Collision Events
        /// <summary>
        /// 다른 오브젝트와 충돌 시 호출 (CollisionManager에서 사용)
        /// </summary>
        public void OnCollision()
        {
            // 충돌 처리는 CollisionManager에서 수행
            Deactivate();
        }
        #endregion
    }
}
