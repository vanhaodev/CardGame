using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using Functions.World.Player.Popup.ItemSelector;
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
        [SerializeField] private GameObject _objBlockInput;
        [SerializeField] private TabSwitcher _tabSwitcherItemType;
        private int _currentItemTypeFilterIndex;
        [SerializeField] private InventoryItemUI _prefabInventoryItemUI;
        [SerializeField] private Transform _inventoryItemContainer;
        [SerializeField] private List<InventoryItemUI> _inventoryItemUIs;
        [SerializeField] private TextMeshProUGUI _txWeigthMax;
        [SerializeField] private TextMeshProUGUI _txWeigthCurrent;

        private void Awake()
        {
            if (_tabSwitcherItemType != null) _tabSwitcherItemType.OnTabSwitched += FilterItemType;
        }

        private void OnDestroy()
        {
            if (_tabSwitcherItemType != null) _tabSwitcherItemType.OnTabSwitched -= FilterItemType;
        }

        private async UniTask SpawnItem(ItemActionModel itemActionModel, ItemSelectorFilterModel filter)
        {
            _objBlockInput.SetActive(true);
            var inv = Global.Instance.Get<CharacterData>().CharacterModel.Inventory;
            await inv.Arrange();
            var items = inv.Items
                .OrderBy(i => i.Item.TemplateId)
                .ThenByDescending(i => i.Item.Rarity)
                .ThenByDescending(i => i.Quantity)
                .ToList();
            items = filter.ApplyFilter(items);

            Debug.Log(items.Count + "\n" +
                      _inventoryItemUIs.Count);
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
                    tasks.Add(_inventoryItemUIs[i].Init(items[i], itemActionModel));
                }
                else
                {
                    Debug.Log($"Slot {i}: ACTIVE false");
                    _inventoryItemUIs[i].gameObject.SetActive(false);
                    tasks.Add(_inventoryItemUIs[i].Init(null, itemActionModel));
                }
            }

            tasks.Add(InitWeight(inv));
            await UniTask.WhenAll(tasks);
            _objBlockInput.SetActive(false);
        }
        public async UniTask Init(ItemActionModel itemActionModel, ItemSelectorFilterModel filter)
        {
            await SpawnItem(itemActionModel, filter);
            itemActionModel.OnRespawnItemList = async () =>
            {
               await SpawnItem(itemActionModel, filter);
            };
            if (_tabSwitcherItemType != null) _tabSwitcherItemType?.Init(switchIndex: filter.ItemTypeNeedIndex + 1);
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
            _currentItemTypeFilterIndex = index-1;
            foreach (var item in _inventoryItemUIs)
            {
                if (item.Item == null || item.Item.Quantity < 1)
                {
                    item.transform.gameObject.SetActive(false);
                    continue;
                }

                bool isShardItem = item.Item.Item.TemplateId is >= 1001 and <= 1999;
                bool isActive;

                if (_currentItemTypeFilterIndex == 2)
                {
                    // Nếu đang lọc shard riêng → chỉ hiện shard
                    isActive = isShardItem;
                }
                else
                {
                    // Nếu lọc theo loại (Equipment/Resource)
                    if (_currentItemTypeFilterIndex == -1)
                    {
                        // All type → hiện tất cả (gồm cả shard)
                        isActive = true;
                    }
                    else
                    {
                        // Chỉ hiện đúng loại, shard tính là item loại riêng nên bị loại
                        isActive = !isShardItem && _currentItemTypeFilterIndex == (int)item.ItemType;
                    }
                }

                item.transform.gameObject.SetActive(isActive);
                // Debug.Log($"FilterItemType: { item.Item.Item.TemplateId} == isshard {isShardItem} isactive {isActive} _currentItemRarityFilterIndex {_currentItemRarityFilterIndex}");
            }

            Debug.Log($"FilterItemType: " + _currentItemTypeFilterIndex);
        }

        #region MyRegion

        // private async void FilterItemEquipmentType(int index)
        // {
        //     _currentItemEquipmentTypeFilterIndex = index -= 1;
        //     foreach (var item in _inventoryItemUIs)
        //     {
        //         if (item.Item.Quantity < 1) continue;
        //         var isActive = false;
        //         if (_currentItemEquipmentTypeFilterIndex != -1)
        //         {
        //             isActive = _currentItemEquipmentTypeFilterIndex == await GetEquipmentType(item.Item.Item);
        //         }
        //         else
        //         {
        //             isActive = true;
        //         }
        //
        //         item.transform.gameObject.SetActive(isActive);
        //     }
        //
        //     async UniTask<int> GetEquipmentType(ItemModel item)
        //     {
        //         var temp =
        //             await Global.Instance.Get<GameConfig>().GetItemTemplate(item.TemplateId) as
        //                 ItemEquipmentTemplateModel;
        //         return (int)temp.EquipmentType;
        //     }
        // }
        //
        // private void FilterItemRarity(int index)
        // {
        //     _currentItemRarityFilterIndex = index -= 1;
        //     foreach (var item in _inventoryItemUIs)
        //     {
        //         if (item.Item.Quantity < 1) continue;
        //         var isActive = false;
        //         if (_currentItemRarityFilterIndex != -1)
        //         {
        //             isActive = _currentItemRarityFilterIndex == (int)item.Item.Item.Rarity;
        //         }
        //         else
        //         {
        //             isActive = true;
        //         }
        //
        //         item.transform.gameObject.SetActive(isActive);
        // }
        //     }

        #endregion
    }
}