using UnityEngine;
using System.Collections.Generic;
using MobileTanmak.Core;

namespace MobileTanmak.Bullet
{
    /// <summary>
    /// 탄막을 생성하고 관리하는 스포너
    /// ObjectPool을 사용하여 성능 최적화
    /// </summary>
    public class BulletSpawner : MonoBehaviour
    {
        #region Settings
        [Header("탄막 프리팹")]
        [SerializeField] private GameObject _bulletPrefab;

        [Header("풀링 설정")]
        [SerializeField] private int _initialPoolSize = 100;
        [SerializeField] private int _maxPoolSize = 500;

        [Header("스폰 영역")]
        [SerializeField] private float _spawnWidth = 8f;
        [SerializeField] private float _spawnHeight = 2f;
        [SerializeField] private float _spawnYPosition = 5f;
        #endregion

        #region Private Fields
        private Queue<Bullet> _bulletPool = new Queue<Bullet>();
        private List<Bullet> _activeBullets = new List<Bullet>();
        private Camera _mainCamera;
        private bool _isInitialized = false;
        #endregion

        #region Properties
        public int ActiveBulletCount => _activeBullets.Count;
        public int PoolCount => _bulletPool.Count;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Start()
        {
            InitializePool();
        }

        private void OnDestroy()
        {
            // 풀 정리
            ClearPool();
        }

        private void OnDrawGizmosSelected()
        {
            // 스폰 영역 가이즈모
            Gizmos.color = Color.red;
            Vector3 center = new Vector3(0, _spawnYPosition, 0);
            Vector3 size = new Vector3(_spawnWidth, _spawnHeight, 0);
            Gizmos.DrawWireCube(center, size);
        }
        #endregion

        #region Object Pool Initialization
        /// <summary>
        /// 탄막 오브젝트 풀 초기화
        /// </summary>
        private void InitializePool()
        {
            if (_isInitialized) return;

            // 프리팹이 없으면 기본 탄막 생성
            if (_bulletPrefab == null)
            {
                CreateDefaultBulletPrefab();
            }

            // 초기 풀 생성
            for (int i = 0; i < _initialPoolSize; i++)
            {
                Bullet bullet = CreateNewBullet();
                _bulletPool.Enqueue(bullet);
            }

            _isInitialized = true;
            Debug.Log($"BulletSpawner 초기화 완료: 풀 크기 {_initialPoolSize}");
        }

        /// <summary>
        /// 기본 탄막 프리팹 생성 (프리팹 미설정 시)
        /// </summary>
        private void CreateDefaultBulletPrefab()
        {
            // 기본 탄막 GameObject 생성
            GameObject bulletObj = new GameObject("PooledBullet");
            bulletObj.transform.SetParent(transform);

            // Circle Collider2D 추가 (Trigger로 설정)
            CircleCollider2D collider = bulletObj.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.3f;

            // SpriteRenderer 추가
            SpriteRenderer renderer = bulletObj.AddComponent<SpriteRenderer>();
            
            // 기본 서클 스프라이트 생성
            bulletObj.AddComponent<Bullet>();

            _bulletPrefab = bulletObj;
            Debug.LogWarning("Bullet 프리팹이 설정되지 않아 기본 탄막을 생성했습니다.");
        }

        /// <summary>
        /// 새로운 탄막 생성
        /// </summary>
        private Bullet CreateNewBullet()
        {
            GameObject bulletObj = Instantiate(_bulletPrefab, transform);
            bulletObj.SetActive(false);

            Bullet bullet = bulletObj.GetComponent<Bullet>();
            if (bullet == null)
            {
                bullet = bulletObj.AddComponent<Bullet>();
            }

            return bullet;
        }
        #endregion

        #region Bullet Spawning
        /// <summary>
        /// 탄막 스폰 (기본)
        /// </summary>
        public Bullet SpawnBullet(Vector2 position, Vector2 direction, float speed, Bullet.BulletType type = Bullet.BulletType.Normal)
        {
            Bullet bullet = GetBulletFromPool();
            if (bullet == null)
            {
                Debug.LogWarning("탄막 풀이 가득 찼습니다.");
                return null;
            }

            bullet.transform.position = position;
            bullet.Initialize(position, direction, speed, type);
            bullet.gameObject.SetActive(true);

            _activeBullets.Add(bullet);

            return bullet;
        }

        /// <summary>
        /// 탄막 스폰 (오버로드 - 회전 각도 지정)
        /// </summary>
        public Bullet SpawnBullet(Vector2 position, float angleDegrees, float speed, Bullet.BulletType type = Bullet.BulletType.Normal)
        {
            float angleRadians = angleDegrees * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
            return SpawnBullet(position, direction, speed, type);
        }

        /// <summary>
        /// 화면 상단 랜덤 위치에서 탄막 스폰
        /// </summary>
        public Bullet SpawnBulletFromTop(float speed, Bullet.BulletType type = Bullet.BulletType.Normal)
        {
            Vector2 spawnPosition = GetRandomSpawnPosition();
            Vector2 direction = Vector2.down;
            return SpawnBullet(spawnPosition, direction, speed, type);
        }

        /// <summary>
        /// 랜덤 스폰 위치 계산
        /// </summary>
        private Vector2 GetRandomSpawnPosition()
        {
            float randomX = Random.Range(-_spawnWidth * 0.5f, _spawnWidth * 0.5f);
            return new Vector2(randomX, _spawnYPosition);
        }
        #endregion

        #region Object Pool Management
        /// <summary>
        /// 풀에서 탄막 가져오기
        /// </summary>
        private Bullet GetBulletFromPool()
        {
            // 풀에 사용 가능한 탄막이 있으면 재사용
            if (_bulletPool.Count > 0)
            {
                return _bulletPool.Dequeue();
            }

            // 풀이 비어있고 최대 크기에 도달하지 않았으면 새로 생성
            if (_activeBullets.Count + _bulletPool.Count < _maxPoolSize)
            {
                return CreateNewBullet();
            }

            return null;
        }

        /// <summary>
        /// 탄막을 풀로 반환
        /// </summary>
        public void ReturnBulletToPool(Bullet bullet)
        {
            if (bullet == null) return;

            bullet.gameObject.SetActive(false);
            _activeBullets.Remove(bullet);
            _bulletPool.Enqueue(bullet);
        }

        /// <summary>
        /// 모든 활성 탄막 제거
        /// </summary>
        public void ClearAllBullets()
        {
            foreach (Bullet bullet in _activeBullets)
            {
                if (bullet != null)
                {
                    bullet.gameObject.SetActive(false);
                    _bulletPool.Enqueue(bullet);
                }
            }
            _activeBullets.Clear();
        }

        /// <summary>
        /// 풀 전체 정리
        /// </summary>
        private void ClearPool()
        {
            foreach (Bullet bullet in _bulletPool)
            {
                if (bullet != null)
                {
                    Destroy(bullet.gameObject);
                }
            }
            _bulletPool.Clear();

            foreach (Bullet bullet in _activeBullets)
            {
                if (bullet != null)
                {
                    Destroy(bullet.gameObject);
                }
            }
            _activeBullets.Clear();
        }
        #endregion

        #region Pattern Spawning Helpers
        /// <summary>
        /// 원형 패턴으로 탄막 스폰
        /// </summary>
        public void SpawnCirclePattern(Vector2 center, int count, float speed, float startAngle = 0f)
        {
            float angleStep = 360f / count;

            for (int i = 0; i < count; i++)
            {
                float angle = startAngle + (i * angleStep);
                SpawnBullet(center, angle, speed);
            }
        }

        /// <summary>
        /// 방사형 패턴 (지정된 각도 범위 내)
        /// </summary>
        public void SpawnSpreadPattern(Vector2 center, int count, float speed, float centerAngle, float spreadAngle)
        {
            float startAngle = centerAngle - (spreadAngle * 0.5f);
            float angleStep = spreadAngle / Mathf.Max(1, count - 1);

            for (int i = 0; i < count; i++)
            {
                float angle = startAngle + (i * angleStep);
                SpawnBullet(center, angle, speed);
            }
        }

        /// <summary>
        /// 플레이어를 향하는 탄막 스폰
        /// </summary>
        public Bullet SpawnTargetedBullet(Vector2 spawnPosition, Transform target, float speed, Bullet.BulletType type = Bullet.BulletType.Targeted)
        {
            if (target == null) return null;

            Vector2 direction = (target.position - (Vector3)spawnPosition).normalized;
            return SpawnBullet(spawnPosition, direction, speed, type);
        }
        #endregion

        #region Public Utility
        /// <summary>
        /// 스폰 영역 크기 설정 (화면 크기 변경 시 호출)
        /// </summary>
        public void SetSpawnArea(float width, float height, float yPosition)
        {
            _spawnWidth = width;
            _spawnHeight = height;
            _spawnYPosition = yPosition;
        }

        /// <summary>
        /// 현재 활성 탄막 수가 최대인지 확인
        /// </summary>
        public bool IsPoolFull()
        {
            return _activeBullets.Count + _bulletPool.Count >= _maxPoolSize;
        }
        #endregion
    }
}
