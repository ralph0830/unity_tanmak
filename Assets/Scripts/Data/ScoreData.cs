using UnityEngine;

namespace MobileTanmak.Core
{
    /// <summary>
    /// 점수 데이터 저장/로드
    /// PlayerPrefs를 사용한 간단한 저장소
    /// </summary>
    public static class ScoreData
    {
        #region Keys
        private const string HIGH_SCORE_KEY = "HighScore_SurvivalTime";
        private const string TOTAL_PLAY_COUNT_KEY = "TotalPlayCount";
        private const string TOTAL_BULLETS_DODGED_KEY = "TotalBulletsDodged";
        #endregion

        #region High Score
        /// <summary>
        /// 최고 기록 저장 (생존 시간)
        /// </summary>
        public static void SaveHighScore(float survivalTime)
        {
            float currentHigh = LoadHighScore();
            if (survivalTime > currentHigh)
            {
                PlayerPrefs.SetFloat(HIGH_SCORE_KEY, survivalTime);
                PlayerPrefs.Save();
                
                if (GameEvents.Instance != null)
                {
                    GameEvents.Instance.InvokeHighScoreUpdated(survivalTime);
                }
            }
        }

        /// <summary>
        /// 최고 기록 로드
        /// </summary>
        public static float LoadHighScore()
        {
            return PlayerPrefs.GetFloat(HIGH_SCORE_KEY, 0f);
        }

        /// <summary>
        /// 최고 기록 삭제
        /// </summary>
        public static void ClearHighScore()
        {
            PlayerPrefs.DeleteKey(HIGH_SCORE_KEY);
            PlayerPrefs.Save();
        }
        #endregion

        #region Play Statistics
        /// <summary>
        /// 총 플레이 횟수 증가
        /// </summary>
        public static void IncrementPlayCount()
        {
            int count = PlayerPrefs.GetInt(TOTAL_PLAY_COUNT_KEY, 0);
            PlayerPrefs.SetInt(TOTAL_PLAY_COUNT_KEY, count + 1);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 총 플레이 횟수 로드
        /// </summary>
        public static int GetPlayCount()
        {
            return PlayerPrefs.GetInt(TOTAL_PLAY_COUNT_KEY, 0);
        }

        /// <summary>
        /// 회피한 총 탄막 수 추가
        /// </summary>
        public static void AddDodgedBullets(int count)
        {
            int total = PlayerPrefs.GetInt(TOTAL_BULLETS_DODGED_KEY, 0);
            PlayerPrefs.SetInt(TOTAL_BULLETS_DODGED_KEY, total + count);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 회피한 총 탄막 수 로드
        /// </summary>
        public static int GetTotalDodgedBullets()
        {
            return PlayerPrefs.GetInt(TOTAL_BULLETS_DODGED_KEY, 0);
        }
        #endregion

        #region Data Management
        /// <summary>
        /// 모든 데이터 삭제
        /// </summary>
        public static void ClearAllData()
        {
            PlayerPrefs.DeleteKey(HIGH_SCORE_KEY);
            PlayerPrefs.DeleteKey(TOTAL_PLAY_COUNT_KEY);
            PlayerPrefs.DeleteKey(TOTAL_BULLETS_DODGED_KEY);
            PlayerPrefs.Save();
            Debug.Log("ScoreData: 모든 데이터가 삭제되었습니다.");
        }

        /// <summary>
        /// 데이터가 존재하는지 확인
        /// </summary>
        public static bool HasData()
        {
            return PlayerPrefs.HasKey(HIGH_SCORE_KEY) || 
                   PlayerPrefs.HasKey(TOTAL_PLAY_COUNT_KEY) ||
                   PlayerPrefs.HasKey(TOTAL_BULLETS_DODGED_KEY);
        }

        /// <summary>
        /// 모든 데이터 출력 (디버깅용)
        /// </summary>
        public static void PrintAllData()
        {
            Debug.Log($"=== ScoreData ===");
            Debug.Log($"High Score: {LoadHighScore():F2}초");
            Debug.Log($"Play Count: {GetPlayCount()}");
            Debug.Log($"Total Dodged: {GetTotalDodgedBullets()}");
            Debug.Log($"=================");
        }
        #endregion
    }
}
