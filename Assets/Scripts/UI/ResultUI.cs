using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MobileTanmak.Core;

namespace MobileTanmak.UI
{
    /// <summary>
    /// 게임 오버 결과 화면 UI
    /// </summary>
    public class ResultUI : MonoBehaviour
    {
        #region UI References
        [Header("결과 표시")]
        [SerializeField] private TextMeshProUGUI _gameOverText;
        [SerializeField] private TextMeshProUGUI _survivalTimeText;
        [SerializeField] private TextMeshProUGUI _dodgedBulletsText;
        [SerializeField] private TextMeshProUGUI _highScoreText;
        [SerializeField] private TextMeshProUGUI _newRecordText;

        [Header("버튼")]
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _mainMenuButton;

        [Header("애니메이션")]
        [SerializeField] private float _showDelay = 0.5f;
        [SerializeField] private bool _useAnimation = true;
        #endregion

        #region Private Fields
        private CanvasGroup _canvasGroup;
        private float _currentSurvivalTime;
        private int _currentDodgedBullets;
        #endregion

        #region Properties
        public bool IsVisible => gameObject.activeSelf;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void Start()
        {
            Initialize();
            Hide();
        }

        private void OnEnable()
        {
            // 이벤트 구독
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.OnGameOver += ShowResult;
            }
        }

        private void OnDisable()
        {
            // 이벤트 구독 해제
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.OnGameOver -= ShowResult;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// UI 초기화
        /// </summary>
        private void Initialize()
        {
            // 버튼 이벤트 연결
            if (_retryButton != null)
            {
                _retryButton.onClick.AddListener(OnRetryClicked);
            }

            if (_mainMenuButton != null)
            {
                _mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            }

            // New Record 텍스트 초기 숨김
            if (_newRecordText != null)
            {
                _newRecordText.gameObject.SetActive(false);
            }
        }
        #endregion

        #region Display Control
        /// <summary>
        /// 결과 화면 표시
        /// </summary>
        public void ShowResult(float survivalTime, int dodgedBullets)
        {
            _currentSurvivalTime = survivalTime;
            _currentDodgedBullets = dodgedBullets;

            // 하이스코어 체크
            float highScore = ScoreData.LoadHighScore();
            bool isNewRecord = survivalTime > highScore;

            if (isNewRecord)
            {
                ScoreData.SaveHighScore(survivalTime);
                highScore = survivalTime;
            }

            // 표시 업데이트
            UpdateDisplay(survivalTime, dodgedBullets, highScore, isNewRecord);

            // 화면 표시
            if (_useAnimation)
            {
                StartCoroutine(ShowWithAnimation());
            }
            else
            {
                Show();
            }
        }

        /// <summary>
        /// 표시 내용 업데이트
        /// </summary>
        private void UpdateDisplay(float survivalTime, int dodgedBullets, float highScore, bool isNewRecord)
        {
            if (_survivalTimeText != null)
            {
                _survivalTimeText.text = FormatTime(survivalTime);
            }

            if (_dodgedBulletsText != null)
            {
                _dodgedBulletsText.text = dodgedBullets.ToString();
            }

            if (_highScoreText != null)
            {
                _highScoreText.text = $"최고 기록: {FormatTime(highScore)}";
            }

            if (_newRecordText != null)
            {
                _newRecordText.gameObject.SetActive(isNewRecord);
            }
        }

        /// <summary>
        /// 애니메이션과 함께 표시
        /// </summary>
        private System.Collections.IEnumerator ShowWithAnimation()
        {
            yield return new WaitForSeconds(_showDelay);

            gameObject.SetActive(true);
            _canvasGroup.alpha = 0f;

            float fadeDuration = 0.3f;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = elapsed / fadeDuration;
                yield return null;
            }

            _canvasGroup.alpha = 1f;
        }

        /// <summary>
        /// 화면 표시
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
            }
        }

        /// <summary>
        /// 화면 숨김
        /// </summary>
        public void Hide()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
            }
            gameObject.SetActive(false);
        }
        #endregion

        #region Button Handlers
        /// <summary>
        /// 재시작 버튼 클릭
        /// </summary>
        private void OnRetryClicked()
        {
            // 게임 재시작
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGame();
            }
            Hide();
        }

        /// <summary>
        /// 메인 메뉴 버튼 클릭
        /// </summary>
        private void OnMainMenuClicked()
        {
            // 메인 메뉴로 이동
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReturnToMainMenu();
            }
            Hide();
        }
        #endregion

        #region Utility
        /// <summary>
        /// 시간 포맷팅
        /// </summary>
        private string FormatTime(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);
            return $"{minutes:00}:{seconds:00}.{milliseconds:00}";
        }
        #endregion
    }
}
