using UnityEngine;
using UnityEngine.UI;

namespace RecyclableScrollRectX.Demo
{
    public class RandomScroller : MonoBehaviour
    {
        private void Start()
        {
            // 点击
            button.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            var value = Random.value;
            scrollRect.ScrollToNormalizedPosition(value);
            Debug.Log($"Scroll to: {value}");
        }

        [SerializeField] private RecyclableScrollRect scrollRect;
        [SerializeField] private Button button;
    }
}