using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils;

namespace World.TheCard
{
    public class CardAttributeUI : MonoBehaviour
    {
        [SerializeField] private CardAttributeItemUI _prefab;
        [SerializeField] private Transform _transformContainer;
        [SerializeField] private ContentSizeFitter2 _contentSizeFitterContainer;
        [SerializeField] private List<CardAttributeItemUI> _cardAttributeItemUIs;

        public void Init(Dictionary<AttributeType, int> atts, Dictionary<AttributeType, int> attPercents = null,
            bool isShow0Value = false)
        {
            var types = (AttributeType[])Enum.GetValues(typeof(AttributeType));
            int uiIndex = 0;

            foreach (var type in types)
            {
                bool hasValue = false;

                if (atts != null && atts.TryGetValue(type, out var baseValue))
                {
                    if (isShow0Value || baseValue != 0)
                    {
                        EnsureUI(uiIndex).Init(type, baseValue, false); // false = không phải phần trăm
                        uiIndex++;
                        hasValue = true;
                    }
                }

                if (attPercents != null && attPercents.TryGetValue(type, out var percentValue))
                {
                    if (isShow0Value || percentValue != 0)
                    {
                        EnsureUI(uiIndex).Init(type, percentValue, true); // true = là phần trăm
                        uiIndex++;
                        hasValue = true;
                    }
                }

                // Nếu không có giá trị và không muốn hiện số 0, bỏ qua
                if (!hasValue && !isShow0Value)
                    continue;
            }

            // Ẩn các UI dư
            for (int i = uiIndex; i < _cardAttributeItemUIs.Count; i++)
            {
                _cardAttributeItemUIs[i].gameObject.SetActive(false);
            }
        }

        private CardAttributeItemUI EnsureUI(int index)
        {
            if (index < _cardAttributeItemUIs.Count)
            {
                var ui = _cardAttributeItemUIs[index];
                ui.gameObject.SetActive(true);
                return ui;
            }
            else
            {
                var ui = Instantiate(_prefab, transform);
                _cardAttributeItemUIs.Add(ui);
                return ui;
            }
        }
        
        public async UniTask RefreshUI()
        {
            await _contentSizeFitterContainer.UpdateSize();
        }
    }
}