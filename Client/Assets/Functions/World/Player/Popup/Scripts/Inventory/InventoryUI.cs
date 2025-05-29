using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using Globals;
using TMPro;
using UnityEngine;
using Utils.Tab;
using CharacterData = World.Player.Character.CharacterData;

namespace World.Player.PopupCharacter
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private TabSwitcher _tabSwitcherItemType;
        [SerializeField] private InventoryItemUI _prefabInventoryItemUI;
        [SerializeField] private Transform _inventoryItemContainer;
        [SerializeField] private List<InventoryItemUI> _inventoryItemUIs;
        [SerializeField] private TextMeshProUGUI _txWeigthMax;
        [SerializeField] private TextMeshProUGUI _txWeigthCurrent;

        public async UniTask Init()
        {
            var inv = Global.Instance.Get<CharacterData>().CharacterModel.Inventory;
            var items = inv.Items
                .OrderByDescending(i => i.Item.Rarity)
                .ThenByDescending(i => i.Quantity)
                .ToList();
            var tasks = new List<UniTask>();
            // Instantiate thêm nếu list chưa đủ
            while (_inventoryItemUIs.Count < items.Count)
            {
                var newItemUI = Instantiate(_prefabInventoryItemUI, _inventoryItemContainer);
                _inventoryItemUIs.Add(newItemUI);
            }

            // Init hoặc SetActive từng item
            for (int i = 0; i < _inventoryItemUIs.Count; i++)
            {
                if (i < items.Count)
                {
                    _inventoryItemUIs[i].gameObject.SetActive(true);
                    tasks.Add(_inventoryItemUIs[i].Init(items[i]));
                }
                else
                {
                    _inventoryItemUIs[i].gameObject.SetActive(false);
                }
            }

            tasks.Add(InitWeight(inv));
            await UniTask.WhenAll(tasks);
            _tabSwitcherItemType.Init();
        }

        private void OnEnable()
        {
            _tabSwitcherItemType.OnTabSwitched += FilterItem;
        }

        private void OnDisable()
        {
            _tabSwitcherItemType.OnTabSwitched -= FilterItem;
        }

        async UniTask InitWeight(InventoryModel inventory)
        {
            var currentWeight = await inventory.GetCurrentWeight();
            var maxWeight = inventory.MaxWeight;

            string color = currentWeight >= maxWeight * 0.8f ? "#AB0000" : "#FFFFFF";
            _txWeigthCurrent.text = $"<color={color}>{currentWeight}</color>";
            _txWeigthMax.text = maxWeight + " <size=80%>kg</size>";
        }


        private void FilterItem(int index)
        {
            var itemType = index -= 1;
            foreach (var item in _inventoryItemUIs)
            {
                var isActive = false;
                if (itemType != -1)
                {
                    isActive = itemType == (int)item.ItemType;
                }
                else
                {
                    isActive = true;
                }

                item.transform.gameObject.SetActive(isActive);
            }
        }
    }
}