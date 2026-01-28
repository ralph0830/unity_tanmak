using UnityEngine;

namespace MobileTanmak.Player
{
    /// <summary>
    /// 플레이어 충돌 판정 범위 (히트박스)
    /// 실제 스프라이트보다 작은 히트박스를 통해 공정한 판정 구현
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class PlayerHitbox : MonoBehaviour
    {
        #region Settings
        [Header("히트박스 설정")]
        [SerializeField] private float _hitboxRadius = 0.2f;      // 히트박스 반경
        [SerializeField] private bool _showGizmo = true;           // 히트박스 가이즈모 표시
        [SerializeField] private Color _gizmoColor = new Color(1, 0, 0, 0.5f); // 가이즈모 색상
        #endregion

        #region Private Fields
        private PlayerController _controller;
        private Collider2D _mainCollider;
        #endregion

        #region Properties
        /// <summary>
        /// 충돌 판정에 사용되는 히트박스 반경
        /// </summary>
        public float HitboxRadius => _hitboxRadius;

        /// <summary>
        /// 히트박스 중심 위치 (월드 좌표)
        /// </summary>
        public Vector3 HitboxCenter => transform.position;

        /// <summary>
        /// 무적 상태 (데모 연출 등)
        /// </summary>
        public bool IsInvincible { get; set; } = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _mainCollider = GetComponent<Collider2D>();
        }

        private void Start()
        {
            // 메인 콜라이더를 비활성화 (히트박스만 사용)
            if (_mainCollider != null)
            {
                _mainCollider.enabled = false;
            }
        }

        private void OnDrawGizmos()
        {
            if (_showGizmo)
            {
                Gizmos.color = _gizmoColor;
                Gizmos.DrawSphere(transform.position, _hitboxRadius);
            }
        }

        private void OnDrawGizmosSelected()
        {
            // 선택 시 더 크게 표시
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _hitboxRadius);
        }
        #endregion

        #region Collision Callbacks
        /// <summary>
        /// 트리거 충돌 감지 (Collider2D.isTrigger = true인 경우)
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsInvincible) return;

            // 탄막과의 충돌 체크
            if (other.CompareTag("Bullet"))
            {
                OnBulletHit(other);
            }
        }

        /// <summary>
        /// 탄막 충돌 처리
        /// </summary>
        private void OnBulletHit(Collider2D bulletCollider)
        {
            Bullet.Bullet bullet = bulletCollider.GetComponent<Bullet.Bullet>();
            if (bullet != null)
            {
                // CollisionManager에서 처리하므로 여기서는 로그만
                Debug.Log("PlayerHitbox: 탄막과 충돌 감지");
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 히트박스 반경 설정
        /// </summary>
        public void SetHitboxRadius(float radius)
        {
            _hitboxRadius = Mathf.Max(0.1f, radius);
        }

        /// <summary>
        /// 일시 무적 활성화
        /// </summary>
        public void SetInvincible(bool invincible, float duration = 0f)
        {
            IsInvincible = invincible;

            if (duration > 0f)
            {
                Invoke(nameof(ClearInvincible), duration);
            }
        }

        /// <summary>
        /// 무적 상태 해제
        /// </summary>
        private void ClearInvincible()
        {
            IsInvincible = false;
        }

        /// <summary>
        /// 현재 히트박스 경계 반환
        /// </summary>
        public Bounds GetHitboxBounds()
        {
            return new Bounds(HitboxCenter, Vector3.one * (_hitboxRadius * 2));
        }
        #endregion
    }
}
