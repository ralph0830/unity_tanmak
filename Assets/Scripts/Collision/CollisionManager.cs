using UnityEngine;
using System.Collections.Generic;
using MobileTanmak.Core;

namespace MobileTanmak.Collision
{
    /// <summary>
    /// 모든 충돌 감지를 관리하는 매니저
    /// ObjectPooling과 함께 사용하여 성능 최적화
    /// </summary>
    public class CollisionManager : MonoBehaviour
    {
        #region Singleton
        private static CollisionManager _instance;
        public static CollisionManager Instance => _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Settings
        [Header("충돌 체크 설정")]
        [SerializeField] private float _checkInterval = 0.02f;  // 충돌 체크 간격 (초)
        [SerializeField] private int _maxCollisionsPerFrame = 100; // 프레임당 최대 충돌 체크 수
        #endregion

        #region Private Fields
        private Player.PlayerController _player;
        private Player.PlayerHitbox _playerHitbox;
        private List<Bullet.Bullet> _activeBullets = new List<Bullet.Bullet>();
        private float _checkTimer = 0f;
        private bool _isInitialized = false;
        #endregion

        #region Properties
        public bool IsEnabled => GameManager.Instance != null && 
                                  GameManager.Instance.CurrentState == GameManager.GameState.Playing;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if (!IsEnabled)
            {
                _activeBullets.Clear();
                return;
            }

            _checkTimer += Time.deltaTime;

            if (_checkTimer >= _checkInterval)
            {
                CheckCollisions();
                _checkTimer = 0f;
            }
        }

        private void FixedUpdate()
        {
            // 활성 탄막 목록 정리 (비활성 제거)
            CleanupInactiveBullets();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// 충돌 매니저 초기화
        /// </summary>
        private void Initialize()
        {
            // 플레이어 찾기
            _player = FindObjectOfType<Player.PlayerController>();
            if (_player == null)
            {
                Debug.LogError("CollisionManager: PlayerController를 찾을 수 없습니다!");
                return;
            }

            // 플레이어 히트박스 찾기
            _playerHitbox = _player.GetComponent<Player.PlayerHitbox>();
            if (_playerHitbox == null)
            {
                Debug.LogWarning("CollisionManager: PlayerHitbox가 없습니다. 기본 Collider2D를 사용합니다.");
            }

            _isInitialized = true;
            Debug.Log("CollisionManager 초기화 완료");

            // 게임 상태 변경 구독
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            }
        }
        #endregion

        #region Collision Detection
        /// <summary>
        /// 충돌 체크 수행
        /// </summary>
        private void CheckCollisions()
        {
            if (!_isInitialized || _player == null) return;
            if (_activeBullets.Count == 0) return;

            // 플레이어 위치
            Vector2 playerPosition = _player.transform.position;
            float playerRadius = _playerHitbox != null ? _playerHitbox.HitboxRadius : 0.3f;

            int checks = 0;

            for (int i = _activeBullets.Count - 1; i >= 0; i--)
            {
                if (checks >= _maxCollisionsPerFrame) break;

                Bullet.Bullet bullet = _activeBullets[i];
                
                // 비활성 탄막은 스킵
                if (bullet == null || !bullet.IsActive)
                {
                    _activeBullets.RemoveAt(i);
                    continue;
                }

                // 원형 충돌 체크
                Vector2 bulletPosition = bullet.transform.position;
                float bulletRadius = GetBulletRadius(bullet);
                float distance = Vector2.Distance(playerPosition, bulletPosition);

                if (distance < (playerRadius + bulletRadius))
                {
                    // 충돌 발생!
                    OnPlayerHit(bullet);
                    _activeBullets.RemoveAt(i);
                    checks++;
                }
            }
        }

        /// <summary>
        /// 탄막 반경 가져오기
        /// </summary>
        private float GetBulletRadius(Bullet.Bullet bullet)
        {
            // Collider2D의 크기를 기반으로 계산
            Collider2D collider = bullet.GetComponent<Collider2D>();
            if (collider is CircleCollider2D circleCollider)
            {
                return circleCollider.radius * Mathf.Abs(bullet.transform.localScale.x);
            }
            return 0.3f; // 기본값
        }

        /// <summary>
        /// 플레이어 피격 처리
        /// </summary>
        private void OnPlayerHit(Bullet.Bullet bullet)
        {
            // 탄막 비활성화
            bullet.Deactivate();

            // 게임 오버
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }

            Debug.Log("플레이어 피격! 게임 오버");
        }
        #endregion

        #region Bullet Registration
        /// <summary>
        /// 활성 탄막 등록 (BulletSpawner에서 호출)
        /// </summary>
        public void RegisterBullet(Bullet.Bullet bullet)
        {
            if (bullet != null && !_activeBullets.Contains(bullet))
            {
                _activeBullets.Add(bullet);
            }
        }

        /// <summary>
        /// 탄막 제거 (Bullet이 비활성화될 때 호출)
        /// </summary>
        public void UnregisterBullet(Bullet.Bullet bullet)
        {
            _activeBullets.Remove(bullet);
        }

        /// <summary>
        /// 비활성 탄막 정리
        /// </summary>
        private void CleanupInactiveBullets()
        {
            _activeBullets.RemoveAll(bullet => bullet == null || !bullet.IsActive);
        }

        /// <summary>
        /// 모든 탄막 제거
        /// </summary>
        public void ClearAllBullets()
        {
            _activeBullets.Clear();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// 게임 상태 변경 처리
        /// </summary>
        private void OnGameStateChanged(GameManager.GameState newState)
        {
            switch (newState)
            {
                case GameManager.GameState.MainMenu:
                case GameManager.GameState.GameOver:
                case GameManager.GameState.Paused:
                    ClearAllBullets();
                    break;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 플레이어 히트박스 수동 설정
        /// </summary>
        public void SetPlayerHitbox(Player.PlayerHitbox hitbox)
        {
            _playerHitbox = hitbox;
        }

        /// <summary>
        /// 충돌 체크 간격 설정
        /// </summary>
        public void SetCheckInterval(float interval)
        {
            _checkInterval = Mathf.Max(0.01f, interval);
        }

        /// <summary>
        /// 현재 활성 탄막 수 반환
        /// </summary>
        public int GetActiveBulletCount()
        {
            return _activeBullets.Count;
        }
        #endregion

        #region Cleanup
        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            }
        }
        #endregion
    }
}
