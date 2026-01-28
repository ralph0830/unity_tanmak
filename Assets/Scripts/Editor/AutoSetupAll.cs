using UnityEngine;
using UnityEditor;

namespace MobileTanmak.Editor
{
    /// <summary>
    /// 프로젝트 전체 자동 설정 - 한 번에 실행
    /// 메뉴: Tools > Auto Setup All (전체 실행)
    /// </summary>
    public class AutoSetupAll
    {
        [MenuItem("Tools/Auto Setup All")]
        public static void SetupAll()
        {
            Debug.Log("<color=cyan>=== 탄막 피하기 게임 자동 설정 시작 ===</color>");

            // 1. Player 설정
            Debug.Log("<color=yellow>1/3. Player 설정 중...</color>");
            AutoSetupPlayer.SetupPlayer();

            // 2. Bullet 설정
            Debug.Log("<color=yellow>2/3. Bullet 설정 중...</color>");
            AutoSetupBullets.SetupBullets();

            // 3. UI 설정
            Debug.Log("<color=yellow>3/3. UI 설정 중...</color>");
            AutoSetupEditor.SetupGameUI();

            // 4. Build Settings에 Scene 추가
            AddSceneToBuildSettings();

            Debug.Log("<color=green>=== ✅ 모든 자동 설정 완료! ===</color>");
            Debug.Log("<color=white>남은 작업:</color>");
            Debug.Log("  1. Unity Editor > Tools > Auto Setup All 실행");
            Debug.Log("  2. Build Settings에서 Android 플랫폼으로 전환");
            Debug.Log("  3. File > Build Settings > Build로 APK 생성");
        }

        /// <summary>
        /// Build Settings에 GameScene 추가
        /// </summary>
        private static void AddSceneToBuildSettings()
        {
            string gameScenePath = "Assets/Scenes/GameScene.unity";

            // Build Settings에 Scene이 있는지 확인
            var scenes = EditorBuildSettings.scenes;
            bool sceneExists = false;

            foreach (var scene in scenes)
            {
                if (scene.path == gameScenePath)
                {
                    sceneExists = true;
                    break;
                }
            }

            if (!sceneExists)
            {
                // 새 Scene 목록 생성
                var newScenes = new EditorBuildSettingsScene[scenes.Length + 1];
                for (int i = 0; i < scenes.Length; i++)
                {
                    newScenes[i] = scenes[i];
                }
                newScenes[scenes.Length] = new EditorBuildSettingsScene(gameScenePath, true);
                EditorBuildSettings.scenes = newScenes;
                Debug.Log($"Build Settings에 '{gameScenePath}' 추가 완료");
            }
            else
            {
                Debug.Log("Build Settings에 GameScene이 이미 존재합니다");
            }
        }
    }
}
