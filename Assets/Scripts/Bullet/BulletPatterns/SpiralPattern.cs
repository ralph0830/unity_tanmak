using UnityEngine;

namespace MobileTanmak.Bullet.Patterns
{
    /// <summary>
    /// 나선형 패턴
    /// 중심에서 나선형으로 퍼져나가는 탄막
    /// </summary>
    public class SpiralPattern : MonoBehaviour, IBulletPattern
    {
        #region Settings
        [Header("스폰 설정")]
        [SerializeField] private float _spawnInterval = 0.1f;      // 스폰 간격
        [SerializeField] private float _bulletSpeed = 3f;          // 탄막 속도
        [SerializeField] private Vector2 _centerOffset = Vector2.zero; // 중심점 오프셋

        [Header("나선형 설정")]
        [SerializeField] private float _rotationSpeed = 180f;      // 회전 속도 (도/초)
        [SerializeField] private int _arms = 1;                    // 나선 팔 개수
        [SerializeField] private bool _clockwise = true;           // 시계 방향 여부
        #endregion

        #region Private Fields
        private BulletSpawner _spawner;
        private bool _isActive = false;
        private float _timer = 0f;
        private float _currentAngle = 0f;
        #endregion

        #region Properties
        public bool IsActive => _isActive;
        public string PatternName => "SpiralPattern";
        #endregion

        #region IBulletPattern Implementation
        public void Initialize(BulletSpawner spawner)
        {
            _spawner = spawner;
        }

        public void Execute()
        {
            if (_spawner == null)
            {
                Debug.LogError($"{PatternName}: BulletSpawner가 초기화되지 않았습니다.");
                return;
            }

            _isActive = true;
            _timer = 0f;
            _currentAngle = 0f;
        }

        public void Stop()
        {
            _isActive = false;
        }
        #endregion

        #region Unity Lifecycle
        private void Update()
        {
            if (!_isActive) return;

            _timer += Time.deltaTime;

            // 각도 업데이트
            float direction = _clockwise ? 1f : -1f;
            _currentAngle += _rotationSpeed * direction * Time.deltaTime;

            // 스폰
            if (_timer >= _spawnInterval)
            {
                SpawnSpiralBullets();
                _timer = 0f;
            }
        }
        #endregion

        #region Spawning Logic
        /// <summary>
        /// 나선형 탄막 스폰
        /// </summary>
        private void SpawnSpiralBullets()
        {
            float angleStep = 360f / _arms;

            for (int i = 0; i < _arms; i++)
            {
                float angle = _currentAngle + (i * angleStep);
                Vector2 spawnPos = (Vector2)transform.position + _centerOffset;
                _spawner.SpawnBullet(spawnPos, angle, _bulletSpeed);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 회전 속도 설정
        /// </summary>
        public void SetRotationSpeed(float speed)
        {
            _rotationSpeed = speed;
        }

        /// <summary>
        /// 나선 팔 개수 설정
        /// </summary>
        public void SetArms(int count)
        {
            _arms = Mathf.Max(1, count);
        }

        /// <summary>
        /// 중심점 오프셋 설정
        /// </summary>
        public void SetCenterOffset(Vector2 offset)
        {
            _centerOffset = offset;
        }
        #endregion
    }
}
