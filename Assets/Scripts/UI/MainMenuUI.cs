using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MobileTanmak.Core;

namespace MobileTanmak.UI
{
    /// <summary>
    /// 메인 메뉴 UI
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        #region UI References
        [Header("제목")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private string _gameTitle = "TANMAK";

        [Header("점수 표시")]
        [SerializeField] private TextMeshProUGUI _highScoreText;
        [SerializeField] private TextMeshProUGUI _playCountText;

        [Header("버튼")]
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _quitButton;

        [Header("설정")]
        [SerializeField] private bool _showQuitButton = false; // 모바일에서는 숨김
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            Initialize();
            UpdateScoreDisplay();
        }

        private void OnEnable()
        {
            UpdateScoreDisplay();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// 메뉴 초기화
        /// </summary>
        private void Initialize()
        {
            // 제목 설정
            if (_titleText != null)
            {
                _titleText.text = _gameTitle;
            }

            // 버튼 이벤트 연결
            if (_startButton != null)
            {
                _startButton.onClick.AddListener(OnStartClicked);
            }

            if (_quitButton != null)
            {
                _quitButton.onClick.AddListener(OnQuitClicked);
                _quitButton.gameObject.SetActive(_showQuitButton);
            }
        }
        #endregion

        #region Display Update
        /// <summary>
        /// 점수 표시 업데이트
        /// </summary>
        private void UpdateScoreDisplay()
        {
            float highScore = ScoreData.LoadHighScore();
            int playCount = ScoreData.GetPlayCount();

            if (_highScoreText != null)
            {
                if (highScore > 0)
                {
                    int minutes = Mathf.FloorToInt(highScore / 60f);
                    int seconds = Mathf.FloorToInt(highScore % 60f);
                    _highScoreText.text = $"최고 기록: {minutes:00}:{seconds:00}";
                }
                else
                {
                    _highScoreText.text = "첫 플레이를 환영합니다!";
                }
            }

            if (_playCountText != null)
            {
                _playCountText.text = $"플레이 횟수: {playCount}";
            }
        }
        #endregion

        #region Button Handlers
        /// <summary>
        /// 시작 버튼 클릭
        /// </summary>
        private void OnStartClicked()
        {
            // 플레이 횟수 증가
            ScoreData.IncrementPlayCount();

            // 게임 시작
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGame();
            }

            // UI 이벤트
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.InvokeGameStarted();
            }
        }

        /// <summary>
        /// 종료 버튼 클릭
        /// </summary>
        private void OnQuitClicked()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 게임 제목 설정
        /// </summary>
        public void SetGameTitle(string title)
        {
            _gameTitle = title;
            if (_titleText != null)
            {
                _titleText.text = title;
            }
        }
        #endregion
    }
}
