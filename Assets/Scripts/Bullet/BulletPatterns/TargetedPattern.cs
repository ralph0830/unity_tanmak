using UnityEngine;

namespace MobileTanmak.Bullet.Patterns
{
    /// <summary>
    /// 타겟 추적 패턴
    /// 플레이어 방향으로 탄막을 발사
    /// </summary>
    public class TargetedPattern : MonoBehaviour, IBulletPattern
    {
        #region Settings
        [Header("스폰 설정")]
        [SerializeField] private float _spawnInterval = 0.8f;      // 스폰 간격
        [SerializeField] private float _bulletSpeed = 4f;          // 탄막 속도
        [SerializeField] private int _bulletsPerShot = 1;          // 한 번에 발사할 탄막 수
        [SerializeField] private float _spreadAngle = 0f;          // 예각 각도 (다발 발사 시)
        #endregion

        #region Private Fields
        private BulletSpawner _spawner;
        private bool _isActive = false;
        private float _timer = 0f;
        private Transform _playerTransform;
        #endregion

        #region Properties
        public bool IsActive => _isActive;
        public string PatternName => "TargetedPattern";
        #endregion

        #region IBulletPattern Implementation
        public void Initialize(BulletSpawner spawner)
        {
            _spawner = spawner;
            // 플레이어 찾기 (태그로 검색)
            FindPlayer();
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

            // 플레이어가 없으면 다시 찾기
            if (_playerTransform == null)
            {
                FindPlayer();
            }

            _timer += Time.deltaTime;

            if (_timer >= _spawnInterval)
            {
                SpawnTargetedBullets();
                _timer = 0f;
            }
        }
        #endregion

        #region Spawning Logic
        /// <summary>
        /// 플레이어를 추적하는 탄막 스폰
        /// </summary>
        private void SpawnTargetedBullets()
        {
            if (_playerTransform == null)
            {
                // 플레이어가 없으면 아래로 발사
                _spawner.SpawnBulletFromTop(_bulletSpeed, Bullet.BulletType.Targeted);
                return;
            }

            Vector2 spawnPos = transform.position;

            if (_bulletsPerShot == 1)
            {
                // 단일 추적탄
                _spawner.SpawnTargetedBullet(spawnPos, _playerTransform, _bulletSpeed, Bullet.BulletType.Targeted);
            }
            else
            {
                // 다발 추적탄 (예각 포함)
                SpawnSpreadTargetedBullets(spawnPos);
            }
        }

        /// <summary>
        /// 예각 다발 추적탄 스폰
        /// </summary>
        private void SpawnSpreadTargetedBullets(Vector2 spawnPos)
        {
            // 플레이어 방향 계산
            Vector2 toPlayer = (_playerTransform.position - (Vector3)spawnPos).normalized;
            float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

            // 예각 계산
            float startAngle = baseAngle - (_spreadAngle * 0.5f);
            float angleStep = _spreadAngle / Mathf.Max(1, _bulletsPerShot - 1);

            for (int i = 0; i < _bulletsPerShot; i++)
            {
                float angle = startAngle + (i * angleStep);
                float angleRad = angle * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
                _spawner.SpawnBullet(spawnPos, direction, _bulletSpeed, Bullet.BulletType.Targeted);
            }
        }

        /// <summary>
        /// 플레이어 찾기
        /// </summary>
        private void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 한 번에 발사할 탄막 수 설정
        /// </summary>
        public void SetBulletsPerShot(int count)
        {
            _bulletsPerShot = Mathf.Max(1, count);
        }

        /// <summary>
        /// 예각 각도 설정
        /// </summary>
        public void SetSpreadAngle(float angle)
        {
            _spreadAngle = angle;
        }

        /// <summary>
        /// 추적 대상 수동 설정
        /// </summary>
        public void SetTarget(Transform target)
        {
            _playerTransform = target;
        }
        #endregion
    }
}
