using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace DopeElections.Editor.Cameras
{
    class CameraPreview : EditorWindow
    {
        Camera camera;
        RenderTexture renderTexture;

        [MenuItem("Tools/Camera Preview")]
        static void Init()
        {
            var editorWindow = (EditorWindow) GetWindow<CameraPreview>(typeof(CameraPreview));
            editorWindow.autoRepaintOnSceneChange = true;
            editorWindow.titleContent = new GUIContent("Camera Preview");
            editorWindow.Show();
        }

        void Awake()
        {
            camera = Camera.main;
        }

        void Update()
        {
            if (camera != null && camera.gameObject.activeInHierarchy)
            {
                EnsureRenderTexture();

                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

                camera.renderingPath = RenderingPath.UsePlayerSettings;
                camera.targetTexture = renderTexture;
                if (prefabStage) camera.scene = prefabStage.scene;
                camera.Render();
                camera.targetTexture = null;
            }
        }

        void OnSelectionChange()
        {
            var obj = Selection.activeGameObject;
            if (obj == null)
                return;

            var cam = obj.GetComponent<Camera>();
            if (cam == null)
                return;

            camera = cam;
        }

        void EnsureRenderTexture()
        {
            if (renderTexture == null
                || (int) position.width != renderTexture.width
                || (int) position.height != renderTexture.height)
            {
                if (renderTexture) DestroyImmediate(renderTexture);
                renderTexture = new RenderTexture((int) position.width, (int) position.height, 24,
                    RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            }
        }

        void OnGUI()
        {
            if (renderTexture != null)
            {
                GUI.DrawTexture(new Rect(0.0f, 0.0f, position.width, position.height), renderTexture);
            }
        }
    }
}