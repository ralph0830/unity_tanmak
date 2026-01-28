using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MobileTanmak.Core;

namespace MobileTanmak.UI
{
    /// <summary>
    /// 게임 플레이 중 UI (점수 표시)
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        #region UI References
        [Header("점수 표시")]
        [SerializeField] private TextMeshProUGUI _survivalTimeText;
        [SerializeField] private TextMeshProUGUI _dodgedBulletsText;

        [Header("표시 형식")]
        [SerializeField] private string _timeFormat = "mm:ss";
        [SerializeField] private bool _showMilliseconds = false;
        #endregion

        #region Private Fields
        private bool _isInitialized = false;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if (!_isInitialized) return;
            if (GameManager.Instance == null) return;

            // 실시간 업데이트
            UpdateScoreDisplay();
        }

        private void OnEnable()
        {
            // 이벤트 구독
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.OnGameStarted += OnGameStarted;
                GameEvents.Instance.OnGameOver += OnGameOver;
            }
        }

        private void OnDisable()
        {
            // 이벤트 구독 해제
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.OnGameStarted -= OnGameStarted;
                GameEvents.Instance.OnGameOver -= OnGameOver;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// UI 초기화
        /// </summary>
        private void Initialize()
        {
            // UI 컴포넌트 자동 찾기 (미설정 시)
            if (_survivalTimeText == null)
            {
                _survivalTimeText = FindChildByName<TextMeshProUGUI>(transform, "SurvivalTimeText");
            }
            if (_dodgedBulletsText == null)
            {
                _dodgedBulletsText = FindChildByName<TextMeshProUGUI>(transform, "DodgedBulletsText");
            }

            _isInitialized = true;
            UpdateScoreDisplay();
        }

        /// <summary>
        /// 자식 오브젝트에서 컴포넌트 찾기
        /// </summary>
        private T FindChildByName<T>(Transform parent, string name) where T : Component
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name == name)
                {
                    T component = child.GetComponent<T>();
                    if (component != null) return component;
                }

                // 재귀 검색
                T found = FindChildByName<T>(child, name);
                if (found != null) return found;
            }
            return null;
        }
        #endregion

        #region Display Updates
        /// <summary>
        /// 점수 표시 업데이트
        /// </summary>
        private void UpdateScoreDisplay()
        {
            if (GameManager.Instance == null) return;

            // 생존 시간
            if (_survivalTimeText != null)
            {
                _survivalTimeText.text = FormatSurvivalTime(GameManager.Instance.SurvivalTime);
            }

            // 회피한 탄막 수
            if (_dodgedBulletsText != null)
            {
                _dodgedBulletsText.text = GameManager.Instance.DodgedBullets.ToString();
            }
        }

        /// <summary>
        /// 생존 시간 포맷팅
        /// </summary>
        private string FormatSurvivalTime(float time)
        {
            if (_showMilliseconds)
            {
                int minutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);
                int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);
                return $"{minutes:00}:{seconds:00}.{milliseconds:00}";
            }
            else
            {
                return GameManager.Instance.GetFormattedSurvivalTime();
            }
        }
        #endregion

        #region Event Handlers
        private void OnGameStarted()
        {
            // 게임 시작 시 표시 초기화
            UpdateScoreDisplay();
        }

        private void OnGameOver(float survivalTime, int dodgedBullets)
        {
            // Game Over UI는 ResultUI에서 처리
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// UI 표시/숨김
        /// </summary>
        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        /// <summary>
        /// 수동으로 점수 표시 업데이트
        /// </summary>
        public void RefreshDisplay()
        {
            UpdateScoreDisplay();
        }
        #endregion
    }
}
