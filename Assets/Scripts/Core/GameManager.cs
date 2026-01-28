using UnityEngine;

namespace MobileTanmak.Core
{
    /// <summary>
    /// 게임 전체 상태를 관리하는 싱글톤 매니저
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        private static GameManager _instance;
        public static GameManager Instance => _instance;

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

        #region Game State
        public enum GameState
        {
            MainMenu,   // 메인 메뉴
            Playing,    // 게임 플레이 중
            Paused,     // 일시 정지
            GameOver    // 게임 오버
        }

        private GameState _currentState = GameState.MainMenu;
        public GameState CurrentState => _currentState;

        // 생존 시간 (점수)
        private float _survivalTime = 0f;
        public float SurvivalTime => _survivalTime;

        // 회피한 탄막 수
        private int _dodgedBullets = 0;
        public int DodgedBullets => _dodgedBullets;
        #endregion

        #region Events
        // 게임 상태 변경 이벤트
        public System.Action<GameState> OnGameStateChanged;
        
        // 점수 업데이트 이벤트
        public System.Action<float> OnSurvivalTimeUpdated;
        public System.Action<int> OnDodgedBulletsUpdated;
        
        // 게임 오버 이벤트
        public System.Action OnGameOver;
        #endregion

        #region Unity Lifecycle
        private void Update()
        {
            if (_currentState == GameState.Playing)
            {
                _survivalTime += Time.deltaTime;
                OnSurvivalTimeUpdated?.Invoke(_survivalTime);
            }
        }
        #endregion

        #region Game State Control
        /// <summary>
        /// 게임 상태를 변경합니다
        /// </summary>
        public void SetGameState(GameState newState)
        {
            if (_currentState == newState) return;

            _currentState = newState;
            OnGameStateChanged?.Invoke(_currentState);

            // 상태별 초기화
            switch (_currentState)
            {
                case GameState.Playing:
                    OnGameStart();
                    break;
                case GameState.GameOver:
                    OnGameOver?.Invoke();
                    break;
            }
        }

        /// <summary>
        /// 게임 시작 시 초기화
        /// </summary>
        private void OnGameStart()
        {
            _survivalTime = 0f;
            _dodgedBullets = 0;
            Time.timeScale = 1f;
        }

        /// <summary>
        /// 게임 플레이 시작
        /// </summary>
        public void StartGame()
        {
            SetGameState(GameState.Playing);
        }

        /// <summary>
        /// 게임 일시 정지
        /// </summary>
        public void PauseGame()
        {
            if (_currentState == GameState.Playing)
            {
                SetGameState(GameState.Paused);
                Time.timeScale = 0f;
            }
        }

        /// <summary>
        /// 게임 재개
        /// </summary>
        public void ResumeGame()
        {
            if (_currentState == GameState.Paused)
            {
                SetGameState(GameState.Playing);
                Time.timeScale = 1f;
            }
        }

        /// <summary>
        /// 게임 오버 처리
        /// </summary>
        public void GameOver()
        {
            SetGameState(GameState.GameOver);
            Time.timeScale = 0f;
        }

        /// <summary>
        /// 메인 메뉴로 돌아가기
        /// </summary>
        public void ReturnToMainMenu()
        {
            SetGameState(GameState.MainMenu);
            Time.timeScale = 1f;
        }
        #endregion

        #region Score Management
        /// <summary>
        /// 탄막 회피 성공 시 호출
        /// </summary>
        public void RegisterDodgedBullet()
        {
            _dodgedBullets++;
            OnDodgedBulletsUpdated?.Invoke(_dodgedBullets);
        }

        /// <summary>
        /// 포맷된 생존 시간을 반환 (분:초)
        /// </summary>
        public string GetFormattedSurvivalTime()
        {
            int minutes = Mathf.FloorToInt(_survivalTime / 60f);
            int seconds = Mathf.FloorToInt(_survivalTime % 60f);
            return $"{minutes:00}:{seconds:00}";
        }
        #endregion
    }
}
