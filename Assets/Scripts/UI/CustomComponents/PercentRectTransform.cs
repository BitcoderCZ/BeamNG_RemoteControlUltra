using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.CustomComponents
{
    [ExecuteAlways]
    public class PercentRectTransform : MonoBehaviour
    {
        public bool UseLeft;
        public float Left;
        public bool UseTop;
        public float Top;
        public bool UseRight;
        public float Right;
        public bool UseBottom;
        public float Bottom;

        private RectTransform rectTransform = null!;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            Vector2 parentSize;
            if (transform.parent == null)
                parentSize = new Vector2(Screen.width, Screen.height);
            else
            {
                Rect? _parentRect = (transform.parent as RectTransform)?.rect;
                if (!_parentRect.HasValue)
                    return;
                Rect parentRect = _parentRect.Value;
                parentSize = new Vector2(parentRect.width, parentRect.height);
            }

            rectTransform.offsetMin = new Vector2(
                UseLeft ? parentSize.x * Left : rectTransform.offsetMin.x,
                UseBottom ? parentSize.y * Bottom : rectTransform.offsetMin.y
            );
            rectTransform.offsetMax = new Vector2(
                UseRight ? parentSize.x * -Right : rectTransform.offsetMax.x,
                UseTop ? parentSize.y * -Top : rectTransform.offsetMax.y
            );
        }
    }
}
