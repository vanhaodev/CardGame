using System;
using TMPro;
using UnityEngine;

namespace World.TheCard
{
    public class CardAttributeItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _txName;
        [SerializeField] private TextMeshProUGUI _txValue;

        public void Init(AttributeType attributeType, int attValue, bool isPercent = false)
        {
            _txName.text = attributeType.ToString();

            if (isPercent)
            {
                float percent = attValue / 100f; //hệ 10k
                _txValue.text = $"{percent:0.##}%"; // Hiển thị dạng "12.5%" hoặc "8%"
            }
            else
            {
                _txValue.text = attValue.ToString();
            }
        }
        public void Reset()
        {
            _txName.text = String.Empty;
            _txValue.text = String.Empty;
        }
    }
}