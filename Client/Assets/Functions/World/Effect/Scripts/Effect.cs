using TMPro;
using UnityEngine;

namespace Effects
{
    public class Effect : MonoBehaviour
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