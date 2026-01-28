using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

namespace MobileTanmak.Editor
{
    /// <summary>
    /// Unity Editor에서 UI와 Canvas 구조를 자동으로 생성하는 스크립트
    /// 메뉴: Tools > Auto Setup Game UI
    /// </summary>
    public class AutoSetupEditor
    {
        private const string UI_PATH = "Assets/Scenes/GameScene.unity";

        [MenuItem("Tools/Auto Setup Game UI")]
        public static void SetupGameUI()
        {
            // GameScene 열기
            var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(UI_PATH);

            // 1. Canvas 생성 또는 찾기
            Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();

                // EventSystem 생성
                if (GameObject.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
                {
                    GameObject eventSystemObj = new GameObject("EventSystem");
                    eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                    eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                }

                Debug.Log("Canvas 생성 완료");
            }

            // 2. UI 패널들 생성
            CreateGamePanel(canvas.transform);
            CreateResultPanel(canvas.transform);
            CreateMainMenuPanel(canvas.transform);

            // 3. Canvas 정리
            Selection.activeGameObject = canvas.gameObject;

            // Scene 저장
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);

            Debug.Log("<color=green>✅ Game UI 자동 설정 완료!</color>");
        }

        /// <summary>
        /// 게임 플레이 중 패널 생성
        /// </summary>
        private static void CreateGamePanel(Transform parent)
        {
            GameObject panel = new GameObject("GamePanel");
            panel.transform.SetParent(parent, false);
            panel.AddComponent<RectTransform>();

            // RectTransform 전체 화면
            RectTransform rectTransform = panel.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;

            // GameUI 컴포넌트 추가
            var gameUI = panel.AddComponent<GameUI>();

            // 상단 점수 표시
            CreateScoreText(panel.transform, "SurvivalTimeText", new Vector2(200, 50), new Vector2(-100, 250), "00:00");
            CreateScoreText(panel.transform, "DodgedBulletsText", new Vector2(200, 50), new Vector2(100, 250), "0");

            // 게임 중에는 비활성
            panel.SetActive(false);

            Debug.Log("GamePanel 생성 완료");
        }

        /// <summary>
        /// 결과 패널 생성
        /// </summary>
        private static void CreateResultPanel(Transform parent)
        {
            GameObject panel = new GameObject("ResultPanel");
            panel.transform.SetParent(parent, false);
            panel.AddComponent<RectTransform>();
            panel.AddComponent<CanvasGroup>();

            RectTransform rectTransform = panel.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;

            // 배경
            CreateBackground(panel.transform);

            // Game Over 텍스트
            CreateTitleText(panel.transform, "GAME OVER", new Vector2(0, 150));

            // 결과 텍스트들
            CreateResultText(panel.transform, "SurvivalTimeText", new Vector2(0, 50), "생존 시간: 00:00");
            CreateResultText(panel.transform, "DodgedBulletsText", new Vector2(0, 0), "회피 수: 0");
            CreateResultText(panel.transform, "HighScoreText", new Vector2(0, -50), "최고 기록: 00:00");
            CreateNewRecordText(panel.transform);

            // 버튼들
            CreateButton(panel.transform, "RetryButton", new Vector2(-100, -150), "재시작");
            CreateButton(panel.transform, "MainMenuButton", new Vector2(100, -150), "메인 메뉴");

            // ResultUI 컴포넌트 추가 및 연결
            var resultUI = panel.AddComponent<ResultUI>();

            // SerializeReference를 통한 연결 (수동으로 Inspector에서 연결 필요)
            // 대신 FindChildByName으로 자동 찾도록 구현됨

            panel.SetActive(false);

            Debug.Log("ResultPanel 생성 완료");
        }

        /// <summary>
        /// 메인 메뉴 패널 생성
        /// </summary>
        private static void CreateMainMenuPanel(Transform parent)
        {
            GameObject panel = new GameObject("MainMenuPanel");
            panel.transform.SetParent(parent, false);
            panel.AddComponent<RectTransform>();

            RectTransform rectTransform = panel.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;

            // 배경
            CreateBackground(panel.transform, new Color(0.1f, 0.1f, 0.1f, 1f));

            // 타이틀
            CreateTitleText(panel.transform, "TANMAK", new Vector2(0, 150), 60);

            // 점수 텍스트
            CreateResultText(panel.transform, "HighScoreText", new Vector2(0, 0), "최고 기록: 00:00");
            CreateResultText(panel.transform, "PlayCountText", new Vector2(0, -50), "플레이 횟수: 0");

            // 시작 버튼
            CreateButton(panel.transform, "StartButton", new Vector2(0, -150), "게임 시작", new Vector2(200, 60));

            // MainMenuUI 컴포넌트
            panel.AddComponent<MainMenuUI>();

            Debug.Log("MainMenuPanel 생성 완료");
        }

        #region UI Helper Methods

        private static void CreateScoreText(Transform parent, string name, Vector2 size, Vector2 position, string defaultText)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);

            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = position;

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = defaultText;
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
        }

        private static void CreateTitleText(Transform parent, string text, Vector2 position, int fontSize = 48)
        {
            GameObject textObj = new GameObject("TitleText");
            textObj.transform.SetParent(parent, false);

            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 100);
            rect.anchoredPosition = position;

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.cyan;
            tmp.fontStyle = FontStyles.Bold;
        }

        private static void CreateResultText(Transform parent, string name, Vector2 position, string defaultText)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);

            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(300, 50);
            rect.anchoredPosition = position;

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = defaultText;
            tmp.fontSize = 20;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
        }

        private static void CreateNewRecordText(Transform parent)
        {
            GameObject textObj = new GameObject("NewRecordText");
            textObj.transform.SetParent(parent, false);

            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(300, 50);
            rect.anchoredPosition = new Vector2(0, -100);

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = "🎉 NEW RECORD! 🎉";
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.yellow;
            tmp.fontStyle = FontStyles.Bold;
        }

        private static void CreateButton(Transform parent, string name, Vector2 position, string labelText, Vector2? size = null)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.sizeDelta = size ?? new Vector2(150, 50);
            rect.anchoredPosition = position;

            // 이미지 (배경)
            Image image = buttonObj.AddComponent<Image>();
            Color buttonColor = new Color(0.2f, 0.6f, 1f, 1f);
            image.color = buttonColor;

            // Button 컴포넌트
            Button button = buttonObj.AddComponent<Button>();

            // 버튼 상태 색상
            ColorBlock colors = button.colors;
            colors.normalColor = buttonColor;
            colors.highlightedColor = new Color(0.3f, 0.7f, 1f, 1f);
            colors.pressedColor = new Color(0.1f, 0.4f, 0.8f, 1f);
            colors.selectedColor = new Color(0.2f, 0.5f, 0.9f, 1f);
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            button.colors = colors;

            // 텍스트
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = labelText;
            tmp.fontSize = 18;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
        }

        private static void CreateBackground(Transform parent, Color? color = null)
        {
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(parent, false);

            RectTransform rect = bgObj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            Image image = bgObj.AddComponent<Image>();
            image.color = color ?? new Color(0, 0, 0, 0.8f);
        }

        #endregion
    }
}
