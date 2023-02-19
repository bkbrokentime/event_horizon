using UnityEngine;
using UnityEngine.UI;

namespace Other
{
    public class ScrollViewFitter : MonoBehaviour
    {
        public RectTransform Viewport;
        public RectTransform ScrollView;
        public HorizontalLayoutGroup ScrollViewLayoutGroup;

        private void OnEnable()
        {
            if (Viewport.sizeDelta.y == ScrollView.sizeDelta.y - ScrollViewLayoutGroup.padding.top - ScrollViewLayoutGroup.padding.bottom)
                return;
            else
                Viewport.sizeDelta = new Vector2(Viewport.sizeDelta.x, ScrollView.sizeDelta.y - ScrollViewLayoutGroup.padding.top - ScrollViewLayoutGroup.padding.bottom);

            if (Viewport.sizeDelta.y < 400)
                return;
            else
                Viewport.sizeDelta = new Vector2(Viewport.sizeDelta.x, 400);
        }

        private void OnValidate()
        {
            OnEnable();
        }
    }
}
