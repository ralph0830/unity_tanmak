using UnityEngine;
using MobileTanmak.Core;

namespace MobileTanmak.UI
{
    /// <summary>
    /// UI 상태를 관리하고 전환하는 매니저
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        #region Singleton
        private static UIManager _instance;
        public static UIManager Instance => _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region UI References
        [Header("UI Panels")]
        [SerializeField] private GameObject _mainMenuPanel;
        [SerializeField] private GameObject _gameUIPanel;
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private GameObject _pausePanel;

        [Header("Components")]
        [SerializeField] private GameUI _gameUI;
        [SerializeField] private ResultUI _resultUI;
        #endregion

        #region Private Fields
        private GameEvents.UIState _currentState = GameEvents.UIState.MainMenu;
        #endregion

        #region Properties
        public GameEvents.UIState CurrentState => _currentState;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            Initialize();
        }

        private void OnEnable()
        {
            // 이벤트 구독
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.OnUIStateChanged += OnUIStateChanged;
                GameEvents.Instance.OnGameStarted += OnGameStarted;
                GameEvents.Instance.OnGameOver += OnGameOver;
            }
        }

        private void OnDisable()
        {
            // 이벤트 구독 해제
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.OnUIStateChanged -= OnUIStateChanged;
                GameEvents.Instance.OnGameStarted -= OnGameStarted;
                GameEvents.Instance.OnGameOver -= OnGameOver;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// UI 매니저 초기화
        /// </summary>
        private void Initialize()
        {
            // 컴포넌트 자동 찾기
            if (_gameUI == null)
            {
                _gameUI = GetComponentInChildren<GameUI>();
            }
            if (_resultUI == null)
            {
                _resultUI = GetComponentInChildren<ResultUI>();
            }

            // 초기 상태 설정
            ShowMainMenu();
        }
        #endregion

        #region UI State Management
        /// <summary>
        /// 메인 메뉴 표시
        /// </summary>
        public void ShowMainMenu()
        {
            SetUIState(GameEvents.UIState.MainMenu);
        }

        /// <summary>
        /// 게임 UI 표시
        /// </summary>
        public void ShowGameUI()
        {
            SetUIState(GameEvents.UIState.Playing);
        }

        /// <summary>
        /// 일시정지 UI 표시
        /// </summary>
        public void ShowPauseUI()
        {
            SetUIState(GameEvents.UIState.Paused);
        }

        /// <summary>
        /// 결과 화면 표시
        /// </summary>
        public void ShowResultUI()
        {
            SetUIState(GameEvents.UIState.GameOver);
        }

        /// <summary>
        /// UI 상태 변경
        /// </summary>
        private void SetUIState(GameEvents.UIState newState)
        {
            if (_currentState == newState) return;

            _currentState = newState;

            // 이벤트 발생
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.InvokeUIStateChanged(newState);
            }

            // 패널 표시/숨김
            UpdatePanels();
        }

        /// <summary>
        /// 패널 표시 상태 업데이트
        /// </summary>
        private void UpdatePanels()
        {
            // 모든 패널 숨김
            if (_mainMenuPanel != null) _mainMenuPanel.SetActive(false);
            if (_gameUIPanel != null) _gameUIPanel.SetActive(false);
            if (_resultPanel != null) _resultPanel.SetActive(false);
            if (_pausePanel != null) _pausePanel.SetActive(false);

            // 현재 상태에 따른 패널 표시
            switch (_currentState)
            {
                case GameEvents.UIState.MainMenu:
                    if (_mainMenuPanel != null) _mainMenuPanel.SetActive(true);
                    break;

                case GameEvents.UIState.Playing:
                    if (_gameUIPanel != null) _gameUIPanel.SetActive(true);
                    break;

                case GameEvents.UIState.Paused:
                    if (_pausePanel != null) _pausePanel.SetActive(true);
                    if (_gameUIPanel != null) _gameUIPanel.SetActive(true);
                    break;

                case GameEvents.UIState.GameOver:
                    if (_resultPanel != null) _resultPanel.SetActive(true);
                    break;
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// UI 상태 변경 이벤트 처리
        /// </summary>
        private void OnUIStateChanged(GameEvents.UIState state)
        {
            _currentState = state;
        }

        /// <summary>
        /// 게임 시작 이벤트 처리
        /// </summary>
        private void OnGameStarted()
        {
            ShowGameUI();
        }

        /// <summary>
        /// 게임 오버 이벤트 처리
        /// </summary>
        private void OnGameOver(float survivalTime, int dodgedBullets)
        {
            ShowResultUI();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 패널 참조 수동 설정
        /// </summary>
        public void SetPanels(GameObject mainMenu, GameObject gameUI, GameObject result, GameObject pause)
        {
            _mainMenuPanel = mainMenu;
            _gameUIPanel = gameUI;
            _resultPanel = result;
            _pausePanel = pause;
        }

        /// <summary>
        /// 컴포넌트 참조 수동 설정
        /// </summary>
        public void SetComponents(GameUI gameUI, ResultUI resultUI)
        {
            _gameUI = gameUI;
            _resultUI = resultUI;
        }

        /// <summary>
        /// 현재 UI 상태가 특정 상태인지 확인
        /// </summary>
        public bool IsState(GameEvents.UIState state)
        {
            return _currentState == state;
        }

        /// <summary>
        /// 게임 중인지 확인
        /// </summary>
        public bool IsPlaying()
        {
            return _currentState == GameEvents.UIState.Playing;
        }

        /// <summary>
        /// 일시정지 중인지 확인
        /// </summary>
        public bool IsPaused()
        {
            return _currentState == GameEvents.UIState.Paused;
        }
        #endregion
    }
}
