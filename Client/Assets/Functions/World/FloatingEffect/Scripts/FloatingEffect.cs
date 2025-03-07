using TMPro;
using UnityEngine;

namespace FloatingEffect
{
    public class FloatingEffect : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI _txContent;
        [SerializeField] protected CanvasGroup _canvasGroup;
        public string PrefabAddress;
        public void SetText(string text)
        {
            _txContent.text = text;
        }
        public CanvasGroup GetCanvasGroup() => _canvasGroup;
    }
}