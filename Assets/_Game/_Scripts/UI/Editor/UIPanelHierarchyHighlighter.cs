using UnityEditor;
using UnityEngine;

namespace _Game._Scripts.UI.Editor
{
    [InitializeOnLoad]
    public static class UIPanelHierarchyHighlighter
    {
        public static bool IsHierarchyFocused => EditorWindow.focusedWindow != null && EditorWindow.focusedWindow.titleContent.text == "Hierarchy";

        static UIPanelHierarchyHighlighter()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
        }

        private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            UnityEngine.Object instance = EditorUtility.InstanceIDToObject(instanceID);

            if (instance != null)
            {
                UIWindow uiWindow = (instance as GameObject).GetComponent<UIWindow>();

                if (uiWindow != null)
                {
                    UIPanelHierarchyItem item = new UIPanelHierarchyItem(instanceID, selectionRect, uiWindow);
                    PaintBackground(item);
                    PaintText(item);
                }
            }
        }

        private static void PaintBackground(UIPanelHierarchyItem item)
        {
            Color32 color;
            if (item.IsSelected) color = MyEditorColors.GetDefaultBackgroundColor(IsHierarchyFocused, item.IsSelected);
            else color = item.UIWindow.IsShowing ? item.UIWindow.ShowingColor : item.UIWindow.HidenColor;

            EditorGUI.DrawRect(item.BackgroundRect, color);
        }

        private static void PaintText(UIPanelHierarchyItem item)
        {
            Color32 color = MyEditorColors.GetDefaultTextColor(IsHierarchyFocused, item.IsSelected, item.GameObject.activeInHierarchy);

            GUIStyle labelGUIStyle = new GUIStyle
            {
                normal = new GUIStyleState { textColor = color },
            };

            EditorGUI.LabelField(item.TextRect, item.UIWindow.name, labelGUIStyle);
        }
    }
}
