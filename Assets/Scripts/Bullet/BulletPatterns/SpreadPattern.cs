using UnityEngine;

namespace MobileTanmak.Bullet.Patterns
{
    /// <summary>
    /// 방사형 패턴
    /// 여러 방향으로 동시에 탄막을 발사
    /// </summary>
    public class SpreadPattern : MonoBehaviour, IBulletPattern
    {
        #region Settings
        [Header("스폰 설정")]
        [SerializeField] private float _spawnInterval = 1f;       // 스폰 간격
        [SerializeField] private float _bulletSpeed = 3f;          // 탄막 속도
        [SerializeField] private int _bulletCount = 8;             // 한 번에 발사할 탄막 수
        [SerializeField] private float _spreadAngle = 360f;        // 전체 퍼짐 각도
        [SerializeField] private float _centerAngle = 90f;         // 중심 각도 (아래쪽)
        #endregion

        #region Private Fields
        private BulletSpawner _spawner;
        private bool _isActive = false;
        private float _timer = 0f;
        #endregion

        #region Properties
        public bool IsActive => _isActive;
        public string PatternName => "SpreadPattern";
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

            if (_timer >= _spawnInterval)
            {
                SpawnSpreadBullets();
                _timer = 0f;
            }
        }
        #endregion

        #region Spawning Logic
        /// <summary>
        /// 방사형 탄막 스폰
        /// </summary>
        private void SpawnSpreadBullets()
        {
            Vector2 spawnPos = transform.position;
            _spawner.SpawnSpreadPattern(spawnPos, _bulletCount, _bulletSpeed, _centerAngle, _spreadAngle);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 탄막 수 설정
        /// </summary>
        public void SetBulletCount(int count)
        {
            _bulletCount = Mathf.Max(1, count);
        }

        /// <summary>
        /// 퍼짐 각도 설정
        /// </summary>
        public void SetSpreadAngle(float angle)
        {
            _spreadAngle = Mathf.Clamp(angle, 0f, 360f);
        }

        /// <summary>
        /// 중심 각도 설정
        /// </summary>
        public void SetCenterAngle(float angle)
        {
            _centerAngle = angle;
        }
        #endregion
    }
}
