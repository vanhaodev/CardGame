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

    [SerializeField] protected GameObject _objAdd;
    public bool IsEmpty => _itemUI.Item == null;
    [SerializeField] protected InventoryItemUI _itemUI;
    [SerializeField] protected Button _btnSelect;
    protected UnityAction<InventoryItemSelectSlot> _onSelect;

    private void Start()
    {
        _btnSelect.onClick.AddListener(() => { _onSelect?.Invoke(this); });
    }

    private void OnDestroy()
    {
        _btnSelect.onClick.RemoveAllListeners();
        _onSelect = null;
    }

    public async UniTask InitSlot(ItemModel item, UnityAction<ItemModel> onUnSelect, UnityAction OnChangedData, UnityAction onClose /*closepopup*/)
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
        }, new()
        {
            OnChangedData = OnChangedData,
            OnUnequip = onUnSelect,
            OnClose = onClose,
        });
    }

    public void ListenOnClick(UnityAction<InventoryItemSelectSlot> onSelect)
    {
        _onSelect = onSelect;
    }
    public void ShowItemInfor()
    {
        _itemUI.OnTouch();
    }
}