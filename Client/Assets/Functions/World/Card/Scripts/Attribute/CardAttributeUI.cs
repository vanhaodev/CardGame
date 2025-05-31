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

        public void Init(Dictionary<AttributeType, int> atts, bool isShow0Value = false)
        {
            var types = (AttributeType[])Enum.GetValues(typeof(AttributeType));

            int uiIndex = 0;

            foreach (var type in types)
            {
                int attValue = 0;
                if (atts.TryGetValue(type, out var val))
                {
                    attValue = val;
                }
                else
                {
                    if (!isShow0Value) continue;
                }

                CardAttributeItemUI ui;
                if (uiIndex < _cardAttributeItemUIs.Count)
                {
                    ui = _cardAttributeItemUIs[uiIndex];
                    ui.gameObject.SetActive(true);
                }
                else
                {
                    ui = Instantiate(_prefab, transform);
                    _cardAttributeItemUIs.Add(ui);
                }

                ui.Init(type, attValue);
                uiIndex++;
            }

            // Ẩn các UI dư nếu có
            for (int i = uiIndex; i < _cardAttributeItemUIs.Count; i++)
            {
                _cardAttributeItemUIs[i].gameObject.SetActive(false);
            }
        }

        public async UniTask RefreshUI()
        {
            await _contentSizeFitterContainer.UpdateSize();
        }
    }
}