using TMPro;
using UnityEngine;

namespace FloatingEffect
{
    public class FloatingEffect : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI _txContent;
        public string PrefabAddress;
        public void SetText(string text)
        {
            _txContent.text = text;
        }
    }
}