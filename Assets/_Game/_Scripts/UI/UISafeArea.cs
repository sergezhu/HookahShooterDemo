using UnityEngine;

// ���� ����� ��������� RectTransform, �� ������� �����, ��� ���������� ���� �������
// �������� ������ �������� � ������ �� �������
namespace _Game._Scripts.UI
{
    public class UISafeArea : MonoBehaviour
    {
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            RefreshSafeArea();
        }

        private void RefreshSafeArea()
        {
            Rect safeArea = Screen.safeArea;

            Vector2 minAnchor = safeArea.position;
            Vector2 maxAnchor = safeArea.position + safeArea.size;

            minAnchor.x /= Screen.width;
            minAnchor.y /= Screen.height;
            maxAnchor.x /= Screen.width;
            maxAnchor.y /= Screen.height;

            _rectTransform.anchorMin = minAnchor;
            _rectTransform.anchorMax = maxAnchor;
        }
    }
}