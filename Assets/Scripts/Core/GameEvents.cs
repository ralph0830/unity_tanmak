using UnityEngine;

namespace MobileTanmak.Core
{
    /// <summary>
    /// 게임 전체 이벤트를 정의하는 중앙 이벤트 클래스
    /// 컴포넌트 간 느슨한 결합을 위해 사용
    /// </summary>
    public class GameEvents : MonoBehaviour
    {
        #region Singleton
        private static GameEvents _instance;
        public static GameEvents Instance => _instance;

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

        #region Game State Events
        /// <summary>
        /// 게임이 시작될 때 발생
        /// </summary>
        public event System.Action OnGameStarted;

        /// <summary>
        /// 게임이 일시정지될 때 발생
        /// </summary>
        public event System.Action OnGamePaused;

        /// <summary>
        /// 게임이 재개될 때 발생
        /// </summary>
        public event System.Action OnGameResumed;

        /// <summary>
        /// 게임 오버 발생 시
        /// 매개변수: 생존 시간, 회피한 탄막 수
        /// </summary>
        public event System.Action<float, int> OnGameOver;
        #endregion

        #region Score Events
        /// <summary>
        /// 생존 시간 업데이트
        /// </summary>
        public event System.Action<float> OnSurvivalTimeUpdated;

        /// <summary>
        /// 회피한 탄막 수 업데이트
        /// </summary>
        public event System.Action<int> OnDodgedBulletsUpdated;

        /// <summary>
        /// 하이스코어 갱신 시
        /// </summary>
        public event System.Action<float> OnHighScoreUpdated;
        #endregion

        #region Player Events
        /// <summary>
        /// 플레이어가 이동할 때
        /// </summary>
        public event System.Action<Vector2> OnPlayerMoved;

        /// <summary>
        /// 플레이어 피격 시
        /// </summary>
        public event System.Action OnPlayerHit;
        #endregion

        #region UI Events
        /// <summary>
        /// UI 상태 변경 요청
        /// </summary>
        public event System.Action<UIState> OnUIStateChanged;

        public enum UIState
        {
            MainMenu,
            Playing,
            Paused,
            GameOver
        }
        #endregion

        #region Event Invoke Methods
        public void InvokeGameStarted() => OnGameStarted?.Invoke();
        public void InvokeGamePaused() => OnGamePaused?.Invoke();
        public void InvokeGameResumed() => OnGameResumed?.Invoke();
        public void InvokeGameOver(float survivalTime, int dodgedBullets) => OnGameOver?.Invoke(survivalTime, dodgedBullets);

        public void InvokeSurvivalTimeUpdated(float time) => OnSurvivalTimeUpdated?.Invoke(time);
        public void InvokeDodgedBulletsUpdated(int count) => OnDodgedBulletsUpdated?.Invoke(count);
        public void InvokeHighScoreUpdated(float highScore) => OnHighScoreUpdated?.Invoke(highScore);

        public void InvokePlayerMoved(Vector2 position) => OnPlayerMoved?.Invoke(position);
        public void InvokePlayerHit() => OnPlayerHit?.Invoke();

        public void InvokeUIStateChanged(UIState state) => OnUIStateChanged?.Invoke(state);
        #endregion

        #region Utility
        /// <summary>
        /// 모든 이벤트 구독 해제 (씬 전환 시 사용)
        /// </summary>
        public void ClearAllListeners()
        {
            OnGameStarted = null;
            OnGamePaused = null;
            OnGameResumed = null;
            OnGameOver = null;
            OnSurvivalTimeUpdated = null;
            OnDodgedBulletsUpdated = null;
            OnHighScoreUpdated = null;
            OnPlayerMoved = null;
            OnPlayerHit = null;
            OnUIStateChanged = null;
        }
        #endregion
    }
}
