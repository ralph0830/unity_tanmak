using UnityEngine;
using UnityEditor;
using MobileTanmak.Bullet;

namespace MobileTanmak.Editor
{
    /// <summary>
    /// 탄막 스프라이트와 BulletSpawner 설정을 자동으로 생성
    /// 메뉴: Tools > Auto Setup Bullets
    /// </summary>
    public class AutoSetupBullets
    {
        [MenuItem("Tools/Auto Setup Bullets")]
        public static void SetupBullets()
        {
            var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");

            // 1. 탄막 스프라이트 생성
            CreateBulletSprites();

            // 2. Bullet 태그 생성
            EnsureBulletTag();

            // 3. BulletSpawner에 프리팹 연결 준비
            SetupBulletSpawner();

            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
            Debug.Log("<color=green>✅ Bullet 자동 설정 완료!</color>");
        }

        /// <summary>
        /// 탄막용 Circle 스프라이트들 생성
        /// </summary>
        private static void CreateBulletSprites()
        {
            string folderPath = "Assets/Textures/Bullets";
            System.IO.Directory.CreateDirectory(folderPath);

            // 다양한 색상의 탄막 스프라이트 생성
            CreateBulletSprite(folderPath, "Bullet_Normal", new Color(1f, 0.5f, 0.5f));     // 빨강
            CreateBulletSprite(folderPath, "Bullet_Spiral", new Color(0.5f, 1f, 0.5f));     // 초록
            CreateBulletSprite(folderPath, "Bullet_Spread", new Color(0.5f, 0.5f, 1f));     // 파랑
            CreateBulletSprite(folderPath, "Bullet_Targeted", new Color(1f, 1f, 0.5f));     // 노랑
            CreateBulletSprite(folderPath, "Bullet_Accel", new Color(1f, 0.5f, 1f));       // 보라

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("탄막 스프라이트 5종 생성 완료");
        }

        /// <summary>
        /// 단일 탄막 스프라이트 생성
        /// </summary>
        private static void CreateBulletSprite(string folderPath, string name, Color color)
        {
            int size = 32;
            Texture2D texture = new Texture2D(size, size);
            texture.filterMode = FilterMode.Bilinear;

            Color transparent = new Color(0, 0, 0, 0);

            int center = size / 2;
            float radius = size * 0.45f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    
                    if (distance <= radius)
                    {
                        texture.SetPixel(x, y, color);
                    }
                    else
                    {
                        texture.SetPixel(x, y, transparent);
                    }
                }
            }

            texture.Apply();

            // 스프라이트 생성
            Rect rect = new Rect(0, 0, size, size);
            Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100f);
            sprite.name = name;

            // 에셋 저장
            string assetPath = $"{folderPath}/{name}.asset";
            AssetDatabase.CreateAsset(texture, assetPath);

            Debug.Log($"  - {name} 생성 완료");
        }

        /// <summary>
        /// Bullet 태그 생성
        /// </summary>
        private static void EnsureBulletTag()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            bool bulletTagExists = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty tagProp = tagsProp.GetArrayElementAtIndex(i);
                if (tagProp.stringValue == "Bullet")
                {
                    bulletTagExists = true;
                    break;
                }
            }

            if (!bulletTagExists)
            {
                tagsProp.InsertArrayElementAtIndex(0);
                SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(0);
                newTag.stringValue = "Bullet";
                tagManager.ApplyModifiedProperties();
                Debug.Log("'Bullet' 태그 생성 완료");
            }
        }

        /// <summary>
        /// BulletSpawner 설정
        /// </summary>
        private static void SetupBulletSpawner()
        {
            GameObject spawner = GameObject.Find("BulletSpawner");
            if (spawner == null)
            {
                Debug.LogWarning("BulletSpawner GameObject를 찾을 수 없습니다.");
                return;
            }

            Bullet.BulletSpawner bs = spawner.GetComponent<Bullet.BulletSpawner>();
            if (bs != null)
            {
                // Inspector에서 스프라이트를 수동으로 할당하도록 안내
                Debug.Log("BulletSpawner: 생성된 스프라이트를 Inspector에서 할당하세요");
            }
        }
    }
}
