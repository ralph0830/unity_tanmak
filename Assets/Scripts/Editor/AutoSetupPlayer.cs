using UnityEngine;
using UnityEditor;

namespace MobileTanmak.Editor
{
    /// <summary>
    /// 플레이어 스프라이트와 설정을 자동으로 생성하는 Editor 스크립트
    /// 메뉴: Tools > Auto Setup Player
    /// </summary>
    public class AutoSetupPlayer
    {
        [MenuItem("Tools/Auto Setup Player")]
        public static void SetupPlayer()
        {
            // GameScene 열기
            var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");

            // Player GameObject 찾기
            GameObject player = GameObject.Find("Player");
            if (player == null)
            {
                Debug.LogError("Player GameObject를 찾을 수 없습니다!");
                return;
            }

            // 1. Circle 스프라이트 생성
            CreateCircleSprite(player);

            // 2. 컴포넌트 설정 확인
            EnsureComponents(player);

            // 3. Player 설정
            SetPlayerDefaults(player);

            // 4. 태그 설정
            SetPlayerTag(player);

            // Scene 저장
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);

            Debug.Log("<color=green>✅ Player 자동 설정 완료!</color>");
        }

        /// <summary>
        /// Circle 스프라이트 생성 및 적용
        /// </summary>
        private static void CreateCircleSprite(GameObject player)
        {
            SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = player.AddComponent<SpriteRenderer>();
            }

            // 스프라이트가 없으면 생성
            if (spriteRenderer.sprite == null)
            {
                // 간단한 원형 텍스처 생성
                Texture2D texture = CreateCircleTexture(64, Color.cyan);
                
                // 스프라이트로 변환
                Rect rect = new Rect(0, 0, texture.width, texture.height);
                Vector2 pivot = new Vector2(0.5f, 0.5f);
                float pixelsPerUnit = 100f;
                
                Sprite sprite = Sprite.Create(texture, rect, pivot, pixelsPerUnit);
                sprite.name = "PlayerCircle";
                
                spriteRenderer.sprite = sprite;

                // 텍스처 에셋으로 저장
                string assetPath = "Assets/Textures/PlayerSprite.asset";
                System.IO.Directory.CreateDirectory("Assets/Textures");
                AssetDatabase.CreateAsset(texture, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log("Circle Sprite 생성 완료");
            }
        }

        /// <summary>
        /// 원형 텍스처 생성
        /// </summary>
        private static Texture2D CreateCircleTexture(int size, Color color)
        {
            Texture2D texture = new Texture2D(size, size);
            texture.filterMode = FilterMode.Bilinear;

            Color transparent = new Color(0, 0, 0, 0);
            Color fillColor = color;

            int center = size / 2;
            float radius = size * 0.4f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    
                    if (distance <= radius)
                    {
                        // 부드러운 가장자리 (안티앨리어싱)
                        float alpha = Mathf.Clamp01(1 - (distance / radius));
                        fillColor.a = alpha;
                        texture.SetPixel(x, y, fillColor);
                    }
                    else
                    {
                        texture.SetPixel(x, y, transparent);
                    }
                }
            }

            texture.Apply();
            return texture;
        }

        /// <summary>
        /// 필수 컴포넌트 확인 및 추가
        /// </summary>
        private static void EnsureComponents(GameObject player)
        {
            // SpriteRenderer
            if (player.GetComponent<SpriteRenderer>() == null)
            {
                player.AddComponent<SpriteRenderer>();
            }

            // CircleCollider2D
            CircleCollider2D collider = player.GetComponent<CircleCollider2D>();
            if (collider == null)
            {
                collider = player.AddComponent<CircleCollider2D>();
            }
            collider.isTrigger = true;
            collider.radius = 0.3f;

            // PlayerController
            if (player.GetComponent<Player.PlayerController>() == null)
            {
                player.AddComponent<Player.PlayerController>();
            }

            // PlayerHitbox
            if (player.GetComponent<Player.PlayerHitbox>() == null)
            {
                player.AddComponent<Player.PlayerHitbox>();
            }

            Debug.Log("Player 컴포넌트 설정 완료");
        }

        /// <summary>
        /// 플레이어 기본값 설정
        /// </summary>
        private static void SetPlayerDefaults(GameObject player)
        {
            // 위치 설정
            player.transform.position = new Vector3(0, -2, 0);

            // 레이어 설정
            player.layer = LayerMask.NameToLayer("Default");

            Debug.Log("Player 기본값 설정 완료");
        }

        /// <summary>
        /// 플레이어 태그 설정
        /// </summary>
        private static void SetPlayerTag(GameObject player)
        {
            // "Player" 태그가 없으면 생성
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            bool playerTagExists = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty tagProp = tagsProp.GetArrayElementAtIndex(i);
                if (tagProp.stringValue == "Player")
                {
                    playerTagExists = true;
                    break;
                }
            }

            if (!playerTagExists)
            {
                tagsProp.InsertArrayElementAtIndex(0);
                SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(0);
                newTag.stringValue = "Player";
                tagManager.ApplyModifiedProperties();
                Debug.Log("'Player' 태그 생성 완료");
            }

            // 플레이어에 태그 적용
            player.tag = "Player";

            Debug.Log("Player 태그 설정 완료");
        }
    }
}
