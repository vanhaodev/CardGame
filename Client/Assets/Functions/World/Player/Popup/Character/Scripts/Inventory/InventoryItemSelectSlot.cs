using System;
using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using World.Player.PopupCharacter;

public class InventoryItemSelectSlot : MonoBehaviour
{
    /// <summary>
    /// Nhận diện slot
    /// </summary>
    public string Identity;

    public bool IsEmpty => _itemUI.Item == null;
    [SerializeField] private InventoryItemUI _itemUI;
    [SerializeField] Button _btnSelect;
    private UnityAction<InventoryItemSelectSlot> _onSelect;

    private void Start()
    {
        _btnSelect.onClick.AddListener(() => { _onSelect?.Invoke(this); });
    }

    private void OnDestroy()
    {
        _btnSelect.onClick.RemoveAllListeners();
        _onSelect = null;
    }

    public async UniTask InitSlot(ItemModel item)
    {
        if (item == null)
        {
            _itemUI.Clear();
            return;
        }

        await _itemUI.Init(new InventoryItemModel()
        {
            Item = item,
            Quantity = 1
        });
    }

    public void ListenOnSelect(UnityAction<InventoryItemSelectSlot> onSelect)
    {
        _onSelect += onSelect;
    }
}