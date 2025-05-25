using System;
using TMPro;
using UnityEngine;

namespace World.TheCard
{
    public class CardAttributeItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _txName;
        [SerializeField] private TextMeshProUGUI _txValue;

        public void Init(AttributeType attributeType, int attValue)
        {
            _txName.text = $"{attributeType.ToString()}";
            _txValue.text = attValue.ToString();
        }
        public void Reset()
        {
            _txName.text = String.Empty;
            _txValue.text = String.Empty;
        }
    }
}