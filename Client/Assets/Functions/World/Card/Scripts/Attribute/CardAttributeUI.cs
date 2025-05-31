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

        public void Init(Dictionary<AttributeType, int> atts)
        {
            var types = (AttributeType[])Enum.GetValues(typeof(AttributeType));

            // Đảm bảo đủ số lượng UI (tái sử dụng hoặc instantiate thêm)
            for (int i = 0; i < types.Length; i++)
            {
                AttributeType type = types[i];
                int attValue = atts.TryGetValue(type, out var val) ? val : 0;

                CardAttributeItemUI ui;
                if (i < _cardAttributeItemUIs.Count)
                {
                    ui = _cardAttributeItemUIs[i];
                    ui.gameObject.SetActive(true);
                }
                else
                {
                    ui = Instantiate(_prefab, transform);
                    _cardAttributeItemUIs.Add(ui);
                }

                ui.Init(type, attValue);
            }

            // Ẩn các UI dư nếu có
            for (int i = types.Length; i < _cardAttributeItemUIs.Count; i++)
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