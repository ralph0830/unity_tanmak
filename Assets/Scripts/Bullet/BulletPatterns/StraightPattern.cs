using UnityEngine;

namespace MobileTanmak.Bullet.Patterns
{
    /// <summary>
    /// 직선 낙하 패턴
    /// 화면 위에서 아래로 수직으로 떨어지는 기본 탄막
    /// </summary>
    public class StraightPattern : MonoBehaviour, IBulletPattern
    {
        #region Settings
        [Header("스폰 설정")]
        [SerializeField] private float _spawnInterval = 0.5f;      // 스폰 간격
        [SerializeField] private float _bulletSpeed = 3f;          // 탄막 속도
        [SerializeField] private int _bulletsPerSpawn = 1;         // 한 번에 스폰할 탄막 수
        [SerializeField] private float _spawnWidth = 2f;           // 스폰 폭 (여러 발일 때)

        [Header("난이도 설정")]
        [SerializeField] private bool _useDifficultyScaling = true; // 시간에 따른 난이도 상승
        [SerializeField] private float _minSpawnInterval = 0.1f;    // 최소 스폰 간격
        [SerializeField] private float _speedIncreaseRate = 0.1f;   // 속도 증가율 (초당)
        #endregion

        #region Private Fields
        private BulletSpawner _spawner;
        private bool _isActive = false;
        private float _timer = 0f;
        private float _currentSpawnInterval;
        private float _currentSpeed;

        // 게임 진행 시간에 따른 난이도
        private float _gameTime = 0f;
        #endregion

        #region Properties
        public bool IsActive => _isActive;
        public string PatternName => "StraightPattern";
        #endregion

        #region IBulletPattern Implementation
        public void Initialize(BulletSpawner spawner)
        {
            _spawner = spawner;
            _currentSpawnInterval = _spawnInterval;
            _currentSpeed = _bulletSpeed;
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
            _gameTime = 0f;
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

            // 난이도 스케일링
            if (_useDifficultyScaling)
            {
                UpdateDifficulty();
            }

            // 스폰 타이머 체크
            if (_timer >= _currentSpawnInterval)
            {
                SpawnBullets();
                _timer = 0f;
            }
        }
        #endregion

        #region Spawning Logic
        /// <summary>
        /// 탄막 스폰
        /// </summary>
        private void SpawnBullets()
        {
            if (_bulletsPerSpawn == 1)
            {
                // 단일 스폰
                _spawner.SpawnBulletFromTop(_currentSpeed);
            }
            else
            {
                // 다중 스폰 (지정된 폭에 균등 분배)
                SpawnMultipleBullets();
            }
        }

        /// <summary>
        /// 여러 발의 탄막 스폰
        /// </summary>
        private void SpawnMultipleBullets()
        {
            float startX = -_spawnWidth * 0.5f;
            float step = _spawnWidth / Mathf.Max(1, _bulletsPerSpawn - 1);

            for (int i = 0; i < _bulletsPerSpawn; i++)
            {
                float x = (_bulletsPerSpawn == 1) ? 0 : startX + (step * i);
                Vector2 position = new Vector2(x, _spawner.transform.position.y);
                _spawner.SpawnBullet(position, Vector2.down, _currentSpeed);
            }
        }

        /// <summary>
        /// 난이도 업데이트 (시간 경과에 따른 스폰 빈도/속도 증가)
        /// </summary>
        private void UpdateDifficulty()
        {
            _gameTime += Time.deltaTime;

            // 스폰 간격 감소 (더 빨리 스폰)
            float intervalReduction = _gameTime * 0.01f; // 1초마다 0.01씩 감소
            _currentSpawnInterval = Mathf.Max(_minSpawnInterval, _spawnInterval - intervalReduction);

            // 속도 증가
            _currentSpeed = _bulletSpeed + (_gameTime * _speedIncreaseRate * 0.1f);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 스폰 간격 설정
        /// </summary>
        public void SetSpawnInterval(float interval)
        {
            _spawnInterval = interval;
            _currentSpawnInterval = interval;
        }

        /// <summary>
        /// 탄막 속도 설정
        /// </summary>
        public void SetBulletSpeed(float speed)
        {
            _bulletSpeed = speed;
            _currentSpeed = speed;
        }

        /// <summary>
        /// 한 번에 스폰할 탄막 수 설정
        /// </summary>
        public void SetBulletsPerSpawn(int count)
        {
            _bulletsPerSpawn = Mathf.Max(1, count);
        }

        /// <summary>
        /// 난이도 스케일링 활성화/비활성화
        /// </summary>
        public void SetDifficultyScaling(bool enabled)
        {
            _useDifficultyScaling = enabled;
        }

        /// <summary>
        /// 현재 난이도 정보 반환
        /// </summary>
        public void GetDifficultyInfo(out float currentInterval, out float currentSpeed)
        {
            currentInterval = _currentSpawnInterval;
            currentSpeed = _currentSpeed;
        }

        /// <summary>
        /// 패턴 리셋
        /// </summary>
        public void Reset()
        {
            _timer = 0f;
            _gameTime = 0f;
            _currentSpawnInterval = _spawnInterval;
            _currentSpeed = _bulletSpeed;
        }
        #endregion
    }
}
