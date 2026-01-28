using UnityEngine;

namespace MobileTanmak.Bullet.Patterns
{
    /// <summary>
    /// 가속도 패턴
    /// 시간이 지날수록 속도가 빨라지는 탄막
    /// </summary>
    public class AcceleratingPattern : MonoBehaviour, IBulletPattern
    {
        #region Settings
        [Header("스폰 설정")]
        [SerializeField] private float _spawnInterval = 0.6f;      // 스폰 간격
        [SerializeField] private float _initialSpeed = 2f;         // 초기 속도
        [SerializeField] private float _maxSpeed = 8f;             // 최대 속도
        [SerializeField] private float _acceleration = 3f;         // 가속도

        [Header("패턴 설정")]
        [SerializeField] private bool _useWavePattern = false;     // 파동 패턴 사용
        [SerializeField] private float _waveAmplitude = 1f;        // 파동 진폭
        [SerializeField] private float _waveFrequency = 2f;        // 파동 주기
        #endregion

        #region Private Fields
        private BulletSpawner _spawner;
        private bool _isActive = false;
        private float _timer = 0f;
        private float _waveTimer = 0f;
        #endregion

        #region Properties
        public bool IsActive => _isActive;
        public string PatternName => "AcceleratingPattern";
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
            _waveTimer = 0f;
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
            _waveTimer += Time.deltaTime;

            if (_timer >= _spawnInterval)
            {
                SpawnAcceleratingBullets();
                _timer = 0f;
            }
        }
        #endregion

        #region Spawning Logic
        /// <summary>
        /// 가속도 탄막 스폰
        /// </summary>
        private void SpawnAcceleratingBullets()
        {
            Vector2 spawnPos = transform.position;

            if (_useWavePattern)
            {
                // 파동 패턴: X 위치를 사인파로 변경
                float xOffset = Mathf.Sin(_waveTimer * _waveFrequency) * _waveAmplitude;
                spawnPos.x += xOffset;
            }

            Bullet bullet = _spawner.SpawnBullet(spawnPos, Vector2.down, _initialSpeed, Bullet.BulletType.Accelerating);
            
            // 가속도 설정
            if (bullet != null)
            {
                bullet.SetAcceleration(_acceleration, _maxSpeed);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 속도 설정
        /// </summary>
        public void SetSpeed(float initialSpeed, float maxSpeed)
        {
            _initialSpeed = initialSpeed;
            _maxSpeed = maxSpeed;
        }

        /// <summary>
        /// 가속도 설정
        /// </summary>
        public void SetAcceleration(float acceleration)
        {
            _acceleration = acceleration;
        }

        /// <summary>
        /// 파동 패턴 활성화
        /// </summary>
        public void SetWavePattern(bool enabled, float amplitude = 1f, float frequency = 2f)
        {
            _useWavePattern = enabled;
            _waveAmplitude = amplitude;
            _waveFrequency = frequency;
        }
        #endregion
    }
}
