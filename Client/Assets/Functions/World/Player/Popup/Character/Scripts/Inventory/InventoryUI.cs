using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using GameConfigs;
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
        private int _currentItemTypeFilterIndex;
        [SerializeField] private TabSwitcher _tabSwitcherItemEquipmentType;
        private int _currentItemEquipmentTypeFilterIndex;
        [SerializeField] private TabSwitcher _tabSwitcherItemRarity;
        private int _currentItemRarityFilterIndex;
        [SerializeField] private InventoryItemUI _prefabInventoryItemUI;
        [SerializeField] private Transform _inventoryItemContainer;
        [SerializeField] private List<InventoryItemUI> _inventoryItemUIs;
        [SerializeField] private TextMeshProUGUI _txWeigthMax;
        [SerializeField] private TextMeshProUGUI _txWeigthCurrent;

        public async UniTask Init()
        {
            var inv = Global.Instance.Get<CharacterData>().CharacterModel.Inventory;
            await inv.Arrange();
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
            if (_tabSwitcherItemType != null) _tabSwitcherItemType?.Init();
            if (_tabSwitcherItemEquipmentType != null) _tabSwitcherItemEquipmentType?.Init();
            if (_tabSwitcherItemRarity != null) _tabSwitcherItemRarity?.Init();
        }

        private void OnEnable()
        {
            if (_tabSwitcherItemType != null) _tabSwitcherItemType.OnTabSwitched += FilterItemType;
            if (_tabSwitcherItemEquipmentType != null)
                _tabSwitcherItemEquipmentType.OnTabSwitched += FilterItemEquipmentType;
            if (_tabSwitcherItemRarity != null) _tabSwitcherItemRarity.OnTabSwitched += FilterItemRarity;
        }

        private void OnDisable()
        {
            if (_tabSwitcherItemType != null) _tabSwitcherItemType.OnTabSwitched -= FilterItemType;
            if (_tabSwitcherItemEquipmentType != null)
                _tabSwitcherItemEquipmentType.OnTabSwitched -= FilterItemEquipmentType;
            if (_tabSwitcherItemRarity != null) _tabSwitcherItemRarity.OnTabSwitched -= FilterItemRarity;
        }

        async UniTask InitWeight(InventoryModel inventory)
        {
            if (_txWeigthCurrent == null) return;
            var currentWeight = await inventory.GetCurrentWeight();
            var maxWeight = inventory.MaxWeight;

            string color = currentWeight >= maxWeight * 0.8f ? "#AB0000" : "#FFFFFF";
            _txWeigthCurrent.text = $"<color={color}>{currentWeight}</color>";
            _txWeigthMax.text = maxWeight + " <size=80%>kg</size>";
        }


        private void FilterItemType(int index)
        {
            _currentItemTypeFilterIndex = index -= 1;
            foreach (var item in _inventoryItemUIs)
            {
                if (item.Item.Quantity < 1) continue;
                var isActive = false;
                if (_currentItemTypeFilterIndex != -1)
                {
                    isActive = _currentItemTypeFilterIndex == (int)item.ItemType;
                }
                else
                {
                    isActive = true;
                }

                item.transform.gameObject.SetActive(isActive);
            }
        }

        private async void FilterItemEquipmentType(int index)
        {
            _currentItemEquipmentTypeFilterIndex = index -= 1;
            foreach (var item in _inventoryItemUIs)
            {
                if (item.Item.Quantity < 1) continue;
                var isActive = false;
                if (_currentItemEquipmentTypeFilterIndex != -1)
                {
                    isActive = _currentItemEquipmentTypeFilterIndex == await GetEquipmentType(item.Item.Item);
                }
                else
                {
                    isActive = true;
                }

                item.transform.gameObject.SetActive(isActive);
            }

            async UniTask<int> GetEquipmentType(ItemModel item)
            {
                var temp =
                    await Global.Instance.Get<GameConfig>().GetItemTemplate(item.TemplateId) as
                        ItemEquipmentTemplateModel;
                return (int)temp.EquipmentType;
            }
        }

        private void FilterItemRarity(int index)
        {
            _currentItemRarityFilterIndex = index -= 1;
            foreach (var item in _inventoryItemUIs)
            {
                if (item.Item.Quantity < 1) continue;
                var isActive = false;
                if (_currentItemRarityFilterIndex != -1)
                {
                    isActive = _currentItemRarityFilterIndex == (int)item.Item.Item.Rarity;
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