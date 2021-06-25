using UnityEngine;
using UnityEngine.UI;

namespace RecyclableScrollRectX.Samples
{
    public class RandomScroller : MonoBehaviour
    {
        private void Start()
        {
            button.onClick.AddListener(OnClicked);
            if (dataSetChange != null)
            {
                dataSetChange.onClick.AddListener(OnDataSetChangedClicked);
            }
        }

        private void OnClicked()
        {
            var value = Random.value;
            scrollRect.ScrollToNormalizedPosition(value);
            Debug.Log($"Scroll to: {value}");
        }

        private void OnDataSetChangedClicked() { scrollRect.DataSetChanged(); }

        [SerializeField] private RecyclableScrollRect scrollRect;
        [SerializeField] private Button button;
        [SerializeField] private Button dataSetChange;
    }
}