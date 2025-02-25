using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Safe area implementation for notched mobile devices. Usage:
    ///  (1) Add this component to the top level of any GUI panel. 
    ///  (2) If the panel uses a full screen background image, then create an immediate child and put the component on that instead, with all other elements childed below it.
    ///      This will allow the background image to stretch to the full extents of the screen behind the notch, which looks nicer.
    ///  (3) For other cases that use a mixture of full horizontal and vertical background stripes, use the Conform X & Y controls on separate elements as needed.
    /// </summary>
    public class SafeArea : MonoBehaviour
    {
        private RectTransform panel;
        private Rect lastSafeArea;
        private ScreenOrientation lastOrientation;

        void Awake()
        {
            panel = GetComponent<RectTransform>();
            if (panel == null)
            {
                Debug.LogError("SafeAreaHandler: Không tìm thấy RectTransform!");
                return;
            }

            lastOrientation = Screen.orientation;
            lastSafeArea = Screen.safeArea;
            ApplySafeArea(lastSafeArea);
        }

        void OnEnable()
        {
            // Unity event khi xoay màn hình hoặc window thay đổi
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
        }

        void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus) CheckSafeArea();
        }

        void OnApplicationPause(bool isPaused)
        {
            if (!isPaused) CheckSafeArea();
        }

        void OnSceneChanged(UnityEngine.SceneManagement.Scene current, UnityEngine.SceneManagement.Scene next)
        {
            CheckSafeArea();
        }

        void Update()
        {
            // Chỉ check khi xoay màn hình
            if (Screen.orientation != lastOrientation)
            {
                lastOrientation = Screen.orientation;
                CheckSafeArea();
            }
        }

        [Sirenix.OdinInspector.Button]
        void CheckSafeArea()
        {
            Rect safeArea = Screen.safeArea;
            if (safeArea != lastSafeArea)
            {
                lastSafeArea = safeArea;
                ApplySafeArea(safeArea);
            }
        }

        void ApplySafeArea(Rect safeArea)
        {
            if (panel == null) return;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            panel.anchorMin = anchorMin;
            panel.anchorMax = anchorMax;
        }
    }
}