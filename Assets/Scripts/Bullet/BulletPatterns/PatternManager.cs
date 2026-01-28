using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MobileTanmak.Core;

namespace MobileTanmak.Bullet.Patterns
{
    /// <summary>
    /// 탄막 패턴들을 관리하고 게임 상태에 따라 실행/중지하는 매니저
    /// </summary>
    public class PatternManager : MonoBehaviour
    {
        #region Settings
        [Header("패턴 설정")]
        [SerializeField] private List<IBulletPattern> _patterns = new List<IBulletPattern>();

        [Header("난이도 설정")]
        [SerializeField] private float _patternSwitchInterval = 10f; // 패턴 전환 간격
        [SerializeField] private bool _useRandomPattern = false;     // 랜덤 패턴 사용
        [SerializeField] private int _simultaneousPatterns = 1;      // 동시에 실행할 패턴 수
        #endregion

        #region Private Fields
        private BulletSpawner _spawner;
        private int _currentPatternIndex = 0;
        private float _switchTimer = 0f;
        private bool _isInitialized = false;
        #endregion

        #region Properties
        public int ActivePatternCount { get; private set; }
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if (!_isInitialized) return;

            // 게임 플레이 중일 때만 패턴 관리
            if (GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            {
                StopAllPatterns();
                return;
            }

            // 패턴 전환 타이머
            _switchTimer += Time.deltaTime;

            if (_switchTimer >= _patternSwitchInterval)
            {
                SwitchPattern();
                _switchTimer = 0f;
            }
        }

        private void OnDisable()
        {
            StopAllPatterns();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// 패턴 매니저 초기화
        /// </summary>
        private void Initialize()
        {
            // BulletSpawner 찾기
            _spawner = FindFirstObjectByType<BulletSpawner>();
            if (_spawner == null)
            {
                Debug.LogError("PatternManager: BulletSpawner를 찾을 수 없습니다!");
                return;
            }

            // 패턴 수집 (자동으로 Scene의 모든 IBulletPattern 찾기)
            CollectPatterns();

            // 모든 패턴 초기화
            foreach (var pattern in _patterns)
            {
                pattern.Initialize(_spawner);
            }

            _isInitialized = true;
            Debug.Log($"PatternManager 초기화 완료: {_patterns.Count}개 패턴 발견");

            // 게임 상태 변경 구독
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            }
        }

        /// <summary>
        /// Scene에서 모든 패턴 수집
        /// </summary>
        private void CollectPatterns()
        {
            _patterns.Clear();

            // Scene에서 모든 IBulletPattern 찾기
            MonoBehaviour[] allMonoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            var foundPatterns = allMonoBehaviours.OfType<IBulletPattern>().ToList();

            _patterns.AddRange(foundPatterns);
        }

        /// <summary>
        /// 수동으로 패턴 추가
        /// </summary>
        public void AddPattern(IBulletPattern pattern)
        {
            if (pattern != null && !_patterns.Contains(pattern))
            {
                _patterns.Add(pattern);
                if (_spawner != null)
                {
                    pattern.Initialize(_spawner);
                }
            }
        }
        #endregion

        #region Pattern Control
        /// <summary>
        /// 게임 시작 시 호출
        /// </summary>
        public void StartPatterns()
        {
            if (!_isInitialized) return;

            _switchTimer = 0f;
            _currentPatternIndex = 0;

            // 초기 패턴 실행
            ExecuteInitialPatterns();
        }

        /// <summary>
        /// 초기 패턴 실행
        /// </summary>
        private void ExecuteInitialPatterns()
        {
            int count = Mathf.Min(_simultaneousPatterns, _patterns.Count);

            for (int i = 0; i < count; i++)
            {
                if (_patterns[i] != null)
                {
                    _patterns[i].Execute();
                }
            }

            ActivePatternCount = count;
        }

        /// <summary>
        /// 패턴 전환
        /// </summary>
        private void SwitchPattern()
        {
            // 모든 패턴 중지
            StopAllPatterns();

            if (_useRandomPattern)
            {
                // 랜덤 패턴 선택
                ExecuteRandomPatterns();
            }
            else
            {
                // 순차적 패턴 선택
                ExecuteNextPatterns();
            }
        }

        /// <summary>
        /// 다음 패턴 실행 (순차)
        /// </summary>
        private void ExecuteNextPatterns()
        {
            int count = Mathf.Min(_simultaneousPatterns, _patterns.Count);

            for (int i = 0; i < count; i++)
            {
                int index = (_currentPatternIndex + i) % _patterns.Count;
                if (_patterns[index] != null)
                {
                    _patterns[index].Execute();
                }
            }

            _currentPatternIndex = (_currentPatternIndex + _simultaneousPatterns) % _patterns.Count;
            ActivePatternCount = count;
        }

        /// <summary>
        /// 랜덤 패턴 실행
        /// </summary>
        private void ExecuteRandomPatterns()
        {
            int count = Mathf.Min(_simultaneousPatterns, _patterns.Count);
            List<int> availableIndices = new List<int>();

            for (int i = 0; i < _patterns.Count; i++)
            {
                if (!_patterns[i].IsActive)
                {
                    availableIndices.Add(i);
                }
            }

            // 사용 가능한 패턴이 없으면 전체에서 선택
            if (availableIndices.Count == 0)
            {
                for (int i = 0; i < _patterns.Count; i++)
                {
                    availableIndices.Add(i);
                }
            }

            // 랜덤 셔플
            for (int i = availableIndices.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                int temp = availableIndices[i];
                availableIndices[i] = availableIndices[j];
                availableIndices[j] = temp;
            }

            // 처음 N개 실행
            for (int i = 0; i < Mathf.Min(count, availableIndices.Count); i++)
            {
                _patterns[availableIndices[i]].Execute();
            }

            ActivePatternCount = Mathf.Min(count, availableIndices.Count);
        }

        /// <summary>
        /// 모든 패턴 중지
        /// </summary>
        public void StopAllPatterns()
        {
            foreach (var pattern in _patterns)
            {
                if (pattern != null && pattern.IsActive)
                {
                    pattern.Stop();
                }
            }
            ActivePatternCount = 0;
        }

        /// <summary>
        /// 특정 패턴만 실행
        /// </summary>
        public void ExecutePattern(int index)
        {
            if (index >= 0 && index < _patterns.Count)
            {
                _patterns[index].Execute();
            }
        }

        /// <summary>
        /// 특정 패턴 중지
        /// </summary>
        public void StopPattern(int index)
        {
            if (index >= 0 && index < _patterns.Count)
            {
                _patterns[index].Stop();
            }
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
                case GameManager.GameState.Playing:
                    StartPatterns();
                    break;

                case GameManager.GameState.Paused:
                case GameManager.GameState.GameOver:
                case GameManager.GameState.MainMenu:
                    StopAllPatterns();
                    break;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 동시에 실행할 패턴 수 설정
        /// </summary>
        public void SetSimultaneousPatterns(int count)
        {
            _simultaneousPatterns = Mathf.Max(1, count);
        }

        /// <summary>
        /// 패턴 전환 간격 설정
        /// </summary>
        public void SetSwitchInterval(float interval)
        {
            _patternSwitchInterval = Mathf.Max(1f, interval);
        }

        /// <summary>
        /// 랜덤 패턴 모드 설정
        /// </summary>
        public void SetRandomPattern(bool enabled)
        {
            _useRandomPattern = enabled;
        }

        /// <summary>
        /// 등록된 패턴 수 반환
        /// </summary>
        public int GetPatternCount()
        {
            return _patterns.Count;
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
